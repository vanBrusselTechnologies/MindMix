using System.Collections.Generic;
using UnityEngine;

public class SudokuLayout : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] private GameObject sudoku;
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
        if (saveScript.intDict["difficulty"] >= 3)
        {
            finishedGameUINewMoreDifficultGameButtonRect.gameObject.SetActive(false);
            finishedGameUINewMoreDifficultGameButtonRect = null;
        }
        base.SetLayoutFinishedGameUI();
        finishedGameUINewMoreDifficultGameButtonRect = _;
    }

    private void SetNumpadLayout()
    {
        float sudokuGrootte = Mathf.Min(Mathf.Min(screenHeightInUnits, screenWidthInUnits), Mathf.Max(screenHeightInUnits, screenWidthInUnits) * 0.70f);
        onScreenNumpadRect.anchoredPosition = new Vector2(0, 0);
        float _numpadMove = sudokuGrootte / 18f * 76f;
        if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
            sudokuRect.anchoredPosition = new Vector2(screenSafeAreaCenterXInUnits, (0.05f * _numpadMove) + screenSafeAreaCenterYInUnits);
        else
            sudokuRect.anchoredPosition = new Vector2(-(0.05f * _numpadMove) + (sudokuGrootte / 50) + screenSafeAreaCenterXInUnits, screenSafeAreaCenterYInUnits);
        for (int i = 0; i < numpadPartsRect.Count; i++)
        {
            RectTransform numpadPartRect = numpadPartsRect[i];
            numpadPartRect.localScale = new Vector3(sudokuGrootte / 6f, sudokuGrootte / 6f, 1);
            float rijLagerY = 0;
            int getalInRij = numpadPartRect.name.Length > 5 ? 10 : int.Parse(numpadPartRect.name);
            if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
            {
                if (getalInRij > 5)
                {
                    getalInRij -= 5;
                    rijLagerY = 0.5f * _numpadMove;
                }
                numpadPartRect.anchoredPosition = new Vector2(-_numpadMove + (0.5f * _numpadMove * (getalInRij - 1f)), -_numpadMove - (0.5f * _numpadMove) - rijLagerY);
            }
            else
            {
                if (getalInRij % 2 == 1)
                {
                    getalInRij = Mathf.FloorToInt(getalInRij / 2) + 1;
                }
                else
                {
                    getalInRij /= 2;
                    rijLagerY = 0.5f * _numpadMove;
                }
                numpadPartRect.anchoredPosition = new Vector2(_numpadMove + (0.5f * _numpadMove) + rijLagerY, _numpadMove - (0.5f * _numpadMove * (getalInRij - 1f)));
            }
        }
        sudoku.transform.localScale = new Vector3(sudokuGrootte / 10f, sudokuGrootte / 10f, 1);
    }
}