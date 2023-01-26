using System.Collections.Generic;
using UnityEngine;

public class SudokuLayout : BaseLayout
{
    [Header("Other scene specific")] [SerializeField]
    private GameObject sudoku;

    [SerializeField] private RectTransform onScreenNumpadRect;
    [SerializeField] private List<RectTransform> numpadPartsRect;
    [SerializeField] private RectTransform sudokuRect;

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        SetNumpadLayout();
    }

    protected override void SetLayoutFinishedGameUI()
    {
        RectTransform _ = finishedGameUINewMoreDifficultGameButtonRect;
        if (saveScript.intDict["SudokuDifficulty"] >= 3)
        {
            finishedGameUINewMoreDifficultGameButtonRect.gameObject.SetActive(false);
            finishedGameUINewMoreDifficultGameButtonRect = null;
        }

        base.SetLayoutFinishedGameUI();
        finishedGameUINewMoreDifficultGameButtonRect = _;
    }

    private void SetNumpadLayout()
    {
        float sudokuSize = Mathf.Min(Mathf.Min(screenHeightInUnits, screenWidthInUnits),
            Mathf.Max(screenHeightInUnits, screenWidthInUnits) * 0.70f);
        onScreenNumpadRect.anchoredPosition = new Vector2(0, 0);
        float numpadMove = sudokuSize / 18f * 76f;
        sudokuRect.anchoredPosition =
            screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown
                ? new Vector2(screenSafeAreaCenterXInUnits, (0.05f * numpadMove) + screenSafeAreaCenterYInUnits)
                : new Vector2(-(0.05f * numpadMove) + (sudokuSize / 50) + screenSafeAreaCenterXInUnits,
                    screenSafeAreaCenterYInUnits);
        foreach (var numpadPartRect in numpadPartsRect)
        {
            numpadPartRect.localScale = new Vector3(sudokuSize / 6f, sudokuSize / 6f, 1);
            float rowDiffY = 0;
            int numInRow = numpadPartRect.name.Length > 5 ? 10 : int.Parse(numpadPartRect.name);
            if (screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown)
            {
                if (numInRow > 5)
                {
                    numInRow -= 5;
                    rowDiffY = 0.5f * numpadMove;
                }

                numpadPartRect.anchoredPosition = new Vector2(-numpadMove + (0.5f * numpadMove * (numInRow - 1f)),
                    -numpadMove - (0.5f * numpadMove) - rowDiffY);
            }
            else
            {
                if (numInRow % 2 == 1)
                {
                    numInRow = Mathf.FloorToInt(numInRow / 2f) + 1;
                }
                else
                {
                    numInRow /= 2;
                    rowDiffY = 0.5f * numpadMove;
                }

                numpadPartRect.anchoredPosition = new Vector2(numpadMove + (0.5f * numpadMove) + rowDiffY,
                    numpadMove - (0.5f * numpadMove * (numInRow - 1f)));
            }
        }

        sudoku.transform.localScale = new Vector3(sudokuSize / 10f, sudokuSize / 10f, 1);
    }
}