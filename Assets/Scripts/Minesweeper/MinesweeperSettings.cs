using UnityEngine;
using UnityEngine.UI;

public class MinesweeperSettings : BaseSceneSettings
{
    [Header("Other settings")]
    [SerializeField] private Toggle autoFlagToggle;
    [SerializeField] private Toggle startAreaToggle;

    protected override void SetSettingStartValues()
    {
        autoFlagToggle.isOn = saveScript.IntDict["MinesweeperAutoFlag"] == 1;
        startAreaToggle.isOn = saveScript.IntDict["MinesweeperStartAreaSetting"] == 1;
    }

    public void ChangeAutoFlag()
    {
        saveScript.IntDict["MinesweeperAutoFlag"] = autoFlagToggle.isOn ? 1 : 0;
    }

    public void ChangeStartAreaSetting()
    {
        saveScript.IntDict["MinesweeperStartAreaSetting"] = startAreaToggle.isOn ? 1 : 0;
    }
}