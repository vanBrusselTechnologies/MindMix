using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SudokuSettings : BaseSceneSettings
{
    [Header("Other settings")]
    [SerializeField] Toggle doubleNumberWarningToggle;
    [SerializeField] Toggle autoEditNotesToggle;

    protected override void SetSettingStartValues()
    {
        autoEditNotesToggle.isOn = saveScript.intDict["SudokuEnabledAutoEditNotes"] == 1;
        doubleNumberWarningToggle.isOn = saveScript.intDict["SudokuEnabledDoubleNumberWarning"] == 1;
    }

    public void ChangeAutoEditNotes()
    {
        saveScript.intDict["SudokuEnabledAutoEditNotes"] = autoEditNotesToggle.isOn ? 1 : 0;
    }

    public void ChangeDoubleNumberWarning()
    {
        saveScript.intDict["SudokuEnabledDoubleNumberWarning"] = doubleNumberWarningToggle.isOn ? 1 : 0;
    }
}
