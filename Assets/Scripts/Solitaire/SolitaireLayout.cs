using System.Collections.Generic;
using UnityEngine;
using VBG.Extensions;

public class SolitaireLayout : BaseLayout
{
    private SolitaireScript solitaireScript;
    private KnoppenScriptSolitaire solitaireUIHandler;

    [Header("Other scene specific")]
    [SerializeField] private Transform solitaire;
    [SerializeField] private RectTransform finishGameButtonRect;
    [SerializeField] private RectTransform clockTextRect;
    [SerializeField] private List<Transform> cardColumns = new List<Transform>();
    [SerializeField] private List<Transform> foundations = new List<Transform>();
    [SerializeField] private Transform stockPileTransform;
    [SerializeField] private Transform stockPileTurnButtonTransform;

    [HideInInspector] public float baseY;
    [HideInInspector] public float baseYFoundation;
    [HideInInspector] public List<float> xPositions = new List<float>();

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
        float cardYScale = solitaireScript.kaarten[2].transform.localScale.y * 8.5f;
        float spaceBetweenCardsFactor = saveScript.floatDict["spaceBetweenCardsFactor"];
        if (spaceBetweenCardsFactor == 0) spaceBetweenCardsFactor = 1;
        float diffY = 0.3f * spaceBetweenCardsFactor;
        xPositions.Clear();
        float _screenWidthInUnits = Mathf.Min(screenWidthInUnits, screenHeightInUnits * (8f / 4.5f));
        float _screenWidth = Mathf.Min(screenWidth, screenHeight * (8f / 4.5f));
        for (int i = 0; i < 7; i++)
        {
            xPositions.Add(screenSafeAreaCenterXInUnits + (_screenWidthInUnits / 81f * (-33f + (i * 11f))));
        }
        float stockPileBaseXUI = screenSafeAreaCenterX + (_screenWidth / 81f * 33f) + (screenWidth / 2f);
        baseYFoundation = Mathf.Min(screenSafeAreaCenterYInUnits + (13f * diffY + cardYScale) / 2f, screenSafeAreaCenterYInUnits + screenSafeAreaHeightInUnits / 2f - cardYScale / 2f);
        float stockPileBaseYUI = ScreenExt.UnitsToPixels(baseYFoundation) + (screenHeight / 2f);
        baseY = baseYFoundation - cardYScale - diffY;
        float baseZ = -2f;
        float diffZ = 0.1f;
        for (int i = 0; i < 7; i++)
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
            for (int ii = 0; ii < CardsStocks[i].Count; ii++)
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
        float finishGameButtonWidth = Mathf.Min(screenSafeAreaHeight * 0.6f, screenSafeAreaWidth * 0.95f);
        finishGameButtonRect.sizeDelta = new Vector2(finishGameButtonWidth, finishGameButtonWidth / 4f);
        finishGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, finishGameButtonYPos);
        Vector2 clockSize = 0.2f * Mathf.Min(screenWidth / 240f, screenHeight / 160f) * new Vector2(320, 80);
        clockTextRect.sizeDelta = clockSize;
        clockTextRect.anchoredPosition = new Vector2(screenSafeAreaX + (clockSize.x * 0.625f), screenSafeAreaY + clockSize.y);
    }

    protected override void SetLayoutFinishedGameUI()
    {
        finishedGameUITitleRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * (25f / 30f)) - (screenHeight / 2f));
        finishedGameUITitleRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (10f / 30f));
        float smallestSide = Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth);
        float biggestSide = Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth);
        if (smallestSide - 1440 > 0)
        {
            float factor = Mathf.Min(smallestSide / 1500f, biggestSide / 2500f);
            finishedGameUITitleRect.localScale = Vector2.one * factor;
            finishedGameUITitleRect.sizeDelta /= factor;
        }
        finishedGameUITextRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * (17f / 30f)) - (screenHeight / 2f));
        finishedGameUITextRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (8f / 30f));
        if (finishedGameUIRewardRect != null)
        {
            finishedGameUIRewardRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * (10f / 30f)) - (screenHeight / 2f));
            if (Application.internetReachability == NetworkReachability.NotReachable || !finishedGameUIRewardDoubleButtonObj.activeInHierarchy)
            {
                finishedGameUIRewardDoubleButtonObj.SetActive(false);
                float scaleFactor = Mathf.Min(screenSafeAreaWidth * 0.85f / 500, screenSafeAreaHeight * (5f / 30f) / 175);
                finishedGameUIRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                finishedGameUIRewardRect.sizeDelta = new Vector2(500, 175);
            }
            else
            {
                float scaleFactor = Mathf.Min(screenSafeAreaWidth * 0.85f / 1000, screenSafeAreaHeight * (5f / 30f) / 175);
                finishedGameUIRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                finishedGameUIRewardRect.sizeDelta = new Vector2(1000, 175);
            }
        }
        Vector2 size = new Vector2(screenSafeAreaWidth * 0.45f, screenSafeAreaHeight * (5f / 30f));
        float posY = screenSafeAreaCenterY + screenSafeAreaYUp + (screenSafeAreaHeight * (3.5f / 30f)) - (screenHeight / 2f);
        if (finishedGameUINewMoreDifficultGameButtonRect == null)
        {
            finishedGameUINewGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX - (screenSafeAreaWidth / 4f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = size;
            finishedGameUIToMenuButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX + (screenSafeAreaWidth / 4f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = size;
        }
        else
        {
            size.x = screenSafeAreaWidth * 0.3f;
            finishedGameUINewGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX - (screenSafeAreaWidth / 3f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = size;
            finishedGameUINewMoreDifficultGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, posY);
            finishedGameUINewMoreDifficultGameButtonRect.sizeDelta = size;
            finishedGameUIToMenuButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX + (screenSafeAreaWidth / 3f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = size;
        }
    }
}