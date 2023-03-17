using UnityEngine;

public class Layout2048 : BaseLayout
{
    [Header("Other scene specific")] [SerializeField]
    private Transform scaler2048;

    [SerializeField] private RectTransform field2048Rect;
    [SerializeField] private RectTransform scoreRect;

    protected override void SetLayoutBasic()
    {
        base.SetLayoutBasic();
        SetLayout2048();
    }

    private void SetLayout2048()
    {
        float scale2048 = Mathf.Min(Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.95f,
            Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth) * 0.675f);
        field2048Rect.anchoredPosition =
            screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown
                ? new Vector2(screenSafeAreaCenterX, 100 + screenSafeAreaCenterY)
                : new Vector2(-200 + (scale2048 / 50) + screenSafeAreaCenterX, screenSafeAreaCenterY);
        scoreRect.anchoredPosition =
            screenOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown
                ? new Vector2(screenSafeAreaCenterX, -550 + screenSafeAreaCenterY)
                : new Vector2(500 + (scale2048 / 50) + screenSafeAreaCenterX, screenSafeAreaCenterY);

        scaler2048.localScale = new Vector3(scale2048 / 1000f, scale2048 / 1000f, 1);
    }
}