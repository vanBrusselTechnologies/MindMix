using UnityEngine;

public class SupportLayout : BaseLayout
{
    [SerializeField] private RectTransform SupportNameRect;
    [SerializeField] private RectTransform SupportUitlegRect;
    [SerializeField] private RectTransform mailKnopRect;

    public override void SetLayout()
    {
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        Vector2 sizeDeltaSupportText = new(-screenSafeAreaWidth * 0.15f, screenSafeAreaHeight / 3f);
        backToMenuButtonRect.sizeDelta = Vector2.one * size;
        backToMenuButtonRect.anchoredPosition = new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f) + screenSafeAreaCenterX, (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
        SupportNameRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaYUp + screenSafeAreaHeight / 3f);
        SupportNameRect.sizeDelta = sizeDeltaSupportText;
        SupportUitlegRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaYUp);
        SupportUitlegRect.sizeDelta = sizeDeltaSupportText;
        mailKnopRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaYUp - screenSafeAreaHeight / 3f);
        mailKnopRect.sizeDelta = new Vector2(0, screenSafeAreaHeight / 3f);
    }
}
