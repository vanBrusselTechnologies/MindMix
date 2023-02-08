using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VBG.Extensions;

public class SudokuUIHandler : BaseUIHandler
{
    [Header("Other scene specific")]
    [SerializeField] private SudokuGameHandler sudokuGameHandler;
    [SerializeField] private Transform backgroundNumberEnterButtonTf;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private MeshRenderer normalNumberEnterButtonMesh;
    [SerializeField] private MeshRenderer noteNumberEnterButtonMesh;

    [HideInInspector] public List<Button> cellInputButtons;
    [HideInInspector] public List<TMP_Text> cellInputButtonTexts;
    
    private bool _isNormalNumberInput = true;
    private int _selectedButtonIndex = -1;
    private int _chosenDifficulty;
    readonly Color _redColor = new(1f, 0, 0, 1f);
    readonly Color _normalCellColor = new(175f / 255f, 175f / 255f, 175f / 255f, 60f / 255f);

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (saveScript == null) return;
        dropdown.value = saveScript.intDict["SudokuDifficulty"];
    }

    public void StartNewSudoku()
    {
        saveScript.intDict["SudokuDifficulty"] = dropdown.value;
        StartNewGame();
    }

    public void StartMoreDifficultSudoku()
    {
        saveScript.intDict["SudokuDifficulty"] += 1;
        StartNewGame();
    }

    public void IsSelected()
    {
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        var n = int.Parse(selectedButton.name);
        var index = Mathf.FloorToInt(n / 10f) * 9 + n % 10;
        _selectedButtonIndex = index;
    }

    public void ClickedNumber()
    {
        string selectedNumber = EventSystem.current.currentSelectedGameObject.name;
        int index = _selectedButtonIndex;
        int selectedNum = int.Parse(selectedNumber);
        if (_isNormalNumberInput)
        {
            EnterNormalNumber(index, selectedNum);
            if (saveScript.intDict["SudokuEnabledAutoEditNotes"] == 1) AutoChangeNotes(index);
        }
        else
            EnterNotesNumber(index, selectedNum);

        saveScript.stringDict["SudokuInputNotes"] = StringifyInputNotesArray(sudokuGameHandler.SudokuPuzzleInputNotes);
        saveScript.stringDict["SudokuInput"] = SaveScript.StringifyArray(sudokuGameHandler.SudokuPuzzleInput);

        if (saveScript.intDict["SudokuEnabledDoubleNumberWarning"] == 1) CheckIfDoubleNumber();
        sudokuGameHandler.receivedInput = true;
    }

    public void EnterNormalNumber(int index, int number)
    {
        TMP_Text text = cellInputButtonTexts[index];
        if (Math.Abs(text.fontSize - 265) > 0.001)
        {
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 265;
            RectTransform rect = cellInputButtonTexts[index].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
        }

        text.text = number.ToString();
        sudokuGameHandler.SudokuPuzzleInput[index] = number;
    }

    public void EnterNotesNumber(int index, int number)
    {
        TMP_Text text = cellInputButtonTexts[index];
        if (Math.Abs(text.fontSize - 80) > 0.001)
        {
            text.alignment = TextAlignmentOptions.Left;
            text.fontSize = 80;
            RectTransform rect = cellInputButtonTexts[index].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(-0.75f, 0);
        }

        sudokuGameHandler.SudokuPuzzleInputNotes[index][number - 1] =
            sudokuGameHandler.SudokuPuzzleInputNotes[index][number - 1] == 0 ? number : 0;
        string str = "";
        int i = 0;
        foreach (int num in sudokuGameHandler.SudokuPuzzleInputNotes[index])
        {
            if (num == 0) str += "    ";
            else str += $"{num}  ";
            i++;
            if (i % 3 == 0) str += "\n";
        }

        text.text = str;
        sudokuGameHandler.SudokuPuzzleInput[index] = 0;
    }

    public void ChangeNumberInputType()
    {
        backgroundNumberEnterButtonTf.Rotate(new Vector3(0, 180, 180));
        _isNormalNumberInput = !_isNormalNumberInput;
        noteNumberEnterButtonMesh.enabled = !_isNormalNumberInput;
        normalNumberEnterButtonMesh.enabled = _isNormalNumberInput;
    }

    public override void OpenSettings()
    {
        bool settingObjActive = settingsCanvasObj.activeSelf;
        base.OpenSettings();
        if (!settingObjActive) return;
        if (saveScript.intDict["SudokuEnabledDoubleNumberWarning"] == 1)
            CheckIfDoubleNumber();
        else
        {
            for (int i = 0; i < 81; i++)
            {
                Button button = cellInputButtons[i];
                ColorBlock colorBlock = button.colors;
                colorBlock.normalColor = _normalCellColor;
                button.colors = colorBlock;
            }
        }
    }

    private void AutoChangeNotes(int index)
    {
        if (index >= 81) return;
        if (sudokuGameHandler.SudokuPuzzleInput[index] == 0) return;
        List<int> indexes = GetConnectedIndexes(index);
        int number = sudokuGameHandler.SudokuPuzzleInput[index];
        foreach (int n in indexes)
        {
            if (index == n) continue;
            if (sudokuGameHandler.SudokuPuzzleInput[n] != 0) continue;
            int note = sudokuGameHandler.SudokuPuzzleInputNotes[n][number - 1];
            if (note != number) continue;
            EnterNotesNumber(n, number);
        }
    }

    public void CheckIfDoubleNumber(int index = 0)
    {
        if (index >= 81) return;
        List<int> indexes = GetConnectedIndexes(index);
        Button button = cellInputButtons[index];
        ColorBlock colorBlock = button.colors;
        foreach (int n in indexes)
        {
            if (index == n) continue;
            if (sudokuGameHandler.SudokuPuzzleInput[index] == 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    int note = sudokuGameHandler.SudokuPuzzleInputNotes[index][i];
                    if (note == 0 || (note != sudokuGameHandler.SudokuPuzzleInput[n] &&
                                      note != sudokuGameHandler.sudokuClues[n])) continue;
                    colorBlock.normalColor = _redColor;
                    button.colors = colorBlock;
                    CheckIfDoubleNumber(index + 1);
                    return;
                }
            }
            else if (sudokuGameHandler.SudokuPuzzleInput[index] == sudokuGameHandler.SudokuPuzzleInput[n] ||
                     sudokuGameHandler.SudokuPuzzleInput[index] == sudokuGameHandler.sudokuClues[n])
            {
                colorBlock.normalColor = _redColor;
                button.colors = colorBlock;
                CheckIfDoubleNumber(index + 1);
                return;
            }
        }

        colorBlock.normalColor = _normalCellColor;
        button.colors = colorBlock;
        CheckIfDoubleNumber(index + 1);
    }

    private static List<int> GetConnectedIndexes(int index)
    {
        int row = Mathf.FloorToInt(index / 9f);
        int column = index % 9;
        int box = 3 * Mathf.FloorToInt(row / 3f) + Mathf.FloorToInt(column / 3f);
        List<int> indexes = new();
        for (int i = 0; i < 9; i++)
            indexes.Add(9 * row + i);
        for (int i = 0; i < 9; i++)
            indexes.Add(9 * i + column);
        for (int i = 0; i < 9; i++)
        {
            int _ = (Mathf.FloorToInt(box / 3f) * 3 + Mathf.FloorToInt(i / 3f)) * 9;
            int _2 = box % 3 * 3 + i % 3;
            indexes.Add(_ + _2);
        }

        return indexes.RemoveDuplicates();
    }

    public static string StringifyInputNotesArray(int[][] inputNotesArray)
    {
        StringBuilder str = new();
        for (int i = 0; i < inputNotesArray.Length; i++)
        {
            if (i != 0) str.Append(",");
            str.Append(SaveScript.StringifyArray(inputNotesArray[i]));
        }

        return str.ToString();
    }
}