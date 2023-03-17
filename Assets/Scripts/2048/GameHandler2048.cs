using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameHandler2048 : MonoBehaviour
{
    private const int HighNumberProbabilityPercent = 10;

    private GegevensHouder _gegevensHouder;
    private SaveScript _saveScript;
    private RewardHandler _rewardHandler;
    [SerializeField] private Layout2048 layout2048;

    [SerializeField] private List<GameObject> cellObjs;
    [SerializeField] private List<TMP_Text> cellTexts;
    [SerializeField] private List<GameObject> horizontalLineObjs;
    [SerializeField] private List<GameObject> verticalLineObjs;
    [SerializeField] private GridLayoutGroup cellLayoutGroup;
    [SerializeField] private GridLayoutGroup horizontalLineLayoutGroup;
    [SerializeField] private GridLayoutGroup verticalLineLayoutGroup;

    [SerializeField] private GameObject finishedGameCanvasObj;
    [SerializeField] private GameObject helpUICanvasObj;
    [SerializeField] private GameObject generalCanvasObj;
    [SerializeField] private GameObject canvas2048Obj;
    [SerializeField] private GameObject menuUICanvasObj;
    [SerializeField] private LocalizeStringEvent finishedCanvasScoreText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private LocalizeStringEvent scoreText;

    private int _mode;
    private int _size;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private float _minTouchMove;

    private void Start()
    {
        _gegevensHouder = GegevensHouder.Instance;
        if (_gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }

        _saveScript = SaveScript.Instance;
        _rewardHandler = RewardHandler.Instance;
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        _minTouchMove = Mathf.Min(Screen.height, Screen.width) * 0.05f;
        _mode = _saveScript.IntDict["2048SelectedMode"];
        _size = _mode + 4;
        SetupGrid();
        if (_gegevensHouder.startNewGame) StartNewGame();
        else ResumeGame();
        OnMove();
    }

    private void StartNewGame()
    {
        DeleteProgress();
        List<int> placedIndexes = new();
        for (int i = 0; i < 3; i++)
        {
            int randNum = Random.Range(0, _size * _size);
            if (placedIndexes.Contains(randNum))
            {
                i--;
                continue;
            }

            SetCellText(cellTexts[randNum]);
            placedIndexes.Add(randNum);
        }
    }

    private void ResumeGame()
    {
        string[] progress = _saveScript.StringDict[$"2048Mode{_mode}Progress"].Split(',');
        for (int i = 0; i < progress.Length; i++)
        {
            if (progress[i][0].Equals('0')) continue;
            SetCellText(cellTexts[i], progress[i]);
        }
    }

    private void SetupGrid()
    {
        cellLayoutGroup.constraintCount = _size;
        cellLayoutGroup.cellSize = Vector2.one * (1000f / _size);
        horizontalLineLayoutGroup.constraintCount = _size;
        verticalLineLayoutGroup.constraintCount = _size;
        float lineThickness = 75f / (_size - 1);
        horizontalLineLayoutGroup.cellSize = new Vector2(1000, lineThickness);
        verticalLineLayoutGroup.cellSize = new Vector2(lineThickness, 1000);
        float lineSpacing = 1000f / _size - lineThickness;
        horizontalLineLayoutGroup.spacing = Vector2.up * lineSpacing;
        verticalLineLayoutGroup.spacing = Vector2.right * lineSpacing;
        for (int i = 8; i > _size; i--)
        {
            GameObject horizontalLine = horizontalLineObjs[^1];
            Destroy(horizontalLine);
            horizontalLineObjs.RemoveAt(horizontalLineObjs.Count - 1);
            GameObject verticalLine = verticalLineObjs[^1];
            Destroy(verticalLine);
            verticalLineObjs.RemoveAt(verticalLineObjs.Count - 1);
        }

        int neededCellCount = _size * _size;
        int removeCount = cellObjs.Count - neededCellCount;
        for (int i = 1; i <= removeCount; i++) Destroy(cellObjs[^(i)]);
        cellObjs.RemoveRange(neededCellCount, removeCount);
        cellTexts.RemoveRange(neededCellCount, removeCount);
    }
    
    private void Update()
    {
        if (finishedGameCanvasObj.activeInHierarchy) return;
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _startTouchPosition = touch.position;
                _endTouchPosition = touch.position;
                break;
            case TouchPhase.Ended:
            {
                _endTouchPosition = touch.position;
                float horizontalMove = Mathf.Abs(_endTouchPosition.x - _startTouchPosition.x);
                float verticalMove = Mathf.Abs(_endTouchPosition.y - _startTouchPosition.y);
                if (horizontalMove < _minTouchMove && verticalMove < _minTouchMove) break;
                MoveInCorrectDirection(_startTouchPosition, _endTouchPosition);
                AddNumberInEmptyCell();
                OnMove();
                break;
            }
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
            case TouchPhase.Canceled:
            default:
                break;
        }
    }

    private void MoveInCorrectDirection(Vector2 startPosition, Vector2 endPosition)
    {
        if (Mathf.Abs(endPosition.x - startPosition.x) >= Mathf.Abs(endPosition.y - startPosition.y))
        {
            if (!CanMoveHorizontal()) return;
            if (endPosition.x > startPosition.x)
                MoveRight();
            else
                MoveLeft();
        }
        else
        {
            if (!CanMoveVertical()) return;
            if (endPosition.y > startPosition.y)
                MoveUp();
            else
                MoveDown();
        }
    }

    private void OnMove()
    {
        int score = 0;
        foreach (TMP_Text cellText in cellTexts)
            if (!cellText.text.Equals(""))
                score += int.Parse(cellText.text);
        scoreText.StringReference.Remove("score");
        scoreText.StringReference.Add("score", new IntVariable { Value = score });
        scoreText.StringReference.RefreshString();
        SaveProgress();
        CheckIfFinished();
    }

    private void CheckIfFinished()
    {
        if (IsMovePossible()) return;
        Scene scene = SceneManager.GetActiveScene();
        float score = ((IntVariable)scoreText.StringReference["score"]).Value;
        rewardText.text = _rewardHandler
            .GetReward(scene: scene, difficulty: _mode, score: score, targetText: rewardText).ToString();
        OpenFinishedCanvas();
    }

    private static string CreateNewCellNumber()
    {
        return Random.Range(0, 100) < HighNumberProbabilityPercent ? "4" : "2";
    }

    private static void SetCellText(TMP_Text cellText, string numberString = "")
    {
        cellText.text = numberString.Equals("") ? CreateNewCellNumber() : numberString;
    }

    private void AddNumberInEmptyCell()
    {
        List<TMP_Text> possibleCells = cellTexts.FindAll(x => x.text == "");
        if (possibleCells.Count == 0) return;
        int randNum = Random.Range(0, possibleCells.Count);
        SetCellText(possibleCells[randNum]);
    }

    private void MoveRight()
    {
        for (int i = 0; i < _size; i++)
        {
            int emptyCellIndex = -1;
            for (int j = _size - 1; j >= 0; j--)
            {
                if (cellTexts[i * _size + j].text.Equals(""))
                {
                    if (emptyCellIndex == -1)
                        emptyCellIndex = j;
                    continue;
                }

                for (int k = j - 1; k >= 0; k--)
                {
                    if (cellTexts[i * _size + k].text.Equals("")) continue;
                    if (!cellTexts[i * _size + j].text.Equals(cellTexts[i * _size + k].text)) break;
                    if (emptyCellIndex != -1)
                    {
                        cellTexts[i * _size + emptyCellIndex].text =
                            (int.Parse(cellTexts[i * _size + j].text) * 2).ToString();
                        cellTexts[i * _size + j].text = "";
                        cellTexts[i * _size + k].text = "";
                        j = emptyCellIndex;
                    }
                    else
                    {
                        cellTexts[i * _size + j].text = (int.Parse(cellTexts[i * _size + j].text) * 2).ToString();
                        cellTexts[i * _size + k].text = "";
                    }

                    emptyCellIndex = -1;
                    break;
                }

                if (emptyCellIndex != -1)
                {
                    cellTexts[i * _size + emptyCellIndex].text = cellTexts[i * _size + j].text;
                    cellTexts[i * _size + j].text = "";
                    j = emptyCellIndex;
                }

                emptyCellIndex = -1;
            }
        }
    }

    private void MoveLeft()
    {
        for (int i = 0; i < _size; i++)
        {
            int emptyCellIndex = -1;
            for (int j = 0; j < _size; j++)
            {
                if (cellTexts[i * _size + j].text.Equals(""))
                {
                    if (emptyCellIndex == -1)
                        emptyCellIndex = j;
                    continue;
                }

                for (int k = j + 1; k < _size; k++)
                {
                    if (cellTexts[i * _size + k].text.Equals("")) continue;
                    if (!cellTexts[i * _size + j].text.Equals(cellTexts[i * _size + k].text)) break;
                    if (emptyCellIndex != -1)
                    {
                        cellTexts[i * _size + emptyCellIndex].text =
                            (int.Parse(cellTexts[i * _size + j].text) * 2).ToString();
                        cellTexts[i * _size + j].text = "";
                        cellTexts[i * _size + k].text = "";
                        j = emptyCellIndex;
                    }
                    else
                    {
                        cellTexts[i * _size + j].text = (int.Parse(cellTexts[i * _size + j].text) * 2).ToString();
                        cellTexts[i * _size + k].text = "";
                    }

                    emptyCellIndex = -1;
                    break;
                }

                if (emptyCellIndex != -1)
                {
                    cellTexts[i * _size + emptyCellIndex].text = cellTexts[i * _size + j].text;
                    cellTexts[i * _size + j].text = "";
                    j = emptyCellIndex;
                }

                emptyCellIndex = -1;
            }
        }
    }

    private void MoveUp()
    {
        for (int i = 0; i < _size; i++)
        {
            int emptyCellIndex = -1;
            for (int j = 0; j < _size; j++)
            {
                if (cellTexts[i + j * _size].text.Equals(""))
                {
                    if (emptyCellIndex == -1)
                        emptyCellIndex = j;
                    continue;
                }

                for (int k = j + 1; k < _size; k++)
                {
                    if (cellTexts[i + k * _size].text.Equals("")) continue;
                    if (!cellTexts[i + j * _size].text.Equals(cellTexts[i + k * _size].text)) break;
                    if (emptyCellIndex != -1)
                    {
                        cellTexts[i + emptyCellIndex * _size].text =
                            (int.Parse(cellTexts[i + j * _size].text) * 2).ToString();
                        cellTexts[i + j * _size].text = "";
                        cellTexts[i + k * _size].text = "";
                        j = emptyCellIndex;
                    }
                    else
                    {
                        cellTexts[i + j * _size].text = (int.Parse(cellTexts[i + j * _size].text) * 2).ToString();
                        cellTexts[i + k * _size].text = "";
                    }

                    emptyCellIndex = -1;
                    break;
                }

                if (emptyCellIndex != -1)
                {
                    cellTexts[i + emptyCellIndex * _size].text = cellTexts[i + j * _size].text;
                    cellTexts[i + j * _size].text = "";
                    j = emptyCellIndex;
                }

                emptyCellIndex = -1;
            }
        }
    }

    private void MoveDown()
    {
        for (int i = 0; i < _size; i++)
        {
            int emptyCellIndex = -1;
            for (int j = _size - 1; j >= 0; j--)
            {
                if (cellTexts[i + j * _size].text.Equals(""))
                {
                    if (emptyCellIndex == -1)
                        emptyCellIndex = j;
                    continue;
                }

                for (int k = j - 1; k >= 0; k--)
                {
                    if (cellTexts[i + k * _size].text.Equals("")) continue;
                    if (!cellTexts[i + j * _size].text.Equals(cellTexts[i + k * _size].text)) break;
                    if (emptyCellIndex != -1)
                    {
                        cellTexts[i + emptyCellIndex * _size].text =
                            (int.Parse(cellTexts[i + j * _size].text) * 2).ToString();
                        cellTexts[i + j * _size].text = "";
                        cellTexts[i + k * _size].text = "";
                        j = emptyCellIndex;
                    }
                    else
                    {
                        cellTexts[i + j * _size].text = (int.Parse(cellTexts[i + j * _size].text) * 2).ToString();
                        cellTexts[i + k * _size].text = "";
                    }

                    emptyCellIndex = -1;
                    break;
                }

                if (emptyCellIndex != -1)
                {
                    cellTexts[i + emptyCellIndex * _size].text = cellTexts[i + j * _size].text;
                    cellTexts[i + j * _size].text = "";
                    j = emptyCellIndex;
                }

                emptyCellIndex = -1;
            }
        }
    }

    private bool IsMovePossible() => CanMoveVertical() || CanMoveHorizontal(true);

    private bool HasAllCellsFilled()
    {
        for (int i = 0; i < _size * _size; i++)
        {
            if (cellTexts[i].text.Equals("")) return false;
        }

        return true;
    }

    private bool CanMoveHorizontal(bool alreadyCheckedAllFilled = false)
    {
        if (!alreadyCheckedAllFilled)
            if (!HasAllCellsFilled())
                return true;
        for (int i = 0; i < _size * _size; i++)
        {
            if (i % _size == _size - 1) continue;
            if (cellTexts[i].text.Equals(cellTexts[i + 1].text)) return true;
        }

        return false;
    }

    private bool CanMoveVertical()
    {
        if (!HasAllCellsFilled()) return true;
        for (int i = 0; i < _size * _size; i++)
        {
            if (Mathf.FloorToInt(i / (1f * _size)) == _size - 1) continue;
            if (cellTexts[i].text.Equals(cellTexts[i + _size].text)) return true;
        }

        return false;
    }

    private void SaveProgress()
    {
        string[] progress = new string[_size * _size];
        for (int i = 0; i < _size * _size; i++)
        {
            progress[i] = cellTexts[i].text;
            if (progress[i].Equals("")) progress[i] = "0";
        }

        _saveScript.StringDict[$"2048Mode{_mode}Progress"] = SaveScript.StringifyArray(progress, ",");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OpenFinishedCanvas()
    {
        finishedCanvasScoreText.StringReference.Clear();
        finishedCanvasScoreText.StringReference.Add("score",
            new IntVariable { Value = ((IntVariable)scoreText.StringReference["score"]).Value });
        DeleteProgress();
        canvas2048Obj.SetActive(false);
        generalCanvasObj.SetActive(false);
        helpUICanvasObj.SetActive(false);
        menuUICanvasObj.SetActive(false);
        finishedGameCanvasObj.SetActive(true);
        layout2048.SetLayout();
    }

    private void DeleteProgress()
    {
        _saveScript.StringDict[$"2048Mode{_mode}Progress"] = "";
    }
}