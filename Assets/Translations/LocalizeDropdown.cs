using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizeDropdown : MonoBehaviour
{
    [SerializeField] private List<LocalizedString> dropdownOptions;
    private TMP_Dropdown _tmpDropdown;

    private void Awake()
    {
        List<TMP_Dropdown.OptionData> tmpDropdownOptions = new();
        foreach (var option in dropdownOptions) 
            tmpDropdownOptions.Add(new TMP_Dropdown.OptionData(option.GetLocalizedString()));
        if (!_tmpDropdown) _tmpDropdown = GetComponent<TMP_Dropdown>();
        _tmpDropdown.options = tmpDropdownOptions;
        LocalizationSettings.SelectedLocaleChanged += ChangedLocale;
    }

    private Locale _currentLocale;
    private void ChangedLocale(Locale newLocale)
    {
        if (_currentLocale == newLocale) return;
        _currentLocale = newLocale;
        List<TMP_Dropdown.OptionData> tmpDropdownOptions = new();
        foreach (var option in dropdownOptions)
            tmpDropdownOptions.Add(new TMP_Dropdown.OptionData(option.GetLocalizedString()));
        _tmpDropdown.options = tmpDropdownOptions;
    }
}
