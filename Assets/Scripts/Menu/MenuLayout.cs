using System;
using UnityEngine;

public class MenuLayout : BaseLayout
{
    private GegevensHouder _gegevensHouder;
    
    [Header("Other scene specific")] [SerializeField]
    private GameObject menuCanvasObj;

    [SerializeField] private GameObject generalCanvasObj;

    [SerializeField] private RectTransform playGameButtonRect;
    [SerializeField] private RectTransform gameWheelRect;
    [SerializeField] private RectTransform gameModeWheelRect;
    [SerializeField] private RectTransform gameWheelColliderRect;

    [SerializeField] private RectTransform shopButtonRect;
    [SerializeField] private RectTransform supportButtonRect;

    protected override void Start()
    {
        _gegevensHouder = GegevensHouder.Instance;
        if (_gegevensHouder == null) return;
        Transform gameModesParent = gameWheelRect.GetChild(_gegevensHouder.currentSelectedGameWheelIndex).GetChild(2);
        foreach (Transform gameMode in gameModesParent)
        {
            Instantiate(gameMode, gameModeWheelRect);
        }
        CreateClones();
        base.Start();
    }

    public override void SetLayout()
    {
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f,
            Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        backToMenuButtonRect.sizeDelta = Vector2.one * size;
        backToMenuButtonRect.anchoredPosition =
            new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f) + screenSafeAreaCenterX,
                (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
        settingsUIOpenButtonRect.anchoredPosition =
            new Vector2(-screenSafeAreaXRight - (size * 0.6f), -screenSafeAreaYUp - (size * 0.6f));
        settingsUIOpenButtonRect.sizeDelta = Vector2.one * size;
        shopButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - size,
            -screenSafeAreaYUp - (size * 0.6f));
        shopButtonRect.sizeDelta = Vector2.one * size;
        supportButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - (size * 2f),
            -screenSafeAreaYUp - (size * 0.6f));
        supportButtonRect.sizeDelta = Vector2.one * size;

        menuCanvasObj.SetActive(true);
        generalCanvasObj.SetActive(true);
        SetLayoutGameWheels(gameWheelRect);
        SetLayoutGameWheels(gameModeWheelRect);
    }

    /// <summary>
    /// Creates clones for filling up the gameWheel
    /// </summary>
    private void CreateClones()
    {
        int gameCount = gameWheelRect.childCount;
        for (int i = 0; i < gameCount; i++)
        {
            Transform game = gameWheelRect.GetChild(i % gameCount);
            Instantiate(game.gameObject, gameWheelRect);
        }
        /*
        for (int j = 0; j < 3; j++) //12 objects minimum for game-mode wheel. But every item only ones if childCount <= 3
        {
            Instantiate(game.gameObject, gameModeWheelRect);
        }
        */
    }

    private void SetLayoutGameWheels(RectTransform wheelRect)
    {
        bool isInnerWheel = String.Equals(wheelRect.name, gameWheelRect.name);
        float gameWheelSize = Mathf.Min(screenSafeAreaHeightInUnits / 450f, screenSafeAreaWidthInUnits / 250f);
        wheelRect.localScale = Vector3.one * (gameWheelSize * (isInnerWheel ? 1 : 0.75f));
        wheelRect.anchoredPosition = new Vector2(screenSafeAreaCenterXInUnits,
            screenSafeAreaCenterYInUnits - screenHeightInUnits / 2f);
        if (isInnerWheel)
        {
            gameWheelColliderRect.localScale = wheelRect.localScale * 365;
            gameWheelColliderRect.anchoredPosition = wheelRect.anchoredPosition;
            
            float playGameButtonSize = Mathf.Min(screenSafeAreaHeightInUnits / 300f, screenSafeAreaWidthInUnits / 250f);
            playGameButtonRect.localScale = Vector3.one * (playGameButtonSize / 3f);
            playGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterXInUnits,
                screenSafeAreaCenterYInUnits - screenHeightInUnits / 2f + screenSafeAreaHeightInUnits * 0.075f);
        }

        int gameCount = wheelRect.childCount;
        if (gameCount == 0) return;
        float angleStep = 360f / gameCount;
        for (int i = 0; i < gameCount; i++)
        {
            float radius = Mathf.Min(screenWidthInUnits / 2f, screenHeightInUnits / 3.25f) / gameWheelSize;
            radius *= isInnerWheel ? 1 : 2.5f;
            float angle = i * angleStep;
            Transform game = wheelRect.GetChild(i % gameCount);
            RectTransform gameRect = game.GetComponent<RectTransform>();
            Vector2 pos = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * radius;
            gameRect.anchoredPosition = pos;
            gameRect.localEulerAngles = new Vector3(0f, 0f, 360f - i * angleStep);

            RectTransform nameRect = game.GetChild(1).GetComponent<RectTransform>();
            float nameRectFactorY = String.Equals(wheelRect.name, gameWheelRect.name) ? 1.125f : -1.125f / 2.125f;
            nameRect.anchoredPosition = new Vector2(0, radius * nameRectFactorY);
            nameRect.sizeDelta = new Vector2(screenWidth * 0.275f, screenHeight / 8f);
        }
    }

    public void SetLayoutConfirmCanvas()
    {
        SetLayoutFinishedGameUI();
    }
}