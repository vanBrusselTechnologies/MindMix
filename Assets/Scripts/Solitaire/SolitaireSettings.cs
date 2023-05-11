using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolitaireSettings : BaseSceneSettings
{
    [Header("Other settings")] [SerializeField]
    private SolitaireGameHandler solitaireGameHandler;

    [SerializeField] private Slider cardSizeFactorSlider;
    [SerializeField] private Slider spaceBetweenCardsFactorSlider;
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

        value = Mathf.Clamp(value, spaceBetweenCardsFactorSlider.minValue, spaceBetweenCardsFactorSlider.maxValue);
        solitaireGameHandler.spaceBetweenCardsFactor = value;
        spaceBetweenCardsFactorSlider.value = value;
        
        value = saveScript.FloatDict["SolitaireCardSizeFactor"];
        if (value == 0f)
        {
            saveScript.FloatDict["SolitaireCardSizeFactor"] = 1f;
            value = 1f;
        }

        value = Mathf.Clamp(value, cardSizeFactorSlider.minValue, cardSizeFactorSlider.maxValue);
        solitaireGameHandler.cardSizeFactor = value;
        cardSizeFactorSlider.value = value;

        cardsSpriteTypeDropdown.value = saveScript.IntDict["SolitaireCardsSpriteType"];

        solitaireGameHandler.CorrectPositions();
    }

    public void ChangeSpaceBetweenCardsFactor()
    {
        saveScript.FloatDict["SolitaireSpaceBetweenCardsFactor"] = spaceBetweenCardsFactorSlider.value;
        solitaireGameHandler.spaceBetweenCardsFactor = spaceBetweenCardsFactorSlider.value;
        solitaireGameHandler.CorrectPositions();
    }

    public void ChangeCardSizeFactor()
    {
        saveScript.FloatDict["SolitaireCardSizeFactor"] = cardSizeFactorSlider.value;
        solitaireGameHandler.cardSizeFactor = cardSizeFactorSlider.value;
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