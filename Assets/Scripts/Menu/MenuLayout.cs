using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuLayout : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] private GameObject menuCanvasObj;
    [SerializeField] private GameObject generalCanvasObj;
    [SerializeField] private List<TMP_Text> playGameButtonsText;
    [SerializeField] private List<RectTransform> playGameButtonsRect;
    [SerializeField] private RectTransform shopButtonRect;
    [SerializeField] private RectTransform supportButtonRect;

    private List<RectTransform> buttonsSortedRect = new List<RectTransform>(0);

    // Start is called before the first frame update
    protected override void Start()
    {
        List<string> knopNamen = new List<string>();
        List<string> knopNamenNietSorted = new List<string>();
        for (int i = 0; i < playGameButtonsText.Count; i++)
        {
            string knopNaam = playGameButtonsText[i].text;
            knopNamen.Add(knopNaam);
            knopNamenNietSorted.Add(knopNaam);
        }
        knopNamen.Sort();
        for (int i = 0; i < knopNamen.Count; i++)
        {
            int index = knopNamenNietSorted.IndexOf(knopNamen[i]);
            buttonsSortedRect.Add(playGameButtonsRect[index]);
        }
        base.Start();
    }

    public override void SetLayout()
    {
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        backToMenuButtonRect.sizeDelta = Vector2.one * size;
        backToMenuButtonRect.anchoredPosition = new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f) + screenSafeAreaCenterX, (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
        settingsUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f), -screenSafeAreaYUp - (size * 0.6f));
        settingsUIOpenButtonRect.sizeDelta = Vector2.one * size;
        shopButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - size, -screenSafeAreaYUp - (size * 0.6f));
        shopButtonRect.sizeDelta = Vector2.one * size;
        supportButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - (size * 2f), -screenSafeAreaYUp - (size * 0.6f));
        supportButtonRect.sizeDelta = Vector2.one * size;

        menuCanvasObj.SetActive(true);
        generalCanvasObj.SetActive(true);
        finishedGameUIObj.SetActive(false);
        SetLayoutGameButtons();
    }

    private void SetLayoutGameButtons()
    {
        int count = buttonsSortedRect.Count;
        for (int i = 0; i < count; i++)
        {
            buttonsSortedRect[i].sizeDelta = new Vector2(screenSafeAreaWidth * 0.9f, screenSafeAreaHeight * 0.81f / count);
            float x = screenSafeAreaCenterX;
            float y = (screenSafeAreaHeight * 0.275f) - (screenSafeAreaHeight * 0.85f / count * i) + screenSafeAreaCenterY;
            buttonsSortedRect[i].anchoredPosition = new Vector2(x, y);

        }
    }

    public void SetLayoutConfirmCanvas()
    {
        SetLayoutFinishedGameUI();
    }
}
