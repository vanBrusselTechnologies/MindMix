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

    private readonly List<string> sceneNames = new()
        { "Sudoku", "Solitaire", "2048", "Minesweeper", "Menu", "ColorSort" };

    public readonly Dictionary<string, int> IntDict = new();
    public readonly Dictionary<string, string> StringDict = new();
    public readonly Dictionary<string, float> FloatDict = new();
    public readonly Dictionary<string, long> LongDict = new();
    private readonly List<string> userLongNames = new();
    private readonly List<string> userIntNames = new();
    private readonly List<string> _sudokuIntNames = new();
    private readonly List<string> _sudokuStringNames = new();
    private readonly List<string> _solitaireIntNames = new();
    private readonly List<string> _solitaireFloatNames = new();
    private readonly List<string> _solitaireStringNames = new();
    private readonly List<string> _minesweeperIntNames = new();
    private readonly List<string> _minesweeperStringNames = new();
    private readonly List<string> intNames2048 = new();
    private readonly List<string> SettingsStringNames = new();
    private readonly List<string> SettingsIntNames = new();
    private readonly List<string> AchtergrondFloatNames = new();
    private readonly List<string> gekochteItems = new();

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
        _sudokuIntNames.Add("SudokuDifficulty");
        //_sudokuIntNames.Add("SudokusGespeeld");
        //for (int i = 0; i < 4; i++)
        //{
        //    _sudokuIntNames.Add("SudokuDiff" + i + "Gespeeld");
        //}
        _sudokuIntNames.Add("SudokuEnabledDoubleNumberWarning");
        _sudokuIntNames.Add("SudokuEnabledAutoEditNotes");
        //String
        _sudokuStringNames.Add("SudokuClues");
        _sudokuStringNames.Add("SudokuInput");
        _sudokuStringNames.Add("SudokuInputNotes");

        //Solitaire
        //Int
        //_solitaireIntNames.Add("SolitairesGespeeld");
        _solitaireIntNames.Add("SolitaireCardsSpriteType");
        //Float
        _solitaireFloatNames.Add("SolitaireTime");
        _solitaireFloatNames.Add("SolitaireSpaceBetweenCardsFactor");
        //String
        _solitaireStringNames.Add("SolitaireProgress");

        //Mijnenveger
        //Int
        _minesweeperIntNames.Add("MinesweeperDifficulty");
        //_minesweeperIntNames.Add("Minesweeper????"); //settings
        //String
        _minesweeperStringNames.Add("MinesweeperMines");
        _minesweeperStringNames.Add("MinesweeperInput");

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
        List<string> sceneNames2 = new() { "All", "Sudoku", "Solitaire", "2048", "Minesweeper", "Menu", "ColorSort" };
        foreach (string sceneName in sceneNames2)
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
        IntDict.AddRange(_sudokuIntNames, 0);
        IntDict.AddRange(_solitaireIntNames, 0);
        IntDict.AddRange(_minesweeperIntNames, 0);
        IntDict.AddRange(intNames2048, 0);
        IntDict.AddRange(SettingsIntNames, 0);
        IntDict.AddRange(userIntNames, 0);
        IntDict.AddRange(gekochteItems, 0);
        //String
        StringDict.AddRange(_sudokuStringNames, "");
        StringDict.AddRange(SettingsStringNames, "");
        StringDict.AddRange(_minesweeperStringNames, "");
        StringDict.AddRange(_solitaireStringNames, "");
        //Float
        FloatDict.AddRange(_solitaireFloatNames, 0f);
        FloatDict.AddRange(AchtergrondFloatNames, 0f);
        //Long
        LongDict.AddRange(userLongNames, 0);

        IntDict["kleur" + 9 + "gekocht"] = 1;
        IntDict["kleur" + 113 + "gekocht"] = 1;
        IntDict["kleur" + 136 + "gekocht"] = 1;
        IntDict["kleur" + 138 + "gekocht"] = 1;
        foreach (string sceneName in sceneNames)
        {
            IntDict["bgWaarde" + sceneName] = 9;
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
            if (IntDict["laatsteXOffline"] == 1)
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

        if (!dataGedownloaded) return;
        if (frameNaDownload == 5)
        {
            dataGedownloaded = false;
            LoadData();
            foreach (var n in sceneNames)
            {
                int soort = IntDict["bgSoort" + n];
                if (soort == 1)
                {
                    int waarde = IntDict["bgWaarde" + n];
                    gegevensHouder.ChangeSavedBackground(n.ToLower(), soort, waarde);
                }
                else
                {
                    int waarde = IntDict["bgWaarde" + n];
                    gegevensHouder.ChangeSavedBackground(n.ToLower(), soort, waarde);
                }
            }

            gegevensHouder.SetLanguage();
        }

        frameNaDownload += 1;
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
            case "minesweeper":
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
        foreach (string n in userLongNames)
        {
            data.Append(",,,long" + "///" + n + ":::" + LongDict[n]);
        }

        IntDict["laatsteXOffline"] = Application.internetReachability == NetworkReachability.NotReachable ||
                                     FirebaseAuth.DefaultInstance.CurrentUser == null
            ? 1
            : 0;
        foreach (string n in userIntNames)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
        }

        data.Append(",,,supportInfo///version:::" + Application.version);
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
        foreach (string n in AchtergrondFloatNames)
        {
            data.Append(",,,float" + "///" + n + ":::" + FloatDict[n]);
        }

        return data;
    }

    private StringBuilder SaveSettings()
    {
        StringBuilder data = new("@@@instellingen");
        foreach (string n in SettingsIntNames)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
        }

        foreach (string n in SettingsStringNames)
        {
            data.Append(",,,string" + "///" + n + ":::" + StringDict[n]);
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
        foreach (string n in _sudokuIntNames)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
        }

        foreach (string n in _sudokuStringNames)
        {
            data.Append(",,,string" + "///" + n + ":::" + StringDict[n]);
        }

        SaveData(data);
    }

    private void SaveSolitaire()
    {
        StringBuilder data = new("@@@solitaire");
        foreach (string n in _solitaireIntNames)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
        }

        foreach (string n in _solitaireFloatNames)
        {
            data.Append(",,,float" + "///" + n + ":::" + FloatDict[n]);
        }
        
        foreach (string n in _solitaireStringNames)
        {
            data.Append(",,,string" + "///" + n + ":::" + StringDict[n]);
        }

        SaveData(data);
    }

    private void SaveMijnenveger()
    {
        StringBuilder data = new("@@@mijnenveger");
        foreach (string n in _minesweeperIntNames)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
        }

        foreach (string n in _minesweeperStringNames)
        {
            data.Append(",,,string" + "///" + n + ":::" + StringDict[n]);
        }

        SaveData(data);
    }

    private void Save2048()
    {
        StringBuilder data = new("@@@2048");
        foreach (string n in intNames2048)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
        }

        SaveData(data);
    }

    private void SaveShop()
    {
        StringBuilder data = new("@@@shop");
        foreach (string n in gekochteItems)
        {
            data.Append(",,,int" + "///" + n + ":::" + IntDict[n]);
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
        if (Application.internetReachability == NetworkReachability.NotReachable ||
            FirebaseAuth.DefaultInstance.CurrentUser == null)
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
                string[] savedVarParts = varParts[1].Split(":::");
                switch (soort)
                {
                    case "int":
                    {
                        IntDict[savedVarParts[0]] = int.Parse(savedVarParts[1]);
                        break;
                    }
                    case "float":
                    {
                        FloatDict[savedVarParts[0]] = float.Parse(savedVarParts[1]);
                        break;
                    }
                    case "long":
                    {
                        FloatDict[savedVarParts[0]] = long.Parse(savedVarParts[1]);
                        break;
                    }
                    case "string":
                    {
                        StringDict[savedVarParts[0]] = savedVarParts[1];
                        break;
                    }
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
        StorageReference saveFile = storage.GetReferenceFromUrl(storage.RootReference + "/Users/" +
                                                                FirebaseAuth.DefaultInstance.CurrentUser.UserId +
                                                                "/save.vbg");
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
        StorageReference saveFile = storage.GetReferenceFromUrl(
            storage.RootReference + "/Users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId + "/save.vbg");
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

    public static string StringifyArray(int[] array, string divisor = "")
    {
        StringBuilder str = new();
        if (divisor.Equals(""))
        {
            foreach (var t in array)
                str.Append(t);
            return str.ToString();
        }

        foreach (var t in array)
        {
            str.Append(divisor);
            str.Append(t);
        }

        return str.ToString()[divisor.Length..];
    }
    
    public static string StringifyArray(string[] array, string divisor = "")
    {
        StringBuilder str = new();
        if (divisor.Equals(""))
        {
            foreach (var t in array)
                str.Append(t);
            return str.ToString();
        }

        foreach (var t in array)
        {
            str.Append(divisor);
            str.Append(t);
        }

        return str.ToString()[divisor.Length..];
    }
}