using UnityEngine;

public class SolitaireLayout : BaseLayout
{
    [Header("Other scene specific")] [SerializeField]
    private SolitaireGameHandler solitaireGameHandler;

    [SerializeField] private RectTransform solitaire;
    [SerializeField] private RectTransform finishGameButtonRect;
    [SerializeField] private RectTransform clockTextRect;

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f,
            Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        helpUIOpenButtonRect.sizeDelta = Vector2.one * size;
        helpUIOpenButtonRect.anchoredPosition =
            new Vector2(-screenSafeAreaXRight - (size * 0.6f), -screenSafeAreaYUp - (size * 0.6f));
        settingsUIOpenButtonRect.sizeDelta = Vector2.one * size;
        settingsUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - size,
            -screenSafeAreaYUp - (size * 0.6f));

        float solitaireScaleWidth = screenSafeAreaWidth / 2040f;
        float solitaireScaleHeight = (screenSafeAreaHeight - size / 2f) / 2160f;
        float solitaireScale = Mathf.Min(solitaireScaleWidth, solitaireScaleHeight *
                                                              solitaireGameHandler.cardSizeFactor) * 0.95f;
        Vector3 localScale = new(solitaireScale, solitaireScale, 1);
        solitaire.localScale = localScale;
        float lowerByButtons = solitaireScale * 2040f < screenSafeAreaWidth - size * 4 ? 0 : 1.1f * size;
        solitaire.anchoredPosition =
            new Vector2(screenSafeAreaCenterX, -screenSafeAreaYUp - size * 0.1f - lowerByButtons);

        SetLayoutOtherObjects();
    }

    private void SetLayoutOtherObjects()
    {
        float finishGameButtonYPos =
            screenSafeAreaCenterY - (screenSafeAreaHeight / 2f) + (screenSafeAreaHeight * 0.10f);
        float finishGameButtonWidth = Mathf.Min(screenSafeAreaHeight * 0.6f, screenSafeAreaWidth * 0.95f);
        finishGameButtonRect.sizeDelta = new Vector2(finishGameButtonWidth, finishGameButtonWidth / 4f);
        finishGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, finishGameButtonYPos);
        Vector2 clockSize = 0.2f * Mathf.Min(screenWidth / 240f, screenHeight / 160f) * new Vector2(320, 80);
        clockTextRect.sizeDelta = clockSize;
        clockTextRect.anchoredPosition =
            new Vector2(screenSafeAreaX + (clockSize.x * 0.625f), screenSafeAreaY + clockSize.y);
    }

    protected override void SetLayoutFinishedGameUI()
    {
        finishedGameUITitleRect.anchoredPosition = new Vector2(screenSafeAreaCenterX,
            screenSafeAreaCenterY + (screenSafeAreaHeight * (25f / 30f)) - (screenHeight / 2f));
        finishedGameUITitleRect.sizeDelta =
            new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (10f / 30f));
        float smallestSide = Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth);
        float biggestSide = Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth);
        if (smallestSide - 1440 > 0)
        {
            float factor = Mathf.Min(smallestSide / 1500f, biggestSide / 2500f);
            finishedGameUITitleRect.localScale = Vector2.one * factor;
            finishedGameUITitleRect.sizeDelta /= factor;
        }

        finishedGameUITextRect.anchoredPosition = new Vector2(screenSafeAreaCenterX,
            screenSafeAreaCenterY + (screenSafeAreaHeight * (17f / 30f)) - (screenHeight / 2f));
        finishedGameUITextRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (8f / 30f));
        if (finishedGameUIRewardRect != null)
        {
            finishedGameUIRewardRect.anchoredPosition = new Vector2(screenSafeAreaCenterX,
                screenSafeAreaCenterY + (screenSafeAreaHeight * (10f / 30f)) - (screenHeight / 2f));
            if (Application.internetReachability == NetworkReachability.NotReachable ||
                !finishedGameUIRewardDoubleButtonObj.activeInHierarchy)
            {
                finishedGameUIRewardDoubleButtonObj.SetActive(false);
                float scaleFactor = Mathf.Min(screenSafeAreaWidth * 0.85f / 500,
                    screenSafeAreaHeight * (5f / 30f) / 175);
                finishedGameUIRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                finishedGameUIRewardRect.sizeDelta = new Vector2(500, 175);
            }
            else
            {
                float scaleFactor = Mathf.Min(screenSafeAreaWidth * 0.85f / 1000,
                    screenSafeAreaHeight * (5f / 30f) / 175);
                finishedGameUIRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                finishedGameUIRewardRect.sizeDelta = new Vector2(1000, 175);
            }
        }

        Vector2 size = new(screenSafeAreaWidth * 0.45f, screenSafeAreaHeight * (5f / 30f));
        float posY = screenSafeAreaCenterY + screenSafeAreaYUp + (screenSafeAreaHeight * (3.5f / 30f)) -
                     (screenHeight / 2f);
        if (finishedGameUINewMoreDifficultGameButtonRect == null)
        {
            finishedGameUINewGameButtonRect.anchoredPosition =
                new Vector2(screenSafeAreaCenterX - (screenSafeAreaWidth / 4f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = size;
            finishedGameUIToMenuButtonRect.anchoredPosition =
                new Vector2(screenSafeAreaCenterX + (screenSafeAreaWidth / 4f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = size;
        }
        else
        {
            size.x = screenSafeAreaWidth * 0.3f;
            finishedGameUINewGameButtonRect.anchoredPosition =
                new Vector2(screenSafeAreaCenterX - (screenSafeAreaWidth / 3f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = size;
            finishedGameUINewMoreDifficultGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, posY);
            finishedGameUINewMoreDifficultGameButtonRect.sizeDelta = size;
            finishedGameUIToMenuButtonRect.anchoredPosition =
                new Vector2(screenSafeAreaCenterX + (screenSafeAreaWidth / 3f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = size;
        }
    }
}