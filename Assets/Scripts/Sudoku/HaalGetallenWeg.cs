using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HaalGetallenWeg : MonoBehaviour
{
    [SerializeField] private GameObject[] vakjes;
    [HideInInspector] public List<GameObject> knoppen = new List<GameObject>();
    private readonly int[] diff = { 31, 41, 51, 56 };
    private int difficulty = 0;
    private PlaatsGetallen plaatsScript;
    [HideInInspector] public List<int> cijfers = new List<int>();
    private List<int> tot81 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 };
    private SaveScript saveScript;
    [SerializeField] private TMP_Dropdown dropdown;

    enum FillCombinationType
    {
        Box,
        Row,
        Column,
    }

    public void Start1a()
    {
        saveScript = SaveScript.Instance;
        difficulty = saveScript.intDict["difficulty"];
        plaatsScript = GetComponent<PlaatsGetallen>();
        for (int i = 0; i < 81; i++)
        {
            cijfers.Add(plaatsScript.allCijfers[i]);
        }
        for (int i = 0; i < diff[difficulty]; i++)
        {
            RemoveNumbers();
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
        saveScript = SaveScript.Instance;
        difficulty = saveScript.intDict["difficulty"];
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
                    Transform child = knoppen[getalTot81].transform.GetChild(0);
                    RectTransform rect = child.GetComponent<RectTransform>();
                    rect.offsetMax = new Vector2(0, 0);
                    rect.offsetMin = new Vector2(0, 0);
                    TMP_Text text = child.GetComponent<TMP_Text>();
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = 285;
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
                        Transform child = knoppen[getalTot81].transform.GetChild(0);
                        RectTransform rect = child.GetComponent<RectTransform>();
                        rect.offsetMax = new Vector2(0, 0);
                        rect.offsetMin = new Vector2(-33f, 0);
                        TMP_Text text = child.GetComponent<TMP_Text>();
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

    int removedNumbers = 0;
    List<int> notYetRemovedNumbers = new List<int>();
    public void RemoveNumbers()
    {
        if (tot81.Count == 0) return;
        int Rand = Random.Range(0, tot81.Count);
        if (CanBeRemoved(tot81[Rand]))
        {
            cijfers[tot81[Rand]] = 0;
            tot81.Remove(tot81[Rand]);
            removedNumbers++;
        }
        else
        {
            if (tot81.Count > 1)
            {
                notYetRemovedNumbers.Add(tot81[Rand]);
                tot81.Remove(tot81[Rand]);
                RemoveNumbers();
            }
            else
            {
                tot81.Clear();
                for(int i = 0; i < diff[difficulty] - removedNumbers; i++)
                {
                    int rand = Random.Range(0, notYetRemovedNumbers.Count);
                    cijfers[notYetRemovedNumbers[rand]] = 0;
                    notYetRemovedNumbers.Remove(rand);
                }
            }
        }
    }

    public bool CanBeRemoved(int cellNumber)
    {
        List<int> boxRowColumn = GetBoxRowColumn(cellNumber);
        List<int> rowNumbers = GetRowNumbers(boxRowColumn[1]);
        List<int> columnNumbers = GetColumnNumbers(boxRowColumn[2]);
        List<int> boxNumbers = GetBoxNumbers(boxRowColumn[0] - 1);
        if (difficulty >= 2) if (HardCombinationPossible(boxNumbers, rowNumbers, columnNumbers)) return true;
        if (difficulty >= 1) if (NormalCombinationPossible(rowNumbers, columnNumbers)) return true;
        if (EasyCombinationPossible(boxNumbers, rowNumbers, columnNumbers)) return true;
        if(EasyFillPossible(boxNumbers, rowNumbers, columnNumbers)) return true;
        return false;
    }

    private List<int> GetBoxRowColumn(int dei)
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

    private bool EasyFillPossible(List<int> boxNumbers, List<int> rowNumbers, List<int> columnNumbers)
    {
        bool possible;
        for (int a = 0; a < 3; a++)
        {
            List<int> numbers = a == 0 ? boxNumbers : a == 1 ? rowNumbers : columnNumbers;
            possible = true;
            for (int i = 0; i < 9; i++)
            {
                if (cijfers[numbers[i]] == 0)
                {
                    possible = false;
                    break;
                }
            }
            if (possible) return true;
        }
        return false;
    }

    private bool EasyCombinationPossible(List<int> boxNumbers, List<int> rowNumbers, List<int> columnNumbers)
    {
        return BoxFillCombinationPossible(boxNumbers, rowNumbers, columnNumbers);
    }

    private bool NormalCombinationPossible(List<int> rowNumbers, List<int> columnNumbers)
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            if (RowFillCombinationPossible(rowNumbers, columnNumbers)) return true;
            return ColumnFillCombinationPossible(rowNumbers, columnNumbers);
        }
        else
        {
            if (ColumnFillCombinationPossible(rowNumbers, columnNumbers)) return true;
            return RowFillCombinationPossible(rowNumbers, columnNumbers);
        }
    }

    private bool HardCombinationPossible(List<int> boxNumbers, List<int> rowNumbers, List<int> columnNumbers)
    {
        return OnlyOnePossible(boxNumbers, rowNumbers, columnNumbers);
    }

    private int GetCellNumber(List<int> rowNumbers, List<int> columnNumbers)
    {
        for (int i = 0; i < rowNumbers.Count; i++)
        {
            for (int j = 0; j < columnNumbers.Count; j++)
            {
                if (rowNumbers[i] == columnNumbers[j])
                {
                    return rowNumbers[i];
                }
            }
        }
        return -1;
    }

    private int GetCellValue(List<int> rowNumbers, List<int> columnNumbers)
    {
        int cellNumber = GetCellNumber(rowNumbers, columnNumbers);
        return cijfers[cellNumber];
    }

    private List<int> GetBoxNumbers(int box)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            result.Add(box * 9 + i);
        }
        return result;
    }

    private List<int> GetRowNumbers(int row)
    {
        return plaatsScript.KrijgGetallenTot81(0, row, 1, 1);
    }

    private List<int> GetColumnNumbers(int column)
    {
        return plaatsScript.KrijgGetallenTot81(column, 0, 1, 1);
    }

    private List<int> GetEmptyCells(List<int> list)
    {
        List<int> emptyCells = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            if (cijfers[list[i]] == 0)
            {
                emptyCells.Add(list[i]);
            }
        }
        return emptyCells;
    }

    private bool BoxFillCombinationPossible(List<int> boxNumbers, List<int> rowNumbers, List<int> columnNumbers)
    {
        return FillCombinationPossible(rowNumbers, columnNumbers, GetEmptyCells(boxNumbers), FillCombinationType.Box);
    }
    
    private bool RowFillCombinationPossible(List<int> rowNumbers, List<int> columnNumbers)
    {
        return FillCombinationPossible(rowNumbers, columnNumbers, GetEmptyCells(rowNumbers), FillCombinationType.Row);
    }

    private bool ColumnFillCombinationPossible(List<int> rowNumbers, List<int> columnNumbers)
    {
        return FillCombinationPossible(rowNumbers, columnNumbers, GetEmptyCells(columnNumbers), FillCombinationType.Column);
    }

    private bool FillCombinationPossible(List<int> rowNumbers, List<int> columnNumbers, List<int> emptyCells, FillCombinationType type)
    {
        int cellValue = GetCellValue(rowNumbers, columnNumbers);
        int cellNumber = GetCellNumber(rowNumbers, columnNumbers);
        for (int i = 0; i < emptyCells.Count; i++)
        {
            List<int> tmpBoxRowColumn = GetBoxRowColumn(emptyCells[i]);
            List<int> list1 = type == FillCombinationType.Box ? GetRowNumbers(tmpBoxRowColumn[1]) : GetBoxNumbers(tmpBoxRowColumn[0] - 1);
            List<int> list2 = type == FillCombinationType.Column ? GetRowNumbers(tmpBoxRowColumn[1]) : GetColumnNumbers(tmpBoxRowColumn[2]);
            bool possible1 = false;
            bool possible2 = false;
            for (int j = 0; j < 9; j++)
            {
                if (list1[j] != cellNumber)
                {
                    if(cijfers[list1[j]] != 0)
                    {
                        if (cijfers[list1[j]] == cellValue)
                        {
                            possible1 = true;
                            break;
                        }
                    }
                }
                if (list2[j] != cellNumber)
                {
                    if (cijfers[list2[j]] != 0)
                    {
                        if (cijfers[list2[j]] == cellValue)
                        {
                            possible2 = true;
                            break;
                        }
                    }
                }
            }
            if (!possible1 && !possible2) return false;
        }
        return true;
    }

    private bool OnlyOnePossible(List<int> boxNumbers, List<int> rowNumbers, List<int> columnNumbers)
    {
        int cellValue = GetCellValue(rowNumbers, columnNumbers);
        List<int> till9 = new List<int>() { 1,2,3,4,5,6,7,8,9};
        till9.Remove(cellValue);
        for (int i = 0; i < 9; i++)
        {
            till9.Remove(cijfers[boxNumbers[i]]);
            till9.Remove(cijfers[rowNumbers[i]]);
            till9.Remove(cijfers[columnNumbers[i]]);
            if (till9.Count == 0) return true;
        }
        return false;
    }
}