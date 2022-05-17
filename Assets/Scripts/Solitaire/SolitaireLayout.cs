using System.Collections.Generic;
using UnityEngine;
using VBG.Extensions;

public class SolitaireLayout : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] private Transform solitaire;
    [SerializeField] private RectTransform finishGameButtonRect;
    [SerializeField] private RectTransform clockTextRect;
    [SerializeField] private List<Transform> cardColumns = new List<Transform>();
    [SerializeField] private List<Transform> foundations = new List<Transform>();
    [SerializeField] private Transform stockPileTransform;
    [SerializeField] private Transform stockPileTurnButtonTransform;

    private SolitaireScript solitaireScript;
    private KnoppenScriptSolitaire solitaireUIHandler;

    // Start is called before the first frame update
    protected override void Start()
    {
        solitaireScript = GetComponent<SolitaireScript>();
        solitaireUIHandler = GetComponent<KnoppenScriptSolitaire>();
        base.Start();
    }

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        helpUIOpenButtonRect.sizeDelta = Vector2.one * size;
        helpUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f), -screenSafeAreaYUp - (size * 0.6f));
        settingsUIOpenButtonRect.sizeDelta = Vector2.one * size;
        settingsUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - size, -screenSafeAreaYUp - (size * 0.6f));
        SetLayoutOtherObjects();
        ScaleCards();
        PositionCards();
    }

    private void ScaleCards()
    {
        float _screenSafeAreaWidthInUnits = Mathf.Min(screenSafeAreaWidthInUnits, screenSafeAreaHeightInUnits * (8f / 4.5f));
        float _screenSafeAreaWidth = Mathf.Min(screenSafeAreaWidth / 81f / 10f, screenSafeAreaHeight / 81f / 10f * (8f / 4.5f));
        Vector3 cardsLocalScale = new Vector3(1f, 1f, 1f / (_screenSafeAreaWidthInUnits / 81f * 10f) / 0.18f) * _screenSafeAreaWidthInUnits / 81f * 10f * 0.18f;
        Vector3 stockLocalScale = new Vector3(1f, 1f, 1f / (_screenSafeAreaWidthInUnits / 81f * 10f)) * _screenSafeAreaWidthInUnits / 81f * 10f;
        for (int i = 0; i < solitaireScript.kaarten.Count; i++)
        {
            solitaireScript.kaarten[i].transform.localScale = cardsLocalScale;
            if (i < 7)
            {
                cardColumns[i].localScale = stockLocalScale;
                if (i < 4)
                {
                    foundations[i].localScale = stockLocalScale;
                }
            }
        }
        stockPileTransform.localScale = stockLocalScale;
        stockPileTurnButtonTransform.localScale = new Vector3(1f, 1f, 1f / _screenSafeAreaWidth) * _screenSafeAreaWidth;
    }

    public void PositionCards()
    {
        List<float> xPositions = new List<float>();
        float _screenWidthInUnits = Mathf.Min(screenWidthInUnits, screenHeightInUnits * (8f / 4.5f));
        float _screenWidth = Mathf.Min(screenWidth, screenHeight * (8f / 4.5f));
        for (int i = 0; i < 7; i++)
        {
            xPositions.Add(screenSafeAreaCenterXInUnits + (_screenWidthInUnits / 81f * (-33f + (i * 11f))));
        }
        float stockPileBaseXUI = screenSafeAreaCenterX + (_screenWidth / 81f * 33f) + (screenWidth / 2f);
        float baseY = screenSafeAreaCenterYInUnits + (screenHeightInUnits * (-1f / 3f)) + (screenHeightInUnits / 35f / 1.5f * (25f / 1.5f));
        float baseYFoundation = screenSafeAreaCenterYInUnits + (screenHeightInUnits * (-1f / 3f)) + (screenHeightInUnits / 35f / 1.5f * (34f + (4f / 9f)));
        float stockPileBaseYUI = screenSafeAreaCenterY + (screenHeight * (-1f / 3f)) + (screenHeight / 35f / 1.5f * (34f + (4f / 9f))) + (screenHeight / 2f);
        float spaceBetweenCardsFactor = saveScript.floatDict["spaceBetweenCardsFactor"];
        if (spaceBetweenCardsFactor == 0) spaceBetweenCardsFactor = 1;
        float diffY = 0.3f * spaceBetweenCardsFactor;
        float baseZ = -2f;
        float diffZ = 0.1f;
        for(int i = 0; i < 7; i++)
        {
            cardColumns[i].position = new Vector3(xPositions[i], baseY, -1f);
            if (i < 4)
            {
                foundations[i].position = new Vector3(xPositions[i], baseYFoundation, -1f);
            }
        }
        stockPileTurnButtonTransform.position = new Vector3(stockPileBaseXUI, stockPileBaseYUI, -1f);
        List<List<GameObject>> CardsStocks = new List<List<GameObject>>();
        CardsStocks.AddRange(solitaireScript.Stapel1, solitaireScript.Stapel2, solitaireScript.Stapel3, solitaireScript.Stapel4, solitaireScript.Stapel5, solitaireScript.Stapel6, solitaireScript.Stapel7);
        for (int i = 0; i < CardsStocks.Count; i++)
        {
            for(int ii = 0; ii < CardsStocks[i].Count; ii++)
            {
                CardsStocks[i][ii].transform.position = new Vector3(xPositions[i], baseY - (ii * diffY), baseZ - (diffZ * ii));
            }
        }
        List<List<GameObject>> CardsFoundations = new List<List<GameObject>>();
        CardsFoundations.AddRange(solitaireScript.EindStapel1, solitaireScript.EindStapel2, solitaireScript.EindStapel3, solitaireScript.EindStapel4);
        for (int i = 0; i < CardsFoundations.Count; i++)
        {
            for (int ii = 0; ii < CardsFoundations[i].Count; ii++)
            {
                CardsFoundations[i][ii].transform.position = new Vector3(xPositions[i], baseYFoundation, baseZ - (diffZ * ii));
            }
        }
        if (solitaireUIHandler.OmgedraaideRest.Count != 0)
        {
            for (int i = 0; i < solitaireUIHandler.OmgedraaideRest.Count; i++)
            {
                solitaireUIHandler.OmgedraaideRest[i].transform.position = new Vector3(xPositions[5], baseYFoundation, baseZ - (diffZ * i));
            }
        }
        for (int i = 0; i < solitaireScript.StapelRest.Count; i++)
        {
            solitaireScript.StapelRest[i].transform.position = new Vector3(xPositions[6], baseYFoundation, baseZ - (diffZ * i));
        }
    }

    private void SetLayoutOtherObjects()
    {
        float finishGameButtonYPos = screenSafeAreaCenterY - (screenSafeAreaHeight / 2f) + (screenSafeAreaHeight * 0.10f);
        float finishGameButtonSize = Mathf.Min(screenSafeAreaHeight * 0.3f, screenSafeAreaWidth * 0.95f);
        finishGameButtonRect.sizeDelta = new Vector2(finishGameButtonSize * 2f, finishGameButtonSize / 2f);
        finishGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, finishGameButtonYPos);
        Vector2 clockSize = 0.2f * Mathf.Min(screenWidth / 240f, screenHeight / 160f) * new Vector2(320, 80);
        clockTextRect.sizeDelta = clockSize;
        clockTextRect.anchoredPosition = new Vector2(screenSafeAreaX + (clockSize.x * 0.625f), screenSafeAreaY + clockSize.y);
    }
}