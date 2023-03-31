using UnityEngine;

public class SettingsLayout : BaseLayout
{
    public override void SetLayout()
    {
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f,
            Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        backToMenuButtonRect.sizeDelta = Vector2.one * size;
        backToMenuButtonRect.anchoredPosition =
            new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f) + screenSafeAreaCenterX,
                (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
        Vector3 scrollDownScale = new(screenSafeAreaWidth * 0.98f / 2250f, screenSafeAreaHeight * 0.85f / 950f, 1);
        settingsUIScrolldownRect.localScale = scrollDownScale;
        float minScaleValue = Mathf.Min(scrollDownScale.x, scrollDownScale.y);
        Vector3 scrollDownContentScale = new(minScaleValue / scrollDownScale.x, minScaleValue / scrollDownScale.y, 1);
        settingsUIScrolldownContentRect.localScale = scrollDownContentScale;
        Vector3 scrollDownPosition = new(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * 0.15f / -2f), 0);
        settingsUIScrolldownRect.anchoredPosition = scrollDownPosition;
    }
}
