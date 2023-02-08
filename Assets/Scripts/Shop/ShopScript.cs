using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    private RewardHandler _rewardHandler;
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    private Achtergrond achtergrondScript;
    private ShopLayout shopLayout;

    [SerializeField] private GameObject infoEnKoopDeelObj;
    [SerializeField] private GameObject bgColorObj;
    [SerializeField] private GameObject bgColorScrolldownObj;
    [SerializeField] private Transform bgColorScrolldownContentTransform;
    [SerializeField] private GameObject bgImgObj;
    [SerializeField] private GameObject bgImgScrolldownObj;
    [SerializeField] private Transform bgImgScrolldownContentTransform;
    [SerializeField] private GameObject spelModiObj;
    [SerializeField] private GameObject spelModiScrolldownObj;
    [SerializeField] private GameObject textButtonPrefab;
    [SerializeField] private GameObject imageButtonPrefab;
    [SerializeField] private TMP_Text infoDeelItemNaam;
    [SerializeField] private Image infoDeelItemAfbeelding;
    [SerializeField] private Image infoDeelItemAfbeeldingMidden;
    [SerializeField] private TMP_Text infoDeelItemPrijs;
    [SerializeField] private GameObject testKnop;
    [SerializeField] private LocalizedString wisselKleurString;
    [SerializeField] private GameObject gekochtSchermObj;
    [SerializeField] private Image gekochtSchermAfbeeldingImg;
    [SerializeField] private TMP_Text gekochtSchermNameText;

    [HideInInspector] public List<RectTransform> bgColorItems = new();
    [HideInInspector] public List<RectTransform> bgImgItems = new();
    [HideInInspector] public List<RectTransform> gameModiItems = new();

    private GameObject bg;
    private Image bgImg;
    private RectTransform bgRect;

    private int prijs;
    [HideInInspector] public string naam;
    [HideInInspector] public bool wisselKleurVoorbeeld;

    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("DataObject");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        gegevensScript = GegevensHouder.Instance;
        saveScript = SaveScript.Instance;
        _rewardHandler = RewardHandler.Instance;
        achtergrondScript = gegevensHouder.GetComponent<Achtergrond>();
        shopLayout = GetComponent<ShopLayout>();
        ZetShopItems();
        bg = GameObject.Find("BackGround");
        bgImg = bg.GetComponent<Image>();
        bgRect = bg.GetComponent<RectTransform>();
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
        for (int ii = -1; ii < achtergrondScript.colorOptionData.Count; ii++)
        {
            if (saveScript.intDict["kleur" + ii + "gekocht"] == 0)
            {
                GameObject button = Instantiate(textButtonPrefab, bgColorScrolldownContentTransform, false);
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                bgColorItems.Add(buttonRect);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = ii == -1 ? wisselKleurString.GetLocalizedString() : achtergrondScript.colorOptionData[ii].text;
                button.name = "kleur" + ii;
                button.GetComponent<Button>().onClick.AddListener(() => Selecteer(button.transform));
            }
        }
        for (int ii = 0; ii < achtergrondScript.imageOptionData.Count; ii++)
        {
            if (saveScript.intDict["afbeelding" + ii + "gekocht"] == 0)
            {
                GameObject button = Instantiate(imageButtonPrefab, bgImgScrolldownContentTransform, false);
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                bgImgItems.Add(buttonRect);
                Image buttonImg = button.GetComponent<Image>();
                buttonImg.sprite = achtergrondScript.imageOptionData[ii].image;
                button.name = "afbeelding" + ii;
                button.GetComponent<Button>().onClick.AddListener(() => Selecteer(button.transform));
            }
        }
        shopLayout.SetLayoutShopItemButtons();
    }

    public void Selecteer(Transform dezeKnop)
    {
        wisselKleurVoorbeeld = false;
        if (!infoEnKoopDeelObj.activeInHierarchy)
        {
            infoEnKoopDeelObj.SetActive(true);
            float scaleFactor = Screen.safeArea.width * 0.9f / 2000;
            infoEnKoopDeelObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
        naam = dezeKnop.name;
        if (bgColorScrolldownObj.activeInHierarchy)
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
                infoDeelItemAfbeelding.color = achtergrondScript.colorList[kleurIndex];
            }
            infoDeelItemAfbeeldingMidden.gameObject.SetActive(false);
            infoDeelItemAfbeelding.gameObject.SetActive(true);
            testKnop.SetActive(false);
            infoDeelItemAfbeelding.sprite = null;
        }
        else if (bgImgScrolldownObj.activeInHierarchy)
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
        else if (spelModiScrolldownObj.activeInHierarchy)
        {
            prijs = 4500;
        }
        infoDeelItemPrijs.text = prijs.ToString();
        shopLayout.SetPositionInfoDeeltje();
    }

    public void Koop()
    {
        if (saveScript.intDict["munten"] >= prijs)
        {
            _rewardHandler.SpendCoins(prijs);
            infoEnKoopDeelObj.SetActive(false);
            if (naam.StartsWith("kleur"))
            {
                int kleurIndex = int.Parse(naam.Split("kleur")[1]);
                achtergrondScript.ColorBought(kleurIndex);
                saveScript.intDict["kleur" + kleurIndex + "gekocht"] = 1;
                for (int i = 0; i < bgColorItems.Count; i++)
                {
                    GameObject button = bgColorItems[i].gameObject;
                    if (button.name == naam)
                    {
                        bgColorItems.RemoveAt(i);
                        Destroy(button);
                        shopLayout.SetLayout();
                        break;
                    }
                }
            }
            else if (naam.StartsWith("afbeelding"))
            {
                int afbeeldingIndex = int.Parse(naam.Split("afbeelding")[1]);
                achtergrondScript.ImageBought(afbeeldingIndex);
                saveScript.intDict["afbeelding" + afbeeldingIndex + "gekocht"] = 1;
                for (int i = 0; i < bgImgItems.Count; i++)
                {
                    GameObject button = bgImgItems[i].gameObject;
                    if (button.name == naam)
                    {
                        bgImgItems.RemoveAt(i);
                        Destroy(button);
                        shopLayout.SetLayout();
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
        infoEnKoopDeelObj.SetActive(false);
    }

    public void WisselShopPagina(int i)
    {
        infoEnKoopDeelObj.SetActive(false);
        switch (i)
        {
            case 0:
                bgColorObj.SetActive(true);
                bgImgObj.SetActive(false);
                spelModiObj.SetActive(false);
                break;
            case 1:
                bgColorObj.SetActive(false);
                bgImgObj.SetActive(true);
                spelModiObj.SetActive(false);
                break;
            case 2:
                bgColorObj.SetActive(false);
                bgImgObj.SetActive(false);
                spelModiObj.SetActive(true);
                break;
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

    public void OpenGekochtScherm()
    {
        gekochtSchermObj.SetActive(true);
        gekochtSchermNameText.text = infoDeelItemNaam.text;
        if (bgColorScrolldownObj.activeInHierarchy)
        {
            gekochtSchermAfbeeldingImg.sprite = null;
            gekochtSchermAfbeeldingImg.color = infoDeelItemAfbeelding.color;
        }
        else if (bgImgScrolldownObj.activeInHierarchy)
        {
            gekochtSchermAfbeeldingImg.sprite = infoDeelItemAfbeeldingMidden.sprite;
            gekochtSchermAfbeeldingImg.color = Color.white;
        }
        shopLayout.OpenGekochtScherm();
    }

    public void SluitGekochtScherm()
    {
        gekochtSchermObj.SetActive(false);
    }
}
