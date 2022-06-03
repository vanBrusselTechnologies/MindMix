using UnityEngine;

public class SettingsLayout : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] private RectTransform bgInstellingScrolldown;
    [SerializeField] private RectTransform bgInstellingScrolldownContent;
    [SerializeField] private RectTransform overigeInstellingScrolldown;
    [SerializeField] private RectTransform overigeInstellingScrolldownContent;
    [SerializeField] private RectTransform instellingPaginaWisselKnoppenHouder;
    [SerializeField] private RectTransform naarAlgemeenSettingPaginaRect;

    public override void SetLayout()
    {
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        backToMenuButtonRect.sizeDelta = Vector2.one * size;
        backToMenuButtonRect.anchoredPosition = new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f) + screenSafeAreaCenterX, (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
        Vector3 scrollDownScale = new Vector3(screenSafeAreaWidth * 0.98f / 2250f, screenSafeAreaHeight * 0.85f / 950f, 1);
        bgInstellingScrolldown.localScale = scrollDownScale;
        overigeInstellingScrolldown.localScale = scrollDownScale;
        float minScaleDeel = Mathf.Min(scrollDownScale.x, scrollDownScale.y);
        Vector3 scrollDownContentScale = new Vector3(minScaleDeel / scrollDownScale.x, minScaleDeel / scrollDownScale.y, 1);
        bgInstellingScrolldownContent.localScale = scrollDownContentScale;
        overigeInstellingScrolldownContent.localScale = scrollDownContentScale;
        Vector3 scrollDownPosition = new Vector3(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * 0.15f / -2f), 0);
        bgInstellingScrolldown.anchoredPosition = scrollDownPosition;
        overigeInstellingScrolldown.anchoredPosition = scrollDownPosition;
        int aantalSettingPages = instellingPaginaWisselKnoppenHouder.childCount;
        float scaleKnoppenHouder1 = screenSafeAreaWidth * 0.5f * 0.8f / (naarAlgemeenSettingPaginaRect.sizeDelta.x * 0.5f * aantalSettingPages);
        float scaleKnoppenHouder2 = screenSafeAreaHeight * 0.5f * 0.2f / naarAlgemeenSettingPaginaRect.sizeDelta.y;
        float scaleKnoppenHouder = Mathf.Min(scaleKnoppenHouder1, scaleKnoppenHouder2);
        instellingPaginaWisselKnoppenHouder.localScale = new Vector3(scaleKnoppenHouder, scaleKnoppenHouder, 1);
        float yPosKnoppenHouder = scrollDownPosition.y + (scrollDownScale.y * bgInstellingScrolldown.sizeDelta.y / 2f) + (scaleKnoppenHouder * naarAlgemeenSettingPaginaRect.sizeDelta.y * 0.5f);
        float xPosKnoppenHouder = (screenSafeAreaWidth * (((0.95f + 0.2f) / 2) - 0.5f)) + screenSafeAreaCenterX;
        instellingPaginaWisselKnoppenHouder.anchoredPosition = new Vector3(xPosKnoppenHouder, yPosKnoppenHouder, 0);
    }
}
