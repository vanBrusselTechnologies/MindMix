using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script2048 : MonoBehaviour
{
    private Layout2048 layout2048;
    private GegevensHouder gegevensHouder;
    private SaveScript saveScript;
    private RewardHandler _rewardHandler;

    [SerializeField] private Transform speelVeld;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lijn;
    [SerializeField] private LocalizeStringEvent scoreText;
    [SerializeField] private GameObject gehaaldCanvas;
    [SerializeField] private GameObject uitlegCanvas;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private GameObject obj2048;
    [SerializeField] private GameObject menuUICanvasObj;

    [SerializeField] private LocalizeStringEvent a;
    [SerializeField] private TMP_Text beloningText;

    private int grootte;
    private Vector3 fp;
    private Vector3 lp;
    private float dragDistanceVert;
    private float dragDistanceHorz;
    private int randNum;
    private List<GameObject> knoppenGesorteerd = new();
    private List<int> knoppenX = new();
    private List<int> possiblePlekken = new();
    private List<GameObject> buttonsOmTeWissen = new();
    private List<GameObject> knoppenRij1 = new();
    private List<GameObject> knoppenRij2 = new();
    private List<GameObject> knoppenRij3 = new();
    private List<GameObject> knoppenRij4 = new();
    private List<GameObject> knoppenRij5 = new();
    private List<GameObject> knoppenRij6 = new();
    private List<GameObject> knoppenRij7 = new();
    private List<GameObject> knoppenRij8 = new();
    private List<GameObject> knoppenKolom1 = new();
    private List<GameObject> knoppenKolom2 = new();
    private List<GameObject> knoppenKolom3 = new();
    private List<GameObject> knoppenKolom4 = new();
    private List<GameObject> knoppenKolom5 = new();
    private List<GameObject> knoppenKolom6 = new();
    private List<GameObject> knoppenKolom7 = new();
    private List<GameObject> knoppenKolom8 = new();
    private List<GameObject> GevuldVeldKnoppen = new();
    private Vector3 knopPositie = Vector3.zero;
    private bool heeftSpelBewogen = true;

    // Use this for initialization
    private void Start()
    {
        gegevensHouder = GegevensHouder.Instance;
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        saveScript = SaveScript.Instance;
        _rewardHandler = RewardHandler.Instance;
        layout2048 = GetComponent<Layout2048>();
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        dragDistanceVert = Screen.height * 10 / 100;
        dragDistanceHorz = Screen.width * 10 / 100;
        grootte = saveScript.intDict["grootte2048"] + 4;
        if (gegevensHouder.startNewGame)
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
        Vector3 localScaleHorzLijn = new(100, 1f / (grootte - 3) * 2f, 1);
        Vector3 localScaleVertLijn = new(1f / (grootte - 3) * 2f, 100, 1);
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
        if (gehaaldCanvas.activeInHierarchy) return;
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
                    beloningText.text = _rewardHandler.Beloning(scene: scene, difficulty: grootte, score: score, doelwitText: beloningText).ToString();
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

    private bool VoegSamen(GameObject objToDelete, GameObject objToDouble)
    {
        TMP_Text objToDoubleText = objToDouble.GetComponentInChildren<TMP_Text>();
        TMP_Text objToDeleteText = objToDelete.GetComponentInChildren<TMP_Text>();
        if (objToDoubleText.text.Equals(objToDeleteText.text))
        {
            buttonsOmTeWissen.Add(objToDelete);
            Destroy(objToDelete.transform.gameObject);
            objToDoubleText.text = (int.Parse(objToDoubleText.text) * 2).ToString();
            return false;
        }
        return true;
    }

    public void NaarRechts()
    {
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
            if (getal < grootte) knoppenRij1.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 2) knoppenRij2.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 3) knoppenRij3.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 4) knoppenRij4.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 5) knoppenRij5.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 6) knoppenRij6.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 7) knoppenRij7.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 8) knoppenRij8.Add(knoppenGesorteerd[i]);
        }
        for (int i = 1; i <= grootte; i++)
        {
            bool magDezeOok = true;
            List<GameObject> list = i == 1 ? knoppenRij1 : i == 2 ? knoppenRij2 : i == 3 ? knoppenRij3 : i == 4 ? knoppenRij4 : i == 5 ? knoppenRij5 : i == 6 ? knoppenRij6 : i == 7 ? knoppenRij7 : i == 8 ? knoppenRij8 : null;
            for (int a = 2; a <= list.Count; a++)
            {
                magDezeOok = !magDezeOok || list.Count < a || VoegSamen(list[^a], list[^(a - 1)]);
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                list.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a <= list.Count; a++)
            {
                list[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (grootte - a)) + (100f / (grootte * 2f)), 50f - (100f / grootte * (i - 1)) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public void NaarLinks()
    {
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
            if (getal < grootte) knoppenRij1.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 2) knoppenRij2.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 3) knoppenRij3.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 4) knoppenRij4.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 5) knoppenRij5.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 6) knoppenRij6.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 7) knoppenRij7.Add(knoppenGesorteerd[i]);
            else if (getal < grootte * 8) knoppenRij8.Add(knoppenGesorteerd[i]);
        }
        for (int i = 1; i <= grootte; i++)
        {
            bool magDezeOok = true;
            List<GameObject> list = i == 1 ? knoppenRij1 : i == 2 ? knoppenRij2 : i == 3 ? knoppenRij3 : i == 4 ? knoppenRij4 : i == 5 ? knoppenRij5 : i == 6 ? knoppenRij6 : i == 7 ? knoppenRij7 : i == 8 ? knoppenRij8 : null;
            for (int a = 1; a < list.Count; a++)
            {
                magDezeOok = !magDezeOok || list.Count < a || VoegSamen(list[a], list[a - 1]);
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                list.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < list.Count; a++)
            {
                list[a].transform.localPosition = new Vector3(-50f + (100f / grootte * a) + (100f / (grootte * 2f)), 50f - (100f / grootte * (i - 1)) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public void NaarBoven()
    {
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
            if (getal % grootte == 0) knoppenKolom1.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 1) knoppenKolom2.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 2) knoppenKolom3.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 3) knoppenKolom4.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 4) knoppenKolom5.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 5) knoppenKolom6.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 6) knoppenKolom7.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 7) knoppenKolom8.Add(knoppenGesorteerd[i]);
        }
        for (int i = 1; i <= grootte; i++)
        {
            bool magDezeOok = true;
            List<GameObject> list = i == 1 ? knoppenKolom1 : i == 2 ? knoppenKolom2 : i == 3 ? knoppenKolom3 : i == 4 ? knoppenKolom4 : i == 5 ? knoppenKolom5 : i == 6 ? knoppenKolom6 : i == 7 ? knoppenKolom7 : i == 8 ? knoppenKolom8 : null;
            for (int a = 1; a < list.Count; a++)
            {
                magDezeOok = !magDezeOok || list.Count < a || VoegSamen(list[a], list[a - 1]);
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                list.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 0; a < list.Count; a++)
            {
                list[a].transform.localPosition = new Vector3(-50f + (100f / grootte * (i - 1)) + (100f / (grootte * 2f)), 50f - (100f / grootte * a) - (100f / (grootte * 2f)), -1f);
            }
        }
    }

    public void NaarOnder()
    {
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
            if (getal % grootte == 0) knoppenKolom1.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 1) knoppenKolom2.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 2) knoppenKolom3.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 3) knoppenKolom4.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 4) knoppenKolom5.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 5) knoppenKolom6.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 6) knoppenKolom7.Add(knoppenGesorteerd[i]);
            else if (getal % grootte == 7) knoppenKolom8.Add(knoppenGesorteerd[i]);
        }
        for (int i = 1; i <= grootte; i++)
        {
            bool magDezeOok = true;
            List<GameObject> list = i == 1 ? knoppenKolom1 : i == 2 ? knoppenKolom2 : i == 3 ? knoppenKolom3 : i == 4 ? knoppenKolom4 : i == 5 ? knoppenKolom5 : i == 6 ? knoppenKolom6 : i == 7 ? knoppenKolom7 : i == 8 ? knoppenKolom8 : null;
            for (int a = 2; a <= list.Count; a++)
            {
                magDezeOok = !magDezeOok || list.Count < a || VoegSamen(list[^a], list[^(a - 1)]);
            }
            for (int w = 0; w < buttonsOmTeWissen.Count; w++)
            {
                list.Remove(buttonsOmTeWissen[w]);
            }
            buttonsOmTeWissen.Clear();
            for (int a = 1; a <= list.Count; a++)
            {
                list[^a].transform.localPosition = new Vector3(-50f + (100f / grootte * (i - 1)) + (100f / (grootte * 2f)), -50f + (100f / grootte * a) - (100f / (grootte * 2f)), -1f);
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
        WisOudeGegevens();
        saveScript.intDict["begonnenAan2048"] = 0;
        obj2048.SetActive(false);
        overigCanvas.SetActive(false);
        uitlegCanvas.SetActive(false);
        menuUICanvasObj.SetActive(false);
        gehaaldCanvas.SetActive(true);
        layout2048.SetLayout();
    }

    private void WisOudeGegevens()
    {
        for (int i = 0; i < 100; i++)
        {
            saveScript.intDict["2048Knop" + i] = 0;
        }
    }
}
