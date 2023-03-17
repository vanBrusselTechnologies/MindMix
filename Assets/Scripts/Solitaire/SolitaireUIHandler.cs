using UnityEngine;

public class SolitaireUIHandler : BaseUIHandler
{
    [Header("Other scene specific")] [SerializeField]
    private SolitaireGameHandler solitaireGameHandler;
    
    [SerializeField] private GameObject stockButton;
    [SerializeField] private RectTransform stock;
    [SerializeField] private RectTransform waistStack;

    public void OnClickStock()
    {
        int stockChildCount = stock.childCount;
        switch (stockChildCount)
        {
            case 0:
                return;
            case 1:
            {
                int waistStackChildCount = waistStack.childCount;
                for (int i = waistStackChildCount - 1; i >= 0; i--)
                {
                    var child = waistStack.GetChild(i);
                    child.SetParent(stock);
                    child.localPosition = Vector3.zero;
                    child.GetChild(0).SetSiblingIndex(1);
                }

                stockButton.transform.SetSiblingIndex(stock.childCount - 1);
                break;
            }
            default:
                if (stockChildCount == 2 && waistStack.childCount == 0) Destroy(stockButton);
                var card = stock.GetChild(stockChildCount - 2);
                card.SetParent(waistStack);
                card.localPosition = Vector3.zero;
                card.GetChild(0).SetSiblingIndex(1);
                break;
        }

        solitaireGameHandler.SaveProgress();
        baseLayout.SetLayout();
    }

    public void FinishSolitaire()
    {
        solitaireGameHandler.FinishGame();
    }

    public override void OpenHelpUI()
    {
        base.OpenHelpUI();
        solitaireGameHandler.solitaireInactive = helpUICanvasObj.activeSelf;
    }

    public override void OpenSettings()
    {
        base.OpenSettings();
        solitaireGameHandler.solitaireInactive = settingsCanvasObj.activeSelf;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ShowFinishedCanvas()
    {
        finishedGameUIObj.SetActive(true);
        gameSpecificRootObj.SetActive(false);
        generalCanvasObj.SetActive(false);
        menuCanvasObj.SetActive(false);
        helpUICanvasObj.SetActive(false);
        settingsCanvasObj.SetActive(false);
        baseLayout.SetLayout();
    }
}