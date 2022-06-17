using System.Collections.Generic;
using UnityEngine;

public class ShopLayout : BaseLayout
{
    private ShopScript shopScript;

    [Header("Other scene specific")]
    [SerializeField] private RectTransform infoEnKoopDeeltje;
    [SerializeField] private RectTransform bgColorScrolldown;
    [SerializeField] private RectTransform bgColorScrolldownContent;
    [SerializeField] private RectTransform bgImgScrolldown;
    [SerializeField] private RectTransform bgImgScrolldownContent;
    [SerializeField] private RectTransform spelModiScrolldown;
    [SerializeField] private RectTransform spelModiScrolldownContent;
    [SerializeField] private RectTransform shopPaginaWisselKnoppenHouder;
    [SerializeField] private RectTransform naarSpelModiKnopRect;
    [SerializeField] private RectTransform gekochtSchermSafezoneRect;
    [SerializeField] private RectTransform gekochtSchermAfbeeldingRect;
    [SerializeField] private RectTransform gekochtSchermNameRect;
    [SerializeField] private RectTransform gekochtSchermGekochtRect;

    protected override void Start()
    {
        shopScript = GetComponent<ShopScript>();
        base.Start();
    }

    public override void SetLayout()
    {
        SetLayoutShopItemButtons();
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        backToMenuButtonRect.sizeDelta = Vector2.one * size;
        backToMenuButtonRect.anchoredPosition = new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f) + screenSafeAreaCenterX, (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
        Vector3 scrollDownSize = new Vector2(screenSafeAreaWidth * 0.98f, screenSafeAreaHeight * 0.85f);
        bgImgScrolldown.sizeDelta = scrollDownSize;
        bgColorScrolldown.sizeDelta = scrollDownSize;
        spelModiScrolldown.sizeDelta = scrollDownSize;
        Vector3 scrollDownPosition = new Vector3(safeZoneAntiX, safeZoneAntiY + (scrollDownSize.y / 2f) - (Screen.safeArea.height / 2f), 0);
        bgImgScrolldown.anchoredPosition = scrollDownPosition;
        bgColorScrolldown.anchoredPosition = scrollDownPosition;
        spelModiScrolldown.anchoredPosition = scrollDownPosition;
        int aantalSettingPages = shopPaginaWisselKnoppenHouder.childCount;
        float scaleKnoppenHouder1 = Screen.safeArea.width * 0.5f * 0.8f / (naarSpelModiKnopRect.sizeDelta.x * 0.5f * aantalSettingPages);
        float scaleKnoppenHouder2 = Screen.safeArea.height * 0.5f * 0.2f / naarSpelModiKnopRect.sizeDelta.y;
        float scaleKnoppenHouder = Mathf.Min(scaleKnoppenHouder1, scaleKnoppenHouder2);
        shopPaginaWisselKnoppenHouder.localScale = new Vector3(scaleKnoppenHouder, scaleKnoppenHouder, 1);
        float yPosKnoppenHouder = scrollDownPosition.y + (scrollDownSize.y / 2f) + (scaleKnoppenHouder * naarSpelModiKnopRect.sizeDelta.y / 2f);
        float xPosKnoppenHouder = (Screen.safeArea.width * (((0.95f + 0.2f) / 2) - 0.5f)) + safeZoneAntiX;
        shopPaginaWisselKnoppenHouder.anchoredPosition = new Vector3(xPosKnoppenHouder, yPosKnoppenHouder, 0);
        infoEnKoopDeeltje.gameObject.SetActive(false);
        shopScript.SluitGekochtScherm();
        shopScript.wisselKleurVoorbeeld = false;
    }

    public void OpenGekochtScherm()
    {
        gekochtSchermSafezoneRect.offsetMin = new Vector2(screenSafeAreaX, screenSafeAreaY);
        gekochtSchermSafezoneRect.offsetMax = -new Vector2(screenSafeAreaXRight, screenSafeAreaYUp);
        if (bgColorScrolldown.gameObject.activeInHierarchy)
        {
            gekochtSchermAfbeeldingRect.sizeDelta = new Vector2(-screenSafeAreaWidth / 10f, screenSafeAreaHeight / 3f);
        }
        else if (bgImgScrolldown.gameObject.activeInHierarchy)
        {
            gekochtSchermAfbeeldingRect.sizeDelta = new Vector2(-screenSafeAreaWidth / 10f, screenSafeAreaHeight / 2f);
        }
        gekochtSchermNameRect.sizeDelta = new Vector2(-screenSafeAreaWidth / 10f, screenSafeAreaHeight / 4f);
        gekochtSchermGekochtRect.sizeDelta = new Vector2(-screenSafeAreaWidth / 10f, screenSafeAreaHeight / 4f);
        float totaleHoogte = screenSafeAreaHeight * (1f / 3f + 0.5f);
        gekochtSchermAfbeeldingRect.anchoredPosition = new Vector2(0, totaleHoogte - screenSafeAreaHeight / 2f - gekochtSchermAfbeeldingRect.sizeDelta.y / 2f + (screenHeight - totaleHoogte) / 2f);
        gekochtSchermNameRect.anchoredPosition = new Vector2(0, totaleHoogte - screenSafeAreaHeight / 2f - gekochtSchermAfbeeldingRect.sizeDelta.y - gekochtSchermNameRect.sizeDelta.y / 2f + (screenHeight - totaleHoogte) / 2f);
        gekochtSchermGekochtRect.anchoredPosition = new Vector2(0, -screenSafeAreaHeight / 2f + gekochtSchermGekochtRect.sizeDelta.y / 2f + (screenHeight - totaleHoogte) / 2f);
    }

    public void SetLayoutShopItemButtons()
    {
        Vector3 scrollDownSize = new Vector2(screenSafeAreaWidth * 0.98f, screenSafeAreaHeight * 0.85f);
        int itemsPerRij;
        if (shopScript.bgColorItems.Count != 0)
        {
            itemsPerRij = Mathf.Max(Mathf.FloorToInt(screenSafeAreaWidth / 350), 4);
            for (int i = 0; i < shopScript.bgColorItems.Count; i++)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                float xSize = screenSafeAreaWidth * 0.98f / (itemsPerRij + 1f);
                RectTransform buttonRect = shopScript.bgColorItems[i];
                buttonRect.sizeDelta = new Vector2(xSize, xSize / 2f);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f) / 2f) - (((xSize / (itemsPerRij + 1f)) + (xSize / 2f)) * rij));
            }
            bgColorScrolldownContent.sizeDelta = new Vector2(0, Mathf.Max(screenSafeAreaHeight * 0.85f, 25f - shopScript.bgColorItems[^1].anchoredPosition.y + shopScript.bgColorItems[^1].sizeDelta.y));
        }
        else
            bgColorScrolldownContent.sizeDelta = scrollDownSize;
        if (shopScript.bgImgItems.Count != 0)
        {
            itemsPerRij = Mathf.Max(Mathf.FloorToInt(screenSafeAreaWidth / 450), 4);
            for (int i = 0; i < shopScript.bgImgItems.Count; i++)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                float xSize = screenSafeAreaWidth * 0.98f / (itemsPerRij + 1f);
                RectTransform buttonRect = shopScript.bgImgItems[i];
                buttonRect.sizeDelta = new Vector2(xSize, xSize);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f)) - (((xSize / (itemsPerRij + 1f)) + xSize) * rij));
            }
            bgImgScrolldownContent.sizeDelta = new Vector2(0, Mathf.Max(screenSafeAreaHeight * 0.85f, 25f - shopScript.bgImgItems[^1].anchoredPosition.y + shopScript.bgImgItems[^1].sizeDelta.y));
        }
        else
            bgImgScrolldownContent.sizeDelta = scrollDownSize;
        if (shopScript.gameModiItems.Count != 0)
        {
            itemsPerRij = Mathf.Max(Mathf.FloorToInt(screenSafeAreaWidth / 450), 4);
            for (int i = 0; i < shopScript.gameModiItems.Count; i++)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                float xSize = screenSafeAreaWidth * 0.98f / (itemsPerRij + 1f);
                RectTransform buttonRect = shopScript.gameModiItems[i];
                buttonRect.sizeDelta = new Vector2(xSize, xSize);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f)) - (((xSize / (itemsPerRij + 1f)) + xSize) * rij));
            }
            bgImgScrolldownContent.sizeDelta = new Vector2(0, Mathf.Max(screenSafeAreaHeight * 0.85f, 25f - shopScript.gameModiItems[^1].anchoredPosition.y + shopScript.gameModiItems[^1].sizeDelta.y));
        }
        else
            spelModiScrolldownContent.sizeDelta = scrollDownSize;
    }

    public void SetPositionInfoDeeltje()
    {
        infoEnKoopDeeltje.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaY);
    }
}
