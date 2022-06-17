using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SolitaireSettings : BaseSceneSettings
{
    [Header("Other settings")]
    [SerializeField] private Slider spaceBetweenCardsFactor;
    [SerializeField] private TMP_Dropdown cardsSpriteTypeDropdown;
    [SerializeField] private List<Sprite> cardsDetailedSprites;
    [SerializeField] private List<Sprite> cardsSimpleSprites;
    [SerializeField] private List<SpriteRenderer> cardsSpriteRenderers;

    protected override void SetSettingStartValues()
    {
        float value = saveScript.floatDict["spaceBetweenCardsFactor"];
        if (value == 0f) value = 1f;
        spaceBetweenCardsFactor.value = Mathf.Clamp(value, spaceBetweenCardsFactor.minValue, spaceBetweenCardsFactor.maxValue);
        cardsSpriteTypeDropdown.value = saveScript.intDict["solitaireCardsSpriteType"];
    }

    public void ChangeSpaceBetweenCardsFactor()
    {
        saveScript.floatDict["spaceBetweenCardsFactor"] = spaceBetweenCardsFactor.value;
    }

    public void ChangeCardsSpriteType()
    {
        saveScript.intDict["solitaireCardsSpriteType"] = cardsSpriteTypeDropdown.value;
        List<Sprite> sprites = cardsSpriteTypeDropdown.value == 0 ? cardsDetailedSprites : cardsSimpleSprites;
        for (int i = 0; i < cardsSpriteRenderers.Count; i++)
        {
            cardsSpriteRenderers[i].sprite = sprites[i];
        }
    }
}