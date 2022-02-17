using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SudokuInstellingen : MonoBehaviour
{
    Achtergrond achtergrondScript;
    SaveScript saveScript;
    GegevensHouder gegevensScript;
    [SerializeField] TMP_Dropdown colorDropDown;
    [SerializeField] TMP_Dropdown imageDropDown;
    [SerializeField] TMP_Dropdown bgSoortDropDown;
    [SerializeField] Toggle dubbelGetalWarningToggle;
    [SerializeField] Toggle notitieBijwerkToggle;
    bool startValues = true;

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
        SetBackgroundStartValues();
        SetSudokuSettingStartValues();
        startValues = false;
    }

    private void SetBackgroundStartValues()
    {
        colorDropDown.options.Clear();
        colorDropDown.options.AddRange(achtergrondScript.gekochteColorOptionData);
        imageDropDown.options.Clear();
        imageDropDown.options.AddRange(achtergrondScript.gekochteImageOptionData);
        int soort = saveScript.intDict["bgSoortSudoku"];
        bgSoortDropDown.value = soort;
        if (soort == 1)
        {
            imageDropDown.gameObject.SetActive(true);
            int bgWaarde = saveScript.intDict["bgWaardeSudoku"];
            int dropdownValue = bgWaarde >= 0 ? achtergrondScript.gekochteImageOptionData.IndexOf(achtergrondScript.imageOptionData[bgWaarde]) : -1;
            imageDropDown.value = dropdownValue;
            colorDropDown.gameObject.SetActive(false);
            colorDropDown.value = 0;
        }
        else
        {
            int bgWaarde = saveScript.intDict["bgWaardeSudoku"];
            int dropdownValue = bgWaarde >= 0 ? achtergrondScript.gekochteColorOptionData.IndexOf(achtergrondScript.colorOptionData[bgWaarde]) : -1;
            imageDropDown.gameObject.SetActive(false);
            imageDropDown.value = 0;
            colorDropDown.gameObject.SetActive(true);
            colorDropDown.value = dropdownValue;
        }
    }

    private void SetSudokuSettingStartValues()
    {
        notitieBijwerkToggle.isOn = saveScript.intDict["notitieBijwerkSettingIsOn"] == 1;
        dubbelGetalWarningToggle.isOn = saveScript.intDict["dubbelGetalWarningIsOn"] == 1;
    }

    public void VeranderAchtergrondImg()
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
        saveScript.intDict["bgSoortSudoku"] = 1;
        saveScript.intDict["bgWaardeSudoku"] = bgWaarde;
        gegevensScript.VeranderOpgeslagenAchtergrond("sudoku", 1, bgWaarde);
    }

    public void VeranderAchtergrondKleur()
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
        saveScript.intDict["bgSoortSudoku"] = 0;
        saveScript.intDict["bgWaardeSudoku"] = bgWaarde;
        gegevensScript.VeranderOpgeslagenAchtergrond("sudoku", 0, bgWaarde);
    }

    public void VeranderAchtergrondSoort()
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

    public void VeranderNotitieBijwerkSetting()
    {
        saveScript.intDict["notitieBijwerkSettingIsOn"] = notitieBijwerkToggle.isOn ? 1 : 0;
    }

    public void VeranderDubbelGetalWarningSetting()
    {
        saveScript.intDict["dubbelGetalWarningIsOn"] = dubbelGetalWarningToggle.isOn ? 1 : 0;
    }
}
