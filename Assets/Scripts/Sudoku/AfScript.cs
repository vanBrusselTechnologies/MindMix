using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AfScript : MonoBehaviour
{
    private PlaatsGetallen plaatsScript;
    private HaalGetallenWeg weghaalScript;
    private KnoppenScript knoppenScript;
    private BeloningScript beloningScript;
    [HideInInspector] public List<int> getallen;
    private GegevensHouder gegevensScript;
    public GameObject GehaaldCanvas;
    [SerializeField] private GameObject SudokuCanvas;
    [SerializeField] private GameObject OverigCanvas;
    [SerializeField] private List<GameObject> knoppen = new List<GameObject>();
    [HideInInspector] public bool ietsVeranderd = true;
    private SaveScript saveScript;
    public Transform buttonHouder;
    [SerializeField] private TMP_Text beloningText;
    [SerializeField] private RectTransform gehaaldCanvasTitelRect;
    [SerializeField] private RectTransform gehaaldCanvasTekstRect;
    [SerializeField] private RectTransform gehaaldCanvasStartNieuweKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasStartMoeilijkerKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasNaarMenuKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasRewardRect;
    [SerializeField] private GameObject gehaaldCanvasRewardVerdubbelObj;

    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        beloningScript = gegevensHouder.GetComponent<BeloningScript>();
        getallen = new List<int>();
        for (int i = 0; i < 81; i++)
        {
            getallen.Add(0);
        }
        plaatsScript = GetComponent<PlaatsGetallen>();
        weghaalScript = GetComponent<HaalGetallenWeg>();
        knoppenScript = GetComponent<KnoppenScript>();
        weghaalScript.knoppen = knoppen;
        knoppenScript.knoppen = knoppen;
        if (gegevensScript.startNewSudoku)
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
        if (GehaaldCanvas.activeInHierarchy)
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
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        GehaaldCanvas.SetActive(true);
        gehaaldCanvasTitelRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * (25f / 30f)) - (Screen.height / 2f));
        gehaaldCanvasTitelRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height * (10f / 30f));
        float kleinsteKant = Mathf.Min(Screen.safeArea.height, Screen.safeArea.width);
        float grootsteKant = Mathf.Max(Screen.safeArea.height, Screen.safeArea.width);
        if (kleinsteKant - 1440 > 0)
        {
            float factor = Mathf.Min(kleinsteKant / 1500f, grootsteKant / 2500f);
            gehaaldCanvasTitelRect.localScale = Vector2.one * factor;
            gehaaldCanvasTitelRect.sizeDelta /= factor;
        }
        gehaaldCanvasTekstRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * (17f / 30f)) - (Screen.height / 2f));
        gehaaldCanvasTekstRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height * (8f / 30f));
        gehaaldCanvasRewardRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * (10f / 30f)) - (Screen.height / 2f));
        if (Application.internetReachability == NetworkReachability.NotReachable || !gehaaldCanvasRewardVerdubbelObj.activeInHierarchy)
        {
            gehaaldCanvasRewardVerdubbelObj.SetActive(false);
            float scaleFactor = Mathf.Min(Screen.safeArea.width * 0.85f / 500, Screen.safeArea.height * (5f / 30f) / 175);
            gehaaldCanvasRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            gehaaldCanvasRewardRect.sizeDelta = new Vector2(500, 175);
        }
        else
        {
            float scaleFactor = Mathf.Min(Screen.safeArea.width * 0.85f / 1000, Screen.safeArea.height * (5f / 30f) / 175);
            gehaaldCanvasRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            gehaaldCanvasRewardRect.sizeDelta = new Vector2(1000, 175);
        }
        if (saveScript.intDict["difficulty"] < 3)
        {
            float sizeX = Screen.safeArea.width * 0.3f;
            float sizeY = Screen.safeArea.height * (5f / 30f);
            float posY = safeZoneAntiY + (Screen.safeArea.height * (3.5f / 30f)) - (Screen.height / 2f);
            gehaaldCanvasStartNieuweKnopRect.anchoredPosition = new Vector2(safeZoneAntiX - (Screen.safeArea.width / 3f), posY);
            gehaaldCanvasStartNieuweKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
            gehaaldCanvasStartMoeilijkerKnopRect.anchoredPosition = new Vector2(safeZoneAntiX, posY);
            gehaaldCanvasStartMoeilijkerKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
            gehaaldCanvasNaarMenuKnopRect.anchoredPosition = new Vector2(safeZoneAntiX + (Screen.safeArea.width / 3f), posY);
            gehaaldCanvasNaarMenuKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
        }
        else
        {
            float sizeX = Screen.safeArea.width * 0.45f;
            float sizeY = Screen.safeArea.height * (5f / 30f);
            float posY = safeZoneAntiY + (Screen.safeArea.height * (3.5f / 30f)) - (Screen.height / 2f);
            gehaaldCanvasStartNieuweKnopRect.anchoredPosition = new Vector2(safeZoneAntiX - (Screen.safeArea.width / 4f), posY);
            gehaaldCanvasStartNieuweKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
            gehaaldCanvasStartMoeilijkerKnopRect.gameObject.SetActive(false);
            gehaaldCanvasNaarMenuKnopRect.anchoredPosition = new Vector2(safeZoneAntiX + (Screen.safeArea.width / 4f), posY);
            gehaaldCanvasNaarMenuKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
        }
        SudokuCanvas.SetActive(false);
        OverigCanvas.SetActive(false);
    }
}
