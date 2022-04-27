using UnityEngine;
using UnityEngine.UI;

public class SolitaireSettings : BaseSceneSettings
{
    [Header("Other settings")]
    [SerializeField] Slider spaceBetweenCardsFactor;

    public override void SetSettingStartValues()
    {
        float value = saveScript.floatDict["spaceBetweenCardsFactor"];
        if (value == 0f) value = 1f;
        spaceBetweenCardsFactor.value = Mathf.Clamp(value, spaceBetweenCardsFactor.minValue, spaceBetweenCardsFactor.maxValue);
    }

    public void ChangeSpaceBetweenCardsFactor()
    {
        saveScript.floatDict["spaceBetweenCardsFactor"] = spaceBetweenCardsFactor.value;
    }
}
