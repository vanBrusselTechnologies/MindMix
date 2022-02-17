using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class Script2048 : MonoBehaviour
{
    private int grootte;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lijn;
    [SerializeField] private LocalizeStringEvent scoreText;
    [SerializeField] private GameObject gehaaldCanvas;
    [SerializeField] private GameObject uitlegCanvas;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private GameObject obj2048;
    private Vector3 fp;
    private Vector3 lp;
    private float dragDistanceVert;
    private float dragDistanceHorz;
    private int randNum;
    private List<GameObject> knoppenGesorteerd = new List<GameObject>();
    private List<int> knoppenX = new List<int>();
    private List<int> possiblePlekken = new List<int>();
    [SerializeField] private Transform speelVeld;
    private List<GameObject> buttonsOmTeWissen = new List<GameObject>();
    private List<GameObject> knoppenRij1 = new List<GameObject>();
    private List<GameObject> knoppenRij2 = new List<GameObject>();
    private List<GameObject> knoppenRij3 = new List<GameObject>();
    private List<GameObject> knoppenRij4 = new List<GameObject>();
    private List<GameObject> knoppenRij5 = new List<GameObject>();
    private List<GameObject> knoppenRij6 = new List<GameObject>();
    private List<GameObject> knoppenRij7 = new List<GameObject>();
    private List<GameObject> knoppenRij8 = new List<GameObject>();
    private List<GameObject> knoppenKolom1 = new List<GameObject>();
    private List<GameObject> knoppenKolom2 = new List<GameObject>();
    private List<GameObject> knoppenKolom3 = new List<GameObject>();
    private List<GameObject> knoppenKolom4 = new List<GameObject>();
    private List<GameObject> knoppenKolom5 = new List<GameObject>();
    private List<GameObject> knoppenKolom6 = new List<GameObject>();
    private List<GameObject> knoppenKolom7 = new List<GameObject>();
    private List<GameObject> knoppenKolom8 = new List<GameObject>();
    private List<GameObject> GevuldVeldKnoppen = new List<GameObject>();
    private Vector3 knopPositie = Vector3.zero;
    private bool heeftSpelBewogen = true;
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    private BeloningScript beloningScript;
    [SerializeField] private LocalizeStringEvent a;
    [SerializeField] private TMP_Text beloningText;
    [SerializeField] private RectTransform gehaaldCanvasTitelRect;
    [SerializeField] private RectTransform gehaaldCanvasTekstRect;
    [SerializeField] private RectTransform gehaaldCanvasStartNieuweKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasNaarMenuKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasRewardRect;
    [SerializeField] private GameObject gehaaldCanvasRewardVerdubbelObj;

    // Use this for initialization
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
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        dragDistanceVert = Screen.height * 10 / 100;
        dragDistanceHorz = Screen.width * 10 / 100;
        grootte = saveScript.intDict["grootte2048"] + 4;
        if (gegevensScript.startNew2048)
        {
            WisOudeGegevens();
            saveScript.intDict["begonnenAan2048"] = 1;
            for (int i = 0; i < 3; i++)
            {
                KrijgHuidigeButtons();
                randNum = Random.Range(0, possiblePlekken.Count);
                if (possiblePlekken.Count != 0)
                {
                    CreateButton(button, possiblePlekken[randNum], grootte, speelVeld, 0);
                }
            }
        }
        else
        {
            for (int i = 0; i < grootte * grootte; i++)
            {
                int getal = saveScript.intDict["2048Knop" + i];
                if (getal != 0)
                {
                    CreateButton(button, i, grootte, speelVeld, getal);
                }
            }
        }
        Vector3 localScaleHorzLijn = new Vector3(100, 1f / (grootte - 3) * 2f, 1);
        Vector3 localScaleVertLijn = new Vector3(1f / (grootte - 3) * 2f, 100, 1);
        for (int i = 1; i < grootte; i++)
        {
            GameObject templijnHorz = Instantiate(lijn);
            templijnHorz.transform.parent = speelVeld;
            templijnHorz.transform.localPosition = new Vector3(0, -50f + (i * 100f / grootte), -2f);
            templijnHorz.transform.localScale = localScaleHorzLijn;
            GameObject templijnVert = Instantiate(lijn);
            templijnVert.transform.parent = speelVeld;
            templijnVert.transform.localPosition = new Vector3(-50f + (i * 100f / grootte), 0, -2f);
            templijnVert.transform.localScale = localScaleVertLijn;
        }
    }

    private bool checkGevuldVeld = true;

    // Update is called once per frame
    private void Update()
    {
        if (gehaaldCanvas.activeInHierarchy)
        {
            return; 
        }
        if (heeftSpelBewogen)
        {
            checkGevuldVeld = true;
            int score = 0;
            KrijgHuidigeButtons();
            KrijgHuidigeButtons();
            for (int i = 0; i < knoppenGesorteerd.Count; i++)
            {
                TMP_Text tmpTmpText = knoppenGesorteerd[i].GetComponentInChildren<TMP_Text>();
                score += int.Parse(tmpTmpText.text);
                if (int.Parse(tmpTmpText.text) > 10000)
                {
                    knoppenGesorteerd[i].transform.GetChild(0).transform.localScale = new Vector3(1f / grootte * (grootte / 8f), 1f / grootte * (grootte / 8.5f), 1f);
                }
                else if (int.Parse(tmpTmpText.text) > 1000)
                {
                    knoppenGesorteerd[i].transform.GetChild(0).transform.localScale = new Vector3(1f / grootte * (grootte / 7f), 1f / grootte * (grootte / 7.25f), 1f);
                }
                else if (int.Parse(tmpTmpText.text) > 100)
                {
                    knoppenGesorteerd[i].transform.GetChild(0).transform.localScale = new Vector3(1f / grootte * (grootte / 6f), 1f / grootte * (grootte / 6f), 1f);
                }
                else if (int.Parse(tmpTmpText.text) > 10)
                {
                    knoppenGesorteerd[i].transform.GetChild(0).transform.localScale = new Vector3(1f / grootte * (grootte / 5f), 1f / grootte * (grootte / 4.75f), 1f);
                }
            }
            scoreText.StringReference.Remove("score");
            scoreText.StringReference.Add("score", new IntVariable { Value = score });
            scoreText.StringReference.RefreshString();
            string posPlekkenString = "";
            for (int p = 0; p < possiblePlekken.Count; p++)
            {
                posPlekkenString += possiblePlekken[p] + ", ";
            }
            SlaVoortgangOp();
        }
        heeftSpelBewogen = false;
        if (Input.touchCount >= 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                lp = touch.position;
                if (Mathf.Abs(lp.x - fp.x) > dragDistanceHorz || Mathf.Abs(lp.y - fp.y) > dragDistanceVert)
                {
                    heeftSpelBewogen = true;
                    if (Mathf.Abs(lp.x - fp.x) >= Mathf.Abs(lp.y - fp.y))
                    {
                        if (lp.x > fp.x)
                        {
                            KrijgHuidigeButtons();
                            NaarRechts();
                            KrijgHuidigeButtons();
                            randNum = Random.Range(0, possiblePlekken.Count);
                            if (possiblePlekken.Count != 0)
                            {
                                CreateButton(button, possiblePlekken[randNum], grootte, speelVeld, 0);
                            }
                        }
                        else
                        {
                            KrijgHuidigeButtons();
                            NaarLinks();
                            KrijgHuidigeButtons();
                            randNum = Random.Range(0, possiblePlekken.Count);
                            if (possiblePlekken.Count != 0)
                            {
                                CreateButton(button, possiblePlekken[randNum], grootte, speelVeld, 0);
                            }
                        }
                    }
                    else if (Mathf.Abs(lp.x - fp.x) < Mathf.Abs(lp.y - fp.y))
                    {
                        if (lp.y > fp.y)
                        {
                            KrijgHuidigeButtons();
                            NaarBoven();
                            KrijgHuidigeButtons();
                            randNum = Random.Range(0, possiblePlekken.Count);
                            if (possiblePlekken.Count != 0)
                            {
                                CreateButton(button, possiblePlekken[randNum], grootte, speelVeld, 0);
                            }
                        }
                        else
                        {
                            KrijgHuidigeButtons();
                            NaarOnder();
                            KrijgHuidigeButtons();
                            randNum = Random.Range(0, possiblePlekken.Count);
                            if (possiblePlekken.Count != 0)
                            {
                                CreateButton(button, possiblePlekken[randNum], grootte, speelVeld, 0);
                            }
                        }
                    }
                }
            }
        }
        if (speelVeld.childCount - ((grootte - 1) * 2) >= grootte * grootte)
        {
            if (checkGevuldVeld && VeldGevuld())
            {
                checkGevuldVeld = false;
                if (!KanNogMoven())
                {
                    Scene scene = SceneManager.GetActiveScene();
                    int grootte = saveScript.intDict["grootte2048"];
                    saveScript.intDict["2048sGespeeld"] += 1;
                    saveScript.intDict["2048Grootte" + grootte + "Gespeeld"] += 1;
                    float score = ((IntVariable)scoreText.StringReference["score"]).Value;
                    beloningText.text = beloningScript.Beloning(scene: scene, difficulty: grootte, score: score, doelwitText: beloningText).ToString();
                    OpenGehaaldCanvas();
                }
            }
        }
    }

    public static Button CreateButton(Button buttonPrefab, float i, float grootte, Transform speelVeld, int getal)
    {
        Button button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, speelVeld);
        TMP_Text tekst = button.GetComponentInChildren<TMP_Text>();
        button.transform.GetChild(0).transform.localScale = new Vector3(1f / grootte * (grootte / 3.5f), 1f / grootte * (grootte / 3.5f), 1f);
        var rectTransform = button.GetComponent<RectTransform>();
        float temp = 1f / grootte * 100f;
        float x = i % grootte;
        float y = (i - x) / grootte;
        rectTransform.localPosition = new Vector3(-50f + (temp * x) + (100f / (grootte * 2f)), 50f - (temp * y) - (100f / (grootte * 2f)), -1f);
        rectTransform.localScale = new Vector3(1f / grootte, 1f / grootte, 1f);
        if (getal == 0)
        {
            if (Random.Range(0, 100) < 90)
            {
                tekst.text = "2";
            }
            else
            {
                tekst.text = "4";
            }
        }
        else
        {
            tekst.text = getal.ToString();
        }
        return button;
    }

    public void KrijgHuidigeButtons()
    {
        possiblePlekken.Clear();
        knoppenX.Clear();
        knoppenGesorteerd.Clear();
        GameObject[] knoppen = GameObject.FindGameObjectsWithTag("2048spelKnop");
        float temp = 1f / grootte * 100f;
        GameObject knop;
        float x;
        float y;
        for (int i = 0; i < grootte * grootte; i++)
        {
            x = i % grootte;
            y = (i - x) / grootte;
            knopPositie.x = -50f + (temp * x) + (100f / (grootte * 2f));
            knopPositie.y = 50f - (temp * y) - (100f / (grootte * 2f));
            int a = 0;
            for (; a < knoppen.Length; a++)
            {
                knop = knoppen[a];
                Vector3 localPos = knop.transform.localPosition;
                localPos.z = 0;
                if (Vector3.Distance(localPos, knopPositie) <= 0.1f)
                {
                    knop.name = i.ToString();
                    knoppenX.Add(i);
                    knoppenGesorteerd.Add(knop);
                    a = 10000;
                }
            }
            if (a < 10000)
            {
                possiblePlekken.Add(i);
            }
        }
        for (int i = 0; i < knoppen.Length; i++)
        {
            if (knoppen[i].name.Equals("Button(Clone)"))
            {
                Destroy(knoppen[i]);
            }
        }
    }

    public void NaarRechts()
    {
        bool magDezeOok = false;
        buttonsOmTeWissen.Clear();
        knoppenRij1.Clear();
        knoppenRij2.Clear();
        knoppenRij3.Clear();
        knoppenRij4.Clear();
        knoppenRij5.Clear();
        knoppenRij6.Clear();
        knoppenRij7.Clear();
        knoppenRij8.Clear();
        for (int i = 0; i < knoppenX.Count; i++)
        {
            int getal = knoppenX[i];
            if (getal < grootte)
            {
                knoppenRij1.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 2)
            {
                knoppenRij2.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 3)
            {
                knoppenRij3.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 4)
            {
                knoppenRij4.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 5)
            {
                knoppenRij5.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 6)
            {
                knoppenRij6.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 7)
            {
                knoppenRij7.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 8)
            {
                knoppenRij8.Add(knoppenGesorteerd[i]);
            }
        }
        if (knoppenRij1.Count == 1)
        {
            knoppenRij1[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * ((grootte - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((grootte - 1f - ((grootte - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij1.Count > 1)
        {
            if (knoppenRij1[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 2 && knoppenRij1[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 3 && knoppenRij1[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 4 && knoppenRij1[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 5 && knoppenRij1[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 6 && knoppenRij1[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 7 && knoppenRij1[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij1.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij1.Count + 1; a++)
            {
                knoppenRij1[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((grootte - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((grootte - a - ((grootte - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (knoppenRij2.Count == 1)
        {
            knoppenRij2[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 2f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 2f) - 1f - (((grootte * 2f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij2.Count > 1)
        {
            if (knoppenRij2[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 2 && knoppenRij2[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 3 && knoppenRij2[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 4 && knoppenRij2[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 5 && knoppenRij2[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 6 && knoppenRij2[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 7 && knoppenRij2[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij2.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij2.Count + 1; a++)
            {
                knoppenRij2[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 2f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 2f) - a - (((grootte * 2f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (knoppenRij3.Count == 1)
        {
            knoppenRij3[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 3f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 3f) - 1f - (((grootte * 3f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij3.Count > 1)
        {
            if (knoppenRij3[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 2 && knoppenRij3[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 3 && knoppenRij3[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 4 && knoppenRij3[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 5 && knoppenRij3[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 6 && knoppenRij3[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 7 && knoppenRij3[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij3.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij3.Count + 1; a++)
            {
                knoppenRij3[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 3f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 3f) - a - (((grootte * 3f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (knoppenRij4.Count == 1)
        {
            knoppenRij4[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 4f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 4f) - 1f - (((grootte * 4f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij4.Count > 1)
        {
            if (knoppenRij4[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 2 && knoppenRij4[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 3 && knoppenRij4[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 4 && knoppenRij4[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 5 && knoppenRij4[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 6 && knoppenRij4[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 7 && knoppenRij4[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij4.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij4.Count + 1; a++)
            {
                knoppenRij4[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 4f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 4f) - a - (((grootte * 4f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 4)
        {
            return;
        }
        magDezeOok = false;
        if (knoppenRij5.Count == 1)
        {
            knoppenRij5[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 5f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 5f) - 1f - (((grootte * 5f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij5.Count > 1)
        {
            if (knoppenRij5[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 2 && knoppenRij5[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 3 && knoppenRij5[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 4 && knoppenRij5[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 5 && knoppenRij5[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 6 && knoppenRij5[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 7 && knoppenRij5[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij5.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij5.Count + 1; a++)
            {
                knoppenRij5[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 5f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 5f) - a - (((grootte * 5f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (grootte <= 5)
        {
            return;
        }
        if (knoppenRij6.Count == 1)
        {
            knoppenRij6[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 6f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 6f) - 1f - (((grootte * 6f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij6.Count > 1)
        {
            if (knoppenRij6[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 2 && knoppenRij6[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 3 && knoppenRij6[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 4 && knoppenRij6[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 5 && knoppenRij6[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 6 && knoppenRij6[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 7 && knoppenRij6[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij6.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij6.Count + 1; a++)
            {
                knoppenRij6[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 6f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 6f) - a - (((grootte * 6f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (grootte <= 6)
        {
            return;
        }
        if (knoppenRij7.Count == 1)
        {
            knoppenRij7[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 7f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 7f) - 1f - (((grootte * 7f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij7.Count > 1)
        {
            if (knoppenRij7[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 2 && knoppenRij7[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 3 && knoppenRij7[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 4 && knoppenRij7[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 5 && knoppenRij7[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 6 && knoppenRij7[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 7 && knoppenRij7[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij7.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij7.Count + 1; a++)
            {
                knoppenRij7[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 7f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 7f) - a - (((grootte * 7f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (grootte <= 7)
        {
            return;
        }
        if (knoppenRij8.Count == 1)
        {
            knoppenRij8[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 8f) - 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 8f) - 1f - (((grootte * 8f) - 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij8.Count > 1)
        {
            if (knoppenRij8[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 2 && knoppenRij8[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 3 && knoppenRij8[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 4 && knoppenRij8[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 5 && knoppenRij8[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 6 && knoppenRij8[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 7 && knoppenRij8[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij8.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenRij8.Count + 1; a++)
            {
                knoppenRij8[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * 8f) - a) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * 8f) - a - (((grootte * 8f) - a) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public void NaarLinks()
    {
        bool magDezeOok;
        buttonsOmTeWissen.Clear();
        knoppenRij1.Clear();
        knoppenRij2.Clear();
        knoppenRij3.Clear();
        knoppenRij4.Clear();
        knoppenRij5.Clear();
        knoppenRij6.Clear();
        knoppenRij7.Clear();
        knoppenRij8.Clear();
        for (int i = 0; i < knoppenX.Count; i++)
        {
            int getal = knoppenX[i];
            if (getal < grootte)
            {
                knoppenRij1.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 2)
            {
                knoppenRij2.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 3)
            {
                knoppenRij3.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 4)
            {
                knoppenRij4.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 5)
            {
                knoppenRij5.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 6)
            {
                knoppenRij6.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 7)
            {
                knoppenRij7.Add(knoppenGesorteerd[i]);
            }
            else if (getal < grootte * 8)
            {
                knoppenRij8.Add(knoppenGesorteerd[i]);
            }
        }
        if (knoppenRij1.Count == 1)
        {
            knoppenRij1[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (0 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 - (0 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij1.Count > 1)
        {
            if (knoppenRij1[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 2 && knoppenRij1[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 3 && knoppenRij1[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 4 && knoppenRij1[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 5 && knoppenRij1[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 6 && knoppenRij1[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij1.Count > 7 && knoppenRij1[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij1[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij1[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij1[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij1[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij1.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij1.Count; a++)
            {
                knoppenRij1[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (a % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a - (a % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (knoppenRij2.Count == 1)
        {
            knoppenRij2[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + grootte) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + grootte - ((0 + grootte) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij2.Count > 1)
        {
            if (knoppenRij2[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 2 && knoppenRij2[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 3 && knoppenRij2[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 4 && knoppenRij2[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 5 && knoppenRij2[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 6 && knoppenRij2[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij2.Count > 7 && knoppenRij2[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij2[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij2[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij2[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij2[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij2.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij2.Count; a++)
            {
                knoppenRij2[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 1f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 1f) - ((a + (grootte * 1f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (knoppenRij3.Count == 1)
        {
            knoppenRij3[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + (grootte * 2)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + (grootte * 2) - ((0 + (grootte * 2)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij3.Count > 1)
        {
            if (knoppenRij3[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 2 && knoppenRij3[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 3 && knoppenRij3[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 4 && knoppenRij3[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 5 && knoppenRij3[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 6 && knoppenRij3[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij3.Count > 7 && knoppenRij3[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij3[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij3[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij3[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij3[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij3.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij3.Count; a++)
            {
                knoppenRij3[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 2f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 2f) - ((a + (grootte * 2f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (knoppenRij4.Count == 1)
        {
            knoppenRij4[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + (grootte * 3)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + (grootte * 3) - ((0 + (grootte * 3)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij4.Count > 1)
        {
            if (knoppenRij4[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 2 && knoppenRij4[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 3 && knoppenRij4[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 4 && knoppenRij4[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 5 && knoppenRij4[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 6 && knoppenRij4[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij4.Count > 7 && knoppenRij4[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij4[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij4[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij4[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij4[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij4.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij4.Count; a++)
            {
                knoppenRij4[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 3f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 3f) - ((a + (grootte * 3f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 4)
        {
            return;
        }
        if (knoppenRij5.Count == 1)
        {
            knoppenRij5[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + (grootte * 4)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + (grootte * 4) - ((0 + (grootte * 4)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij5.Count > 1)
        {
            if (knoppenRij5[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 2 && knoppenRij5[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 3 && knoppenRij5[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 4 && knoppenRij5[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 5 && knoppenRij5[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 6 && knoppenRij5[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij5.Count > 7 && knoppenRij5[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij5[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij5[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij5[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij5[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij5.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij5.Count; a++)
            {
                knoppenRij5[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 4f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 4f) - ((a + (grootte * 4f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 5)
        {
            return;
        }
        if (knoppenRij6.Count == 1)
        {
            knoppenRij6[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + (grootte * 5)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + (grootte * 5) - ((0 + (grootte * 5)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij6.Count > 1)
        {
            if (knoppenRij6[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 2 && knoppenRij6[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 3 && knoppenRij6[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 4 && knoppenRij6[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 5 && knoppenRij6[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 6 && knoppenRij6[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij6.Count > 7 && knoppenRij6[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij6[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij6[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij6[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij6[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij6.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij6.Count; a++)
            {
                knoppenRij6[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 5f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 5f) - ((a + (grootte * 5f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 6)
        {
            return;
        }
        if (knoppenRij7.Count == 1)
        {
            knoppenRij7[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + (grootte * 6)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + (grootte * 6) - ((0 + (grootte * 6)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij7.Count > 1)
        {
            if (knoppenRij7[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 2 && knoppenRij7[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 3 && knoppenRij7[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 4 && knoppenRij7[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 5 && knoppenRij7[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 6 && knoppenRij7[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij7.Count > 7 && knoppenRij7[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij7[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij7[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij7[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij7[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij7.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij7.Count; a++)
            {
                knoppenRij7[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 6f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 6f) - ((a + (grootte * 6f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 7)
        {
            return;
        }
        if (knoppenRij8.Count == 1)
        {
            knoppenRij8[0].transform.localPosition = new Vector3(-50f + (100f / grootte * ((0 + (grootte * 7)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 + (grootte * 7) - ((0 + (grootte * 7)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenRij8.Count > 1)
        {
            if (knoppenRij8[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 2 && knoppenRij8[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 3 && knoppenRij8[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 4 && knoppenRij8[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 5 && knoppenRij8[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 6 && knoppenRij8[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenRij8.Count > 7 && knoppenRij8[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenRij8[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenRij8[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenRij8[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenRij8[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenRij8.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenRij8.Count; a++)
            {
                knoppenRij8[a].transform.localPosition = new Vector3(-50f + (100f / grootte * ((a + (grootte * 7f)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((a + (grootte * 7f) - ((a + (grootte * 7f)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public void NaarBoven()
    {
        bool magDezeOok;
        buttonsOmTeWissen.Clear();
        knoppenKolom1.Clear();
        knoppenKolom2.Clear();
        knoppenKolom3.Clear();
        knoppenKolom4.Clear();
        knoppenKolom5.Clear();
        knoppenKolom6.Clear();
        knoppenKolom7.Clear();
        knoppenKolom8.Clear();
        for (int i = 0; i < knoppenX.Count; i++)
        {
            int getal = knoppenX[i];
            if (getal % grootte == 0)
            {
                knoppenKolom1.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 01)
            {
                knoppenKolom2.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 02)
            {
                knoppenKolom3.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 03)
            {
                knoppenKolom4.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 04)
            {
                knoppenKolom5.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 05)
            {
                knoppenKolom6.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 06)
            {
                knoppenKolom7.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 07)
            {
                knoppenKolom8.Add(knoppenGesorteerd[i]);
            }
        }
        if (knoppenKolom1.Count == 1)
        {
            knoppenKolom1[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (0 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((0 - (0 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom1.Count > 1)
        {
            if (knoppenKolom1[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 2 && knoppenKolom1[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 3 && knoppenKolom1[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 4 && knoppenKolom1[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 5 && knoppenKolom1[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 6 && knoppenKolom1[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 7 && knoppenKolom1[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom1.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom1.Count; a++)
            {
                knoppenKolom1[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (a * grootte % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) - (a * grootte % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (knoppenKolom2.Count == 1)
        {
            knoppenKolom2[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (1 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((1 - (1 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom2.Count > 1)
        {
            if (knoppenKolom2[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 2 && knoppenKolom2[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 3 && knoppenKolom2[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 4 && knoppenKolom2[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 5 && knoppenKolom2[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 6 && knoppenKolom2[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 7 && knoppenKolom2[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom2.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom2.Count; a++)
            {
                knoppenKolom2[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 1f - (((a * grootte) + 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (knoppenKolom3.Count == 1)
        {
            knoppenKolom3[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (2 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((2 - (2 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom3.Count > 1)
        {
            if (knoppenKolom3[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 2 && knoppenKolom3[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 3 && knoppenKolom3[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 4 && knoppenKolom3[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 5 && knoppenKolom3[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 6 && knoppenKolom3[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 7 && knoppenKolom3[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom3.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom3.Count; a++)
            {
                knoppenKolom3[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 2f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 2 - (((a * grootte) + 2f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (knoppenKolom4.Count == 1)
        {
            knoppenKolom4[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (3 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((3 - (3 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom4.Count > 1)
        {
            if (knoppenKolom4[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 2 && knoppenKolom4[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 3 && knoppenKolom4[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 4 && knoppenKolom4[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 5 && knoppenKolom4[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 6 && knoppenKolom4[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 7 && knoppenKolom4[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom4.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom4.Count; a++)
            {
                knoppenKolom4[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 3f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 3 - (((a * grootte) + 3f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 4)
        {
            return;
        }
        if (knoppenKolom5.Count == 1)
        {
            knoppenKolom5[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (4 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((4 - (4 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom5.Count > 1)
        {
            if (knoppenKolom5[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 2 && knoppenKolom5[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 3 && knoppenKolom5[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 4 && knoppenKolom5[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 5 && knoppenKolom5[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 6 && knoppenKolom5[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 7 && knoppenKolom5[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom5.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom5.Count; a++)
            {
                knoppenKolom5[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 4f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 4 - (((a * grootte) + 4f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 5)
        {
            return;
        }
        if (knoppenKolom6.Count == 1)
        {
            knoppenKolom6[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (5 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((5 - (5 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom6.Count > 1)
        {
            if (knoppenKolom6[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 2 && knoppenKolom6[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 3 && knoppenKolom6[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 4 && knoppenKolom6[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 5 && knoppenKolom6[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 6 && knoppenKolom6[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 7 && knoppenKolom6[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom6.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom6.Count; a++)
            {
                knoppenKolom6[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 5f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 5 - (((a * grootte) + 5f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 6)
        {
            return;
        }
        if (knoppenKolom7.Count == 1)
        {
            knoppenKolom7[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (6 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((6 - (6 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom7.Count > 1)
        {
            if (knoppenKolom7[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 2 && knoppenKolom7[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 3 && knoppenKolom7[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 4 && knoppenKolom7[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 5 && knoppenKolom7[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 6 && knoppenKolom7[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 7 && knoppenKolom7[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom7.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom7.Count; a++)
            {
                knoppenKolom7[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 6f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 6 - (((a * grootte) + 6f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 7)
        {
            return;
        }
        if (knoppenKolom8.Count == 1)
        {
            knoppenKolom8[0].transform.localPosition = new Vector3(-50f + (100f / grootte * (7 % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * ((7 - (7 % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom8.Count > 1)
        {
            if (knoppenKolom8[0].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[1].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[1];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[0].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[0].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 2 && knoppenKolom8[1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 3 && knoppenKolom8[2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 4 && knoppenKolom8[3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 5 && knoppenKolom8[4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 6 && knoppenKolom8[5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 7 && knoppenKolom8[6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom8.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < knoppenKolom8.Count; a++)
            {
                knoppenKolom8[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((a * grootte) + 7f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((a * grootte) + 7 - (((a * grootte) + 7f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public void NaarOnder()
    {
        bool magDezeOok = false;
        buttonsOmTeWissen.Clear();
        knoppenKolom1.Clear();
        knoppenKolom2.Clear();
        knoppenKolom3.Clear();
        knoppenKolom4.Clear();
        knoppenKolom5.Clear();
        knoppenKolom6.Clear();
        knoppenKolom7.Clear();
        knoppenKolom8.Clear();
        for (int i = 0; i < knoppenX.Count; i++)
        {
            int getal = knoppenX[i];
            if (getal % grootte == 0)
            {
                knoppenKolom1.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 01)
            {
                knoppenKolom2.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 02)
            {
                knoppenKolom3.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 03)
            {
                knoppenKolom4.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 04)
            {
                knoppenKolom5.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 05)
            {
                knoppenKolom6.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 06)
            {
                knoppenKolom7.Add(knoppenGesorteerd[i]);
            }
            else if (getal % grootte == 07)
            {
                knoppenKolom8.Add(knoppenGesorteerd[i]);
            }
        }
        if (knoppenKolom1.Count == 1)
        {
            knoppenKolom1[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte - (((grootte * grootte) - grootte) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom1.Count > 1)
        {
            if (knoppenKolom1[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 2 && knoppenKolom1[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 3 && knoppenKolom1[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 4 && knoppenKolom1[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 5 && knoppenKolom1[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 6 && knoppenKolom1[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom1.Count > 7 && knoppenKolom1[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom1[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom1[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom1[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom1[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom1.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom1.Count + 1; a++)
            {
                knoppenKolom1[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a)) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) - (((grootte * grootte) - (grootte * a)) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (knoppenKolom2.Count == 1)
        {
            knoppenKolom2[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 1f - (((grootte * grootte) - grootte + 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom2.Count > 1)
        {
            if (knoppenKolom2[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 2 && knoppenKolom2[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 3 && knoppenKolom2[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 4 && knoppenKolom2[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 5 && knoppenKolom2[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 6 && knoppenKolom2[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom2.Count > 7 && knoppenKolom2[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom2[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom2[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom2[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom2[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom2.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom2.Count + 1; a++)
            {
                knoppenKolom2[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 1f) % grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 1f - (((grootte * grootte) - (grootte * a) + 1f) % grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (knoppenKolom3.Count == 1)
        {
            knoppenKolom3[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 2f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 2 - (((grootte * grootte) - grootte + 2f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom3.Count > 1)
        {
            if (knoppenKolom3[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 2 && knoppenKolom3[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 3 && knoppenKolom3[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 4 && knoppenKolom3[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 5 && knoppenKolom3[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 6 && knoppenKolom3[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom3.Count > 7 && knoppenKolom3[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom3[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom3[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom3[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom3[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom3.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom3.Count + 1; a++)
            {
                knoppenKolom3[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 2f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 2 - (((grootte * grootte) - (grootte * a) + 2f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (knoppenKolom4.Count == 1)
        {
            knoppenKolom4[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 3f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 3 - (((grootte * grootte) - grootte + 3f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom4.Count > 1)
        {
            if (knoppenKolom4[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 2 && knoppenKolom4[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 3 && knoppenKolom4[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 4 && knoppenKolom4[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 5 && knoppenKolom4[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 6 && knoppenKolom4[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom4.Count > 7 && knoppenKolom4[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom4[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom4[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom4[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom4[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom4.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom4.Count + 1; a++)
            {
                knoppenKolom4[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 3f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 3 - (((grootte * grootte) - (grootte * a) + 3f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        if (grootte <= 4)
        {
            return;
        }
        magDezeOok = false;
        if (knoppenKolom5.Count == 1)
        {
            knoppenKolom5[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 4f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 4 - (((grootte * grootte) - grootte + 4f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom5.Count > 1)
        {
            if (knoppenKolom5[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 2 && knoppenKolom5[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 3 && knoppenKolom5[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 4 && knoppenKolom5[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 5 && knoppenKolom5[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 6 && knoppenKolom5[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom5.Count > 7 && knoppenKolom5[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom5[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom5[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom5[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom5[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom5.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom5.Count + 1; a++)
            {
                knoppenKolom5[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 4f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 4 - (((grootte * grootte) - (grootte * a) + 4f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (grootte <= 5)
        {
            return;
        }
        if (knoppenKolom6.Count == 1)
        {
            knoppenKolom6[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 5f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 5 - (((grootte * grootte) - grootte + 5f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom6.Count > 1)
        {
            if (knoppenKolom6[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 2 && knoppenKolom6[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 3 && knoppenKolom6[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 4 && knoppenKolom6[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 5 && knoppenKolom6[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 6 && knoppenKolom6[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom6.Count > 7 && knoppenKolom6[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom6[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom6[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom6[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom6[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom6.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom6.Count + 1; a++)
            {
                knoppenKolom6[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 5f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 5 - (((grootte * grootte) - (grootte * a) + 5f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (grootte <= 6)
        {
            return;
        }
        if (knoppenKolom7.Count == 1)
        {
            knoppenKolom7[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 6f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 6 - (((grootte * grootte) - grootte + 6f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom7.Count > 1)
        {
            if (knoppenKolom7[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 2 && knoppenKolom7[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 3 && knoppenKolom7[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 4 && knoppenKolom7[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 5 && knoppenKolom7[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 6 && knoppenKolom7[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom7.Count > 7 && knoppenKolom7[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom7[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom7[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom7[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom7[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom7.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom7.Count + 1; a++)
            {
                knoppenKolom7[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 6f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 6 - (((grootte * grootte) - (grootte * a) + 6f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
        magDezeOok = false;
        if (grootte <= 7)
        {
            return;
        }
        if (knoppenKolom8.Count == 1)
        {
            knoppenKolom8[^1].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - grootte + 7f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - grootte + 7 - (((grootte * grootte) - grootte + 7f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
        }
        else if (knoppenKolom8.Count > 1)
        {
            if (knoppenKolom8[^1].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^2].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^2];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^1].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^1].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 2 && knoppenKolom8[^2].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^3].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^3];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^2].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^2].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 3 && knoppenKolom8[^3].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^4].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^4];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^3].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^3].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 4 && knoppenKolom8[^4].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^5].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^5];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^4].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^4].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 5 && knoppenKolom8[^5].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^6].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^6];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^5].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^5].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 6 && knoppenKolom8[^6].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^7].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^7];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^6].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^6].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
                magDezeOok = false;
            }
            else
            {
                magDezeOok = true;
            }
            if (magDezeOok && knoppenKolom8.Count > 7 && knoppenKolom8[^7].GetComponentInChildren<TMP_Text>().text.Equals(knoppenKolom8[^8].GetComponentInChildren<TMP_Text>().text))
            {
                GameObject a = knoppenKolom8[^8];
                buttonsOmTeWissen.Add(a);
                Destroy(a.transform.gameObject);
                knoppenKolom8[^7].GetComponentInChildren<TMP_Text>().text = (int.Parse(knoppenKolom8[^7].GetComponentInChildren<TMP_Text>().text) * 2).ToString();
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                knoppenKolom8.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a < knoppenKolom8.Count + 1; a++)
            {
                knoppenKolom8[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (((grootte * grootte) - (grootte * a) + 7f)% grootte)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (((grootte * grootte) - (grootte * a) + 7 - (((grootte * grootte) - (grootte * a) + 7f)% grootte)) / grootte)) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public bool KanNogMoven()
    {
        bool kanOmhoog = KanOmhoog();
        if (kanOmhoog)
        {
            return true;
        }
        bool kanNaarRechts = KanNaarRechts();
        if (kanNaarRechts)
        {
            return true;
        }
        saveScript.intDict["begonnenAan2048"] = 0;
        return false;
    }

    private bool VeldGevuld()
    {
        GevuldVeldKnoppen.Clear();
        for (int i = 0; i < grootte * grootte; i++)
        {
            Transform knop = speelVeld.transform.Find("" + i);
            if (knop == null)
            {
                return false;
            }
            GevuldVeldKnoppen.Add(knop.gameObject);
        }
        return true;
    }

    private bool KanOmhoog()
    {
        for (int i = 1; i < grootte; i++)
        {
            for (int ii = 0; ii < grootte; ii++)
            {
                if (GevuldVeldKnoppen[(i * grootte) + ii].transform.GetChild(0).GetComponent<TMP_Text>().text.Equals(GevuldVeldKnoppen[((i - 1) * grootte) + ii].transform.GetChild(0).GetComponent<TMP_Text>().text))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool KanNaarRechts()
    {
        for (int i = 0; i < grootte; i++)
        {
            for (int ii = 1; ii < grootte; ii++)
            {
                if (GevuldVeldKnoppen[(i * grootte) + ii].transform.GetChild(0).GetComponent<TMP_Text>().text.Equals(GevuldVeldKnoppen[(i * grootte) + ii - 1].transform.GetChild(0).GetComponent<TMP_Text>().text))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SlaVoortgangOp()
    {
        int knoppenXgetal = 0;
        for (int i = 0; i < grootte * grootte; i++)
        {
            if (knoppenX.Count > knoppenXgetal)
            {
                if (knoppenX[knoppenXgetal] == i)
                {
                    saveScript.intDict["2048Knop" + knoppenX[knoppenXgetal]] = int.Parse(knoppenGesorteerd[knoppenXgetal].GetComponentInChildren<TMP_Text>().text);
                    knoppenXgetal += 1;
                }
                else
                {
                    saveScript.intDict["2048Knop" + i] = 0;
                }
            }
            else
            {
                saveScript.intDict["2048Knop" + i] = 0;
            }
        }
    }

    public void OpenGehaaldCanvas()
    {
        a.StringReference.Clear();
        a.StringReference.Add("score", new IntVariable { Value = ((IntVariable)scoreText.StringReference["score"]).Value });
        obj2048.SetActive(false);
        overigCanvas.SetActive(false);
        uitlegCanvas.SetActive(false);
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        gehaaldCanvas.SetActive(true);
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
        float sizeX = Screen.safeArea.width * 0.45f;
        float sizeY = Screen.safeArea.height * (5f / 30f);
        float posY = safeZoneAntiY + (Screen.safeArea.height * (3.5f / 30f)) - (Screen.height / 2f);
        gehaaldCanvasStartNieuweKnopRect.anchoredPosition = new Vector2(safeZoneAntiX - (Screen.safeArea.width / 4f), posY);
        gehaaldCanvasStartNieuweKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
        gehaaldCanvasNaarMenuKnopRect.anchoredPosition = new Vector2(safeZoneAntiX + (Screen.safeArea.width / 4f), posY);
        gehaaldCanvasNaarMenuKnopRect.sizeDelta = new Vector2(sizeX, sizeY);
        WisOudeGegevens();
        saveScript.intDict["begonnenAan2048"] = 0;
    }

    private void WisOudeGegevens()
    {
        for (int i = 0; i < 100; i++)
        {
            saveScript.intDict["2048Knop" + i] = 0;
        }
    }
}
