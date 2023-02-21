using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolitaireSettings : BaseSceneSettings
{
    [Header("Other settings")] [SerializeField]
    private SolitaireGameHandler solitaireGameHandler;

    [SerializeField] private Slider spaceBetweenCardsFactor;
    [SerializeField] private TMP_Dropdown cardsSpriteTypeDropdown;
    [SerializeField] private List<Sprite> cardsDetailedSprites;
    [SerializeField] private List<Sprite> cardsSimpleSprites;
    [SerializeField] private List<Image> cardsFrontImages;

    protected override void SetSettingStartValues()
    {
        float value = saveScript.FloatDict["SolitaireSpaceBetweenCardsFactor"];
        if (value == 0f)
        {
            saveScript.FloatDict["SolitaireSpaceBetweenCardsFactor"] = 1f;
            value = 1f;
        }

        value = Mathf.Clamp(value, spaceBetweenCardsFactor.minValue, spaceBetweenCardsFactor.maxValue);
        solitaireGameHandler.spaceBetweenCardsFactor = value;
        spaceBetweenCardsFactor.value = value;
        solitaireGameHandler.CorrectPositions();

        cardsSpriteTypeDropdown.value = saveScript.IntDict["SolitaireCardsSpriteType"];
    }

    public void ChangeSpaceBetweenCardsFactor()
    {
        saveScript.FloatDict["SolitaireSpaceBetweenCardsFactor"] = spaceBetweenCardsFactor.value;
        solitaireGameHandler.spaceBetweenCardsFactor = spaceBetweenCardsFactor.value;
        solitaireGameHandler.CorrectPositions();
    }

    public void ChangeCardsSpriteType()
    {
        saveScript.IntDict["SolitaireCardsSpriteType"] = cardsSpriteTypeDropdown.value;
        List<Sprite> sprites = cardsSpriteTypeDropdown.value == 0 ? cardsDetailedSprites : cardsSimpleSprites;
        for (int i = 0; i < cardsFrontImages.Count; i++)
        {
            cardsFrontImages[i].sprite = sprites[i];
        }
    }
}