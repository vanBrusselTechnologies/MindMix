using UnityEngine;

public class Layout2048 : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] private Transform vak2048;
    [SerializeField] private RectTransform scoreRect;

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
        SetLayout2048();
    }

    protected void SetLayout2048()
    {
        float schaal2048 = Mathf.Min(Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth), Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth) * 0.75f) * 0.95f;
        schaal2048 = schaal2048 / screenHeight * 10f;
        vak2048.localScale = new Vector3(schaal2048, schaal2048, 1);
        if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            float vijfProcentSchermOmhoog = screenSafeAreaHeightInUnits * 0.05f;
            vak2048.localPosition = new Vector3(screenSafeAreaCenterXInUnits, screenSafeAreaCenterYInUnits + vijfProcentSchermOmhoog, -0.1f);
            scoreRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaHeight * 0.1f + screenSafeAreaY - screenHeight / 2f);
        }
        else
        {
            float vijfProcentSchermOmhoog = screenSafeAreaWidthInUnits * 0.05f;
            vak2048.localPosition = new Vector3(screenSafeAreaCenterXInUnits + vijfProcentSchermOmhoog, screenSafeAreaCenterYInUnits, -0.1f);
            scoreRect.anchoredPosition = new Vector2(screenSafeAreaWidth * 0.1f + screenSafeAreaX - screenWidth / 2f, screenSafeAreaCenterY);
        }
    }
}