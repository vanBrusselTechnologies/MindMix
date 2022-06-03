using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GegevensHouder : MonoBehaviour
{
    public static GegevensHouder Instance;
    bool testBuild = false;
    [HideInInspector] public bool startNewGame = false;
    public Texture2D zwart;
    private bool isPaused = false;
    private bool wasPaused = false;
    public WaitForSecondsRealtime wachtHonderdste = new WaitForSecondsRealtime(0.01f);
    public RuntimePlatform platform;
    public List<Sprite> achtergronden;
    public Sprite spriteWit;
    private List<int> achtergrondSudoku = new List<int>() { 0, -2 };
    private List<int> achtergrondMenu = new List<int>() { 0, -2 };
    private List<int> achtergrond2048 = new List<int>() { 0, -2 };
    private List<int> achtergrondMV = new List<int>() { 0, -2 };
    private List<int> achtergrondSolitaire = new List<int>() { 0, -2 };
    private List<int> achtergrondColorSort = new List<int>() { 0, -2 };
    private Achtergrond bgScript;
    private SaveScript saveScript;
    [HideInInspector] public bool loginWarningGehad;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            testBuild = true;
        }
        bgScript = GetComponent<Achtergrond>();
        saveScript = GetComponent<SaveScript>();
        Application.targetFrameRate = Mathf.Min(30, Screen.currentResolution.refreshRate);
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (!testBuild)
        {
            Debug.unityLogger.logEnabled = false;
        }
        ZetGegevens();
        saveScript.longDict["laatsteLogin"] = System.DateTime.UtcNow.Ticks;
    }

    private bool gewisseldeKleur = false;
    // Update is called once per frame
    private void Update()
    {
        if (!isPaused && wasPaused)
        {
            ZetGegevens();
        }
        wasPaused = isPaused;
        if (gewisseldeKleur)
        {
            gewisseldeKleur = false;
            bgScript.aangepasteKleur = true;
        }
    }

    public List<int> AchtergrondList(string sceneNaam = "noscene")
    {
        if (sceneNaam.Equals("noscene"))
        {
            sceneNaam = SceneManager.GetActiveScene().name.ToLower();
        }
        List<int> list = new List<int>() { -1000, -1000 };
        switch (sceneNaam)
        {
            case "sudoku": list = achtergrondSudoku; break;
            case "spellenoverzicht": list = achtergrondMenu; break;
            case "2048": list = achtergrond2048; break;
            case "mijnenveger": list = achtergrondMV; break;
            case "solitaire": list = achtergrondSolitaire; break;
            case "colorsort": list = achtergrondColorSort; break;
            case "inlogenvoorplaatapp": list = new List<int>() { 1, -1 }; break;
            case "instellingen": list = achtergrondMenu; break;
            case "shop": list = achtergrondMenu; break;
            default: break;
        }
        return list;
    }

    public void VeranderOpgeslagenAchtergrond(string sceneNaam, int bgSoort, int waarde)
    {
        gewisseldeKleur = true;
        switch(sceneNaam){
            case "sudoku":
                achtergrondSudoku[0] = bgSoort;
                achtergrondSudoku[1] = waarde; 
                break;
            case "menu":
                achtergrondMenu[0] = bgSoort;
                achtergrondMenu[1] = waarde; 
                break;
            case "2048":
                achtergrond2048[0] = bgSoort;
                achtergrond2048[1] = waarde; 
                break;
            case "mijnenveger":
                achtergrondMV[0] = bgSoort;
                achtergrondMV[1] = waarde; 
                break;
            case "solitaire":
                achtergrondSolitaire[0] = bgSoort;
                achtergrondSolitaire[1] = waarde;
                break;
            case "colorsort":
                achtergrondColorSort[0] = bgSoort;
                achtergrondColorSort[1] = waarde;
                break;
            default: break;
        }
    }

    public void VeranderTaal(int taalWaarde)
    {
        if (taalWaarde == -1)
        {
            return;
        }
        saveScript.stringDict["taal"] = LocalizationSettings.AvailableLocales.Locales[taalWaarde].LocaleName;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[taalWaarde];
        Firebase.Auth.FirebaseAuth.DefaultInstance.LanguageCode = LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }

    private void ZetGegevens()
    {
        platform = Application.platform;
        zwart = new Texture2D(1, 1);
        zwart.SetPixel(0, 0, Color.black);
        zwart.Apply();
        ZetTaal();
    }

    public void ZetTaal()
    {
        if (saveScript.stringDict["taal"] != "")
        {
            LocalizationSettings.InitializationOperation.WaitForCompletion();
            foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.LocaleName.Equals(saveScript.stringDict["taal"]))
                {
                    LocalizationSettings.SelectedLocale = locale;
                    break;
                }
            }
        }
        else
        {
            LocalizationSettings.InitializationOperation.WaitForCompletion();
            foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.LocaleName.Equals(LocalizationSettings.SelectedLocale.LocaleName))
                {
                    saveScript.stringDict["taal"] = locale.LocaleName;
                    break;
                }
            }
        }
    }

    public void ZetAchtergronden()
    {
        List<string> sceneNames = new List<string>() { "All", "Sudoku", "Solitaire", "2048", "Mijnenveger", "Menu", "ColorSort" };
        foreach (string sceneName in sceneNames)
        {
            VeranderOpgeslagenAchtergrond(sceneName.ToLower(), saveScript.intDict["bgSoort" + sceneName], saveScript.intDict["bgWaarde" + sceneName]);
        }
    }
}
