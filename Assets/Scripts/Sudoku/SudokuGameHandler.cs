using System.Collections.Generic;
using Sudoku;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class SudokuGameHandler : MonoBehaviour
{
    private SudokuUIHandler _sudokuUIHandler;
    private SudokuLayout _sudokuLayout;
    private BeloningScript _rewardHandler;
    private GegevensHouder _gegevensHouder;
    private SaveScript _saveScript;
    [SerializeField] private GameObject finishedCanvas;
    [SerializeField] private GameObject sudokuCanvas;
    [SerializeField] private GameObject generalCanvas;
    [SerializeField] private GameObject menuUICanvasObj;
    [SerializeField] private List<Button> inputCellButtons = new();
    [SerializeField] private List<TMP_Text> inputCellButtonTexts = new();
    [SerializeField] private List<TMP_Text> preSetCellObjects = new();
    [HideInInspector] public bool receivedInput = true;
    [SerializeField] private TMP_Text rewardText;
    private SudokuPuzzle _puzzle;
    [HideInInspector] public int[] sudokuClues;
    public readonly int[] SudokuPuzzleInput = new int[81];
    public readonly int[][] SudokuPuzzleInputNotes = new int[81][];
    private int[] _solution;
    private readonly int[] _cluesPerDiff = { 50, 40, 30, 22, 17 };
    private const int MaxMillisecondsCreateTime = 3500;

    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        _gegevensHouder = GegevensHouder.Instance;
        if (_gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }

        _saveScript = SaveScript.Instance;
        _rewardHandler = BeloningScript.Instance;
        _sudokuLayout = GetComponent<SudokuLayout>();
        _sudokuUIHandler = GetComponent<SudokuUIHandler>();
        _sudokuUIHandler.cellInputButtons = inputCellButtons;
        _sudokuUIHandler.cellInputButtonTexts = inputCellButtonTexts;

        if (_gegevensHouder.startNewGame)
        {
            ClearProgress();
            CreateNewSudoku();
        }
        else
        {
            LoadProgress();
            if (_saveScript.intDict["SudokuEnabledDoubleNumberWarning"] == 1) _sudokuUIHandler.CheckIfDoubleNumber();
        }
    }

    private void CreateNewSudoku()
    {
        _puzzle = SudokuPuzzle.RandomGrid(9);

        int maxClues = _cluesPerDiff[_saveScript.intDict["SudokuDifficulty"]];

        sudokuClues = SudokuPuzzle.CreateClues(_puzzle, maxClues, MaxMillisecondsCreateTime);
        _solution = SudokuPuzzle.GetSolution(sudokuClues);
        string cluesString = "";
        for (int i = 0; i < preSetCellObjects.Count; i++)
        {
            int clue = sudokuClues[i];
            if (clue == 0)
            {
                cluesString += ".";
                continue;
            }

            cluesString += clue;
            preSetCellObjects[i].text = clue.ToString();
            inputCellButtons[i].interactable = false;
        }

        for (var i = 0; i < 81; i++)
        {
            SudokuPuzzleInputNotes[i] = new int[9];
        }

        _saveScript.stringDict["SudokuClues"] = cluesString;
        _saveScript.stringDict["SudokuInput"] = SudokuUIHandler.StringifyInputArray(SudokuPuzzleInput);
        _saveScript.stringDict["SudokuInputNotes"] = SudokuUIHandler.StringifyInputNotesArray(SudokuPuzzleInputNotes);
    }

    private void LoadProgress()
    {
        sudokuClues = new int[81];
        char[] clues = _saveScript.stringDict["SudokuClues"].ToCharArray();
        for (int i = 0; i < clues.Length; i++)
        {
            char clueChar = clues[i];
            int clue = 0;
            if (!clueChar.Equals('.')) clue = clueChar - '0';
            sudokuClues[i] = clue;
            if (clue == 0) continue;
            preSetCellObjects[i].text = clueChar.ToString();
            inputCellButtons[i].interactable = false;
        }

        _solution = SudokuPuzzle.GetSolution(sudokuClues);

        char[] inputChars = _saveScript.stringDict["SudokuInput"].ToCharArray();
        for (int i = 0; i < inputChars.Length; i++)
        {
            int number = inputChars[i] - '0';
            if (number != 0) _sudokuUIHandler.EnterNormalNumber(i, number);
        }

        string[] inputNotesString = _saveScript.stringDict["SudokuInputNotes"].Split(",");
        for (int i = 0; i < inputNotesString.Length; i++)
        {
            SudokuPuzzleInputNotes[i] = new int[9];
            char[] inputNoteChars = inputNotesString[i].ToCharArray();
            foreach (var n in inputNoteChars)
            {
                int number = n - '0';
                if (number != 0) _sudokuUIHandler.EnterNotesNumber(i, number);
            }
        }
    }

    private void Update()
    {
        if (finishedCanvas.activeInHierarchy) return;
        if (!receivedInput) return;

        for (var i = 0; i < 81; i++)
        {
            if (sudokuClues[i] != 0) continue;
            if (SudokuPuzzleInput[i] != _solution[i]) return;
        }

        Scene scene = SceneManager.GetActiveScene();
        int diff = _saveScript.intDict["SudokuDifficulty"];
        rewardText.text = _rewardHandler.Beloning(scene, difficulty: diff, doelwitText: rewardText).ToString();
        //_saveScript.intDict["SudokuDiff" + diff + "Gespeeld"] += 1;
        //_saveScript.intDict["SudokusGespeeld"] += 1;
        OpenFinishedCanvas();
    }

    private void OpenFinishedCanvas()
    {
        ClearProgress();
        finishedCanvas.SetActive(true);
        sudokuCanvas.SetActive(false);
        generalCanvas.SetActive(false);
        menuUICanvasObj.SetActive(false);
        _sudokuLayout.SetLayout();
    }

    private void ClearProgress()
    {
        _saveScript.stringDict["SudokuClues"] = "";
        _saveScript.stringDict["SudokuInput"] = "";
        _saveScript.stringDict["SudokuInputNotes"] = "";
    }
}