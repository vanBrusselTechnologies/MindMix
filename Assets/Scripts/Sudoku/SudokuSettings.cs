using UnityEngine;
using UnityEngine.UI;

public class SudokuSettings : BaseSceneSettings
{
    [Header("Other settings")]
    [SerializeField] Toggle doubleNumberWarningToggle;
    [SerializeField] Toggle autoEditNotesToggle;

    protected override void SetSettingStartValues()
    {
        autoEditNotesToggle.isOn = saveScript.IntDict["SudokuEnabledAutoEditNotes"] == 1;
        doubleNumberWarningToggle.isOn = saveScript.IntDict["SudokuEnabledDoubleNumberWarning"] == 1;
    }

    public void ChangeAutoEditNotes()
    {
        saveScript.IntDict["SudokuEnabledAutoEditNotes"] = autoEditNotesToggle.isOn ? 1 : 0;
    }

    public void ChangeDoubleNumberWarning()
    {
        saveScript.IntDict["SudokuEnabledDoubleNumberWarning"] = doubleNumberWarningToggle.isOn ? 1 : 0;
    }
}
