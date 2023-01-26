using System.Collections.Generic;
using System.IO;
using System.Text;
using Firebase.Auth;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using VBG.Extensions;

public class SaveScript : MonoBehaviour
{
    public static SaveScript Instance;
    Achtergrond achtergrond;
    GegevensHouder gegevensHouder;
    [HideInInspector] public bool ready;
    private List<string> sceneNames = new() { "Sudoku", "Solitaire", "2048", "Mijnenveger", "Menu", "ColorSort" };
    public Dictionary<string, int> intDict = new();
    public Dictionary<string, string> stringDict = new();
    public Dictionary<string, float> floatDict = new();
    public Dictionary<string, long> longDict = new();
    private List<string> userLongNames = new();
    private List<string> userIntNames = new();
    private List<string> SudokuIntNames = new();
    private List<string> SudokuStringNames = new();
    private List<string> SolitaireIntNames = new();
    private List<string> SolitaireFloatNames = new();
    private List<string> MijnenVegerIntNames = new();
    private List<string> intNames2048 = new();
    private List<string> SettingsStringNames = new();
    private List<string> SettingsIntNames = new();
    private List<string> AchtergrondFloatNames = new();
    private List<string> gekochteItems = new();

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gegevensHouder = GetComponent<GegevensHouder>();
        achtergrond = GetComponent<Achtergrond>();
        DontDestroyOnLoad(this);
        VulNames();
        LoadData();
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SaveSceneData;
        Application.quitting += Quitting;
    }

    private void VulNames()
    {
        //Userdata
        //Long
        userLongNames.Add("laatsteLogin");
        //Int
        userIntNames.Add("munten");
        userIntNames.Add("laatsteXOffline");

        //Sudoku
        //Int
        SudokuIntNames.Add("SudokuDifficulty");
        //SudokuIntNames.Add("SudokusGespeeld");
        //for (int i = 0; i < 4; i++)
        //{
        //    SudokuIntNames.Add("SudokuDiff" + i + "Gespeeld");
        //}
        SudokuIntNames.Add("SudokuEnabledDoubleNumberWarning");
        SudokuIntNames.Add("SudokuEnabledAutoEditNotes");
        //String
        SudokuStringNames.Add("SudokuClues");
        SudokuStringNames.Add("SudokuInput");
        SudokuStringNames.Add("SudokuInputNotes");

        //Solitaire
        //Int
        for (int i = 0; i < 8; i++)
        {
            SolitaireIntNames.Add("Stapel" + i + "Gedraaid");
            SolitaireIntNames.Add("Stapel" + i + "Grootte");
            SolitaireIntNames.Add("Eindstapel" + i + "Grootte");
            for (int ii = 0; ii < 20; ii++)
            {
                SolitaireIntNames.Add("Stapel" + i + ":" + ii);
                SolitaireIntNames.Add("Eindstapel" + i + ":" + ii);
            }
        }
        SolitaireIntNames.Add("ReststapelGrootte");
        SolitaireIntNames.Add("ReststapelOmgekeerdGrootte");
        SolitaireIntNames.Add("aanSolitaireBegonnen");
        SolitaireIntNames.Add("SolitairesGespeeld");
        for (int ii = 0; ii < 30; ii++)
        {
            SolitaireIntNames.Add("Reststapel:" + ii);
            SolitaireIntNames.Add("ReststapelOmgekeerd:" + ii);
        }
        SolitaireIntNames.Add("solitaireCardsSpriteType");
        //Float
        SolitaireFloatNames.Add("SolitaireTijd");
        SolitaireFloatNames.Add("SolitaireSnelsteTijd");
        SolitaireFloatNames.Add("spaceBetweenCardsFactor");

        //Mijnenveger
        //Int
        MijnenVegerIntNames.Add("difficultyMijnenVeger");
        MijnenVegerIntNames.Add("begonnenAanMV");
        for (int i = 0; i < 4; i++)
        {
            MijnenVegerIntNames.Add("MijnenvegerDiff" + i + "Gespeeld");
        }
        MijnenVegerIntNames.Add("MijnenvegersGespeeld");
        for (int i = 0; i < 100; i++)
        {
            MijnenVegerIntNames.Add("mijnenvegerBom" + i);
        }
        for (int i = 0; i < 25 * 25; i++)
        {
            MijnenVegerIntNames.Add("mijnenvegerVakjesgehad" + i);
        }
        for (int i = 0; i < 22; i++)
        {
            for (int ii = 0; ii < 22; ii++)
            {
                int index = (i * 100) + ii;
                MijnenVegerIntNames.Add("mijnenvegerVlaggenGezet" + index);
            }
        }

        //2048
        //Int
        intNames2048.Add("grootte2048");
        intNames2048.Add("begonnenAan2048");
        for (int i = 0; i < 4; i++)
        {
            intNames2048.Add("2048Grootte" + i + "Gespeeld");
        }
        intNames2048.Add("2048sGespeeld");
        for (int i = 0; i < 100; i++)
        {
            intNames2048.Add("2048Knop" + i);
        }

        //Settings
        //String
        SettingsStringNames.Add("taal");
        //Int
        List<string> sceneNames = new() { "All", "Sudoku", "Solitaire", "2048", "Mijnenveger", "Menu", "ColorSort" };
        foreach (string sceneName in sceneNames)
        {
            SettingsIntNames.Add("bgSoort" + sceneName);
            SettingsIntNames.Add("bgWaarde" + sceneName);
        }

        //AchtergrondSettings
        //Float
        AchtergrondFloatNames.Add("color.r");
        AchtergrondFloatNames.Add("color.g");
        AchtergrondFloatNames.Add("color.b");

        //GekochteItems
        //String
        for (int i = -1; i < 140; i++)
        {
            gekochteItems.Add("kleur" + i + "gekocht");
        }
        for (int i = 0; i < 50; i++)
        {
            gekochteItems.Add("afbeelding" + i + "gekocht");
        }

        //Alles
        //Int
        intDict.AddRange(SudokuIntNames, 0);
        intDict.AddRange(SolitaireIntNames, 0);
        intDict.AddRange(MijnenVegerIntNames, 0);
        intDict.AddRange(intNames2048, 0);
        intDict.AddRange(SettingsIntNames, 0);
        intDict.AddRange(userIntNames, 0);
        intDict.AddRange(gekochteItems, 0);
        //String
        stringDict.AddRange(SudokuStringNames, "  ");
        stringDict.AddRange(SettingsStringNames, "");
        //Float
        floatDict.AddRange(SolitaireFloatNames, 0f);
        floatDict.AddRange(AchtergrondFloatNames, 0f);
        //Long
        longDict.AddRange(userLongNames, 0);

        intDict["kleur" + 9 + "gekocht"] = 1;
        intDict["kleur" + 113 + "gekocht"] = 1;
        intDict["kleur" + 136 + "gekocht"] = 1;
        intDict["kleur" + 138 + "gekocht"] = 1;
        foreach (string sceneName in sceneNames)
        {
            intDict["bgWaarde" + sceneName] = 9;
        }
    }

    private bool updateData;
    private bool dataGedownloaded;
    private int frameNaDownload;

    // Update is called once per frame
    private void Update()
    {
        if (updateData)
        {
            updateData = false;
            if (intDict["laatsteXOffline"] == 1)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable) return;
                UploadStorageData();
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable) return;
                DownloadStorageData();
            }
        }
        if (dataGedownloaded)
        {
            if (frameNaDownload == 5)
            {
                dataGedownloaded = false;
                LoadData();
                for (int i = 0; i < sceneNames.Count; i++)
                {
                    int soort = intDict["bgSoort" + sceneNames[i]];
                    if (soort == 1)
                    {
                        int waarde = intDict["bgWaarde" + sceneNames[i]];
                        gegevensHouder.ChangeSavedBackground(sceneNames[i].ToLower(), soort, waarde);
                    }
                    else
                    {
                        int waarde = intDict["bgWaarde" + sceneNames[i]];
                        gegevensHouder.ChangeSavedBackground(sceneNames[i].ToLower(), soort, waarde);
                    }
                }
                gegevensHouder.SetLanguage();
            }
            frameNaDownload += 1;
        }
    }

    private void OnApplicationQuit()
    {
        Quitting();
    }

    private void OnApplicationPause(bool pause)
    {
        Quitting();
    }

    private void OnApplicationFocus(bool focus)
    {
        Quitting();
    }

    private void SaveSceneData(Scene prevScene)
    {
        _quitting = true;
        Save(prevScene);
    }

    private bool _quitting;

    private void Quitting()
    {
        if (_quitting)
        {
            return;
        }
        _quitting = true;
        Save(SceneManager.GetActiveScene());
    }

    private void Save(Scene scene)
    {
        switch (scene.name.ToLower())
        {
            case "spellenoverzicht":
                SaveNull();
                break;
            case "sudoku":
                SaveSudoku();
                break;
            case "2048":
                Save2048();
                break;
            case "mijnenveger":
                SaveMijnenveger();
                break;
            case "solitaire":
                SaveSolitaire();
                break;
            case "instellingen":
                SaveNull();
                break;
            case "shop":
                SaveShop();
                break;
            case "colorsort":
                SaveColorSort();
                break;
            default:
                _quitting = false;
                break;
        }
    }

    private StringBuilder SaveUserData()
    {
        StringBuilder data = new("@@@data");
        foreach (string name in userLongNames)
        {
            data.Append(",,,long" + "///" + name + ":::" + longDict[name]);
        }
        intDict["laatsteXOffline"] = Application.internetReachability == NetworkReachability.NotReachable || FirebaseAuth.DefaultInstance.CurrentUser == null ? 1 : 0;
        foreach (string name in userIntNames)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        data.Append(",,,supportInfo///versie:::" + Application.version);
        data.Append(",,,supportInfo///installerName:::" + Application.installerName);
        data.Append(",,,supportInfo///deviceModel:::" + SystemInfo.deviceModel);
        data.Append(",,,supportInfo///operatingSystem:::" + SystemInfo.operatingSystem);
        data.Append(",,,supportInfo///graphicsDeviceID:::" + SystemInfo.graphicsDeviceID);
        data.Append(",,,supportInfo///graphicsDeviceName:::" + SystemInfo.graphicsDeviceName);
        return data;
    }

    private StringBuilder SaveAchtergrond()
    {
        StringBuilder data = new("@@@achtergrond");
        foreach (string name in AchtergrondFloatNames)
        {
            data.Append(",,,float" + "///" + name + ":::" + floatDict[name]);
        }
        return data;
    }

    private StringBuilder SaveSettings()
    {
        StringBuilder data = new("@@@instellingen");
        foreach (string name in SettingsIntNames)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        foreach (string name in SettingsStringNames)
        {
            data.Append(",,,string" + "///" + name + ":::" + stringDict[name]);
        }
        return data;
    }

    private void SaveNull()
    {
        StringBuilder data = new("@@@nul,,,int///0:::0");
        SaveData(data);
    }

    private void SaveSudoku()
    {
        StringBuilder data = new("@@@sudoku");
        foreach (string name in SudokuIntNames)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        foreach (string name in SudokuStringNames)
        {
            data.Append(",,,string" + "///" + name + ":::" + stringDict[name]);
        }
        SaveData(data);
    }

    private void SaveSolitaire()
    {
        StringBuilder data = new("@@@solitaire");
        foreach (string name in SolitaireIntNames)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        foreach (string name in SolitaireFloatNames)
        {
            data.Append(",,,float" + "///" + name + ":::" + floatDict[name]);
        }
        SaveData(data);
    }

    private void SaveMijnenveger()
    {
        StringBuilder data = new("@@@mijnenveger");
        foreach (string name in MijnenVegerIntNames)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        SaveData(data);
    }

    private void Save2048()
    {
        StringBuilder data = new("@@@2048");
        foreach (string name in intNames2048)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        SaveData(data);
    }

    private void SaveShop()
    {
        StringBuilder data = new("@@@shop");
        foreach (string name in gekochteItems)
        {
            data.Append(",,,int" + "///" + name + ":::" + intDict[name]);
        }
        SaveData(data);
    }

    private void SaveColorSort()
    {
        SaveNull();
    }

    private void SaveData(StringBuilder data)
    {
        string path = $"{Application.persistentDataPath}/save.vbg";
        FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamReader reader = new(file);
        string oldData = file.Length == 0 ? "." : reader.ReadToEnd();
        reader.Close();
        string[] oldDataParts = oldData.Split("@@@");
        string dataNew = "";
        if (oldData != ".")
        {
            string sceneName = data.ToString().Split(",,,")[0][3..];
            bool vorigeDataGevonden = false;
            bool userDataGevonden = false;
            bool settingsDataFound = false;
            for (int i = 1; i < oldDataParts.Length; i++)
            {
                if (oldDataParts[i].StartsWith(sceneName))
                {
                    vorigeDataGevonden = true;
                    oldDataParts[i] = data.ToString()[3..];
                }
                else if (oldDataParts[i].StartsWith("data"))
                {
                    userDataGevonden = true;
                    oldDataParts[i] = SaveUserData().ToString()[3..];
                }
                else if (oldDataParts[i].StartsWith("achtergrond"))
                {
                    oldDataParts[i] = SaveAchtergrond().ToString()[3..];
                }
                else if (oldDataParts[i].StartsWith("instellingen"))
                {
                    settingsDataFound = true;
                    oldDataParts[i] = SaveSettings().ToString()[3..];
                }
            }
            if (!vorigeDataGevonden && !sceneName.Equals("nul"))
            {
                oldDataParts = oldDataParts.Add(data.ToString()[3..]);
            }
            if (!userDataGevonden)
            {
                List<string> tmp = new() { SaveUserData().ToString()[3..], SaveAchtergrond().ToString()[3..] };
                oldDataParts = oldDataParts.AddRange(tmp);
            }
            if (!settingsDataFound)
            {
                List<string> tmp = new() { SaveSettings().ToString()[3..] };
                oldDataParts = oldDataParts.AddRange(tmp);
            }
            for (int i = 1; i < oldDataParts.Length; i++)
            {
                dataNew += ("@@@" + oldDataParts[i]).Trim();
            }
            StreamWriter writer = new(path);
            writer.WriteLine(dataNew);
            writer.Close();
        }
        else
        {
            StreamWriter writer = new(path);
            writer.WriteLine(data);
            writer.Close();
        }
        file.Close();
        if (Application.internetReachability == NetworkReachability.NotReachable || FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            _quitting = false;
            return;
        }
        UploadStorageData();
    }

    private void LoadData()
    {
        string path = $"{Application.persistentDataPath}/save.vbg";
        if (!File.Exists(path))
        {
            ready = true;
            gegevensHouder.SetBackground();
            achtergrond.StartValues();
            return;
        }
        FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamReader reader = new(file);
        string oldData = file.Length == 0 ? "." : reader.ReadToEnd();
        reader.Close();
        if (oldData.Equals("."))
        {
            file.Close();
            File.Delete(path);
            return;
        }
        string data = oldData;
        string[] dataParts = data.Split("@@@");
        for (int i = 1; i < dataParts.Length; i++)
        {
            string[] vars = dataParts[i].Split(",,,");
            for (int ii = 1; ii < vars.Length; ii++)
            {
                string[] varParts = vars[ii].Split("///");
                string soort = varParts[0];
                if (soort.Equals("int"))
                {
                    string[] savedVarParts = varParts[1].Split(":::");
                    intDict[savedVarParts[0]] = int.Parse(savedVarParts[1]);
                }
                else if (soort.Equals("float"))
                {
                    string[] savedVarParts = varParts[1].Split(":::");
                    floatDict[savedVarParts[0]] = float.Parse(savedVarParts[1]);
                }
                else if (soort.Equals("long"))
                {
                    string[] savedVarParts = varParts[1].Split(":::");
                    floatDict[savedVarParts[0]] = long.Parse(savedVarParts[1]);
                }
                else if (soort.Equals("string"))
                {
                    string[] savedVarParts = varParts[1].Split(":::");
                    stringDict[savedVarParts[0]] = savedVarParts[1];
                }
            }
        }
        file.Close();
        ready = true;
        _quitting = false;
        gegevensHouder.SetBackground();
        achtergrond.StartValues();
    }

    private void UploadStorageData()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) return;
        if (FirebaseAuth.DefaultInstance.CurrentUser.IsAnonymous) return;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference saveFile = storage.GetReferenceFromUrl(storage.RootReference + "/Users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId + "/save.vbg");
        string path = $"{Application.persistentDataPath}/save.vbg";
        _quitting = true;
        FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);
        saveFile.PutStreamAsync(file).ContinueWith(task =>
        {
            file.Close();
            _quitting = false;
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception?.ToString());
            }
        });
    }

    public void UpdateData()
    {
        updateData = true;
    }

    private void DownloadStorageData()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) return;
        if (FirebaseAuth.DefaultInstance.CurrentUser.IsAnonymous) return;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference saveFile = storage.GetReferenceFromUrl(storage.RootReference + "/Users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId + "/save.vbg");
        string path = $"{Application.persistentDataPath}/save.vbg";
        saveFile.GetFileAsync(path).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception?.InnerException?.Message.ToString());
            }
            dataGedownloaded = true;
        });
    }
}