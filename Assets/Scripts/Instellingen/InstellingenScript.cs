using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Audio;
using UnityEngine.UI;

public class InstellingenScript : MonoBehaviour
{
    private GegevensHouder gegevensScript;
    [SerializeField] private TMP_Dropdown taalDropdown;
    private bool isPaused = false;
    private bool wasPaused = false;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private RectTransform terugNaarMenuKnop;
    [SerializeField] private GameObject bgInstelingen;
    [SerializeField] private RectTransform bgInstellingScrolldown;
    [SerializeField] private RectTransform bgInstellingScrolldownContent;
    [SerializeField] private GameObject overigeInstellingen;
    [SerializeField] private RectTransform overigeInstellingScrolldown;
    [SerializeField] private RectTransform overigeInstellingScrolldownContent;
    [SerializeField] private RectTransform instellingPaginaWisselKnoppenHouder;
    [SerializeField] private RectTransform naarAlgemeenSettingPaginaRect;
    private List<string> sceneNames = new List<string>(){ "Sudoku", "Solitaire", "2048", "Mijnenveger", "Menu" };
    private SaveScript saveScript;
    private Achtergrond achtergrondScript;
    private bool startValues = true;
    [SerializeField] private Slider muziekVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    bool volumeSetInStart = false;
    private AudioSource muziek;

    // Start is called before the first frame update
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
        achtergrondScript = gegevensHouder.GetComponent<Achtergrond>();
        SetTaalStartWaarde();
        SetBackgroundStartValues();
        startValues = false;
        SetLayout();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
        volumeSetInStart = true;
        muziekVolumeSlider.value = PlayerPrefs.GetFloat("achtergrondMuziekVolume", 0.25f);
        muziek = GameObject.Find("AchtergrondMuziek").GetComponent<AudioSource>();
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

    private void SetTaalStartWaarde()
    {
        List<string> tmpTalen = new List<string>();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
        {
            tmpTalen.Add(locale.LocaleName);
            options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
        }
        taalDropdown.options = options;
        for (int i = 0; i < tmpTalen.Count; i++)
        {
            if (saveScript.stringDict["taal"].Equals(tmpTalen[i]))
            {
                taalDropdown.value = i;
                break;
            }
        }
    }

    private void SetBackgroundStartValues()
    {
        Transform content = bgInstellingScrolldownContent.transform;
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            Transform colorDropdownObj = child.Find("BGColor");
            TMP_Dropdown colorDropDown = colorDropdownObj.GetComponent<TMP_Dropdown>();
            colorDropDown.options.Clear();
            colorDropDown.options.AddRange(achtergrondScript.gekochteColorOptionData);
            Transform imageDropdownObj = child.Find("BGImage");
            TMP_Dropdown imageDropDown = imageDropdownObj.GetComponent<TMP_Dropdown>();
            imageDropDown.options.Clear();
            imageDropDown.options.AddRange(achtergrondScript.gekochteImageOptionData);
            string sceneNaam = child.gameObject.name[2..];
            int soort = saveScript.intDict["bgSoort" + sceneNaam];
            child.Find("BGSoort").GetComponent<TMP_Dropdown>().value = soort;
            if (soort == 1)
            {
                imageDropdownObj.gameObject.SetActive(true);
                int bgWaarde = saveScript.intDict["bgWaarde" + sceneNaam];
                int dropdownValue = bgWaarde >= 0 ? achtergrondScript.gekochteImageOptionData.IndexOf(achtergrondScript.imageOptionData[bgWaarde]) : -1;
                imageDropDown.value = dropdownValue;
                colorDropdownObj.gameObject.SetActive(false);
                colorDropDown.value = 0;
            }
            else
            {
                int bgWaarde = saveScript.intDict["bgWaarde" + sceneNaam];
                int dropdownValue = bgWaarde >= 0 ? achtergrondScript.gekochteColorOptionData.IndexOf(achtergrondScript.colorOptionData[bgWaarde]) : -1;
                imageDropdownObj.gameObject.SetActive(false);
                imageDropDown.value = 0;
                colorDropdownObj.gameObject.SetActive(true);
                colorDropDown.value = dropdownValue;
            }
        }
    }

    private void SetLayout()
    {
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        klaar += 1;
        terugNaarMenuKnop.sizeDelta = Vector2.one * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f;
        terugNaarMenuKnop.anchoredPosition = new Vector2((-Screen.width / 2) + Screen.safeArea.x + (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        Vector3 scrollDownScale = new Vector3(Screen.safeArea.width * 0.98f / 2250f, Screen.safeArea.height * 0.85f / 950f, 1);
        bgInstellingScrolldown.localScale = scrollDownScale;
        overigeInstellingScrolldown.localScale = scrollDownScale;
        float minScaleDeel = Mathf.Min(scrollDownScale.x, scrollDownScale.y);
        Vector3 scrollDownContentScale = new Vector3(minScaleDeel / scrollDownScale.x, minScaleDeel / scrollDownScale.y, 1);
        bgInstellingScrolldownContent.localScale = scrollDownContentScale;
        overigeInstellingScrolldownContent.localScale = scrollDownContentScale;
        Vector3 scrollDownPosition = new Vector3(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * 0.15f / -2f), 0);
        bgInstellingScrolldown.anchoredPosition = scrollDownPosition;
        overigeInstellingScrolldown.anchoredPosition = scrollDownPosition;
        int aantalSettingPages = instellingPaginaWisselKnoppenHouder.childCount;
        float scaleKnoppenHouder1 = Screen.safeArea.width * 0.5f * 0.8f / (naarAlgemeenSettingPaginaRect.sizeDelta.x * 0.5f * aantalSettingPages);
        float scaleKnoppenHouder2 = Screen.safeArea.height * 0.5f * 0.2f / naarAlgemeenSettingPaginaRect.sizeDelta.y;
        float scaleKnoppenHouder = Mathf.Min(scaleKnoppenHouder1, scaleKnoppenHouder2);
        instellingPaginaWisselKnoppenHouder.localScale = new Vector3(scaleKnoppenHouder, scaleKnoppenHouder, 1);
        float yPosKnoppenHouder = scrollDownPosition.y + (scrollDownScale.y * bgInstellingScrolldown.sizeDelta.y / 2f) + (scaleKnoppenHouder * naarAlgemeenSettingPaginaRect.sizeDelta.y * 0.5f);
        float xPosKnoppenHouder = (Screen.safeArea.width * (((0.95f + 0.2f) / 2) - 0.5f)) + safeZoneAntiX;
        instellingPaginaWisselKnoppenHouder.anchoredPosition = new Vector3(xPosKnoppenHouder, yPosKnoppenHouder, 0);
    }

    public void TerugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void VeranderTaal()
    {
        string taal = taalDropdown.options[taalDropdown.value].text;
        int taalWaarde = -1;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if (taal.ToLower().Equals(LocalizationSettings.AvailableLocales.Locales[i].LocaleName.ToLower()))
            {
                taalWaarde = i;
                break;
            }
        }
        gegevensScript.VeranderTaal(taalWaarde);
    }

    public void VeranderAchtergrondImg(GameObject obj)
    {
        if (startValues) return;
        string sceneNaam = obj.transform.parent.name[2..];
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown.value == -1)
        {
            return;
        }
        if (sceneNaam.ToLower().Equals("all"))
        {
            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.imageOptionData.IndexOf(achtergrondScript.gekochteImageOptionData[dropdownValue]);
            saveScript.intDict["bgSoortAll"] = 1;
            saveScript.intDict["bgWaardeAll"] = bgWaarde;
            for (int i = 0; i < sceneNames.Count; i++)
            {
                saveScript.intDict["bgSoort" + sceneNames[i]] = 1;
                saveScript.intDict["bgWaarde" + sceneNames[i]] = bgWaarde;
                gegevensScript.VeranderOpgeslagenAchtergrond(sceneNames[i].ToLower(), 1, bgWaarde);
            }
            foreach (Transform andereDropdown in obj.transform.parent.parent)
            {
                andereDropdown.Find("BGSoort").GetComponent<TMP_Dropdown>().value = 1;
                andereDropdown.Find("BGImage").GetComponent<TMP_Dropdown>().value = dropdownValue;
            }
        }
        else
        {

            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.imageOptionData.IndexOf(achtergrondScript.gekochteImageOptionData[dropdownValue]);
            if (saveScript.intDict["bgWaardeAll"] != bgWaarde || saveScript.intDict["bgSoortAll"] != 1)
            {
                saveScript.intDict["bgWaardeAll"] = -2;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGImage").GetComponent<TMP_Dropdown>().value = -1;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGColor").GetComponent<TMP_Dropdown>().value = -1;
            }
            saveScript.intDict["bgSoort" + sceneNaam] = 1;
            saveScript.intDict["bgWaarde" + sceneNaam] = bgWaarde;
            gegevensScript.VeranderOpgeslagenAchtergrond(sceneNaam.ToLower(), 1, bgWaarde);
        }
    }

    public void VeranderAchtergrondKleur(GameObject obj)
    {
        if (startValues) return;
        string sceneNaam = obj.transform.parent.name[2..];
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if(dropdown.value == -1)
        {
            return;
        }
        if (sceneNaam.ToLower() == "all")
        {
            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.colorOptionData.IndexOf(achtergrondScript.gekochteColorOptionData[dropdownValue]);
            saveScript.intDict["bgSoortAll"] = 0;
            saveScript.intDict["bgWaardeAll"] = bgWaarde;
            for (int i = 0; i < sceneNames.Count; i++)
            {
                saveScript.intDict["bgSoort" + sceneNames[i]] = 0;
                saveScript.intDict["bgWaarde" + sceneNames[i]] = bgWaarde;
                gegevensScript.VeranderOpgeslagenAchtergrond(sceneNames[i].ToLower(), 0, bgWaarde);
            }
            foreach(Transform andereDropdown in obj.transform.parent.parent)
            {
                andereDropdown.Find("BGSoort").GetComponent<TMP_Dropdown>().value = 0;
                andereDropdown.Find("BGColor").GetComponent<TMP_Dropdown>().value = dropdownValue;
            }
        }
        else
        {
            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.colorOptionData.IndexOf(achtergrondScript.gekochteColorOptionData[dropdownValue]);
            if (saveScript.intDict["bgWaardeAll"] != bgWaarde || saveScript.intDict["bgSoortAll"] != 0)
            {
                saveScript.intDict["bgWaardeAll"] = -2;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGImage").GetComponent<TMP_Dropdown>().value = -1;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGColor").GetComponent<TMP_Dropdown>().value = -1;
            }
            saveScript.intDict["bgSoort" + sceneNaam] = 0;
            saveScript.intDict["bgWaarde" + sceneNaam] = bgWaarde;
            gegevensScript.VeranderOpgeslagenAchtergrond(sceneNaam.ToLower(), 0, bgWaarde);
        }
    }

    public void VeranderAchtergrondSoort(GameObject obj)
    {
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown.value == -1)
        {
            return;
        }
        if (dropdown.value == 1)
        {
            GameObject imageDropdown = obj.transform.parent.Find("BGImage").gameObject;
            imageDropdown.GetComponent<TMP_Dropdown>().value = -1;
            imageDropdown.SetActive(true);
            GameObject colorDropdown = obj.transform.parent.Find("BGColor").gameObject;
            colorDropdown.SetActive(false);
        }
        else
        {
            GameObject imageDropdown = obj.transform.parent.Find("BGImage").gameObject;
            imageDropdown.SetActive(false);
            GameObject colorDropdown = obj.transform.parent.Find("BGColor").gameObject;
            colorDropdown.GetComponent<TMP_Dropdown>().value = -1;
            colorDropdown.SetActive(true);
        }
    }

    public void WisselInstellingPagina(int paginaIndex)
    {
        if(paginaIndex == 0)
        {
            overigeInstellingen.SetActive(true);
            bgInstelingen.SetActive(false);
        }
        else if(paginaIndex == 1)
        {
            overigeInstellingen.SetActive(false);
            bgInstelingen.SetActive(true);
        }
    }

    public void VeranderGeluidsSterkte(float sterkte)
    {
        if (volumeSetInStart)
        {
            volumeSetInStart = false;
            return;
        }
        PlayerPrefs.SetFloat("achtergrondMuziekVolume", sterkte);
        muziek.mute = sterkte == 0.001f;
        audioMixer.SetFloat("Muziek", Mathf.Log10(sterkte) * 20f);
    }
}
