using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinesweeperGameHandler : MonoBehaviour
{
    private SaveScript _saveScript;
    private GegevensHouder _gegevensHouder;
    private RewardHandler _rewardHandler;
    [SerializeField] private MinesweeperLayout mvLayout;
    
    [SerializeField] private Sprite flag;
    [SerializeField] private Sprite mine;
    [SerializeField] private Sprite maskSprite;
    [SerializeField] private GameObject helpUICanvasObj;
    [SerializeField] private GameObject finishedGameCanvasObj;
    [SerializeField] private GameObject generalCanvasObj;
    [SerializeField] private GameObject minesweeperCanvasObj;
    [SerializeField] private GameObject menuUICanvasObj;
    [SerializeField] private TMP_Text minesToFindText;
    [SerializeField] private TMP_Text rewardText;
    
    public List<Image> cellInputButtonImages;
    public List<TMP_Text> cellInputButtonTexts;
    
    private readonly int[] _minesweeperMines = new int[352];
    private readonly int[] _minesweeperInput = new int[352];
    private int _notFoundBombs;
    [HideInInspector] public bool gameOver;
    [HideInInspector] public bool isFlagInputMode;
    
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

        if (_gegevensHouder.startNewGame)
        {
            DeleteProgress();
            CreateNewPuzzle();
        }
        else
        {
            LoadProgress();
        }

        minesToFindText.text = _notFoundBombs.ToString();
    }

    private void CreateNewPuzzle()
    {
        _notFoundBombs = 25 + (int)(10f * Mathf.Pow(1.75f, _saveScript.intDict["MinesweeperDifficulty"]));
        for (var i = 0; i < _notFoundBombs; i++)
        {
            int rand = Random.Range(0, _minesweeperMines.Length);
            int num = _minesweeperMines[rand];
            if (num != 1)
                _minesweeperMines[rand] = 1;
            else
                i--;
        }

        _saveScript.stringDict["MinesweeperMines"] = SaveScript.StringifyArray(_minesweeperMines);
    }

    private void LoadProgress()
    {
        string bombs = _saveScript.stringDict["MinesweeperMines"];
        var chars = bombs.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            var ch = chars[i];
            _minesweeperMines[i] = ch - '0';
            if (ch - '0' == 1) _notFoundBombs += 1;
        }
        
        string input = _saveScript.stringDict["MinesweeperInput"];
        chars = input.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            var ch = chars[i];
            switch (ch)
            {
                case '2':
                    _minesweeperInput[i] = 2;
                    InstantiateFlag(i);
                    _notFoundBombs -= 1;
                    break;
                case '1':
                    _minesweeperInput[i] = 1;
                    InstantiateButton(i);
                    break;
            }
        }
    }

    private IEnumerator WaitForFinishedCanvas()
    {
        yield return new WaitForSecondsRealtime(1f);
        OpenFinishedCanvas();
    }

    private int GetMineCount(int index)
    {
        int longSide = Mathf.Max(MinesweeperLayout.HorizontalSideBoxCount, MinesweeperLayout.VerticalSideBoxCount);
        if (_minesweeperMines[index] == 1) return 99;
        int mineCount = 0;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                int mineIndex = index + i - 1 + (j - 1) * longSide;
                if (mineIndex < 0 || mineIndex >= _minesweeperMines.Length) continue;
                if (mineIndex % longSide == 0 && i == 2 || index % longSide == 0 && i == 0) continue;
                mineCount += _minesweeperMines[mineIndex];
            }
        }

        return mineCount;
    }

    private void FindOtherSafeCells(int index)
    {
        int longSide = Mathf.Max(MinesweeperLayout.HorizontalSideBoxCount, MinesweeperLayout.VerticalSideBoxCount);
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                int mineIndex = index + i - 1 + (j - 1) * longSide;
                if (mineIndex < 0 || mineIndex >= _minesweeperMines.Length) continue;
                if (mineIndex % longSide == 0 && i == 2 || index % longSide == 0 && i == 0) continue;
                if (_minesweeperInput[mineIndex] == 0)  InstantiateButton(mineIndex);
            }
        }
    }

    private void ShowAllBombs()
    {
        for (int i = 0; i < _minesweeperMines.Length; i++)
        {
            if (_minesweeperMines[i] != 1 || _minesweeperInput[i] == 2) continue; // a flag or just not a mine
            cellInputButtonTexts[i].text = "";
            cellInputButtonImages[i].sprite = mine;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OpenFinishedCanvas()
    {
        finishedGameCanvasObj.SetActive(true);
        helpUICanvasObj.SetActive(false);
        generalCanvasObj.SetActive(false);
        minesweeperCanvasObj.SetActive(false);
        menuUICanvasObj.SetActive(false);
        mvLayout.SetLayout();
    }

    private void DeleteProgress()
    {
        _saveScript.stringDict["MinesweeperMines"] = "";
        _saveScript.stringDict["MinesweeperInput"] = "";
    }

    private void InstantiateButton(int index)
    {
        int mines = GetMineCount(index);
        switch (mines)
        {
            case 0:
                cellInputButtonTexts[index].text = "-";
                _minesweeperInput[index] = 1;
                FindOtherSafeCells(index);
                break;
            case 99:
                cellInputButtonTexts[index].text = "";
                cellInputButtonImages[index].sprite = mine;
                ShowAllBombs();
                OnTouchMine();
                gameOver = true;
                break;
            default:
                cellInputButtonTexts[index].text = mines.ToString();
                _minesweeperInput[index] = 1;
                break;
        }
    }

    private void InstantiateFlag(int index)
    {
        cellInputButtonTexts[index].text = "";
        cellInputButtonImages[index].sprite = flag;
        _notFoundBombs -= 1;
        minesToFindText.text = _notFoundBombs.ToString();
        _minesweeperInput[index] = 2;
    }

    private void RemoveFlag(int index)
    {
        cellInputButtonTexts[index].text = "";
        cellInputButtonImages[index].sprite = maskSprite;
        _notFoundBombs += 1;
        minesToFindText.text = _notFoundBombs.ToString();
        _minesweeperInput[index] = 0;
    }

    public void OnClickButton()
    {
        if (gameOver) return;
        StartCoroutine(OnTouch());
    }

    private IEnumerator OnTouch()
    {
        yield return new WaitForEndOfFrame();
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        int index = int.Parse(selectedButton.name);
        if (isFlagInputMode)
        {
            switch (_minesweeperInput[index])
            {
                case 2:
                    RemoveFlag(index);
                    break;
                case 0:
                    InstantiateFlag(index);
                    break;
            }
        }
        else
        {
            if (_minesweeperInput[index] != 0) yield break;
            InstantiateButton(index);
            CheckIfFinished();
        }
        _saveScript.stringDict["MinesweeperInput"] = SaveScript.StringifyArray(_minesweeperInput);
    }

    private void CheckIfFinished()
    {
        int inputCount = 0;
        foreach (var i in _minesweeperInput)
            if (i > 0) inputCount += 1;

        if (inputCount + _notFoundBombs < _minesweeperInput.Length) return;
        Scene scene = SceneManager.GetActiveScene();
        int diff = _saveScript.intDict["MinesweeperDifficulty"];
        //_saveScript.intDict["MijnenvegerDiff" + diff + "Gespeeld"] += 1;
        //_saveScript.intDict["MijnenvegersGespeeld"] += 1;
        rewardText.text = _rewardHandler.Beloning(scene: scene, difficulty: diff, doelwitText: rewardText)
            .ToString();
        OpenFinishedCanvas();
        DeleteProgress();
    }

    private void OnTouchMine()
    {
        gameOver = true;
        DeleteProgress();
        StartCoroutine(WaitForFinishedCanvas());
    }
}