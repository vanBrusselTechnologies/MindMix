using UnityEngine;
using UnityEngine.UI;

public class MinesweeperLayout : BaseLayout
{
    [Header("Other scene specific")] [SerializeField]
    private MinesweeperGameHandler mvGameHandler;

    [SerializeField] private RectTransform finishedGameUIWinTextRect;

    [SerializeField] private RectTransform finishedGameUILossTextRect;
    [SerializeField] private RectTransform mvField;
    [SerializeField] private RectTransform flagOrShovelButtonRect;
    [SerializeField] private RectTransform minesToFindRect;
    [SerializeField] private GridLayoutGroup buttonsGridLayoutGroup;
    [SerializeField] private GridLayoutGroup horizontalLinesGridLayoutGroup;
    [SerializeField] private GridLayoutGroup verticalLinesGridLayoutGroup;

    public const int HorizontalSideBoxCount = 22;
    public const int VerticalSideBoxCount = 16;
    private const int LongSidePixelSize = 2300;
    private const int ShortSidePixelSize = 1675;

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        float cornerButtonSize = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f,
            Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        float cornerButtonYPos = -screenSafeAreaYUp - (cornerButtonSize * 0.6f);
        float cornerButtonXPos = -screenSafeAreaXRight - (cornerButtonSize * 0.6f);
        settingsUIOpenButtonRect.anchoredPosition =
            screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown
                ? new Vector2(cornerButtonXPos, cornerButtonYPos - cornerButtonSize)
                : new Vector2(cornerButtonXPos - cornerButtonSize, cornerButtonYPos);

        SetMinesweeperFieldLayout();
    }

    protected override void SetLayoutFinishedGameUI()
    {
        bool gameOver = mvGameHandler.gameOver;
        finishedGameUIRewardDoubleButtonObj.SetActive(!gameOver);
        finishedGameUIRewardRect.gameObject.SetActive(!gameOver);
        finishedGameUIWinTextRect.gameObject.SetActive(!gameOver);
        finishedGameUILossTextRect.gameObject.SetActive(gameOver);
        finishedGameUITextRect = gameOver ? finishedGameUILossTextRect : finishedGameUIWinTextRect;
        RectTransform _ = finishedGameUINewMoreDifficultGameButtonRect;
        if (gameOver || saveScript.IntDict["MinesweeperDifficulty"] >= 3)
        {
            finishedGameUINewMoreDifficultGameButtonRect.gameObject.SetActive(false);
            finishedGameUINewMoreDifficultGameButtonRect = null;
        }

        base.SetLayoutFinishedGameUI();
        finishedGameUINewMoreDifficultGameButtonRect = _;
    }

    private void SetMinesweeperFieldLayout()
    {
        if (screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown)
        {
            flagOrShovelButtonRect.anchoredPosition =
                new Vector2(-screenSafeAreaXRight + ShortSidePixelSize / 6f,
                    -LongSidePixelSize * 0.325f - screenSafeAreaYUp);
            minesToFindRect.anchoredPosition =
                new Vector2(-screenSafeAreaXRight - ShortSidePixelSize / 6f,
                    -LongSidePixelSize * 0.325f - screenSafeAreaYUp);
            mvField.anchoredPosition =
                new Vector2(-screenSafeAreaXRight, LongSidePixelSize * 0.1125f - screenSafeAreaYUp);
            mvField.sizeDelta = new Vector2(ShortSidePixelSize, LongSidePixelSize);
            buttonsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            buttonsGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
            buttonsGridLayoutGroup.startCorner = screenOrientation == ScreenOrientation.Portrait
                ? GridLayoutGroup.Corner.UpperRight
                : GridLayoutGroup.Corner.LowerLeft;
            verticalLinesGridLayoutGroup.cellSize = new Vector2(9, LongSidePixelSize);
            verticalLinesGridLayoutGroup.spacing = new Vector2(96, 0);
            verticalLinesGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            horizontalLinesGridLayoutGroup.cellSize = new Vector2(ShortSidePixelSize, 9);
            horizontalLinesGridLayoutGroup.spacing = new Vector2(0, 96);
            horizontalLinesGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        }
        else
        {
            flagOrShovelButtonRect.anchoredPosition =
                new Vector2(-LongSidePixelSize * 0.325f - screenSafeAreaXRight,
                    -screenSafeAreaYUp - ShortSidePixelSize / 6f);
            minesToFindRect.anchoredPosition =
                new Vector2(-LongSidePixelSize * 0.325f - screenSafeAreaXRight,
                    -screenSafeAreaYUp + ShortSidePixelSize / 6f);
            mvField.anchoredPosition =
                new Vector2(LongSidePixelSize * 0.1125f - screenSafeAreaXRight, -screenSafeAreaYUp);
            mvField.sizeDelta = new Vector2(LongSidePixelSize, ShortSidePixelSize);
            buttonsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            buttonsGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            buttonsGridLayoutGroup.startCorner = screenOrientation == ScreenOrientation.LandscapeLeft
                ? GridLayoutGroup.Corner.UpperLeft
                : GridLayoutGroup.Corner.LowerRight;
            verticalLinesGridLayoutGroup.cellSize = new Vector2(LongSidePixelSize, 9);
            verticalLinesGridLayoutGroup.spacing = new Vector2(0, 96);
            verticalLinesGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            horizontalLinesGridLayoutGroup.cellSize = new Vector2(9, ShortSidePixelSize);
            horizontalLinesGridLayoutGroup.spacing = new Vector2(96, 0);
            horizontalLinesGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
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