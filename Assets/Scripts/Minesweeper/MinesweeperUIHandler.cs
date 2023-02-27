using TMPro;
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
        difficultyDropdown.value = saveScript.IntDict["MinesweeperDifficulty"];
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
        saveScript.IntDict["MinesweeperDifficulty"] = chosenDiff;
        StartNewGame();
    }

    public override void OpenSettings()
    {
        bool settingObjActive = settingsCanvasObj.activeSelf;
        base.OpenSettings();
        if (!settingObjActive) return;
        if (saveScript.IntDict["MinesweeperAutoFlag"] == 1) mvGameHandler.AutoFlag();
    }
}