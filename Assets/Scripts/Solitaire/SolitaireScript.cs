using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SolitaireScript : MonoBehaviour
{
    public List<GameObject> kaarten = new List<GameObject>();
    private List<GameObject> kaartenGeschud = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel1 = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel2 = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel3 = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel4 = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel5 = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel6 = new List<GameObject>();
    [HideInInspector] public List<GameObject> Stapel7 = new List<GameObject>();
    [HideInInspector] public List<GameObject> StapelRest = new List<GameObject>();
    [HideInInspector] public List<GameObject> EindStapel1 = new List<GameObject>();
    [HideInInspector] public List<GameObject> EindStapel2 = new List<GameObject>();
    [HideInInspector] public List<GameObject> EindStapel3 = new List<GameObject>();
    [HideInInspector] public List<GameObject> EindStapel4 = new List<GameObject>();
    private KnoppenScriptSolitaire knoppenScript;
    private float tijd;
    private float startTijdSolitaire;
    [SerializeField] private TMP_Text tijdtijd;
    [HideInInspector] public bool voltooid = false;
    private GegevensHouder gegevensScript;
    private BeloningScript beloningScript;
    private int minAantalKaartenStapel1 = 10;
    private int minAantalKaartenStapel2 = 10;
    private int minAantalKaartenStapel3 = 10;
    private int minAantalKaartenStapel4 = 10;
    private int minAantalKaartenStapel5 = 10;
    private int minAantalKaartenStapel6 = 10;
    private int minAantalKaartenStapel7 = 10;
    [SerializeField] private GameObject Stapel1Houder;
    [SerializeField] private GameObject Stapel2Houder;
    [SerializeField] private GameObject Stapel3Houder;
    [SerializeField] private GameObject Stapel4Houder;
    [SerializeField] private GameObject Stapel5Houder;
    [SerializeField] private GameObject Stapel6Houder;
    [SerializeField] private GameObject Stapel7Houder;
    [SerializeField] private GameObject Eindstapel1Houder;
    [SerializeField] private GameObject Eindstapel2Houder;
    [SerializeField] private GameObject Eindstapel3Houder;
    [SerializeField] private GameObject Eindstapel4Houder;
    [SerializeField] private GameObject RestStapelOmdraaiKnop;
    [SerializeField] private GameObject maakAfKnop;
    [SerializeField] private TMP_Text beloningText;
    private SaveScript saveScript;
    private float totaleTijdstraf = 0f;
    private float tijdstraf = 5f;
    [HideInInspector] public bool uitlegActief = false;

    // Use this for initialization
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
        maakAfKnop.SetActive(false);
        knoppenScript = GetComponent<KnoppenScriptSolitaire>();
        if (gegevensScript.startNewSolitaire)
        {
            knoppenScript.WisOudeGegevens();
            VindKaarten();
            SchudKaarten();
            ZetBeginKaartenInStapel(true);
            ZetKaartenOpGoedePlek(true);
            DraaiVoorsteKaartOm(false);
            tijd = 0f;
            startTijdSolitaire = Time.time;
            saveScript.floatDict["SolitaireTijd"] = 0;
            saveScript.intDict["aanSolitaireBegonnen"] = 1;
        }
        else
        {
            VindKaarten();
            minAantalKaartenStapel1 = saveScript.intDict["Stapel1Gedraaid"];
            minAantalKaartenStapel2 = saveScript.intDict["Stapel2Gedraaid"];
            minAantalKaartenStapel3 = saveScript.intDict["Stapel3Gedraaid"];
            minAantalKaartenStapel4 = saveScript.intDict["Stapel4Gedraaid"];
            minAantalKaartenStapel5 = saveScript.intDict["Stapel5Gedraaid"];
            minAantalKaartenStapel6 = saveScript.intDict["Stapel6Gedraaid"];
            minAantalKaartenStapel7 = saveScript.intDict["Stapel7Gedraaid"];
            ZetBeginKaartenInStapel(false);
            ZetKaartenOpGoedePlek(true);
            DraaiVoorsteKaartOm(true);
            tijd = saveScript.floatDict["SolitaireTijd"];
            startTijdSolitaire = Time.time - tijd;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (voltooid || uitlegActief)
        {
            if(uitlegActief) startTijdSolitaire = Time.time - tijd;
            return;
        }
        else
        {
            DraaiVoorsteKaartOm(false);
            if (EindStapel1.Count >= 13 && EindStapel2.Count >= 13 && EindStapel2.Count >= 13 && EindStapel2.Count >= 13)
            {
                if (EindStapel1[^1].name.EndsWith('K') && EindStapel2[^1].name.EndsWith('K') && EindStapel3[^1].name.EndsWith('K') && EindStapel4[^1].name.EndsWith('K')) {
                    voltooid = true;
                    maakAfKnop.SetActive(false);
                    Scene scene = SceneManager.GetActiveScene();
                    float tijd = saveScript.floatDict["SolitaireTijd"];
                    saveScript.floatDict["SolitaireSnelsteTijd"] = Mathf.Min(saveScript.floatDict["SolitaireSnelsteTijd"], tijd);
                    saveScript.intDict["SolitairesGespeeld"] += 1;
                    beloningText.text = beloningScript.Beloning(scene: scene, score: tijd, doelwitText: beloningText).ToString();
                    knoppenScript.MaakSolitaireAf();
                    return;
                }
            }
            HoelangAlBezig();
        }
    }

    public void VindKaarten()
    {
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        schermWijdte = Mathf.Min(schermWijdte, schermHoogte * (8f / 4.5f));
        Vector3 scaleKaart = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
        for (int i = 0; i < 52; i++)
        {
            kaarten[i].transform.localScale = scaleKaart;
            kaarten[i].transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        Vector3 scaleStapel = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * (schermWijdte / 81f * 10);
        Stapel1Houder.transform.localScale = scaleStapel;
        Stapel2Houder.transform.localScale = scaleStapel;
        Stapel3Houder.transform.localScale = scaleStapel;
        Stapel4Houder.transform.localScale = scaleStapel;
        Stapel5Houder.transform.localScale = scaleStapel;
        Stapel6Houder.transform.localScale = scaleStapel;
        Stapel7Houder.transform.localScale = scaleStapel;
        Eindstapel1Houder.transform.localScale = scaleStapel;
        Eindstapel2Houder.transform.localScale = scaleStapel;
        Eindstapel3Houder.transform.localScale = scaleStapel;
        Eindstapel4Houder.transform.localScale = scaleStapel;
        RestStapelOmdraaiKnop.transform.localScale = new Vector3(1, 1, 1 / (Screen.safeArea.width / 81f / 10)) * (Screen.safeArea.width / 81f / 10);
    }

    public void SchudKaarten()
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < 52; i++)
        {
            temp.Add(kaarten[i]);
        }
        for (int i = 0; i < temp.Count;)
        {
            int rand = Random.Range(0, temp.Count);
            kaartenGeschud.Add(temp[rand]);
            temp.RemoveAt(rand);
        }
    }

    public void ZetBeginKaartenInStapel(bool eersteKeer)
    {
        if (eersteKeer) {
            for (int i = 0; i < 52; i++)
            {
                if (i == 0)
                {
                    Stapel1.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel1:" + i] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel1Grootte"] = Stapel1.Count;
                }
                else if (i <= 2)
                {
                    Stapel2.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel2:" + (i - 1)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel2Grootte"] = Stapel2.Count;
                }
                else if (i <= 5)
                {
                    Stapel3.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel3:" + (i - 3)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel3Grootte"] = Stapel3.Count;
                }
                else if (i <= 9)
                {
                    Stapel4.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel4:" + (i - 6)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel4Grootte"] = Stapel4.Count;
                }
                else if (i <= 14)
                {
                    Stapel5.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel5:" + (i - 10)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel5Grootte"] = Stapel5.Count;
                }
                else if (i <= 20)
                {
                    Stapel6.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel6:" + (i - 15)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel6Grootte"] = Stapel6.Count;
                }
                else if (i <= 27)
                {
                    Stapel7.Add(kaartenGeschud[i]);
                    saveScript.intDict["Stapel7:" + (i - 21)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["Stapel7Grootte"] = Stapel7.Count;
                }
                else
                {
                    StapelRest.Add(kaartenGeschud[i]);
                    saveScript.intDict["Reststapel:" + (i - 28)] = kaarten.IndexOf(kaartenGeschud[i]);
                    saveScript.intDict["ReststapelGrootte"] = StapelRest.Count;
                }
            }
        }
        else
        {
            for(int i = 0; i < saveScript.intDict["Stapel1Grootte"]; i++)
            {
                Stapel1.Add(kaarten[saveScript.intDict["Stapel1:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Stapel2Grootte"]; i++)
            {
                Stapel2.Add(kaarten[saveScript.intDict["Stapel2:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Stapel3Grootte"]; i++)
            {
                Stapel3.Add(kaarten[saveScript.intDict["Stapel3:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Stapel4Grootte"]; i++)
            {
                Stapel4.Add(kaarten[saveScript.intDict["Stapel4:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Stapel5Grootte"]; i++)
            {
                Stapel5.Add(kaarten[saveScript.intDict["Stapel5:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Stapel6Grootte"]; i++)
            {
                Stapel6.Add(kaarten[saveScript.intDict["Stapel6:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Stapel7Grootte"]; i++)
            {
                Stapel7.Add(kaarten[saveScript.intDict["Stapel7:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Eindstapel1Grootte"]; i++)
            {
                EindStapel1.Add(kaarten[saveScript.intDict["Eindstapel1:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Eindstapel2Grootte"]; i++)
            {
                EindStapel2.Add(kaarten[saveScript.intDict["Eindstapel2:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Eindstapel3Grootte"]; i++)
            {
                EindStapel3.Add(kaarten[saveScript.intDict["Eindstapel3:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["Eindstapel4Grootte"]; i++)
            {
                EindStapel4.Add(kaarten[saveScript.intDict["Eindstapel4:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["ReststapelGrootte"]; i++)
            {
                StapelRest.Add(kaarten[saveScript.intDict["Reststapel:" + i]]);
            }
            for (int i = 0; i < saveScript.intDict["ReststapelOmgekeerdGrootte"]; i++)
            {
                knoppenScript.OmgedraaideRest.Add(kaarten[saveScript.intDict["ReststapelOmgekeerd:" + i]]);
            }
        }
    }

    public void ZetKaartenOpGoedePlek(bool eersteKeer)
    {
        minAantalKaartenStapel1 = Mathf.Min(minAantalKaartenStapel1, Stapel1.Count);
        minAantalKaartenStapel2 = Mathf.Min(minAantalKaartenStapel2, Stapel2.Count);
        minAantalKaartenStapel3 = Mathf.Min(minAantalKaartenStapel3, Stapel3.Count);
        minAantalKaartenStapel4 = Mathf.Min(minAantalKaartenStapel4, Stapel4.Count);
        minAantalKaartenStapel5 = Mathf.Min(minAantalKaartenStapel5, Stapel5.Count);
        minAantalKaartenStapel6 = Mathf.Min(minAantalKaartenStapel6, Stapel6.Count);
        minAantalKaartenStapel7 = Mathf.Min(minAantalKaartenStapel7, Stapel7.Count);
        saveScript.intDict["Stapel1Gedraaid"] = minAantalKaartenStapel1;
        saveScript.intDict["Stapel2Gedraaid"] = minAantalKaartenStapel2;
        saveScript.intDict["Stapel3Gedraaid"] = minAantalKaartenStapel3;
        saveScript.intDict["Stapel4Gedraaid"] = minAantalKaartenStapel4;
        saveScript.intDict["Stapel5Gedraaid"] = minAantalKaartenStapel5;
        saveScript.intDict["Stapel6Gedraaid"] = minAantalKaartenStapel6;
        saveScript.intDict["Stapel7Gedraaid"] = minAantalKaartenStapel7;
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float saveZoneLinks = Camera.main.orthographicSize * Screen.width / Screen.height / (Screen.width / Screen.safeArea.x);
        float saveZoneRechts = Camera.main.orthographicSize * Screen.width / Screen.height / (Screen.width / (Screen.width - Screen.safeArea.width - Screen.safeArea.x));
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        float saveZoneOnder = Camera.main.orthographicSize / (Screen.height / Screen.safeArea.y);
        float saveZoneBoven = Camera.main.orthographicSize / (Screen.height / (Screen.height - Screen.safeArea.height - Screen.safeArea.y));
        schermWijdte = Mathf.Min(schermWijdte, schermHoogte * (8f / 4.5f));
        float xStapel1 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * -33f); 
        float xStapel2 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * -22f);
        float xStapel3 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * -11f);
        float xStapel4 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * -0f);
        float xStapel5 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * 11f);
        float xStapel6 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * 22f);
        float xStapel7 = saveZoneLinks - saveZoneRechts + (schermWijdte / 81f * 33f);
        float xRestStapelUI = Screen.safeArea.x + Mathf.Min(Screen.safeArea.width / 81f * 33f, Screen.safeArea.height / 81f * 33f * (8f / 4.5f)) + (Screen.safeArea.width / 2f);
        float basisY = saveZoneOnder - saveZoneBoven + (schermHoogte * -1f / 2f / 1.5f) + (schermHoogte / 35f / 1.5f * ((2f + (3f / 6f)) * 10f / 1.5f));
        float basisYeind = saveZoneOnder - saveZoneBoven + (schermHoogte * -1f / 2f / 1.5f) + (schermHoogte / 35f / 1.5f * ((5f + (1f / 6f)) * 10f / 1.5f));
        float restStapelBasisYUI = Screen.safeArea.y + (Screen.safeArea.height / 2) + (Screen.safeArea.height / 1.5f / 2f * -1f) + (Screen.safeArea.height / 35f / 1.5f * ((5f + (1f / 6f)) * 10f / 1.5f));
        float spaceBetweenCardsFactor = saveScript.floatDict["spaceBetweenCardsFactor"];
        if (spaceBetweenCardsFactor == 0) spaceBetweenCardsFactor = 1;
        float verschilY = 0.3f * spaceBetweenCardsFactor;
        float basisZ = -2f;
        float verschilZ = 0.1f;
        if (eersteKeer)
        {
            Stapel1Houder.transform.position = new Vector3(xStapel1, basisY, -1f);
            Stapel2Houder.transform.position = new Vector3(xStapel2, basisY, -1f);
            Stapel3Houder.transform.position = new Vector3(xStapel3, basisY, -1f);
            Stapel4Houder.transform.position = new Vector3(xStapel4, basisY, -1f);
            Stapel5Houder.transform.position = new Vector3(xStapel5, basisY, -1f);
            Stapel6Houder.transform.position = new Vector3(xStapel6, basisY, -1f);
            Stapel7Houder.transform.position = new Vector3(xStapel7, basisY, -1f);
            Eindstapel1Houder.transform.position = new Vector3(xStapel1, basisYeind, -1f);
            Eindstapel2Houder.transform.position = new Vector3(xStapel2, basisYeind, -1f);
            Eindstapel3Houder.transform.position = new Vector3(xStapel3, basisYeind, -1f);
            Eindstapel4Houder.transform.position = new Vector3(xStapel4, basisYeind, -1f);
            RestStapelOmdraaiKnop.transform.position = new Vector3(xRestStapelUI, restStapelBasisYUI, -1f);
            RestStapelOmdraaiKnop.transform.localScale = new Vector3(1, 1, 1 / Mathf.Min(Screen.safeArea.width / 81f / 10, Screen.safeArea.height / 81f / 10 * (8f / 4.5f))) * Mathf.Min(Screen.safeArea.width / 81f / 10, Screen.safeArea.height / 81f / 10 * (8f / 4.5f));
        }
        for (int i = 0; i < Stapel1.Count; i++)
        {
            Stapel1[i].transform.position = new Vector3(xStapel1, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < Stapel2.Count; i++)
        {
            Stapel2[i].transform.position = new Vector3(xStapel2, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < Stapel3.Count; i++)
        {
            Stapel3[i].transform.position = new Vector3(xStapel3, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < Stapel4.Count; i++)
        {
            Stapel4[i].transform.position = new Vector3(xStapel4, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < Stapel5.Count; i++)
        {
            Stapel5[i].transform.position = new Vector3(xStapel5, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < Stapel6.Count; i++)
        {
            Stapel6[i].transform.position = new Vector3(xStapel6, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < Stapel7.Count; i++)
        {
            Stapel7[i].transform.position = new Vector3(xStapel7, basisY - (i * verschilY), basisZ - (verschilZ * i));
        }
        for (int i = 0; i < EindStapel1.Count; i++)
        {
            EindStapel1[i].transform.position = new Vector3(xStapel1, basisYeind, basisZ - (verschilZ * i));
        }
        for (int i = 0; i < EindStapel2.Count; i++)
        {
            EindStapel2[i].transform.position = new Vector3(xStapel2, basisYeind, basisZ - (verschilZ * i));
        }
        for (int i = 0; i < EindStapel3.Count; i++)
        {
            EindStapel3[i].transform.position = new Vector3(xStapel3, basisYeind, basisZ - (verschilZ * i));
        }
        for (int i = 0; i < EindStapel4.Count; i++)
        {
            EindStapel4[i].transform.position = new Vector3(xStapel4, basisYeind, basisZ - (verschilZ * i));
        }
        if (knoppenScript.OmgedraaideRest.Count != 0)
        {
            for (int i = 0; i < knoppenScript.OmgedraaideRest.Count; i++)
            {
                knoppenScript.OmgedraaideRest[i].transform.position = new Vector3(xStapel6, basisYeind, basisZ - (verschilZ * i));
            }
        }
        for (int i = 0; i < StapelRest.Count; i++)
        {
            StapelRest[i].transform.position = new Vector3(xStapel7, basisYeind, basisZ - (verschilZ * i));
        }
    }

    public void DraaiVoorsteKaartOm(bool meerDraaienDoorGameResume)
    {
        if (Stapel1.Count != 0)
        {
            if (Stapel1[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel1[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (Stapel2.Count != 0)
        {
            if (Stapel2[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel2[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (Stapel3.Count != 0)
        {
            if (Stapel3[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel3[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (Stapel4.Count != 0)
        {
            if (Stapel4[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel4[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (Stapel5.Count != 0)
        {
            if (Stapel5[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel5[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (Stapel6.Count != 0)
        {
            if (Stapel6[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel6[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (Stapel7.Count != 0)
        {
            if (Stapel7[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel7[^1].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        if (meerDraaienDoorGameResume)
        {
            for(int i = 0; i < Stapel1.Count - Mathf.Max(minAantalKaartenStapel1, 1); i++)
            {
                Stapel1[Stapel1.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < Stapel2.Count - Mathf.Max(minAantalKaartenStapel2, 1); i++)
            {
                Stapel2[Stapel2.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < Stapel3.Count - Mathf.Max(minAantalKaartenStapel3, 1); i++)
            {
                Stapel3[Stapel3.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < Stapel4.Count - Mathf.Max(minAantalKaartenStapel4, 1); i++)
            {
                Stapel4[Stapel4.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < Stapel5.Count - Mathf.Max(minAantalKaartenStapel5, 1); i++)
            {
                Stapel5[Stapel5.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < Stapel6.Count - Mathf.Max(minAantalKaartenStapel6, 1); i++)
            {
                Stapel6[Stapel6.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < Stapel7.Count - Mathf.Max(minAantalKaartenStapel7, 1); i++)
            {
                Stapel7[Stapel7.Count - i - 2].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < EindStapel1.Count; i++)
            {
                EindStapel1[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < EindStapel2.Count; i++)
            {
                EindStapel2[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < EindStapel3.Count; i++)
            {
                EindStapel3[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < EindStapel4.Count; i++)
            {
                EindStapel4[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            for(int i = 0; i < StapelRest.Count; i++)
            {
                StapelRest[i].transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            for (int i = 0; i < knoppenScript.OmgedraaideRest.Count; i++)
            {
                knoppenScript.OmgedraaideRest[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    public void HoelangAlBezig()
    {
        string extraNul = "";
        string extraNulMin = "";
        string extraNulSec = "";
        tijd = Time.time - startTijdSolitaire + totaleTijdstraf;
        saveScript.floatDict["SolitaireTijd"] = tijd;
        if (Mathf.FloorToInt((tijd - Mathf.FloorToInt(tijd)) * 100) < 10)
        {
            extraNul = "0";
        }
        if ((Mathf.FloorToInt(tijd) - (Mathf.FloorToInt(Mathf.FloorToInt(tijd) / 60) * 60)) < 10)
        {
            extraNulSec = "0";
        }
        if (Mathf.FloorToInt(Mathf.FloorToInt(tijd) / 60) < 10)
        {
            extraNulMin = "0";
        }
        string tijdtext = extraNulMin + Mathf.FloorToInt(Mathf.FloorToInt(tijd) / 60) + ":" + extraNulSec + (Mathf.FloorToInt(tijd) - (Mathf.FloorToInt(Mathf.FloorToInt(tijd) / 60) * 60)) + "." + extraNul + Mathf.FloorToInt((tijd - Mathf.FloorToInt(tijd)) * 100);
        tijdtijd.text = tijdtext;
    }

    public void TijdStraf()
    {
        totaleTijdstraf += tijdstraf;
    }
}
