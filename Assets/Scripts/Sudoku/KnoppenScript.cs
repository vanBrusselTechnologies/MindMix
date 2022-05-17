using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class KnoppenScript : BaseUIHandler
{
    [Header("Other scene specific")]
    [SerializeField] private GameObject achtergrondNormaalOfNotitieKnop;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private MeshRenderer normaalKnopMesh;
    [SerializeField] private MeshRenderer notitieKnopMesh;

    [HideInInspector] public List<GameObject> knoppen;
    private int gekozendifficulty;
    [HideInInspector] public int knopIndex = -1;
    [HideInInspector] public bool isButtonSelected = false;
    private List<int> buttonsGehad = new List<int>();
    private bool naamIsEventSystem = true;
    Color dubbelGetalKleurRood = new Color(1f, 0, 0, 1f);
    Color normaalVakjesKleur = new Color(175f / 255f, 175f / 255f, 175f / 255f, 60f / 255f);

    private PlaatsGetallen plaatsScript;
    private AfScript afScript;
    private SudokuLayout sudokuLayout;

    protected override void SetLayout()
    {
        sudokuLayout.SetLayout();
    }

    // Use this for initialization
    public void Startt()
    {
        if (saveScript == null) return;
        plaatsScript = GetComponent<PlaatsGetallen>();
        afScript = GetComponent<AfScript>();
        sudokuLayout = GetComponent<SudokuLayout>();
        dropdown.value = saveScript.intDict["difficulty"];
        buttonsGehad.Clear();
        for (int i = 1; i < 82; i++)
        {
            saveScript.intDict["Button " + i] = 0;
        }
        saveScript.intDict["DeI"] = 0;
        if (!gameObject.name.Equals("EventSystem"))
        {
            naamIsEventSystem = false;
            return;
        }
        if (saveScript.intDict["dubbelGetalWarningIsOn"] == 1)
        {
            CheckVoorDubbelGetal();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!naamIsEventSystem || finishedGameUIObj.activeInHierarchy || saveScript == null)
        {
            return;
        }
        int DeI = saveScript.intDict["DeI"];
        if (knoppen[DeI] == gameObject)
        {
            if (buttonsGehad.IndexOf(DeI) == -1)
            {
                buttonsGehad.Add(DeI);
                saveScript.stringDict["Button " + DeI + " notitie1"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie2"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie3"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie4"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie5"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie6"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie7"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie8"] = "  ";
                saveScript.stringDict["Button " + DeI + " notitie9"] = "  ";
            }
        }
    }

    public void nieuweSudoku()
    {
        gekozendifficulty = dropdown.value;
        saveScript.intDict["difficulty"] = gekozendifficulty;
        nogEenSudoku();
    }

    public void nogEenSudoku()
    {
        gegevensHouder.startNewSudoku = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void moeilijkerSudoku()
    {
        int difficulty = saveScript.intDict["difficulty"];
        saveScript.intDict["difficulty"] = difficulty + 1;
        nogEenSudoku();
    }

    public void isSelected()
    {
        GameObject selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < 81; i++)
        {
            if (knoppen[i] == selectedButton)
            {
                saveScript.intDict["Button " + i] = 1;
                saveScript.intDict["DeI"] = i;
            }
            else
            {
                saveScript.intDict["Button " + i] = 0;
            }
        }
    }

    public void getalGeklikt()
    {
        string selectedNumber = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        int dei = saveScript.intDict["DeI"];
        if (saveScript.intDict["Button " + dei] == 1)
        {
            if (plaatsScript.NormaalGetal)
            {
                RectTransform rect = knoppen[dei].transform.GetChild(0).GetComponent<RectTransform>();
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, 0);
                TMP_Text text = knoppen[dei].transform.GetChild(0).GetComponent<TMP_Text>();
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 285;
                text.text = selectedNumber;
                saveScript.intDict["DoorSpelerIngevuldBij" + dei] = int.Parse(selectedNumber);
                afScript.getallen[dei] = int.Parse(selectedNumber);
                if (saveScript.intDict["notitieBijwerkSettingIsOn"] == 1)
                {
                    WerkNotitiesBij(dei);
                }
            }
            else
            {
                RectTransform rect = knoppen[dei].transform.GetChild(0).GetComponent<RectTransform>();
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(-33f, 0);
                TMP_Text text = knoppen[dei].transform.GetChild(0).GetComponent<TMP_Text>();
                text.alignment = TextAlignmentOptions.Left;
                text.fontSize = 80;
                string een = saveScript.stringDict["Button " + dei + " notitie1"];
                string twee = saveScript.stringDict["Button " + dei + " notitie2"];
                string drie = saveScript.stringDict["Button " + dei + " notitie3"];
                string vier = saveScript.stringDict["Button " + dei + " notitie4"];
                string vijf = saveScript.stringDict["Button " + dei + " notitie5"];
                string zes = saveScript.stringDict["Button " + dei + " notitie6"];
                string zeven = saveScript.stringDict["Button " + dei + " notitie7"];
                string acht = saveScript.stringDict["Button " + dei + " notitie8"];
                string negen = saveScript.stringDict["Button " + dei + " notitie9"];
                switch (selectedNumber)
                {
                    case "1": een = (een == "  ") ? "1" : "  "; break;
                    case "2": twee = (twee == "  ") ? "2" : "  "; break;
                    case "3": drie = (drie == "  ") ? "3" : "  "; break;
                    case "4": vier = (vier == "  ") ? "4" : "  "; break;
                    case "5": vijf = (vijf == "  ") ? "5" : "  "; break;
                    case "6": zes = (zes == "  ") ? "6" : "  "; break;
                    case "7": zeven = (zeven == "  ") ? "7" : "  "; break;
                    case "8": acht = (acht == "  ") ? "8" : "  "; break;
                    case "9": negen = (negen == "  ") ? "9" : "  "; break;
                }
                saveScript.stringDict["Button " + dei + " notitie1"] = een;
                saveScript.stringDict["Button " + dei + " notitie2"] = twee;
                saveScript.stringDict["Button " + dei + " notitie3"] = drie;
                saveScript.stringDict["Button " + dei + " notitie4"] = vier;
                saveScript.stringDict["Button " + dei + " notitie5"] = vijf;
                saveScript.stringDict["Button " + dei + " notitie6"] = zes;
                saveScript.stringDict["Button " + dei + " notitie7"] = zeven;
                saveScript.stringDict["Button " + dei + " notitie8"] = acht;
                saveScript.stringDict["Button " + dei + " notitie9"] = negen;
                string a = een + "  " + twee + "  " + drie + "\n" +
                           vier + "  " + vijf + "  " + zes + "\n" +
                           zeven + "  " + acht + "  " + negen;
                text.text = a;
                saveScript.intDict["DoorSpelerIngevuldBij" + dei] = 0;
                afScript.getallen[dei] = 0;
            }
            if (saveScript.intDict["dubbelGetalWarningIsOn"] == 1)
            {
                CheckVoorDubbelGetal();
            }
        }
        afScript.ietsVeranderd = true;
    }

    public void NormaalOfNotitie()
    {
        achtergrondNormaalOfNotitieKnop.transform.Rotate(new Vector3(0, 180, 180));
        plaatsScript.NormaalGetal = !plaatsScript.NormaalGetal;
        if (!plaatsScript.NormaalGetal)
        {
            notitieKnopMesh.enabled = true;
            normaalKnopMesh.enabled = false;
        }
        else
        {
            notitieKnopMesh.enabled = false;
            normaalKnopMesh.enabled = true;
        }
    }

    public override void OpenSettings()
    {
        bool settingObjActive = settingsCanvasObj.activeSelf;
        base.OpenSettings();
        if (settingObjActive)
        {
            if (saveScript.intDict["dubbelGetalWarningIsOn"] == 1)
            {
                CheckVoorDubbelGetal();
            }
            else
            {
                for (int dei = 0; dei < 81; dei++)
                {
                    Button knop = knoppen[dei].GetComponent<Button>();
                    ColorBlock colorBlock = knop.colors;
                    colorBlock.normalColor = normaalVakjesKleur;
                    knop.colors = colorBlock;
                }
            }
        }
    }

    private List<int> KrijgVakEnRijEnKolomVanDeI(int dei)
    {
        List<int> vakRijEnKolom = new List<int>();
        int vak = dei / 9;
        int rij = vak / 3 * 3 + dei % 9 / 3 + 1;
        int kolom = vak % 3 * 3 + dei % 9 % 3 + 1;
        vakRijEnKolom.Add(vak + 1);
        vakRijEnKolom.Add(rij);
        vakRijEnKolom.Add(kolom);
        return vakRijEnKolom;
    }

    private void WerkNotitiesBij(int dei)
    {
        List<int> vakRijEnKolom = KrijgVakEnRijEnKolomVanDeI(dei);
        int vak = vakRijEnKolom[0];
        int rij = vakRijEnKolom[1];
        int kolom = vakRijEnKolom[2];
        List<int> kolomGetallen = plaatsScript.KrijgGetallenTot81(kolom, 0, 1, 1);
        List<int> rijGetallen = plaatsScript.KrijgGetallenTot81(0, rij, 1, 1);
        List<int> vakGetallen = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            vakGetallen.Add((vak - 1) * 9 + i);
        }
        foreach (int getal in kolomGetallen)
        {
            if (getal != dei)
            {
                if (afScript.getallen[getal] == 0)
                {
                    if (saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] != "  ")
                    {
                        saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] = "  ";
                        string een = saveScript.stringDict["Button " + getal + " notitie1"];
                        string twee = saveScript.stringDict["Button " + getal + " notitie2"];
                        string drie = saveScript.stringDict["Button " + getal + " notitie3"];
                        string vier = saveScript.stringDict["Button " + getal + " notitie4"];
                        string vijf = saveScript.stringDict["Button " + getal + " notitie5"];
                        string zes = saveScript.stringDict["Button " + getal + " notitie6"];
                        string zeven = saveScript.stringDict["Button " + getal + " notitie7"];
                        string acht = saveScript.stringDict["Button " + getal + " notitie8"];
                        string negen = saveScript.stringDict["Button " + getal + " notitie9"];
                        string a = een + "  " + twee + "  " + drie + "\n" +
                                   vier + "  " + vijf + "  " + zes + "\n" +
                                   zeven + "  " + acht + "  " + negen;
                        TMP_Text text = knoppen[getal].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.text = a;
                    }
                }
            }
        }
        foreach (int getal in rijGetallen)
        {
            if (getal != dei)
            {
                if (afScript.getallen[getal] == 0)
                {
                    if (saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] != "  ")
                    {
                        saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] = "  ";
                        string een = saveScript.stringDict["Button " + getal + " notitie1"];
                        string twee = saveScript.stringDict["Button " + getal + " notitie2"];
                        string drie = saveScript.stringDict["Button " + getal + " notitie3"];
                        string vier = saveScript.stringDict["Button " + getal + " notitie4"];
                        string vijf = saveScript.stringDict["Button " + getal + " notitie5"];
                        string zes = saveScript.stringDict["Button " + getal + " notitie6"];
                        string zeven = saveScript.stringDict["Button " + getal + " notitie7"];
                        string acht = saveScript.stringDict["Button " + getal + " notitie8"];
                        string negen = saveScript.stringDict["Button " + getal + " notitie9"];
                        string a = een + "  " + twee + "  " + drie + "\n" +
                                   vier + "  " + vijf + "  " + zes + "\n" +
                                   zeven + "  " + acht + "  " + negen;
                        TMP_Text text = knoppen[getal].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.text = a;
                    }
                }
            }
        }
        foreach (int getal in vakGetallen)
        {
            if (getal != dei)
            {
                if (afScript.getallen[getal] == 0)
                {
                    if (saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] != "  ")
                    {
                        saveScript.stringDict["Button " + getal + " notitie" + afScript.getallen[dei]] = "  ";
                        string een = saveScript.stringDict["Button " + getal + " notitie1"];
                        string twee = saveScript.stringDict["Button " + getal + " notitie2"];
                        string drie = saveScript.stringDict["Button " + getal + " notitie3"];
                        string vier = saveScript.stringDict["Button " + getal + " notitie4"];
                        string vijf = saveScript.stringDict["Button " + getal + " notitie5"];
                        string zes = saveScript.stringDict["Button " + getal + " notitie6"];
                        string zeven = saveScript.stringDict["Button " + getal + " notitie7"];
                        string acht = saveScript.stringDict["Button " + getal + " notitie8"];
                        string negen = saveScript.stringDict["Button " + getal + " notitie9"];
                        string a = een + "  " + twee + "  " + drie + "\n" +
                                   vier + "  " + vijf + "  " + zes + "\n" +
                                   zeven + "  " + acht + "  " + negen;
                        TMP_Text text = knoppen[getal].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.text = a;
                    }
                }
            }
        }
    }

    public void CheckVoorDubbelGetal(int dei = 0)
    {
        if (dei == 81) return;
        List<int> vakRijEnKolom = KrijgVakEnRijEnKolomVanDeI(dei);
        int vak = vakRijEnKolom[0];
        int rij = vakRijEnKolom[1];
        int kolom = vakRijEnKolom[2];
        List<int> kolomGetallen = plaatsScript.KrijgGetallenTot81(kolom, 0, 1, 1);
        List<int> rijGetallen = plaatsScript.KrijgGetallenTot81(0, rij, 1, 1);
        List<int> vakGetallen = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            vakGetallen.Add((vak - 1) * 9 + i);
        }
        Button knop = knoppen[dei].GetComponent<Button>();
        ColorBlock colorBlock = knop.colors;
        foreach (int getal2 in kolomGetallen)
        {
            if (dei == getal2) {; }
            else if (afScript.getallen[dei] == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    string _ = saveScript.stringDict["Button " + dei + " notitie" + i].Trim();
                    if (!_.Equals(""))
                    {
                        int __ = int.Parse(_);
                        if (__ == afScript.getallen[getal2])
                        {
                            colorBlock.normalColor = dubbelGetalKleurRood;
                            knop.colors = colorBlock;
                            CheckVoorDubbelGetal(dei + 1);
                            return;
                        }
                    }
                }
            }
            else if (afScript.getallen[dei] == afScript.getallen[getal2])
            {
                colorBlock.normalColor = dubbelGetalKleurRood;
                knop.colors = colorBlock;
                CheckVoorDubbelGetal(dei + 1);
                return;
            }
        }
        foreach (int getal2 in rijGetallen)
        {
            if (dei == getal2) {; }
            else if (afScript.getallen[dei] == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    string _ = saveScript.stringDict["Button " + dei + " notitie" + i].Trim();
                    if (!_.Equals(""))
                    {
                        int __ = int.Parse(_);
                        if (__ == afScript.getallen[getal2])
                        {
                            colorBlock.normalColor = dubbelGetalKleurRood;
                            knop.colors = colorBlock;
                            CheckVoorDubbelGetal(dei + 1);
                            return;
                        }
                    }
                }
            }
            else if (afScript.getallen[dei] == afScript.getallen[getal2])
            {
                colorBlock.normalColor = dubbelGetalKleurRood;
                knop.colors = colorBlock;
                CheckVoorDubbelGetal(dei + 1);
                return;
            }
        }
        foreach (int getal2 in vakGetallen)
        {
            if (dei == getal2) {; }
            else if (afScript.getallen[dei] == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    string _ = saveScript.stringDict["Button " + dei + " notitie" + i].Trim();
                    if (!_.Equals(""))
                    {
                        int __ = int.Parse(_);
                        if (__ == afScript.getallen[getal2])
                        {
                            colorBlock.normalColor = dubbelGetalKleurRood;
                            knop.colors = colorBlock;
                            CheckVoorDubbelGetal(dei + 1);
                            return;
                        }
                    }
                }
            }
            else if (afScript.getallen[dei] == afScript.getallen[getal2])
            {
                colorBlock.normalColor = dubbelGetalKleurRood;
                knop.colors = colorBlock;
                CheckVoorDubbelGetal(dei + 1);
                return;
            }
        }
        colorBlock.normalColor = normaalVakjesKleur;
        knop.colors = colorBlock;
        CheckVoorDubbelGetal(dei + 1);
    }
}