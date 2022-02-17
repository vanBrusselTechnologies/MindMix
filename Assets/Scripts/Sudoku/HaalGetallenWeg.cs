using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HaalGetallenWeg : MonoBehaviour
{
    [SerializeField] private GameObject[] vakjes;
    [HideInInspector] public List<GameObject> knoppen = new List<GameObject>();
    private readonly int[] diff = { 35, 40, 45, 50 };
    private int difficulty = 0;
    private PlaatsGetallen plaatsScript;
    [HideInInspector] public List<int> cijfers = new List<int>();
    private List<int> tot81 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 };
    //private readonly List<string> vakjesVolgorde = new List<string>() { "Linksboven", "Middenboven", "Rechtsboven", "Linksmidden", "Midden", "Rechtsmidden", "Linksonder", "Middenonder", "Rechtsonder" };
    private SaveScript saveScript;
    [SerializeField] private TMP_Dropdown dropdown;

    public void Start1a()
    {
        saveScript = GameObject.Find("gegevensHouder").GetComponent<SaveScript>();
        difficulty = saveScript.intDict["difficulty"];
        dropdown.value = difficulty;
        plaatsScript = GetComponent<PlaatsGetallen>();
        for (int i = 0; i < 81; i++)
        {
            cijfers.Add(plaatsScript.allCijfers[i]);
        }
        for (int i = 0; i < diff[difficulty]; i++)
        {
            int Rand = Random.Range(0, tot81.Count);
            cijfers[tot81[Rand]] = 0;
            tot81.Remove(tot81[Rand]);
        }
        int getalTot81 = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int a = 0; a < 9; a++)
            {
                saveScript.intDict["doorComputerIngevuldCijfer" + getalTot81] = cijfers[getalTot81];
                if (cijfers[getalTot81] != 0)
                {
                    TMP_Text b = vakjes[i].transform.GetChild(a).GetComponent<TMP_Text>();
                    b.text = cijfers[getalTot81].ToString();
                    b.fontStyle = FontStyles.Bold;
                    GameObject buttonObj = knoppen[getalTot81];
                    buttonObj.GetComponent<Button>().interactable = false;
                    buttonObj.GetComponent<Image>().type = Image.Type.Tiled;
                }
                getalTot81 += 1;
            }
        }
    }

    public void Start1b()
    {
        saveScript = GameObject.Find("gegevensHouder").GetComponent<SaveScript>();
        difficulty = saveScript.intDict["difficulty"];
        dropdown.value = difficulty;
        int getalTot81 = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int a = 0; a < 9; a++)
            {
                int cijfer = saveScript.intDict["doorComputerIngevuldCijfer" + getalTot81];
                if (cijfer != 0)
                {
                    TMP_Text b = vakjes[i].transform.GetChild(a).GetComponent<TMP_Text>();
                    b.text = cijfer.ToString();
                    cijfers.Add(cijfer);
                    b.fontStyle = FontStyles.Bold;
                    GameObject buttonObj = knoppen[getalTot81];
                    buttonObj.GetComponent<Button>().interactable = false;
                    buttonObj.GetComponent<Image>().type = Image.Type.Tiled;
                }
                else if (saveScript.intDict["DoorSpelerIngevuldBij" + getalTot81] != 0)
                {
                    RectTransform rect = knoppen[getalTot81].transform.GetChild(0).GetComponent<RectTransform>();
                    rect.offsetMax = new Vector2(0, 0);
                    rect.offsetMin = new Vector2(0, 0);
                    TMP_Text text = knoppen[getalTot81].transform.GetChild(0).GetComponent<TMP_Text>();
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = 250;
                    int doorSpelerIngevuld = saveScript.intDict["DoorSpelerIngevuldBij" + getalTot81];
                    text.text = doorSpelerIngevuld.ToString();
                    cijfers.Add(doorSpelerIngevuld);
                }
                else
                {
                    string een = saveScript.stringDict["Button " + getalTot81 + " notitie1"];
                    string twee = saveScript.stringDict["Button " + getalTot81 + " notitie2"];
                    string drie = saveScript.stringDict["Button " + getalTot81 + " notitie3"];
                    string vier = saveScript.stringDict["Button " + getalTot81 + " notitie4"];
                    string vijf = saveScript.stringDict["Button " + getalTot81 + " notitie5"];
                    string zes = saveScript.stringDict["Button " + getalTot81 + " notitie6"];
                    string zeven = saveScript.stringDict["Button " + getalTot81 + " notitie7"];
                    string acht = saveScript.stringDict["Button " + getalTot81 + " notitie8"];
                    string negen = saveScript.stringDict["Button " + getalTot81 + " notitie9"];
                    if (!(een == "  " && twee == "  " && drie == "  " && vier == "  " && vijf == "  " && zes == "  " && zeven == "  " && acht == "  " && negen == "  "))
                    {
                        RectTransform rect = knoppen[getalTot81].transform.GetChild(0).GetComponent<RectTransform>();
                        rect.offsetMax = new Vector2(0, 0);
                        rect.offsetMin = new Vector2(-33f, 0);
                        TMP_Text text = knoppen[getalTot81].transform.GetChild(0).GetComponent<TMP_Text>();
                        text.alignment = TextAlignmentOptions.Left;
                        text.fontSize = 80;
                        string notitie = een + "  " + twee + "  " + drie + "\n" + vier + "  " + vijf + "  " + zes + "\n" + zeven + "  " + acht + "  " + negen;
                        text.text = notitie;
                    }
                    cijfers.Add(0);
                }
                getalTot81 += 1;
            }
        }
    }
}