using System;
using UnityEngine;

public class MenuLayout : BaseLayout
{
    [Header("Other scene specific")] [SerializeField]
    private GameObject menuCanvasObj;

    [SerializeField] private GameObject generalCanvasObj;

    [SerializeField] private RectTransform playGameButtonRect;
    [SerializeField] private RectTransform gameWheelRect;
    [SerializeField] private RectTransform gameModeWheelRect;
    [SerializeField] private RectTransform gameWheelColliderRect;
    [SerializeField] private RectTransform shopButtonRect;
    [SerializeField] private RectTransform supportButtonRect;

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

    public void SetLayoutGameWheels(RectTransform wheelRect)
    {
        bool isInnerWheel = String.Equals(wheelRect.name, gameWheelRect.name);
        float gameWheelSize = Mathf.Min(screenSafeAreaHeightInUnits / 450f, screenSafeAreaWidthInUnits / 250f);
        float wheelSizeFactor = isInnerWheel ? 1 : 0.75f;
        wheelRect.localScale = Vector3.one * (gameWheelSize * wheelSizeFactor);
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
        float angleStep = isInnerWheel ? 360f / gameCount : MenuUIHandler.GameModeWheelRotationDivPerChild;
        for (int i = 0; i < gameCount; i++)
        {
            float radius = Mathf.Min(screenWidthInUnits / 2f, screenHeightInUnits / 3.25f) / gameWheelSize;
            radius *= isInnerWheel ? 1 : 2.5f;
            float angle = i * angleStep;
            Transform game = wheelRect.GetChild(i % gameCount);
            RectTransform gameRect = (RectTransform)game.transform;
            Vector2 pos = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * radius;
            gameRect.anchoredPosition = pos;
            gameRect.localEulerAngles = new Vector3(0f, 0f, 360f - i * angleStep);

            RectTransform nameRect = (RectTransform)game.GetChild(1).transform;
            float nameRectFactorY = String.Equals(wheelRect.name, gameWheelRect.name) ? 1.125f : 0.275f;
            nameRect.anchoredPosition = new Vector2(0, radius * nameRectFactorY);
            nameRect.sizeDelta = new Vector2(screenWidth * 0.275f, screenHeight / 8f) * wheelSizeFactor;
        }
    }
}