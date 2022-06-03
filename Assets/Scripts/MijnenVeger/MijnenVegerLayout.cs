using UnityEngine;

public class MijnenVegerLayout : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] private RectTransform finishedGameUIWinTextRect;
    [SerializeField] private RectTransform finishedGameUILossTextRect;
    [SerializeField] private Transform gridTransform;
    [SerializeField] private Transform mvField;
    [SerializeField] private GameObject line;
    [SerializeField] private RectTransform flagOrShovelButtonRect;
    [SerializeField] private Transform bombsToGoTransform;
    [SerializeField] private Transform minesweeperRootObjTransform;

    [HideInInspector] public int verticalSideBoxCount = 22;
    [HideInInspector] public int horizontalSideBoxCount = 16;
    [HideInInspector] public float mvButtonSize = 0.0003726f;

    private MijnenVegerScript mvScript;

    // Start is called before the first frame update
    protected override void Start()
    {
        mvScript = GetComponent<MijnenVegerScript>();
        base.Start();
    }

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        float cornerButtonSize = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        float cornerButtonYPos = -screenSafeAreaYUp - (cornerButtonSize * 0.6f);
        float cornerButtonXPos = -screenSafeAreaXRight - (cornerButtonSize * 0.6f);
        if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
            settingsUIOpenButtonRect.anchoredPosition = new Vector2(cornerButtonXPos - cornerButtonSize, cornerButtonYPos);
        else
            settingsUIOpenButtonRect.anchoredPosition = new Vector2(cornerButtonXPos, cornerButtonYPos - cornerButtonSize);
        SetSideBoxCount();
        if (gridTransform.childCount == 0) { CreateGridLines(); }
        SetLinesLayout();
        SetButtonsLayout();
        cornerButtonSize = 1.1f * Mathf.Min(Mathf.Max(screenSafeAreaWidthInUnits, screenSafeAreaHeightInUnits) / 12f, Mathf.Min(screenSafeAreaWidthInUnits, screenSafeAreaHeightInUnits) / 10f);
        SetMineSweeperFieldLayout(cornerButtonSize);
    }
    
    protected override void SetLayoutFinishedGameUI()
    {
        bool gameOver = mvScript.GameOver;
        finishedGameUIRewardDoubleButtonObj.SetActive(!gameOver);
        finishedGameUIRewardRect.gameObject.SetActive(!gameOver);
        finishedGameUIWinTextRect.gameObject.SetActive(!gameOver);
        finishedGameUILossTextRect.gameObject.SetActive(gameOver);
        finishedGameUITextRect = gameOver ? finishedGameUILossTextRect : finishedGameUIWinTextRect;
        RectTransform _ = finishedGameUINewMoreDifficultGameButtonRect;
        if (gameOver || saveScript.intDict["difficultyMijnenVeger"] >= 3)
        {
            finishedGameUINewMoreDifficultGameButtonRect.gameObject.SetActive(false);
            finishedGameUINewMoreDifficultGameButtonRect = null;
        }
        base.SetLayoutFinishedGameUI();
        finishedGameUINewMoreDifficultGameButtonRect = _;
    }

    private void SetSideBoxCount()
    {
        if (screenHeight > screenWidth)
        {
            horizontalSideBoxCount = 16;
            verticalSideBoxCount = 22;
        }
        else
        {
            horizontalSideBoxCount = 22;
            verticalSideBoxCount = 16;
        }
    }

    private void CreateGridLines()
    {
        for (int i = 1; i < Mathf.Max(verticalSideBoxCount, horizontalSideBoxCount); i++)
        {
            Instantiate(line, gridTransform);
            if(i < Mathf.Min(verticalSideBoxCount, horizontalSideBoxCount))
            {
                Instantiate(line, gridTransform);
            }
        }
    }

    private void SetLinesLayout()
    {
        Vector3 horzLineScale = new Vector3(1f, 1f / (verticalSideBoxCount - 3f) * 0.04f, 0.01f);
        Vector3 vertLineScale = new Vector3(1f / (horizontalSideBoxCount - 3f) * 0.04f, 1f, 0.01f);
        for (int i = 0; i < gridTransform.childCount; i++)
        {
            Transform line = gridTransform.GetChild(i);
            float iHorizontal = i + 1f;
            if (iHorizontal < horizontalSideBoxCount)
            {
                line.localPosition = new Vector3(-0.5f + (iHorizontal / horizontalSideBoxCount), 0, -0.02f);
                line.localScale = vertLineScale;
            }
            else
            {
                float iVertical = iHorizontal - horizontalSideBoxCount + 1f;
                line.localPosition = new Vector3(0, -0.5f + (iVertical / verticalSideBoxCount), -0.02f);
                line.localScale = horzLineScale;
            }
        }
    }

    public void SetButtonsLayout()
    {
        for (int i = 0; i < mvScript.buttons.Count; i++)
        {
            Transform button = mvScript.buttons[i].transform;
            int boxNumber = int.Parse(button.gameObject.name);
            if (boxNumber == -1) boxNumber = 0;
            int column = boxNumber % 100;
            int row = (boxNumber - column) / 100;
            float xScale = mvButtonSize / 2f;
            float yScale = mvButtonSize / 2f;
            float xPos, yPos;
            if (verticalSideBoxCount > horizontalSideBoxCount)
            {
                xPos = (mvButtonSize * 75f) + (10.5f * horizontalSideBoxCount * column * mvButtonSize);
                yPos = (mvButtonSize * 60f) + (verticalSideBoxCount * 5.55f * row * mvButtonSize);
                xScale *= (float)verticalSideBoxCount / horizontalSideBoxCount;
            }
            else
            {
                xPos = (mvButtonSize * 60f) + (horizontalSideBoxCount * 5.55f * (21f - row) * mvButtonSize);
                yPos = (mvButtonSize * 75f) + (10.5f * verticalSideBoxCount * column * mvButtonSize);
                yScale *= (float)horizontalSideBoxCount / verticalSideBoxCount;
            }
            button.localPosition = new Vector3(xPos, yPos, -0.45f);
            button.localScale = new Vector3(xScale, yScale, 1f);
        }
    }

    private void SetMineSweeperFieldLayout(float cornerButtonSize)
    {
        float longSideBoxCount = Mathf.Max(horizontalSideBoxCount, verticalSideBoxCount);
        float shortSideBoxCount = Mathf.Min(horizontalSideBoxCount, verticalSideBoxCount);
        float smallestSideInUnits = Mathf.Min(screenSafeAreaHeightInUnits, screenSafeAreaWidthInUnits);
        float biggestSideInUnits = Mathf.Max(screenSafeAreaHeightInUnits, screenSafeAreaWidthInUnits);
        float boxSize = Mathf.Min(smallestSideInUnits * .95f / shortSideBoxCount, ((biggestSideInUnits * 0.75f) - cornerButtonSize) / longSideBoxCount);
        float scaleHorizontal = boxSize * horizontalSideBoxCount;
        float scaleVertical = boxSize * verticalSideBoxCount;
        float scale = Mathf.Max(biggestSideInUnits * .05f, smallestSideInUnits * 0.175f);
        float mvFieldPosX, mvFieldPosY, xPosRootObj, yPosRootObj;
        if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            mvFieldPosX = screenSafeAreaCenterXInUnits;
            mvFieldPosY = screenSafeAreaCenterYInUnits + (screenSafeAreaHeightInUnits - scaleVertical) / 2f - cornerButtonSize;
            mvField.localPosition = new Vector3(mvFieldPosX, mvFieldPosY, -0.5f);
            float yPos = mvFieldPosY - (scaleVertical / 2f) - (scale / 2f * 1.5f);
            flagOrShovelButtonRect.localPosition = new Vector3(mvFieldPosX + (2f / 3f) * scale, yPos, -5);
            bombsToGoTransform.localPosition = new Vector3(mvFieldPosX - (2f / 3f) * scale, yPos, -5);
            xPosRootObj = 0;
            yPosRootObj = -(screenSafeAreaHeightInUnits - (mvFieldPosY + (scaleVertical * 0.5f) - (yPos - (scale * 0.5f)) + cornerButtonSize + (screenSafeAreaHeightInUnits * 0.04f))) / 2f;
        }
        else
        {
            mvFieldPosX = (screenSafeAreaWidthInUnits - scaleHorizontal) / 2f + screenSafeAreaCenterXInUnits - cornerButtonSize;
            mvFieldPosY = screenSafeAreaCenterYInUnits;
            mvField.localPosition = new Vector3(mvFieldPosX, mvFieldPosY, -0.5f);
            float xPos = mvFieldPosX - (scaleHorizontal / 2f) - (scale / 2f * 1.5f);
            flagOrShovelButtonRect.localPosition = new Vector3(xPos, mvFieldPosY + (2f / 3f) * scale, -5);
            bombsToGoTransform.localPosition = new Vector3(xPos, mvFieldPosY - (2f / 3f) * scale, -5);
            xPosRootObj = -(screenSafeAreaWidthInUnits - (mvFieldPosX + (scaleHorizontal * 0.5f) - (xPos - (scale * 0.5f)) + cornerButtonSize + (screenSafeAreaWidthInUnits * 0.04f))) / 2f;
            yPosRootObj = 0;
        }
        mvField.localScale = new Vector3(scaleHorizontal, scaleVertical, 1);
        Vector3 lScale = new Vector3(scale, scale, 1);
        flagOrShovelButtonRect.localScale = lScale;
        bombsToGoTransform.localScale = lScale;
        float halfHorizontalSizeGrid = scaleHorizontal / 2f;
        float halfVerticalSizeGrid = scaleVertical / 2f;
        minesweeperRootObjTransform.position = new Vector3(xPosRootObj, yPosRootObj, 0);
        mvScript.mvSpeelveldLinks = mvFieldPosX + xPosRootObj - halfHorizontalSizeGrid;
        mvScript.mvSpeelveldRechts = mvFieldPosX + xPosRootObj + halfHorizontalSizeGrid;
        mvScript.mvSpeelveldBoven = mvFieldPosY + yPosRootObj + halfVerticalSizeGrid;
        mvScript.mvSpeelveldOnder = mvFieldPosY + yPosRootObj - halfVerticalSizeGrid;
    }
}
