using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

//using UnityEngine.UI;

public class SettingsManager : BaseSceneSettings
{
    //private AudioHandler _audioHandler;

    [Header("Other scene specific")] [SerializeField]
    private TMP_Dropdown taalDropdown;

    //[SerializeField] private GameObject bgInstelingen;
    [SerializeField] private RectTransform settingScrolldownContent;
    //[SerializeField] private GameObject overigeInstellingen;
    //[SerializeField] private Slider musicVolumeSlider;

    private readonly List<string> _sceneNames = new()
        { "Sudoku", "Solitaire", "2048", "Minesweeper", "Menu", "ColorSort" };

    // Start is called before the first frame update
    private void Start()
    {
        GameObject dataObj = GameObject.Find("DataObject");
        if (dataObj == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }

        saveScript = SaveScript.Instance;
        BackgroundManager = dataObj.GetComponent<Achtergrond>();
        gegevensScript = GegevensHouder.Instance;
        //_audioHandler = GameObject.Find("AchtergrondMuziek").GetComponent<AudioHandler>();
        SetBackgroundStartValues();
        SetSettingStartValues();
        IsStartValues = false;
    }

    private void SetLanguageStartValue()
    {
        List<string> availableLocales = new();
        List<TMP_Dropdown.OptionData> options = new();
        foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
        {
            availableLocales.Add(locale.LocaleName);
            options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
        }

        taalDropdown.options = options;
        for (int i = 0; i < availableLocales.Count; i++)
        {
            if (saveScript.StringDict["language"].Equals(availableLocales[i]))
            {
                taalDropdown.value = i;
                break;
            }
        }
    }

    private void SetBackgroundStartValues()
    {
        Transform content = settingScrolldownContent.transform;
        for (int i = 3; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            GameObject colorDropdownObj = child.Find("BGColor").gameObject;
            TMP_Dropdown colorDropDown = colorDropdownObj.GetComponent<TMP_Dropdown>();
            colorDropDown.options.Clear();
            colorDropDown.options.AddRange(BackgroundManager.boughtColorOptionData);
            //GameObject imageDropdownObj = child.Find("BGImage").gameObject;
            //TMP_Dropdown imageDropDown = imageDropdownObj.GetComponent<TMP_Dropdown>();
            //imageDropDown.options.Clear();
            //imageDropDown.options.AddRange(BackgroundManager.boughtImageOptionData);
            string sceneName = child.gameObject.name[2..];
            //int soort = saveScript.IntDict["bgSoort" + sceneName];
            //child.Find("BGSoort").GetComponent<TMP_Dropdown>().value = soort;
            /*if (soort == 1)
            {
                //imageDropdownObj.SetActive(true);
                int bgWaarde = saveScript.IntDict["bgWaarde" + sceneName];
                int dropdownValue = bgWaarde >= 0
                    ? BackgroundManager.boughtImageOptionData.IndexOf(BackgroundManager.imageOptionData[bgWaarde])
                    : -1;
                //imageDropDown.value = dropdownValue;
                colorDropdownObj.SetActive(false);
                colorDropDown.value = 0;
            }
            else
            {*/
            int bgWaarde = saveScript.IntDict["bgWaarde" + sceneName];
            int dropdownValue = bgWaarde >= 0
                ? BackgroundManager.boughtColorOptionData.IndexOf(BackgroundManager.colorOptionData[bgWaarde])
                : -1;
            //if (bgWaarde == -1) dropdownValue = 0;
            //imageDropdownObj.SetActive(false);
            //imageDropDown.value = 0;
            colorDropdownObj.SetActive(true);
            colorDropDown.value = dropdownValue;
            //}
        }
    }

    protected override void SetSettingStartValues()
    {
        SetLanguageStartValue();
        //musicVolumeSlider.value = PlayerPrefs.GetFloat("achtergrondMuziekVolume", 0.25f);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("GameChoiceMenu");
    }

    public void ChangeLanguage()
    {
        string language = taalDropdown.options[taalDropdown.value].text;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if (!language.ToLower()
                    .Equals(LocalizationSettings.AvailableLocales.Locales[i].LocaleName.ToLower())) continue;
            gegevensScript.ChangeLanguage(i);
            break;
        }
    }

    public void ChangeBackgroundImage(GameObject obj)
    {
        if (IsStartValues) return;
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown.value == -1) return;
        string sceneNaam = obj.transform.parent.name[2..];
        if (sceneNaam.ToLower().Equals("all"))
        {
            int dropdownValue = dropdown.value;
            int bgWaarde =
                BackgroundManager.imageOptionData.IndexOf(BackgroundManager.boughtImageOptionData[dropdownValue]);
            saveScript.IntDict["bgSoortAll"] = 1;
            saveScript.IntDict["bgWaardeAll"] = bgWaarde;
            for (int i = 0; i < _sceneNames.Count; i++)
            {
                saveScript.IntDict["bgSoort" + _sceneNames[i]] = 1;
                saveScript.IntDict["bgWaarde" + _sceneNames[i]] = bgWaarde;
                gegevensScript.ChangeSavedBackground(_sceneNames[i].ToLower(), 1, bgWaarde);
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
            int bgWaarde =
                BackgroundManager.imageOptionData.IndexOf(BackgroundManager.boughtImageOptionData[dropdownValue]);
            if (saveScript.IntDict["bgWaardeAll"] != bgWaarde || saveScript.IntDict["bgSoortAll"] != 1)
            {
                saveScript.IntDict["bgWaardeAll"] = -2;
                settingScrolldownContent.Find("BGAll").Find("BGImage").GetComponent<TMP_Dropdown>().value = -1;
                settingScrolldownContent.Find("BGAll").Find("BGColor").GetComponent<TMP_Dropdown>().value = -1;
            }

            saveScript.IntDict["bgSoort" + sceneNaam] = 1;
            saveScript.IntDict["bgWaarde" + sceneNaam] = bgWaarde;
            gegevensScript.ChangeSavedBackground(sceneNaam.ToLower(), 1, bgWaarde);
        }
    }

    public void ChangeBackgroundColor(GameObject obj)
    {
        if (IsStartValues) return;
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        if (dropdown.value == -1) return;
        string sceneName = obj.transform.parent.name[2..];
        /*if (sceneName.ToLower().Equals("all"))
        {
            int dropdownValue = dropdown.value;
            int bgWaarde =
                BackgroundManager.colorOptionData.IndexOf(BackgroundManager.boughtColorOptionData[dropdownValue]);
            saveScript.IntDict["bgSoortAll"] = 0;
            saveScript.IntDict["bgWaardeAll"] = bgWaarde;
            for (int i = 0; i < _sceneNames.Count; i++)
            {
                saveScript.IntDict["bgSoort" + _sceneNames[i]] = 0;
                saveScript.IntDict["bgWaarde" + _sceneNames[i]] = bgWaarde;
                gegevensScript.ChangeSavedBackground(_sceneNames[i].ToLower(), 0, bgWaarde);
            }

            foreach (Transform andereDropdown in obj.transform.parent.parent)
            {
                andereDropdown.Find("BGSoort").GetComponent<TMP_Dropdown>().value = 0;
                andereDropdown.Find("BGColor").GetComponent<TMP_Dropdown>().value = dropdownValue;
            }
        }
        else
        {*/
        int dropdownValue = dropdown.value;
        int bgWaarde =
            BackgroundManager.colorOptionData.IndexOf(BackgroundManager.boughtColorOptionData[dropdownValue]);
        /*if (saveScript.IntDict["bgWaardeAll"] != bgWaarde || saveScript.IntDict["bgSoortAll"] != 0)
        {
            saveScript.IntDict["bgWaardeAll"] = -2;
            settingScrolldownContent.Find("BGAll").Find("BGImage").GetComponent<TMP_Dropdown>().value = -1;
            settingScrolldownContent.Find("BGAll").Find("BGColor").GetComponent<TMP_Dropdown>().value = -1;
        }*/

        saveScript.IntDict["bgSoort" + sceneName] = 0;
        saveScript.IntDict["bgWaarde" + sceneName] = bgWaarde;
        gegevensScript.ChangeSavedBackground(sceneName.ToLower(), 0, bgWaarde);
        //}
    }

    public void ChangeBackgroundType(GameObject obj)
    {
        TMP_Dropdown dropdown = obj.GetComponent<TMP_Dropdown>();
        Transform parent = obj.transform.parent;
        if (dropdown.value == -1) return;
        if (dropdown.value == 1)
        {
            GameObject imageDropdown = parent.Find("BGImage").gameObject;
            imageDropdown.GetComponent<TMP_Dropdown>().value = -1;
            imageDropdown.SetActive(true);
            GameObject colorDropdown = parent.Find("BGColor").gameObject;
            colorDropdown.SetActive(false);
        }
        else
        {
            GameObject imageDropdown = parent.Find("BGImage").gameObject;
            imageDropdown.SetActive(false);
            GameObject colorDropdown = parent.Find("BGColor").gameObject;
            colorDropdown.GetComponent<TMP_Dropdown>().value = -1;
            colorDropdown.SetActive(true);
        }
    }

    public void ChangeSettingPage(int pageIndex)
    {
        //overigeInstellingen.SetActive(pageIndex == 0);
        //bgInstelingen.SetActive(pageIndex == 1);
    }

    public void ChangeMusicVolume(float volume)
    {
        //if (IsStartValues) return;
        //PlayerPrefs.SetFloat("achtergrondMuziekVolume", volume);
        //_audioHandler.SetVolume(volume);
    }
}