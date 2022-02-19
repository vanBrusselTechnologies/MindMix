using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MijnenVegerScript : MonoBehaviour
{
    [HideInInspector] public float buttonScale;
    [HideInInspector] public float mvSpeelveldLinks;
    [HideInInspector] public float mvSpeelveldRechts;
    [HideInInspector] public float mvSpeelveldBoven;
    [HideInInspector] public float mvSpeelveldOnder;
    [HideInInspector] public float mvKnopLinks;
    [HideInInspector] public float mvKnopRechts;
    [HideInInspector] public float mvKnopBoven;
    [HideInInspector] public float mvKnopOnder;
    private bool isAlGedrukt;
    private List<int> vakjesGehad = new List<int>();
    [HideInInspector] public List<Button> buttons = new List<Button>();
    private int kolom;
    private int rij;
    public Button knop;
    public Sprite vlag;
    public Sprite bom;
    [HideInInspector] public bool vlagNietSchep = false;
    private List<int> bommenList = new List<int>();
    private List<int> randVakjes = new List<int>();
    private int aantalBommen;
    public TMP_Text bommenTeGaan;
    [HideInInspector] public bool GameOver = false;
    private bool gameIsFinished = false;
    [SerializeField] private GameObject speelVeld;
    private MijnenVegerLayout mvLayout;
    [SerializeField] private GameObject uitlegUI;
    public GameObject gehaaldCanvas;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private GameObject MijnenvegerObj;
    private SaveScript saveScript;
    private GegevensHouder gegevensScript;
    private BeloningScript beloningScript;
    [SerializeField] private TMP_Text beloningText;
    [SerializeField] private RectTransform gehaaldCanvasTitelRect;
    [SerializeField] private RectTransform gehaaldCanvasWinstTekstRect;
    [SerializeField] private RectTransform gehaaldCanvasVerliesTekstRect;
    [SerializeField] private RectTransform gehaaldCanvasStartNieuweKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasStartMoeilijkerKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasNaarMenuKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasRewardRect;
    [SerializeField] private GameObject gehaaldCanvasRewardVerdubbelObj;

    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        beloningScript = gegevensHouder.GetComponent<BeloningScript>();
        saveScript.intDict["begonnenAanMV"] = 1;
        mvLayout = GetComponent<MijnenVegerLayout>();
        if (gegevensScript.startNewMV)
        {
            for (int i = 0; i < 100; i++)
            {
                saveScript.intDict["mijnenvegerBom" + i] = 0;
            }
        }
        buttonScale = mvLayout.breedteMvKnop;
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        GameOver = false;
        for (int i = 0; i <= mvLayout.korteKant * mvLayout.langeKant; i++)
        {
            if (gegevensScript.startNewMV)
            {
                saveScript.intDict["mijnenvegerVakjesgehad" + i] = 0;
                if (i == 0)
                {
                    for (int iii = 0; iii < 22; iii++)
                    {
                        for (int ii = 0; ii < 22; ii++)
                        {
                            int index = (iii * 100) + ii;
                            saveScript.intDict["mijnenvegerVlaggenGezet" + index] = 0;
                        }
                    }
                }
                randVakjes.Add(i);
            }
            else
            {
                if (saveScript.intDict["mijnenvegerVakjesgehad" + i] != 0)
                {
                    if (saveScript.intDict["mijnenvegerVakjesgehad" + i] == -1)
                    {
                        vakjesGehad.Add(0);
                    }
                    else
                    { 
                        vakjesGehad.Add(saveScript.intDict["mijnenvegerVakjesgehad" + i]);
                    }
                }
            }
        }
        aantalBommen = 25 + (int)(10f * Mathf.Pow(1.75f, saveScript.intDict["difficultyMijnenVeger"]));
        for (int i = 0; i < aantalBommen; i++)
        {
            if (gegevensScript.startNewMV)
            {
                int getal = randVakjes[Random.Range(0, randVakjes.Count)];
                randVakjes.Remove(getal);
                kolom = Mathf.FloorToInt(getal / mvLayout.langeKant);
                rij = getal % mvLayout.langeKant;
                saveScript.intDict["mijnenvegerBom" + i] = kolom + (rij * 100);
                bommenList.Add(kolom + (rij * 100));
            }
            else
            {
                bommenList.Add(saveScript.intDict["mijnenvegerBom" + i]);
            }
        }
        if (!gegevensScript.startNewMV)
        {
            for (int i = 0; i < vakjesGehad.Count; i++)
            {
                if (saveScript.intDict["mijnenvegerVlaggenGezet" + vakjesGehad[i]] != 1)
                {
                    Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
                    a.name = vakjesGehad[i].ToString();
                    buttons.Add(a);
                    if (mvLayout.korteKant > mvLayout.langeKant)
                    {
                        a.transform.localPosition = new Vector3((buttonScale * 75f) + (10.5f * mvLayout.langeKant * kolom * buttonScale), (buttonScale * 60f) + (mvLayout.korteKant * 5.55f * rij * buttonScale), -0.45f);
                    }
                    else
                    {
                        a.transform.localPosition = new Vector3((buttonScale * 60f) + (mvLayout.langeKant * 5.55f * (21 - rij) * buttonScale), (buttonScale * 75f) + (10.5f * mvLayout.korteKant * kolom * buttonScale), -0.45f);
                    }
                    a.transform.localScale = new Vector3(buttonScale * .5f, buttonScale * .5f, 1f);
                    rij = Mathf.FloorToInt(vakjesGehad[i] / 100f);
                    kolom = vakjesGehad[i] - (rij * 100);
                    float mines = GetMineCount(kolom, rij);
                    if (mines == 0)
                    {
                        a.GetComponentInChildren<TMP_Text>().text = "-";
                        a.tag = "Untagged";
                    }
                    else if (mines == 99)
                    {
                        a.GetComponentInChildren<TMP_Text>().text = "";
                        a.GetComponent<Image>().sprite = bom;
                        ShowAllBombs();
                        return;
                    }
                    else
                    {
                        a.GetComponentInChildren<TMP_Text>().text = mines.ToString();
                        a.tag = "Untagged";
                    }
                }
                else
                {
                    Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
                    a.name = vakjesGehad[i].ToString();
                    buttons.Add(a);
                    if (mvLayout.korteKant > mvLayout.langeKant)
                    {
                        a.transform.localPosition = new Vector3((buttonScale * 75f) + (10.5f * mvLayout.langeKant * kolom * buttonScale), (buttonScale * 60f) + (mvLayout.korteKant * 5.55f * rij * buttonScale), -0.45f);
                    }
                    else
                    {
                        a.transform.localPosition = new Vector3((buttonScale * 60f) + (mvLayout.langeKant * 5.55f * (21 - rij) * buttonScale), (buttonScale * 75f) + (10.5f * mvLayout.korteKant * kolom * buttonScale), -0.45f);
                    }
                    a.transform.localScale = new Vector3(buttonScale * .5f, buttonScale * .5f, 1f);
                    a.GetComponentInChildren<TMP_Text>().text = "";
                    a.GetComponent<Image>().sprite = vlag;
                    a.tag = "vlag";
                    aantalBommen -= 1;
                }
            }
        }
        bommenTeGaan.text = aantalBommen.ToString();
    }

    private void Update()
    {
        if (gameIsFinished) return;
        if (vakjesGehad.Count + int.Parse(bommenTeGaan.text) >= mvLayout.korteKant * mvLayout.langeKant && !GameOver)
        {
            Scene scene = SceneManager.GetActiveScene();
            int diff = saveScript.intDict["difficultyMijnenVeger"];
            saveScript.intDict["MijnenvegerDiff" + diff + "Gespeeld"] += 1;
            saveScript.intDict["MijnenvegersGespeeld"] += 1;
            beloningText.text = beloningScript.Beloning(scene: scene, difficulty: diff, doelwitText: beloningText).ToString();
            OpenGehaaldCanvas(true);
            gameIsFinished = true;
            //Reset all progress
            saveScript.intDict["begonnenAanMV"] = 0;
            for (int i = 0; i < 100; i++)
            {
                saveScript.intDict["mijnenvegerBom" + i] = 0;
            }
            for(int i =0; i < mvLayout.langeKant * mvLayout.korteKant; i++)
            {
                saveScript.intDict["mijnenvegerVakjesgehad" + i] = 0;
            }
            for (int i = 0; i < 22; i++)
            {
                for (int ii = 0; ii < 22; ii++)
                {
                    int index = (i * 100) + ii;
                    if (index == 0) index = -1;
                    saveScript.intDict["mijnenvegerVlaggenGezet" + index] = 0;
                }
            }
        }
        else if (GameOver)
        {
            gehaaldCanvasVerliesTekstRect.gameObject.SetActive(true);
            gehaaldCanvasWinstTekstRect.gameObject.SetActive(false);
            gehaaldCanvasRewardRect.gameObject.SetActive(false);
            StartCoroutine(WachtVoorGehaaldCanvas());
            gameIsFinished = true;
        }
        else
        {
            if (uitlegUI.activeSelf) return;
            if (Input.touchCount == 1 && !vlagNietSchep)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Ended)
                {
                    return;
                }
                Vector3 muisPos = Camera.main.ScreenToWorldPoint(touch.position);
                if (muisPos.x > mvSpeelveldLinks && muisPos.x < mvSpeelveldRechts && muisPos.y > mvSpeelveldOnder && muisPos.y < mvSpeelveldBoven)
                {
                    List<int> kolomRijLijst = KrijgVakBijPos(muisPos);
                    kolom = kolomRijLijst[0];
                    rij = kolomRijLijst[1];
                    if (!isAlGedrukt)
                    {
                        Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
                        int buttonNum = (kolom + (rij * 100));
                        if (buttonNum == 0) buttonNum = -1;
                        a.name = buttonNum.ToString();
                        buttons.Add(a);
                        if (mvLayout.korteKant > mvLayout.langeKant)
                        {
                            a.transform.localPosition = new Vector3((buttonScale * 75f) + (10.5f * mvLayout.langeKant * kolom * buttonScale), (buttonScale * 60f) + (mvLayout.korteKant * 5.55f * rij * buttonScale), -0.45f);
                        }
                        else
                        {
                            a.transform.localPosition = new Vector3((buttonScale * 60f) + (mvLayout.langeKant * 5.55f * (21 - rij) * buttonScale), (buttonScale * 75f) + (10.5f * mvLayout.korteKant * kolom * buttonScale), -0.45f);
                        }
                        a.transform.localScale = new Vector3(buttonScale * .5f, buttonScale * .5f, 1f);
                        int mines = GetMineCount(kolom, rij);
                        if (mines == 0)
                        {
                            a.GetComponentInChildren<TMP_Text>().text = "-";
                            a.tag = "Untagged";
                            VindMeerVakkenZonderBomBuren(kolom, rij);
                        }
                        else if (mines == 99)
                        {
                            a.GetComponentInChildren<TMP_Text>().text = "";
                            a.GetComponent<Image>().sprite = bom;
                            ShowAllBombs();
                            GameOver = true;
                        }
                        else
                        {
                            a.GetComponentInChildren<TMP_Text>().text = mines.ToString();
                            a.tag = "Untagged";
                        }
                    }
                }
            }
            else if (Input.touchCount == 1 && vlagNietSchep)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Ended)
                {
                    return;
                }
                Vector3 muisPos = Camera.main.ScreenToWorldPoint(touch.position);
                if (muisPos.x > mvSpeelveldLinks && muisPos.x < mvSpeelveldRechts && muisPos.y > mvSpeelveldOnder && muisPos.y < mvSpeelveldBoven)
                {
                    List<int> kolomRijLijst = KrijgVakBijPos(muisPos);
                    kolom = kolomRijLijst[0];
                    rij = kolomRijLijst[1];
                    if (!isAlGedrukt)
                    {
                        Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
                        int buttonNum = (rij * 100) + kolom;
                        if (buttonNum == 0) buttonNum = -1;
                        a.name = buttonNum.ToString();
                        buttons.Add(a);
                        if (mvLayout.korteKant > mvLayout.langeKant)
                        {
                            a.transform.localPosition = new Vector3((buttonScale * 75f) + (10.5f * mvLayout.langeKant * kolom * buttonScale), (buttonScale * 60f) + (mvLayout.korteKant * 5.55f * rij * buttonScale), -0.45f);
                        }
                        else
                        {
                            a.transform.localPosition = new Vector3((buttonScale * 60f) + (mvLayout.langeKant * 5.55f * (21 - rij) * buttonScale), (buttonScale * 75f) + (10.5f * mvLayout.korteKant * kolom * buttonScale), -0.45f);
                        }
                        a.transform.localScale = new Vector3(buttonScale * .5f, buttonScale * .5f, 1f);
                        a.tag = "vlag";
                        a.GetComponentInChildren<TMP_Text>().text = "";
                        a.GetComponent<Image>().sprite = vlag;
                        aantalBommen -= 1;
                        bommenTeGaan.text = aantalBommen.ToString();
                        saveScript.intDict["mijnenvegerVlaggenGezet" + buttonNum] = 1;
                    }
                    else if (isAlGedrukt)
                    {
                        int buttonNum = (rij * 100) + kolom;
                        if (buttonNum == 0) buttonNum = -1;
                        int index = vakjesGehad.IndexOf(buttonNum);
                        if (index == -1)
                        {
                            vakjesGehad.Add(buttonNum);
                            return;
                        }
                        for (int i = 0; i < buttons.Count; i++)
                        {
                            if (int.Parse(buttons[i].name) == buttonNum)
                            {
                                if (buttons[i].CompareTag("vlag"))
                                {
                                    Button a = buttons[i];
                                    buttons.RemoveAt(i);
                                    Destroy(a.gameObject);
                                    aantalBommen += 1;
                                    bommenTeGaan.text = aantalBommen.ToString();
                                    saveScript.intDict["mijnenvegerVlaggenGezet" + buttonNum] = 0;
                                    int startIndex = vakjesGehad.IndexOf(buttonNum);
                                    for(int ii = startIndex; ii < vakjesGehad.Count; ii++)
                                    {
                                        saveScript.intDict["mijnenvegerVakjesgehad" + ii] = saveScript.intDict["mijnenvegerVakjesgehad" + (ii + 1)];
                                    }
                                    saveScript.intDict["mijnenvegerVakjesgehad" + vakjesGehad.Count] = 0;
                                    vakjesGehad.Remove(buttonNum);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator WachtVoorGehaaldCanvas()
    {
        saveScript.intDict["begonnenAanMV"] = 0;
        for (int i = 0; i < 100; i++)
        {
            saveScript.intDict["mijnenvegerBom" + i] = 0;
        }
        for (int i = 0; i < mvLayout.langeKant * mvLayout.korteKant; i++)
        {
            saveScript.intDict["mijnenvegerVakjesgehad" + i] = 0;
        }
        for (int i = 0; i < 22; i++)
        {
            for (int ii = 0; ii < 22; ii++)
            {
                int index = (i * 100) + ii;
                saveScript.intDict["mijnenvegerVlaggenGezet" + index] = 0;
            }
        }
        yield return new WaitForSecondsRealtime(1f);
        OpenGehaaldCanvas(false);
    }

    private List<int> KrijgVakBijPos(Vector3 muisPos)
    {
        if (mvLayout.korteKant > mvLayout.langeKant)
        {
            float x = muisPos.x - mvSpeelveldLinks + (buttonScale / 2);
            float y = muisPos.y - mvSpeelveldOnder - (buttonScale / 2);
            kolom = Mathf.FloorToInt(x / (mvSpeelveldRechts - mvSpeelveldLinks) * mvLayout.langeKant); //0 is linker rij
            rij = Mathf.FloorToInt(y / (mvSpeelveldBoven - mvSpeelveldOnder) * mvLayout.korteKant); //0 is onderste rij
            int vakjesGehadNummer = (rij * 100) + kolom;
            if (vakjesGehadNummer == 0) vakjesGehadNummer = -1;
            if (vakjesGehad.IndexOf(vakjesGehadNummer) != -1)
            {
                isAlGedrukt = true;
            }
            else
            {
                saveScript.intDict["mijnenvegerVakjesgehad" + vakjesGehad.Count] = vakjesGehadNummer;
                isAlGedrukt = false;
                vakjesGehad.Add(vakjesGehadNummer);
            }
            List<int> outputlijst = new List<int> { kolom, rij };
            return outputlijst;
        }
        else
        {
            float x = muisPos.x - mvSpeelveldLinks + (buttonScale / 2);
            float y = muisPos.y - mvSpeelveldOnder - (buttonScale / 2);
            rij = 21 - Mathf.FloorToInt(x / (mvSpeelveldRechts - mvSpeelveldLinks) * mvLayout.langeKant); //0 is onderste rij
            kolom = Mathf.FloorToInt(y / (mvSpeelveldBoven - mvSpeelveldOnder) * mvLayout.korteKant); //0 is linker rij
            int vakjesGehadNummer = (rij * 100) + kolom;
            if (vakjesGehadNummer == 0) vakjesGehadNummer = -1;
            if (vakjesGehad.IndexOf(vakjesGehadNummer) != -1)
            {
                isAlGedrukt = true;
            }
            else
            {
                saveScript.intDict["mijnenvegerVakjesgehad" + vakjesGehad.Count] = vakjesGehadNummer;
                isAlGedrukt = false;
                vakjesGehad.Add(vakjesGehadNummer);
            }
            List<int> outputlijst = new List<int> { kolom, rij };
            return outputlijst;
        }
    }

    private int GetMineCount(int kolom, int rij)
    {
        int lKant = mvLayout.korteKant - 1;
        int kKant = mvLayout.langeKant - 1;
        if (mvLayout.korteKant > mvLayout.langeKant)
        {
            lKant = mvLayout.langeKant - 1;
            kKant = mvLayout.korteKant - 1;
        }
        int aantalBommen = 0;
        if (bommenList.IndexOf(kolom + (rij * 100)) != -1)
        {
            aantalBommen = 99;
        }
        else
        {
            if (kolom == 0)
            {
                if (rij == 0)
                {
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else if (rij == kKant)
                {
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else
                {
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
            }
            else if (kolom == lKant)
            {
                if (rij == 0)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else if (rij == kKant)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else
                {
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
            }
            else
            {
                if (rij == 0)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else if (rij == kKant)
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                }
                else
                {
                    if (bommenList.IndexOf(kolom - 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + (rij * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij - 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom - 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + 1 + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                    if (bommenList.IndexOf(kolom + ((rij + 1) * 100)) != -1)
                        aantalBommen += 1;
                }
            }
        }
        return aantalBommen;
    }

    private void KrijgVakBijKolomRij(int kolom, int rij)
    {
        int vakjesGehadNummer = (rij * 100) + kolom;
        if (vakjesGehad.IndexOf(vakjesGehadNummer) != -1)
        {
            isAlGedrukt = true;
        }
        else
        {
            if (vakjesGehadNummer == 0) vakjesGehadNummer = -1;
            saveScript.intDict["mijnenvegerVakjesgehad" + vakjesGehad.Count] = vakjesGehadNummer;
            isAlGedrukt = false;
            vakjesGehad.Add(vakjesGehadNummer);
        }
    }

    private void LegeVakken(int kolom, int rij)
    {
        KrijgVakBijKolomRij(kolom, rij);
        if (!isAlGedrukt)
        {
            Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
            a.name = (kolom + (rij * 100)).ToString();
            buttons.Add(a);
            if (mvLayout.korteKant > mvLayout.langeKant)
            {
                a.transform.localPosition = new Vector3((buttonScale * 75f) + (10.5f * mvLayout.langeKant * kolom * buttonScale), (buttonScale * 60f) + (mvLayout.korteKant * 5.55f * rij * buttonScale), -0.45f);
            }
            else
            {
                a.transform.localPosition = new Vector3((buttonScale * 60f) + (mvLayout.langeKant * 5.55f * (21 - rij) * buttonScale), (buttonScale * 75f) + (10.5f * mvLayout.korteKant * kolom * buttonScale), -0.45f);
            }
            a.transform.localScale = new Vector3(buttonScale * .5f, buttonScale * .5f, 1f);
            float mines = GetMineCount(kolom, rij);
            if (mines == 0)
            {
                a.GetComponentInChildren<TMP_Text>().text = "-";
                a.tag = "Untagged";
                VindMeerVakkenZonderBomBuren(kolom, rij);
            }
            else if (mines == 99)
            {
                a.GetComponentInChildren<TMP_Text>().text = "";
                a.GetComponent<Image>().sprite = bom;
            }
            else
            {
                a.GetComponentInChildren<TMP_Text>().text = mines.ToString();
                a.tag = "Untagged";
            }
        }
    }

    private void VindMeerVakkenZonderBomBuren(int kolom, int rij)
    {
        int lKant = mvLayout.korteKant - 1;
        int kKant = mvLayout.langeKant - 1;
        if (mvLayout.korteKant > mvLayout.langeKant)
        {
            lKant = mvLayout.langeKant - 1;
            kKant = mvLayout.korteKant - 1;
        }
        if (kolom == 0)
        {
            if (rij == 0)
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
            }
            else if (rij == kKant)
            {
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij - 1);
            }
            else
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom + 1, rij - 1);
            }
        }
        else if (kolom == lKant)
        {
            if (rij == 0)
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
            }
            else if (rij == kKant)
            {
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij - 1);
            }
            else
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij - 1);
            }
        }
        else
        {
            if (rij == 0)
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
            }
            else if (rij == kKant)
            {
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij - 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij - 1);
            }
            else
            {
                LegeVakken(kolom, rij + 1);
                LegeVakken(kolom, rij - 1);
                LegeVakken(kolom - 1, rij);
                LegeVakken(kolom - 1, rij + 1);
                LegeVakken(kolom - 1, rij - 1);
                LegeVakken(kolom + 1, rij);
                LegeVakken(kolom + 1, rij + 1);
                LegeVakken(kolom + 1, rij - 1);
            }
        }
    }

    private void ShowAllBombs()
    {
        for (int i = 0; i < bommenList.Count; i++)
        {
            int bomNum = bommenList[i];
            kolom = bomNum % 100;
            rij = (bomNum - kolom) / 100;
            KrijgVakBijKolomRij(kolom, rij);
            if (!isAlGedrukt)
            {
                Button a = Instantiate(knop, Vector3.zero, Quaternion.identity, speelVeld.transform);
                a.name = bomNum.ToString();
                buttons.Add(a);
                if (mvLayout.korteKant > mvLayout.langeKant)
                {
                    a.transform.localPosition = new Vector3((buttonScale * 75f) + (10.5f * mvLayout.langeKant * kolom * buttonScale), (buttonScale * 60f) + (mvLayout.korteKant * 5.55f * rij * buttonScale), -0.45f);
                }
                else
                {
                    a.transform.localPosition = new Vector3((buttonScale * 60f) + (mvLayout.langeKant * 5.55f * (21 - rij) * buttonScale), (buttonScale * 75f) + (10.5f * mvLayout.korteKant * kolom * buttonScale), -0.45f);
                }
                a.transform.localScale = new Vector3(buttonScale * .5f, buttonScale * .5f, 1f);
                a.GetComponentInChildren<TMP_Text>().text = "";
                a.GetComponent<Image>().sprite = bom;
            }
        }
    }

    public void OpenGehaaldCanvas(bool winst)
    {
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        GameObject GehaaldCanvas = gehaaldCanvas;
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
        gehaaldCanvasWinstTekstRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * (17f / 30f)) - (Screen.height / 2f));
        gehaaldCanvasWinstTekstRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height * (8f / 30f));
        gehaaldCanvasVerliesTekstRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * (15f / 30f)) - (Screen.height / 2f));
        gehaaldCanvasVerliesTekstRect.sizeDelta = gehaaldCanvasWinstTekstRect.sizeDelta;
        gehaaldCanvasRewardRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + (Screen.safeArea.height * (10f / 30f)) - (Screen.height / 2f));
        if (Application.internetReachability == NetworkReachability.NotReachable
            || (winst && !gehaaldCanvasRewardVerdubbelObj.activeInHierarchy))
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
        if (saveScript.intDict["difficultyMijnenVeger"] < 3 && winst)
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
        speelVeld.SetActive(false);
        uitlegUI.SetActive(false);
        overigCanvas.SetActive(false);
        MijnenvegerObj.SetActive(false);
    }
}
