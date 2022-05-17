using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public abstract class BaseSceneSettings : MonoBehaviour
{
    Achtergrond achtergrondScript;
    protected SaveScript saveScript;
    protected GegevensHouder gegevensScript;
    [Header("Background settings")]
    [SerializeField] TMP_Dropdown colorDropDown;
    [SerializeField] TMP_Dropdown imageDropDown;
    [SerializeField] TMP_Dropdown bgSoortDropDown;
    bool startValues = true;
    string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        achtergrondScript = gegevensHouder.GetComponent<Achtergrond>();
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        sceneName = SceneManager.GetActiveScene().name;
        SetBackgroundStartValues();
        SetSettingStartValues();
        startValues = false;
    }

    private void SetBackgroundStartValues()
    {
        colorDropDown.options.Clear();
        colorDropDown.options.AddRange(achtergrondScript.gekochteColorOptionData);
        imageDropDown.options.Clear();
        imageDropDown.options.AddRange(achtergrondScript.gekochteImageOptionData);
        int soort = saveScript.intDict["bgSoort" + sceneName];
        bgSoortDropDown.value = soort;
        if (soort == 1)
        {
            imageDropDown.gameObject.SetActive(true);
            int bgWaarde = saveScript.intDict["bgWaarde" + sceneName];
            int dropdownValue = bgWaarde >= 0 ? achtergrondScript.gekochteImageOptionData.IndexOf(achtergrondScript.imageOptionData[bgWaarde]) : -1;
            imageDropDown.value = dropdownValue;
            colorDropDown.gameObject.SetActive(false);
            colorDropDown.value = 0;
        }
        else
        {
            int bgWaarde = saveScript.intDict["bgWaarde" + sceneName];
            int dropdownValue = bgWaarde >= 0 ? achtergrondScript.gekochteColorOptionData.IndexOf(achtergrondScript.colorOptionData[bgWaarde]) : -1;
            imageDropDown.gameObject.SetActive(false);
            imageDropDown.value = 0;
            colorDropDown.gameObject.SetActive(true);
            colorDropDown.value = dropdownValue;
        }
    }

    public void ChangeBackgroundImage()
    {
        if (startValues) return;
        if (imageDropDown.value == -1)
        {
            return;
        }
        int dropdownValue = imageDropDown.value;
        int bgWaarde = achtergrondScript.imageOptionData.IndexOf(achtergrondScript.gekochteImageOptionData[dropdownValue]);
        if (saveScript.intDict["bgWaardeAll"] != bgWaarde || saveScript.intDict["bgSoortAll"] != 1)
        {
            saveScript.intDict["bgWaardeAll"] = -2;
        }
        saveScript.intDict["bgSoort" + sceneName] = 1;
        saveScript.intDict["bgWaarde" + sceneName] = bgWaarde;
        gegevensScript.VeranderOpgeslagenAchtergrond(sceneName.ToLower(), 1, bgWaarde);
    }

    public void ChangeBackgroundColor()
    {
        if (startValues) return;
        if (colorDropDown.value == -1)
        {
            return;
        }
        int dropdownValue = colorDropDown.value;
        int bgWaarde = achtergrondScript.colorOptionData.IndexOf(achtergrondScript.gekochteColorOptionData[dropdownValue]);
        if (saveScript.intDict["bgWaardeAll"] != bgWaarde || saveScript.intDict["bgSoortAll"] != 0)
        {
            saveScript.intDict["bgWaardeAll"] = -2;
        }
        saveScript.intDict["bgSoort" + sceneName] = 0;
        saveScript.intDict["bgWaarde" + sceneName] = bgWaarde;
        gegevensScript.VeranderOpgeslagenAchtergrond(sceneName.ToLower(), 0, bgWaarde);
    }

    public void ChangeBackgroundType()
    {
        if (bgSoortDropDown.value == -1)
        {
            return;
        }
        if (bgSoortDropDown.value == 1)
        {
            imageDropDown.value = -1;
            imageDropDown.gameObject.SetActive(true);
            colorDropDown.gameObject.SetActive(false);
        }
        else
        {
            imageDropDown.gameObject.SetActive(false);
            colorDropDown.value = -1;
            colorDropDown.gameObject.SetActive(true);
        }
    }

    protected abstract void SetSettingStartValues();
}