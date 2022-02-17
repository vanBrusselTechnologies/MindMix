using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KnoppenScriptSolitaire : MonoBehaviour
{
    private List<GameObject> StapelRest = new List<GameObject>();
    [HideInInspector] public List<GameObject> OmgedraaideRest = new List<GameObject>();
    private SolitaireScript ScriptSolitaire;
    private SolitaireLayout solitaireLayout;
    [SerializeField] private GameObject showMenuKnop;
    [SerializeField] private RectTransform showMenuKnopRect;
    [SerializeField] private RectTransform menuImageRect;
    [SerializeField] private RectTransform menuNieuweKnopRect;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private GameObject SolitaireMenu;
    [SerializeField] private RectTransform SolitaireMenuRect;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private RectTransform uitlegTitelRect;
    [SerializeField] private RectTransform uitlegTekstRect;
    [SerializeField] private RectTransform uitlegSluitKnopRect;
    [SerializeField] private GameObject solitaire;
    [SerializeField] private GameObject overigCanvas;
    public GameObject gehaaldCanvas;
    [SerializeField] private TMP_Text beloningText;
    [SerializeField] private RectTransform gehaaldCanvasTitelRect;
    [SerializeField] private RectTransform gehaaldCanvasTekstRect;
    [SerializeField] private RectTransform gehaaldCanvasStartNieuweKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasNaarMenuKnopRect;
    [SerializeField] private RectTransform gehaaldCanvasRewardRect;
    [SerializeField] private GameObject gehaaldCanvasRewardVerdubbelObj;
    [SerializeField] private GameObject maakAfKnop;
    [SerializeField] private GameObject terugNaarMenuKnop;
    [SerializeField] private GameObject restStapelOmdraaiKnop;
    private GegevensHouder gegevensScript;
    private BeloningScript beloningScript;
    private bool klaar = false;
    private SaveScript saveScript;
    private bool omdraaiKnopGedeactiveerd = false;

    private void Awake()
    {
        solitaireLayout = GetComponent<SolitaireLayout>();
        ScriptSolitaire = GetComponent<SolitaireScript>();
        StapelRest = ScriptSolitaire.StapelRest;
        SolitaireMenu.SetActive(false);
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        beloningScript = gegevensHouder.GetComponent<BeloningScript>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        if (gegevensScript.startNewSolitaire)
        {
            WisOudeGegevens();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gehaaldCanvas.activeInHierarchy)
        {
            return;
        }
        if (!omdraaiKnopGedeactiveerd && (StapelRest.Count + OmgedraaideRest.Count == 0 || (OmgedraaideRest.Count == 1 && StapelRest.Count == 0)))
        {
            omdraaiKnopGedeactiveerd = true;
            restStapelOmdraaiKnop.SetActive(false);
        }
        if (klaar) return;
        bool een = false;
        bool twee = false;
        bool drie = false;
        bool vier = false;
        bool vijf = false;
        bool zes = false;
        bool zeven = false;
        if (ScriptSolitaire.Stapel1.Count == 0)
        {
            een = true;
        }
        else if (ScriptSolitaire.Stapel1[0].name == "leeg_leeg")
        {
            een = true;
        }
        else if (ScriptSolitaire.Stapel1[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            een = true;
        }
        if (ScriptSolitaire.Stapel2.Count == 0)
        {
            twee = true;
        }
        else if (ScriptSolitaire.Stapel2[0].name == "leeg_leeg")
        {
            twee = true;
        }
        else if (ScriptSolitaire.Stapel2[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            twee = true;
        }
        if (ScriptSolitaire.Stapel3.Count == 0)
        {
            drie = true;
        }
        else if (ScriptSolitaire.Stapel3[0].name == "leeg_leeg")
        {
            drie = true;
        }
        else if (ScriptSolitaire.Stapel3[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            drie = true;
        }
        if (ScriptSolitaire.Stapel4.Count == 0)
        {
            vier = true;
        }
        else if (ScriptSolitaire.Stapel4[0].name == "leeg_leeg")
        {
            vier = true;
        }
        else if (ScriptSolitaire.Stapel4[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            vier = true;
        }
        if (ScriptSolitaire.Stapel5.Count == 0)
        {
            vijf = true;
        }
        else if (ScriptSolitaire.Stapel5[0].name == "leeg_leeg")
        {
            vijf = true;
        }
        else if (ScriptSolitaire.Stapel5[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            vijf = true;
        }
        if (ScriptSolitaire.Stapel6.Count == 0)
        {
            zes = true;
        }
        else if (ScriptSolitaire.Stapel6[0].name == "leeg_leeg")
        {
            zes = true;
        }
        else if (ScriptSolitaire.Stapel6[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            zes = true;
        }
        if (ScriptSolitaire.Stapel7.Count == 0)
        {
            zeven = true;
        }
        else if (ScriptSolitaire.Stapel7[0].name == "leeg_leeg")
        {
            zeven = true;
        }
        else if (ScriptSolitaire.Stapel7[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            zeven = true;
        }
        if (een && twee && drie && vier && vijf && zes && zeven)
        {
            maakAfKnop.transform.localPosition = new Vector3(0, 0f, -1f);
            klaar = true;
        }
    }

    public void OnClickButtonStapelRest()
    {
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float saveZoneLinks = Camera.main.orthographicSize * Screen.width / Screen.height / (Screen.width / Screen.safeArea.x);
        float saveZoneRechts = Camera.main.orthographicSize * Screen.width / Screen.height / (Screen.width / (Screen.width - Screen.safeArea.width - Screen.safeArea.x));
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        float saveZoneOnder = Camera.main.orthographicSize / (Screen.height / Screen.safeArea.y);
        float saveZoneBoven = Camera.main.orthographicSize / (Screen.height / (Screen.height - Screen.safeArea.height - Screen.safeArea.y));
        schermWijdte = Mathf.Min(schermWijdte, schermHoogte * (8f / 4.5f));
        float xStapel6 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * 22f);
        float xStapel7 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * 33f);
        float basisYeind = saveZoneOnder - saveZoneBoven + (schermHoogte / 1.5f / 2f * -1f) + (schermHoogte / 35f / 1.5f * ((5f + (1f / 6f)) * 10f / 1.5f));
        if (StapelRest.Count != 0)
        {
            OmgedraaideRest.Add(StapelRest[^1]);
            StapelRest.RemoveAt(StapelRest.Count - 1);
            for (int i = 0; i < OmgedraaideRest.Count; i++)
            {
                OmgedraaideRest[i].transform.position = new Vector3(xStapel6, basisYeind, (-1f) - (i * 0.1f));
                saveScript.intDict["ReststapelOmgekeerd:" + i] = ScriptSolitaire.kaarten.IndexOf(OmgedraaideRest[i]);
            }
            OmgedraaideRest[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            saveScript.intDict["ReststapelGrootte"] = StapelRest.Count;
            saveScript.intDict["ReststapelOmgekeerdGrootte"] = OmgedraaideRest.Count;
        }
        else if (OmgedraaideRest.Count != 0)
        {
            int totI = OmgedraaideRest.Count;
            saveScript.intDict["ReststapelGrootte"] = totI;
            saveScript.intDict["ReststapelOmgekeerdGrootte"] = 0;
            for (int i = 0; i < totI; i++)
            {
                StapelRest.Add(OmgedraaideRest[totI - (i + 1)]);
                saveScript.intDict["Reststapel:" + i] = ScriptSolitaire.kaarten.IndexOf(OmgedraaideRest[totI - (i + 1)]);
                OmgedraaideRest.RemoveAt(totI - (i + 1));
                saveScript.intDict["ReststapelOmgekeerd:" + i] = 0;
                StapelRest[i].transform.position = new Vector3(xStapel7, basisYeind, -1f - (i * 0.1f));
                StapelRest[i].transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            saveScript.intDict["ReststapelGrootte"] = StapelRest.Count;
            saveScript.intDict["ReststapelOmgekeerdGrootte"] = OmgedraaideRest.Count;
        }
    }

    public void MaakSolitaireAf(GameObject knop = null)
    {
        if(knop != null)
        {
            Scene scene = SceneManager.GetActiveScene();
            float tijd = saveScript.floatDict["SolitaireTijd"];
            saveScript.floatDict["SolitaireSnelsteTijd"] = Mathf.Min(saveScript.floatDict["SolitaireSnelsteTijd"], tijd);
            saveScript.intDict["SolitairesGespeeld"] += 1;
            beloningText.text = beloningScript.Beloning(scene: scene, score: tijd, doelwitText: beloningText).ToString();
        }
        ScriptSolitaire.voltooid = true;
        for (int i = 0; i < ScriptSolitaire.kaarten.Count; i++)
        {
            if (ScriptSolitaire.kaarten[i].name.Split('_')[1] != "K")
            {
                ScriptSolitaire.kaarten[i].transform.position = new Vector3(0, 0, 10);
            }
            else if (ScriptSolitaire.kaarten[i].name.Split('_')[0] == "Klaver")
            {
                ScriptSolitaire.kaarten[i].transform.SetPositionAndRotation(new Vector3(-3f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
            else if (ScriptSolitaire.kaarten[i].name.Split('_')[0] == "Ruiten")
            {
                ScriptSolitaire.kaarten[i].transform.SetPositionAndRotation(new Vector3(-1.25f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
            else if (ScriptSolitaire.kaarten[i].name.Split('_')[0] == "Harten")
            {
                ScriptSolitaire.kaarten[i].transform.SetPositionAndRotation(new Vector3(0.5f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
            else if (ScriptSolitaire.kaarten[i].name.Split('_')[0] == "Schoppe")
            {
                ScriptSolitaire.kaarten[i].transform.SetPositionAndRotation(new Vector3(2.25f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
        }
        maakAfKnop.transform.localPosition = new Vector3(0, -10000000f, -9.45f);
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
        solitaire.SetActive(false);
        overigCanvas.SetActive(false);
        uitlegUI.SetActive(false);
        WisOudeGegevens();
        saveScript.intDict["aanSolitaireBegonnen"] = 0;
    }

    public void nieuweSolitaire()
    {
        gegevensScript.startNewSolitaire = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void terugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void OpenMenu()
    {
        bool verticaal = Screen.safeArea.width < Screen.safeArea.height;
        bool openen;
        if (showMenuKnop.transform.localEulerAngles == new Vector3(0, 0, 0) || showMenuKnop.transform.localEulerAngles == new Vector3(0, 0, 270))
        {
            showMenuKnop.transform.localEulerAngles += new Vector3(0, 0, 180);
            openen = false;
        }
        else
        {
            showMenuKnop.transform.localEulerAngles -= new Vector3(0, 0, 180);
            openen = true;
        }
        SolitaireMenu.SetActive(true);
        if (openen)
        {
            if (verticaal)
            {
                SolitaireMenuRect.sizeDelta = new Vector2(Screen.width, Screen.safeArea.y + Screen.safeArea.height * 0.15f);
                SolitaireMenuRect.anchoredPosition = new Vector2(0, -SolitaireMenuRect.sizeDelta.y / 2f + Screen.safeArea.y);
                float schaal = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) / 1080f * 1.1f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) / 2520f * 1.1f);
                menuNieuweKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y / 2f);
                menuNieuweKnopRect.localScale = new Vector3(schaal, schaal, 1);
            }
            else
            {
                SolitaireMenuRect.sizeDelta = new Vector2(Screen.safeArea.x + Screen.safeArea.width * 0.2f, Screen.height);
                SolitaireMenuRect.anchoredPosition = new Vector2(Screen.safeArea.x - SolitaireMenuRect.sizeDelta.x / 2f - (Screen.width / 2), Screen.height / 2f);
                float schaal = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) / 1080f * 1f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) / 2520f * 1f);
                terugNaarMenuKnopRect.transform.SetParent(SolitaireMenu.transform);
                terugNaarMenuKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 11, Screen.safeArea.height / 11);
                terugNaarMenuKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - SolitaireMenuRect.sizeDelta.x / 2f + terugNaarMenuKnopRect.sizeDelta.x / 2f, -Screen.safeArea.y / 2f + Screen.height / 2f - terugNaarMenuKnopRect.sizeDelta.y / 2f);
                menuNieuweKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x / 2f, 0);
                menuNieuweKnopRect.localScale = new Vector3(schaal, schaal, 1);
            }
        }
        StartCoroutine(LaatMenuZien(openen, verticaal));
    }

    private IEnumerator LaatMenuZien(bool welLatenZien, bool verticaal)
    {
        float speed = 50f;
        SolitaireMenu.SetActive(true);
        if (verticaal)
        {
            if (welLatenZien)
            {
                showMenuKnop.transform.Translate(Vector3.up * speed);
                SolitaireMenu.transform.Translate(Vector3.up * speed);
                if (SolitaireMenuRect.anchoredPosition.y > SolitaireMenuRect.sizeDelta.y / 2f)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(0, SolitaireMenuRect.sizeDelta.y + showMenuKnopRect.sizeDelta.y / 2f);
                    SolitaireMenuRect.anchoredPosition = new Vector2(0, SolitaireMenuRect.sizeDelta.y / 2f);
                    StopAllCoroutines();
                }
            }
            else
            {
                showMenuKnop.transform.Translate(1.5f * speed * Vector3.up);
                SolitaireMenu.transform.Translate(1.5f * speed * Vector3.down);
                if (SolitaireMenuRect.anchoredPosition.y < -SolitaireMenuRect.sizeDelta.y / 2f + Screen.safeArea.y)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + showMenuKnopRect.sizeDelta.y / 2f);
                    SolitaireMenu.SetActive(false);
                    StopAllCoroutines();
                }
            }
        }
        else
        {
            if (welLatenZien)
            {
                showMenuKnop.transform.Translate(Vector3.up * speed);
                SolitaireMenu.transform.Translate(Vector3.right * speed);
                if (SolitaireMenuRect.anchoredPosition.x > SolitaireMenuRect.sizeDelta.x / 2f - (Screen.width / 2))
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(SolitaireMenuRect.sizeDelta.x - (Screen.width / 2f) + showMenuKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
                    SolitaireMenuRect.anchoredPosition = new Vector2((SolitaireMenuRect.sizeDelta.x / 2f) - (Screen.width / 2f), Screen.height / 2f);
                    StopAllCoroutines();
                }
            }
            else
            {
                showMenuKnop.transform.Translate(1.5f * speed * Vector3.up);
                SolitaireMenu.transform.Translate(1.5f * speed * Vector3.left);
                if (SolitaireMenuRect.anchoredPosition.x < -SolitaireMenuRect.sizeDelta.x / 2f - (Screen.width / 2) + Screen.safeArea.x)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + showMenuKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
                    SolitaireMenu.SetActive(false);
                    StopAllCoroutines();
                }
            }
        }
        yield return gegevensScript.wachtHonderdste;
        StartCoroutine(LaatMenuZien(welLatenZien, verticaal));
    }

    public void OpenUitleg()
    {
        if (uitlegUI.activeSelf)
        {
            ScriptSolitaire.uitlegActief = false;
            uitlegUI.SetActive(false);
            solitaire.SetActive(true);
            overigCanvas.SetActive(true);
            solitaireLayout.StartLayout();
        }
        else
        {
            ScriptSolitaire.uitlegActief = true;
            solitaire.SetActive(false);
            overigCanvas.SetActive(false);
            uitlegUI.SetActive(true);
            uitlegTitelRect.anchoredPosition = new Vector2(0, Screen.safeArea.height * (1f / 3f));
            uitlegTitelRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.75f, Screen.safeArea.height);
            uitlegTekstRect.anchoredPosition = new Vector2(0, -Screen.safeArea.height / 8f);
            uitlegTekstRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height / 2);
            uitlegSluitKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f));
            uitlegSluitKnopRect.localScale = new Vector2(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f, Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f) / 108f;
            float kleinsteKant = Mathf.Min(Screen.safeArea.height, Screen.safeArea.width);
            float grootsteKant = Mathf.Max(Screen.safeArea.height, Screen.safeArea.width);
            if (kleinsteKant - 1440 > 0)
            {
                float factor = Mathf.Min(kleinsteKant / 1500f, grootsteKant / 2500f);
                uitlegTitelRect.localScale = Vector2.one * factor;
                uitlegTitelRect.sizeDelta /= factor;
            }
        }
    }

    public void WisOudeGegevens()
    {
        for (int i = 0; i < 30; i++)
        {
            saveScript.intDict["Reststapel: " + i] = 0;
            saveScript.intDict["ReststapelOmgekeerd:" + i] = 0;
        }
        for (int i = 0; i < 8; i++)
        {
            saveScript.intDict["Stapel" + i + "Grootte"] = 0;
            saveScript.intDict["Stapel" + i + "Gedraaid"] = 0;
            saveScript.intDict["Eindstapel" + i + "Grootte"] = 0;
            for (int ii = 0; ii < 20; ii++)
            {
                saveScript.intDict["Stapel" + i + ":" + ii] = 0;
                saveScript.intDict["Eindstapel" + i + ":" + ii] = 0;
            }
        }
        saveScript.intDict["ReststapelGrootte"] = 0;
        saveScript.intDict["ReststapelOmgekeerdGrootte"] = 0;
    }
}
