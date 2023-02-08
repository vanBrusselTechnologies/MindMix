using UnityEngine;

public class MinesweeperLayout : BaseLayout
{
    [Header("Other scene specific")] 
    [SerializeField] private MinesweeperGameHandler mvScript;
    [SerializeField] private RectTransform finishedGameUIWinTextRect;

    [SerializeField] private RectTransform finishedGameUILossTextRect;
    [SerializeField] private RectTransform mvField;
    [SerializeField] private RectTransform flagOrShovelButtonRect;
    [SerializeField] private RectTransform minesToFindRect;

    public const int VerticalSideBoxCount = 22;
    public const int HorizontalSideBoxCount = 16;
    private const int LongSidePixelSize = 2300;
    private const int ShortSidePixelSize = 1675;

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        float cornerButtonSize = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f,
            Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        float cornerButtonYPos = -screenSafeAreaYUp - (cornerButtonSize * 0.6f);
        float cornerButtonXPos = -screenSafeAreaXRight - (cornerButtonSize * 0.6f);
        settingsUIOpenButtonRect.anchoredPosition = new Vector2(cornerButtonXPos - cornerButtonSize, cornerButtonYPos);
        SetMineSweeperFieldLayout();
    }

    protected override void SetLayoutFinishedGameUI()
    {
        bool gameOver = mvScript.gameOver;
        finishedGameUIRewardDoubleButtonObj.SetActive(!gameOver);
        finishedGameUIRewardRect.gameObject.SetActive(!gameOver);
        finishedGameUIWinTextRect.gameObject.SetActive(!gameOver);
        finishedGameUILossTextRect.gameObject.SetActive(gameOver);
        finishedGameUITextRect = gameOver ? finishedGameUILossTextRect : finishedGameUIWinTextRect;
        RectTransform _ = finishedGameUINewMoreDifficultGameButtonRect;
        if (gameOver || saveScript.intDict["MinesweeperDifficulty"] >= 3)
        {
            finishedGameUINewMoreDifficultGameButtonRect.gameObject.SetActive(false);
            finishedGameUINewMoreDifficultGameButtonRect = null;
        }

        base.SetLayoutFinishedGameUI();
        finishedGameUINewMoreDifficultGameButtonRect = _;
    }

    private void SetMineSweeperFieldLayout()
    {
        if (screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown)
        {
            flagOrShovelButtonRect.anchoredPosition =
                new Vector2(-screenSafeAreaXRight + ShortSidePixelSize / 6f, -LongSidePixelSize * 0.325f - screenSafeAreaYUp);
            minesToFindRect.anchoredPosition =
                new Vector2(-screenSafeAreaXRight - ShortSidePixelSize / 6f, -LongSidePixelSize * 0.325f - screenSafeAreaYUp);
            mvField.anchoredPosition =
                new Vector2(-screenSafeAreaXRight, LongSidePixelSize * 0.1125f - screenSafeAreaYUp);
        }
        else
        {
            flagOrShovelButtonRect.anchoredPosition =
                new Vector2(-LongSidePixelSize * 0.325f - screenSafeAreaXRight, -screenSafeAreaYUp - ShortSidePixelSize / 6f);
            minesToFindRect.anchoredPosition =
                new Vector2(-LongSidePixelSize * 0.325f - screenSafeAreaXRight, -screenSafeAreaYUp + ShortSidePixelSize / 6f);
            mvField.anchoredPosition =
                new Vector2(LongSidePixelSize * 0.1125f - screenSafeAreaXRight, -screenSafeAreaYUp);
        }

        float mvFieldScaleMax = Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth) / LongSidePixelSize;
        float mvFieldScaleMin = Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) / ShortSidePixelSize;
        float mvFieldScale = Mathf.Min(mvFieldScaleMax * 0.8f, mvFieldScaleMin) * 0.95f;
        Vector3 localScale = new(mvFieldScale, mvFieldScale, 1);
        mvField.localScale = localScale;
        flagOrShovelButtonRect.localScale = localScale;
        minesToFindRect.localScale = localScale;
    }
}