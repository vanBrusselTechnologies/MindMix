using UnityEngine;
using UnityEngine.UI;

public class SudokuSettings : BaseSceneSettings
{
    [Header("Other settings")]
    [SerializeField] Toggle dubbelGetalWarningToggle;
    [SerializeField] Toggle notitieBijwerkToggle;

    protected override void SetSettingStartValues()
    {
        notitieBijwerkToggle.isOn = saveScript.intDict["notitieBijwerkSettingIsOn"] == 1;
        dubbelGetalWarningToggle.isOn = saveScript.intDict["dubbelGetalWarningIsOn"] == 1;
    }

    public void VeranderNotitieBijwerkSetting()
    {
        saveScript.intDict["notitieBijwerkSettingIsOn"] = notitieBijwerkToggle.isOn ? 1 : 0;
    }

    public void VeranderDubbelGetalWarningSetting()
    {
        saveScript.intDict["dubbelGetalWarningIsOn"] = dubbelGetalWarningToggle.isOn ? 1 : 0;
    }
}
