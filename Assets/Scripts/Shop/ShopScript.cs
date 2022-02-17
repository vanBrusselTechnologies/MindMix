using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Localization;

public class ShopScript : MonoBehaviour
{
    private bool isPaused = false;
    private bool wasPaused = false;
    private int klaar = 3;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    private BeloningScript beloningScript;
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    private Achtergrond achtergrondScript;
    [SerializeField] private RectTransform infoEnKoopDeeltje;
    private int prijs;
    [HideInInspector] public string naam;
    [SerializeField] private RectTransform achtergrondKleurenScrolldown;
    [SerializeField] private RectTransform achtergrondKleurenScrolldownContent;
    [SerializeField] private RectTransform achtergrondAfbeeldingenScrolldown;
    [SerializeField] private RectTransform achtergrondAfbeeldingenScrolldownContent;
    [SerializeField] private RectTransform spelModiScrolldown;
    [SerializeField] private RectTransform spelModiScrolldownContent;
    [SerializeField] private GameObject textButtonPrefab;
    [SerializeField] private GameObject imageButtonPrefab;
    [SerializeField] private RectTransform terugNaarMenuKnop;
    [SerializeField] private RectTransform shopPaginaWisselKnoppenHouder;
    [SerializeField] private RectTransform naarSpelModiKnopRect;
    private List<RectTransform> achtergrondKleurItems = new List<RectTransform>();
    private List<RectTransform> achtergrondAfbeeldingItems = new List<RectTransform>();
    [SerializeField] private TMP_Text infoDeelItemNaam;
    [SerializeField] private Image infoDeelItemAfbeelding;
    [SerializeField] private Image infoDeelItemAfbeeldingMidden;
    [SerializeField] private TMP_Text infoDeelItemPrijs;
    [SerializeField] private GameObject testKnop;
    private bool wisselKleurVoorbeeld = false;
    [SerializeField] private LocalizedString wisselKleurString;
    [SerializeField] private GameObject gekochtSchermObj;
    [SerializeField] private RectTransform gekochtSchermSafezoneRect;
    [SerializeField] private RectTransform gekochtSchermAfbeeldingRect;
    [SerializeField] private Image gekochtSchermAfbeeldingImg;
    [SerializeField] private RectTransform gekochtSchermNameRect;
    [SerializeField] private TMP_Text gekochtSchermNameText;
    [SerializeField] private RectTransform gekochtSchermGekochtRect;
    private GameObject bg;
    private Image bgImg;
    private RectTransform bgRect;

    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        beloningScript = gegevensHouder.GetComponent<BeloningScript>();
        achtergrondScript = gegevensHouder.GetComponent<Achtergrond>();
        ZetShopItems();
        SetLayout(true);
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
        bg = GameObject.Find("BackGround");
        bgImg = bg.GetComponent<Image>();
        bgRect = bg.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isPaused && wasPaused)
        {
            SetLayout();
        }
        wasPaused = isPaused;
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 3)
            {
                SetLayout();
            }
            return;
        }
        klaar = 0;
        SetLayout();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    private void FixedUpdate()
    {
        if (wisselKleurVoorbeeld)
        {
            WisselKleur();
        }
    }

    private void ZetShopItems()
    {
        int itemsPerRij = Mathf.Max(Mathf.FloorToInt(Screen.safeArea.width / 350), 4);
        int i = 0;
        if (saveScript.intDict["kleur-1gekocht"] == 0)
        {
            int rij = Mathf.FloorToInt(i / itemsPerRij);
            int kolom = i - (rij * itemsPerRij);
            GameObject button = Instantiate(textButtonPrefab, achtergrondKleurenScrolldownContent.transform, false);
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            float xSize = Screen.safeArea.width * 0.98f / (itemsPerRij + 1f);
            buttonRect.sizeDelta = new Vector2(xSize, xSize / 2f);
            buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f) / 2f) - (((xSize / (itemsPerRij + 1f)) + (xSize / 2f)) * rij));
            achtergrondKleurItems.Add(buttonRect);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = wisselKleurString.GetLocalizedString();
            button.name = "kleur-1";
            button.GetComponent<Button>().onClick.AddListener(() => Selecteer(button.transform));
            i += 1;
        }
        for (int ii = 0; ii < achtergrondScript.colorOptionData.Count; ii++)
        {
            if (saveScript.intDict["kleur" + ii + "gekocht"] == 0)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                GameObject button = Instantiate(textButtonPrefab, achtergrondKleurenScrolldownContent.transform, false);
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                float xSize = Screen.safeArea.width * 0.98f / (itemsPerRij + 1f);
                buttonRect.sizeDelta = new Vector2(xSize, xSize / 2f);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f) / 2f) - (((xSize / (itemsPerRij + 1f)) + (xSize / 2f)) * rij));
                achtergrondKleurItems.Add(buttonRect);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = achtergrondScript.colorOptionData[ii].text;
                button.name = "kleur" + ii;
                button.GetComponent<Button>().onClick.AddListener(() => Selecteer(button.transform));
                i += 1;
            }
        }
        itemsPerRij = Mathf.Max(Mathf.FloorToInt(Screen.safeArea.width / 450), 4);
        i = 0;
        for (int ii = 0; ii < achtergrondScript.imageOptionData.Count; ii++)
        {
            if (saveScript.intDict["afbeelding" + ii + "gekocht"] == 0)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                GameObject button = Instantiate(imageButtonPrefab, achtergrondAfbeeldingenScrolldownContent.transform, false);
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                float xSize = Screen.safeArea.width * 0.98f / (itemsPerRij + 1f);
                buttonRect.sizeDelta = new Vector2(xSize, xSize);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f)) - (((xSize / (itemsPerRij + 1f)) + xSize) * rij));
                achtergrondAfbeeldingItems.Add(buttonRect);
                Image buttonImg = button.GetComponent<Image>();
                buttonImg.sprite = achtergrondScript.imageOptionData[ii].image;
                button.name = "afbeelding" + ii;
                button.GetComponent<Button>().onClick.AddListener(() => Selecteer(button.transform));
                i += 1;
            }
        }
    }

    private void SetLayout(bool inStart = false)
    {
        if (!inStart)
        {
            int itemsPerRij = Mathf.Max(Mathf.FloorToInt(Screen.safeArea.width / 350), 4);
            for (int i = 0; i < achtergrondKleurItems.Count; i++)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                float xSize = Screen.safeArea.width * 0.98f / (itemsPerRij + 1f);
                RectTransform buttonRect = achtergrondKleurItems[i];
                buttonRect.sizeDelta = new Vector2(xSize, xSize / 2f);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f) / 2f) - (((xSize / (itemsPerRij + 1f)) + (xSize / 2f)) * rij));
            }
            itemsPerRij = Mathf.Max(Mathf.FloorToInt(Screen.safeArea.width / 450), 4);
            for (int i = 0; i < achtergrondAfbeeldingItems.Count; i++)
            {
                int rij = Mathf.FloorToInt(i / itemsPerRij);
                int kolom = i - (rij * itemsPerRij);
                float xSize = Screen.safeArea.width * 0.98f / (itemsPerRij + 1f);
                RectTransform buttonRect = achtergrondAfbeeldingItems[i];
                buttonRect.sizeDelta = new Vector2(xSize, xSize);
                buttonRect.anchoredPosition = new Vector2((xSize / (itemsPerRij + 1f)) + (((xSize / (itemsPerRij + 1f)) + xSize) * kolom), (-xSize / (itemsPerRij + 1f)) - (((xSize / (itemsPerRij + 1f)) + xSize) * rij));
            }
        }
        klaar += 1;
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        terugNaarMenuKnop.sizeDelta = Vector2.one * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f;
        terugNaarMenuKnop.anchoredPosition = new Vector2((-Screen.width / 2) + Screen.safeArea.x + (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        Vector3 scrollDownSize = new Vector2(Screen.safeArea.width * 0.98f, Screen.safeArea.height * 0.85f);
        achtergrondAfbeeldingenScrolldown.sizeDelta = scrollDownSize;
        achtergrondKleurenScrolldown.sizeDelta = scrollDownSize;
        spelModiScrolldown.sizeDelta = scrollDownSize;
        Vector3 scrollDownContentSize = scrollDownSize;
        achtergrondAfbeeldingenScrolldownContent.sizeDelta = scrollDownContentSize;
        if (achtergrondKleurItems.Count > 0)
        {
            achtergrondKleurenScrolldownContent.sizeDelta = new Vector2(0, Mathf.Max(scrollDownSize.y, 25f - achtergrondKleurItems[^1].anchoredPosition.y + achtergrondKleurItems[^1].sizeDelta.y));
        }
        if (achtergrondAfbeeldingItems.Count > 0)
        {
            achtergrondAfbeeldingenScrolldownContent.sizeDelta = new Vector2(0, Mathf.Max(scrollDownSize.y, 25f - achtergrondAfbeeldingItems[^1].anchoredPosition.y + achtergrondAfbeeldingItems[^1].sizeDelta.y));
        }
        spelModiScrolldownContent.sizeDelta = scrollDownContentSize;
        Vector3 scrollDownPosition = new Vector3(safeZoneAntiX, safeZoneAntiY + (scrollDownSize.y / 2f) - (Screen.safeArea.height / 2f), 0);
        achtergrondAfbeeldingenScrolldown.anchoredPosition = scrollDownPosition;
        achtergrondKleurenScrolldown.anchoredPosition = scrollDownPosition;
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
        SluitGekochtScherm();
        wisselKleurVoorbeeld = false;
    }

    public void Selecteer(Transform dezeKnop)
    {
        wisselKleurVoorbeeld = false;
        if (!infoEnKoopDeeltje.gameObject.activeInHierarchy)
        {
            infoEnKoopDeeltje.gameObject.SetActive(true);
            float scaleFactor = Screen.safeArea.width * 0.9f / 2000;
            infoEnKoopDeeltje.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
        naam = dezeKnop.name; 
        if (achtergrondKleurenScrolldown.gameObject.activeInHierarchy)
        {
            prijs = 100;
            infoDeelItemNaam.text = dezeKnop.GetComponentInChildren<TMP_Text>().text;
            int kleurIndex = int.Parse(naam.Split("kleur")[1]);
            if (kleurIndex == -1)
            {
                prijs = 450;
                infoDeelItemAfbeelding.color = Color.black;
                wisselKleurVoorbeeld = true;
            }
            else
            {
                infoDeelItemAfbeelding.color = achtergrondScript.kleuren[kleurIndex];
            }
            infoDeelItemAfbeeldingMidden.gameObject.SetActive(false);
            infoDeelItemAfbeelding.gameObject.SetActive(true);
            testKnop.SetActive(false);
            infoDeelItemAfbeelding.sprite = null;
        }
        else if(achtergrondAfbeeldingenScrolldown.gameObject.activeInHierarchy)
        {
            prijs = 2025;
            Sprite dezeKnopSprite = dezeKnop.GetComponentInChildren<Image>().sprite;
            infoDeelItemNaam.text = "";
            infoDeelItemAfbeeldingMidden.gameObject.SetActive(true);
            infoDeelItemAfbeelding.gameObject.SetActive(false);
            infoDeelItemAfbeeldingMidden.sprite = dezeKnopSprite;
            infoDeelItemAfbeeldingMidden.color = Color.white;
            testKnop.SetActive(true);
        }
        else if (spelModiScrolldown.gameObject.activeInHierarchy)
        {
            prijs = 2500;
        }
        infoDeelItemPrijs.text = prijs.ToString();
    }

    public void Koop()
    {
        if(saveScript.intDict["munten"] >= prijs)
        {
            beloningScript.GeefMuntenUit(prijs);
            infoEnKoopDeeltje.gameObject.SetActive(false);
            if (naam.StartsWith("kleur"))
            {
                int kleurIndex = int.Parse(naam.Split("kleur")[1]);
                achtergrondScript.KleurGekocht(kleurIndex);
                saveScript.intDict["kleur" + kleurIndex + "gekocht"] = 1;
                for (int i = 0; i < achtergrondKleurItems.Count; i++)
                {
                    GameObject button = achtergrondKleurItems[i].gameObject;
                    if (button.name == naam)
                    {
                        achtergrondKleurItems.RemoveAt(i);
                        Destroy(button);
                        SetLayout();
                        break;
                    }
                }
            }
            else if (naam.StartsWith("afbeelding"))
            {
                int afbeeldingIndex = int.Parse(naam.Split("afbeelding")[1]);
                achtergrondScript.AfbeeldingGekocht(afbeeldingIndex);
                saveScript.intDict["afbeelding" + afbeeldingIndex + "gekocht"] = 1;
                for (int i = 0; i < achtergrondAfbeeldingItems.Count; i++)
                {
                    GameObject button = achtergrondAfbeeldingItems[i].gameObject;
                    if (button.name == naam)
                    {
                        achtergrondAfbeeldingItems.RemoveAt(i);
                        Destroy(button);
                        SetLayout();
                        break;
                    }
                }
            }
        }

        OpenGekochtScherm();
    }

    public void TerugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void SluitInfoEnKoopDeeltje()
    {
        infoEnKoopDeeltje.gameObject.SetActive(false);
    }

    public void WisselShopPagina(int i)
    {
        infoEnKoopDeeltje.gameObject.SetActive(false);
        switch (i)
        {
            case 0:
                achtergrondKleurenScrolldown.parent.gameObject.SetActive(true);
                achtergrondAfbeeldingenScrolldown.parent.gameObject.SetActive(false);
                spelModiScrolldown.parent.gameObject.SetActive(false);
                break;
            case 1:
                achtergrondKleurenScrolldown.parent.gameObject.SetActive(false);
                achtergrondAfbeeldingenScrolldown.parent.gameObject.SetActive(true);
                spelModiScrolldown.parent.gameObject.SetActive(false);
                break;
            case 2:
                achtergrondKleurenScrolldown.parent.gameObject.SetActive(false);
                achtergrondAfbeeldingenScrolldown.parent.gameObject.SetActive(false);
                spelModiScrolldown.parent.gameObject.SetActive(true);
                break;
            default: break;
        }
    }

    public void TestAchtergrond()
    {
        bgImg.sprite = gegevensScript.achtergronden[int.Parse(naam.Split("afbeelding")[1])];
        bgImg.color = Color.white;
        Rect imgRect = bgImg.sprite.rect;
        float width = Screen.width;
        float height = Screen.height;
        if (imgRect.width / width < imgRect.height / height)
        {
            height = 1000 * width;
        }
        else
        {
            width = 1000 * height;
        }
        bgRect.sizeDelta = new Vector2(width, height);
        return;
    }

    private void WisselKleur()
    {
        Color oldColor = infoDeelItemAfbeelding.color;
        if (oldColor.Equals(Color.black))
        {
            float tmpR = saveScript.floatDict["color.r"];
            float tmpG = saveScript.floatDict["color.g"];
            float tmpB = saveScript.floatDict["color.b"];
            if (tmpR == 0 && tmpG == 0 && tmpB == 0)
            {
                oldColor = Color.red;
                oldColor.a = 1f;
            }
            else
            {
                oldColor = new Color(tmpR, tmpG, tmpB, 1f);
            }
        }
        Color nextColor = oldColor;
        if (Mathf.Approximately(nextColor.r, 1f) && !Mathf.Approximately(nextColor.g, 1f) && Mathf.Approximately(nextColor.b, 0f))
        {
            nextColor.g += 1f / 255f;
            nextColor.g = Mathf.Min(nextColor.g, 1f);
        }
        else if ((Mathf.Approximately(nextColor.r, 1f) && Mathf.Approximately(nextColor.g, 1f)) || (!Mathf.Approximately(nextColor.r, 0f) && Mathf.Approximately(nextColor.g, 1f)))
        {
            nextColor.r -= 1f / 255f;
            nextColor.r = Mathf.Max(nextColor.r, 0f);
        }
        else if (Mathf.Approximately(nextColor.r, 0f) && Mathf.Approximately(nextColor.g, 1f) && !Mathf.Approximately(nextColor.b, 1f))
        {
            nextColor.b += 1f / 255f;
            nextColor.b = Mathf.Min(nextColor.b, 1f);
        }
        else if ((Mathf.Approximately(nextColor.g, 1f) && Mathf.Approximately(nextColor.b, 1f)) || (!Mathf.Approximately(nextColor.g, 0f) && Mathf.Approximately(nextColor.b, 1f)))
        {
            nextColor.g -= 1f / 255f;
            nextColor.g = Mathf.Max(nextColor.g, 0f);
        }
        else if (Mathf.Approximately(nextColor.b, 1f) && !Mathf.Approximately(nextColor.r, 1f))
        {
            nextColor.r += 1f / 255f;
            nextColor.r = Mathf.Min(nextColor.r, 1f);
        }
        else if ((Mathf.Approximately(nextColor.b, 1f) && Mathf.Approximately(nextColor.r, 1f)) || (!Mathf.Approximately(nextColor.b, 0f) && Mathf.Approximately(nextColor.r, 1f)))
        {
            nextColor.b -= 1f / 255f;
            nextColor.b = Mathf.Max(nextColor.b, 0f);
        }
        infoDeelItemAfbeelding.color = nextColor;
        saveScript.floatDict["color.r"] = nextColor.r;
        saveScript.floatDict["color.g"] = nextColor.g;
        saveScript.floatDict["color.b"] = nextColor.b;
    }

    private void OpenGekochtScherm()
    {
        gekochtSchermObj.SetActive(true);
        gekochtSchermSafezoneRect.offsetMin = new Vector2(Screen.safeArea.x, Screen.safeArea.y);
        gekochtSchermSafezoneRect.offsetMax = -new Vector2(Screen.width - Screen.safeArea.width - Screen.safeArea.x, Screen.height - Screen.safeArea.height - Screen.safeArea.y);
        if (achtergrondKleurenScrolldown.gameObject.activeInHierarchy)
        {
            gekochtSchermAfbeeldingImg.sprite = null;
            gekochtSchermAfbeeldingImg.color = infoDeelItemAfbeelding.color;
            gekochtSchermAfbeeldingRect.sizeDelta = new Vector2(-Screen.safeArea.width / 10f, Screen.safeArea.height / 3f);
        }
        else if (achtergrondAfbeeldingenScrolldown.gameObject.activeInHierarchy)
        {
            gekochtSchermAfbeeldingImg.sprite = infoDeelItemAfbeeldingMidden.sprite;
            gekochtSchermAfbeeldingImg.color = Color.white;
            gekochtSchermAfbeeldingRect.sizeDelta = new Vector2(-Screen.safeArea.width / 10f, Screen.safeArea.height / 2f);
        }
        gekochtSchermNameText.text = infoDeelItemNaam.text;
        gekochtSchermNameRect.sizeDelta = new Vector2(-Screen.safeArea.width / 10f, Screen.safeArea.height / 4f);
        gekochtSchermGekochtRect.sizeDelta = new Vector2(-Screen.safeArea.width / 10f, Screen.safeArea.height / 4f);
        float totaleHoogte = Screen.safeArea.height * (1f / 3f + 0.5f);
        gekochtSchermAfbeeldingRect.anchoredPosition = new Vector2(0, totaleHoogte - Screen.safeArea.height / 2f - gekochtSchermAfbeeldingRect.sizeDelta.y / 2f + (Screen.height - totaleHoogte) / 2f);
        gekochtSchermNameRect.anchoredPosition = new Vector2(0, totaleHoogte - Screen.safeArea.height / 2f - gekochtSchermAfbeeldingRect.sizeDelta.y - gekochtSchermNameRect.sizeDelta.y / 2f + (Screen.height - totaleHoogte) / 2f);
        gekochtSchermGekochtRect.anchoredPosition = new Vector2(0, -Screen.safeArea.height / 2f + gekochtSchermGekochtRect.sizeDelta.y / 2f + (Screen.height - totaleHoogte) / 2f);
    }

    public void SluitGekochtScherm()
    {
        gekochtSchermObj.SetActive(false);
    }
}
