using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class InstellingenScript : MonoBehaviour
{
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    private Achtergrond achtergrondScript;
    private AudioHandler audioHandler;

    [SerializeField] private TMP_Dropdown taalDropdown;
    [SerializeField] private GameObject bgInstelingen;
    [SerializeField] private RectTransform bgInstellingScrolldownContent;
    [SerializeField] private GameObject overigeInstellingen;
    [SerializeField] private Slider muziekVolumeSlider;

    private List<string> sceneNames = new() { "Sudoku", "Solitaire", "2048", "Mijnenveger", "Menu", "ColorSort" };
    private bool startValues = true;
    private bool volumeSetInStart;

    // Start is called before the first frame update
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
        achtergrondScript = gegevensHouder.GetComponent<Achtergrond>();
        audioHandler = GameObject.Find("AchtergrondMuziek").GetComponent<AudioHandler>();
        SetTaalStartWaarde();
        SetBackgroundStartValues();
        startValues = false;
        volumeSetInStart = true;
        muziekVolumeSlider.value = PlayerPrefs.GetFloat("achtergrondMuziekVolume", 0.25f);
    }

    private void SetTaalStartWaarde()
    {
        List<string> tmpTalen = new();
        List<TMP_Dropdown.OptionData> options = new();
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
            colorDropDown.options.AddRange(achtergrondScript.boughtColorOptionData);
            Transform imageDropdownObj = child.Find("BGImage");
            TMP_Dropdown imageDropDown = imageDropdownObj.GetComponent<TMP_Dropdown>();
            imageDropDown.options.Clear();
            imageDropDown.options.AddRange(achtergrondScript.boughtImageOptionData);
            string sceneNaam = child.gameObject.name[2..];
            int soort = saveScript.intDict["bgSoort" + sceneNaam];
            child.Find("BGSoort").GetComponent<TMP_Dropdown>().value = soort;
            if (soort == 1)
            {
                imageDropdownObj.gameObject.SetActive(true);
                int bgWaarde = saveScript.intDict["bgWaarde" + sceneNaam];
                int dropdownValue = bgWaarde >= 0 ? achtergrondScript.boughtImageOptionData.IndexOf(achtergrondScript.imageOptionData[bgWaarde]) : -1;
                imageDropDown.value = dropdownValue;
                colorDropdownObj.gameObject.SetActive(false);
                colorDropDown.value = 0;
            }
            else
            {
                int bgWaarde = saveScript.intDict["bgWaarde" + sceneNaam];
                int dropdownValue = bgWaarde >= 0 ? achtergrondScript.boughtColorOptionData.IndexOf(achtergrondScript.colorOptionData[bgWaarde]) : -1;
                if (bgWaarde == -1) dropdownValue = 0;
                imageDropdownObj.gameObject.SetActive(false);
                imageDropDown.value = 0;
                colorDropdownObj.gameObject.SetActive(true);
                colorDropDown.value = dropdownValue;
            }
        }
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
        gegevensScript.ChangeLanguage(taalWaarde);
    }

    public void VeranderAchtergrondImg(GameObject obj)
    {
        if (startValues) return;
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown.value == -1) return;
        string sceneNaam = obj.transform.parent.name[2..];
        if (sceneNaam.ToLower().Equals("all"))
        {
            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.imageOptionData.IndexOf(achtergrondScript.boughtImageOptionData[dropdownValue]);
            saveScript.intDict["bgSoortAll"] = 1;
            saveScript.intDict["bgWaardeAll"] = bgWaarde;
            for (int i = 0; i < sceneNames.Count; i++)
            {
                saveScript.intDict["bgSoort" + sceneNames[i]] = 1;
                saveScript.intDict["bgWaarde" + sceneNames[i]] = bgWaarde;
                gegevensScript.ChangeSavedBackground(sceneNames[i].ToLower(), 1, bgWaarde);
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
            int bgWaarde = achtergrondScript.imageOptionData.IndexOf(achtergrondScript.boughtImageOptionData[dropdownValue]);
            if (saveScript.intDict["bgWaardeAll"] != bgWaarde || saveScript.intDict["bgSoortAll"] != 1)
            {
                saveScript.intDict["bgWaardeAll"] = -2;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGImage").GetComponent<TMP_Dropdown>().value = -1;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGColor").GetComponent<TMP_Dropdown>().value = -1;
            }
            saveScript.intDict["bgSoort" + sceneNaam] = 1;
            saveScript.intDict["bgWaarde" + sceneNaam] = bgWaarde;
            gegevensScript.ChangeSavedBackground(sceneNaam.ToLower(), 1, bgWaarde);
        }
    }

    public void VeranderAchtergrondKleur(GameObject obj)
    {
        if (startValues) return;
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown.value == -1) return;
        string sceneNaam = obj.transform.parent.name[2..];
        if (sceneNaam.ToLower() == "all")
        {
            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.colorOptionData.IndexOf(achtergrondScript.boughtColorOptionData[dropdownValue]);
            saveScript.intDict["bgSoortAll"] = 0;
            saveScript.intDict["bgWaardeAll"] = bgWaarde;
            for (int i = 0; i < sceneNames.Count; i++)
            {
                saveScript.intDict["bgSoort" + sceneNames[i]] = 0;
                saveScript.intDict["bgWaarde" + sceneNames[i]] = bgWaarde;
                gegevensScript.ChangeSavedBackground(sceneNames[i].ToLower(), 0, bgWaarde);
            }
            foreach (Transform andereDropdown in obj.transform.parent.parent)
            {
                andereDropdown.Find("BGSoort").GetComponent<TMP_Dropdown>().value = 0;
                andereDropdown.Find("BGColor").GetComponent<TMP_Dropdown>().value = dropdownValue;
            }
        }
        else
        {
            int dropdownValue = dropdown.value;
            int bgWaarde = achtergrondScript.colorOptionData.IndexOf(achtergrondScript.boughtColorOptionData[dropdownValue]);
            if (saveScript.intDict["bgWaardeAll"] != bgWaarde || saveScript.intDict["bgSoortAll"] != 0)
            {
                saveScript.intDict["bgWaardeAll"] = -2;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGImage").GetComponent<TMP_Dropdown>().value = -1;
                bgInstellingScrolldownContent.Find("BGAll").Find("BGColor").GetComponent<TMP_Dropdown>().value = -1;
            }
            saveScript.intDict["bgSoort" + sceneNaam] = 0;
            saveScript.intDict["bgWaarde" + sceneNaam] = bgWaarde;
            gegevensScript.ChangeSavedBackground(sceneNaam.ToLower(), 0, bgWaarde);
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
        if (paginaIndex == 0)
        {
            overigeInstellingen.SetActive(true);
            bgInstelingen.SetActive(false);
        }
        else if (paginaIndex == 1)
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
        audioHandler.SetVolume(sterkte);
    }
}
