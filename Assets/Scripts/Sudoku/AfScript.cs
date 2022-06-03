using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AfScript : MonoBehaviour
{
    private PlaatsGetallen plaatsScript;
    private HaalGetallenWeg weghaalScript;
    private KnoppenScript knoppenScript;
    private SudokuLayout sudokuLayout;
    private BeloningScript beloningScript;
    [HideInInspector] public List<int> getallen;
    private GegevensHouder gegevensHouder;
    [SerializeField] private GameObject gehaaldCanvas;
    [SerializeField] private GameObject sudokuCanvas;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private GameObject menuUICanvasObj;
    [SerializeField] private List<GameObject> knoppen = new List<GameObject>();
    [HideInInspector] public bool ietsVeranderd = true;
    private SaveScript saveScript;
    [SerializeField] private TMP_Text beloningText;

    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        gegevensHouder = GegevensHouder.Instance;
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        saveScript = SaveScript.Instance;
        beloningScript = BeloningScript.Instance;
        getallen = new List<int>();
        for (int i = 0; i < 81; i++)
        {
            getallen.Add(0);
        }
        plaatsScript = GetComponent<PlaatsGetallen>();
        weghaalScript = GetComponent<HaalGetallenWeg>();
        knoppenScript = GetComponent<KnoppenScript>();
        sudokuLayout = GetComponent<SudokuLayout>();
        weghaalScript.knoppen = knoppen;
        knoppenScript.knoppen = knoppen;
        if (gegevensHouder.startNewGame)
        {
            ClearProgress();
            plaatsScript.Start0();
            weghaalScript.Start1a();
        }
        else
        {
            weghaalScript.Start1b();
        }
        for (int i = 0; i < weghaalScript.cijfers.Count; i++)
        {
            getallen[i] = weghaalScript.cijfers[i];
        }
        knoppenScript.Startt();
    }

    private void Update()
    {
        if (gehaaldCanvas.activeInHierarchy)
        {
            return;
        }
        if (ietsVeranderd)
        {
            ietsVeranderd = false;
            int aantalkloppend = 0;
            for (int i = 0; i < getallen.Count; i++)
            {
                int pcCijfer = saveScript.intDict["doorComputerIngevuldCijfer" + i];
                int spelerCijfer = saveScript.intDict["DoorSpelerIngevuldBij" + i];
                if (pcCijfer == 0 && spelerCijfer == 0)
                {
                    return;
                }
                int juisteCijfer = saveScript.intDict["kloppendCijferBijInt" + i];
                if (pcCijfer - juisteCijfer == 0 || spelerCijfer - juisteCijfer == 0)
                {
                    aantalkloppend += 1;
                }
            }
            if (aantalkloppend == 81)
            {
                Scene scene = SceneManager.GetActiveScene();
                int diff = saveScript.intDict["difficulty"];
                beloningText.text = beloningScript.Beloning(scene: scene, difficulty: diff, doelwitText: beloningText).ToString();
                saveScript.intDict["SudokuDiff" + diff + "Gespeeld"] += 1;
                saveScript.intDict["SudokusGespeeld"] += 1;
                OpenGehaaldCanvas();
            }
            else
            {
                for(int i = 1; i <= 9; i++)
                {
                    List<int> rijNummers = plaatsScript.KrijgGetallenTot81(0, i, 1, 1);
                    List<int> rijWaardes = new List<int>();
                    foreach(int nummer in rijNummers)
                    {
                        rijWaardes.Add(getallen[nummer]);
                    }
                    rijWaardes.Sort();
                    for (int a = 0; a < rijWaardes.Count; a++)
                    {
                        int waarde = a + 1;
                        if (rijWaardes[a] != waarde)
                        {
                            return;
                        }
                    }
                    List<int> kolomNummers = plaatsScript.KrijgGetallenTot81(i, 0, 1, 1);
                    List<int> kolomWaardes = new List<int>();
                    foreach (int nummer in kolomNummers)
                    {
                        kolomWaardes.Add(getallen[nummer]);
                    }
                    kolomWaardes.Sort();
                    for (int a = 0; a < kolomWaardes.Count; a++)
                    {
                        int waarde = a + 1;
                        if (kolomWaardes[a] != waarde)
                        {
                            return;
                        }
                    }
                    List<int> vakjeWaardes = new List<int>();
                    for (int a = 0; a < 9; a++)
                    {
                        vakjeWaardes.Add(getallen[a + (i - 1) * 9]);
                    }
                    vakjeWaardes.Sort();
                    for (int a = 0; a < vakjeWaardes.Count; a++)
                    {
                        int waarde = a + 1;
                        if (vakjeWaardes[a] != waarde)
                        {
                            return;
                        }
                    }
                }
                Scene scene = SceneManager.GetActiveScene();
                int diff = saveScript.intDict["difficulty"];
                beloningText.text = beloningScript.Beloning(scene: scene, difficulty: diff, doelwitText: beloningText).ToString();
                saveScript.intDict["SudokuDiff" + diff + "Gespeeld"] += 1;
                saveScript.intDict["SudokusGespeeld"] += 1;
                OpenGehaaldCanvas();
            }
        }
    }

    public void OpenGehaaldCanvas()
    {
        ClearProgress();
        gehaaldCanvas.SetActive(true);
        sudokuCanvas.SetActive(false);
        overigCanvas.SetActive(false);
        menuUICanvasObj.SetActive(false);
        sudokuLayout.SetLayout();
    }

    private void ClearProgress()
    {
        for (int i = 0; i < 81; i++)
        {
            saveScript.intDict["kloppendCijferBijInt" + i] = 0;
            saveScript.intDict["Button " + i] = 0;
            saveScript.intDict["DoorSpelerIngevuldBij" + i] = 0;
            for (int ii = 1; ii < 10; ii++)
            {
                saveScript.stringDict["Button " + i + " notitie" + ii] = "  ";
            }
        }
    }
}