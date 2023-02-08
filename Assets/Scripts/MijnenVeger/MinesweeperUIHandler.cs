﻿using TMPro;
using UnityEngine;

public class MinesweeperUIHandler : BaseUIHandler
{
    [Header("Other scene specific")] 
    [SerializeField] private MinesweeperGameHandler mvGameHandler;
    [SerializeField] private GameObject spadeButtonObj;
    [SerializeField] private GameObject flagButtonObj;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (saveScript == null) return;
        difficultyDropdown.value = saveScript.intDict["MinesweeperDifficulty"];
    }

    public void ChangeInputType()
    {
        bool isFlagInputMode = !mvGameHandler.isFlagInputMode;
        mvGameHandler.isFlagInputMode = isFlagInputMode;
        flagButtonObj.SetActive(isFlagInputMode);
        spadeButtonObj.SetActive(!isFlagInputMode);
    }

    public void StartNewMinesweeper(bool moreDifficult)
    {
        int chosenDiff = difficultyDropdown.value;
        if (moreDifficult) chosenDiff += 1;
        saveScript.intDict["MinesweeperDifficulty"] = chosenDiff;
        StartNewGame();
    }
}