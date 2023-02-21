using TMPro;
using UnityEngine;

public abstract class BaseSceneSettings : MonoBehaviour
{
    private Achtergrond achtergrondScript;
    protected SaveScript saveScript;
    protected GegevensHouder gegevensScript;
    [Header("Background settings")]
    [SerializeField] private TMP_Dropdown colorDropDown;
    [SerializeField] private TMP_Dropdown imageDropDown;
    [SerializeField] private TMP_Dropdown bgSoortDropDown;
    private bool _startValues = true;
    private string _sceneName;

    // Start is called before the first frame update
    void Start()
    {
        GameObject dataObj = GameObject.Find("DataObject");
        if (dataObj == null) return;
        saveScript = SaveScript.Instance;
        achtergrondScript = dataObj.GetComponent<Achtergrond>();
        gegevensScript = GegevensHouder.Instance;
        _sceneName = SceneManager.GetActiveScene().name;
        SetBackgroundStartValues();
        SetSettingStartValues();
        _startValues = false;
    }

    private void SetBackgroundStartValues()
    {
        colorDropDown.options.Clear();
        colorDropDown.options.AddRange(achtergrondScript.boughtColorOptionData);
        imageDropDown.options.Clear();
        imageDropDown.options.AddRange(achtergrondScript.boughtImageOptionData);
        int type = saveScript.IntDict["bgSoort" + _sceneName];
        bgSoortDropDown.value = type;
        if (type == 1)
        {
            imageDropDown.gameObject.SetActive(true);
            int backgroundValue = saveScript.IntDict["bgWaarde" + _sceneName];
            int dropdownValue = backgroundValue >= 0 ? achtergrondScript.boughtImageOptionData.IndexOf(achtergrondScript.imageOptionData[backgroundValue]) : -1;
            imageDropDown.value = dropdownValue;
            colorDropDown.gameObject.SetActive(false);
            colorDropDown.value = 0;
        }
        else
        {
            int backgroundValue = saveScript.IntDict["bgWaarde" + _sceneName];
            int dropdownValue = backgroundValue >= 0 ? achtergrondScript.boughtColorOptionData.IndexOf(achtergrondScript.colorOptionData[backgroundValue]) : -1;
            if (backgroundValue == -1) dropdownValue = 0;
            imageDropDown.gameObject.SetActive(false);
            imageDropDown.value = 0;
            colorDropDown.gameObject.SetActive(true);
            colorDropDown.value = dropdownValue;
        }
    }

    public void ChangeBackgroundImage()
    {
        if (_startValues) return;
        if (imageDropDown.value == -1)
        {
            return;
        }
        int dropdownValue = imageDropDown.value;
        int backgroundValue = achtergrondScript.imageOptionData.IndexOf(achtergrondScript.boughtImageOptionData[dropdownValue]);
        if (saveScript.IntDict["bgWaardeAll"] != backgroundValue || saveScript.IntDict["bgSoortAll"] != 1)
        {
            saveScript.IntDict["bgWaardeAll"] = -2;
        }
        saveScript.IntDict["bgSoort" + _sceneName] = 1;
        saveScript.IntDict["bgWaarde" + _sceneName] = backgroundValue;
        gegevensScript.ChangeSavedBackground(_sceneName.ToLower(), 1, backgroundValue);
    }

    public void ChangeBackgroundColor()
    {
        if (_startValues) return;
        if (colorDropDown.value == -1)
        {
            return;
        }
        int dropdownValue = colorDropDown.value;
        int backgroundValue = achtergrondScript.colorOptionData.IndexOf(achtergrondScript.boughtColorOptionData[dropdownValue]);
        if (saveScript.IntDict["bgWaardeAll"] != backgroundValue || saveScript.IntDict["bgSoortAll"] != 0)
        {
            saveScript.IntDict["bgWaardeAll"] = -2;
        }
        saveScript.IntDict["bgSoort" + _sceneName] = 0;
        saveScript.IntDict["bgWaarde" + _sceneName] = backgroundValue;
        gegevensScript.ChangeSavedBackground(_sceneName.ToLower(), 0, backgroundValue);
    }

    public void ChangeBackgroundType()
    {
        switch (bgSoortDropDown.value)
        {
            case -1:
                return;
            case 1:
                imageDropDown.value = -1;
                imageDropDown.gameObject.SetActive(true);
                colorDropDown.gameObject.SetActive(false);
                break;
            default:
                imageDropDown.gameObject.SetActive(false);
                colorDropDown.value = -1;
                colorDropDown.gameObject.SetActive(true);
                break;
        }
    }

    protected abstract void SetSettingStartValues();
}