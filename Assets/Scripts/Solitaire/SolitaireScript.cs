using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SolitaireScript : MonoBehaviour
{
    private GegevensHouder gegevensHouder;
    private BeloningScript beloningScript;
    private SaveScript saveScript;
    private SolitaireLayout solitaireLayout;
    private KnoppenScriptSolitaire knoppenScript;

    public List<GameObject> kaarten = new List<GameObject>();

    [SerializeField] private TMP_Text tijdtijd;
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

    [HideInInspector] public bool uitlegActief = false;
    [HideInInspector] public bool voltooid = false;
    private float tijd;
    private float startTijdSolitaire;
    private float totaleTijdstraf = 0f;
    private float tijdstraf = 5f;
    private int minAantalKaartenStapel1 = 10;
    private int minAantalKaartenStapel2 = 10;
    private int minAantalKaartenStapel3 = 10;
    private int minAantalKaartenStapel4 = 10;
    private int minAantalKaartenStapel5 = 10;
    private int minAantalKaartenStapel6 = 10;
    private int minAantalKaartenStapel7 = 10;

    // Use this for initialization
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
        solitaireLayout = GetComponent<SolitaireLayout>();
        knoppenScript = GetComponent<KnoppenScriptSolitaire>();
        if (gegevensHouder.startNewGame)
        {
            SchudKaarten();
            ZetBeginKaartenInStapel(true);
            DraaiVoorsteKaartOm(false);
            tijd = 0f;
            startTijdSolitaire = Time.time;
            saveScript.floatDict["SolitaireTijd"] = 0;
            saveScript.intDict["aanSolitaireBegonnen"] = 1;
        }
        else
        {
            minAantalKaartenStapel1 = saveScript.intDict["Stapel1Gedraaid"];
            minAantalKaartenStapel2 = saveScript.intDict["Stapel2Gedraaid"];
            minAantalKaartenStapel3 = saveScript.intDict["Stapel3Gedraaid"];
            minAantalKaartenStapel4 = saveScript.intDict["Stapel4Gedraaid"];
            minAantalKaartenStapel5 = saveScript.intDict["Stapel5Gedraaid"];
            minAantalKaartenStapel6 = saveScript.intDict["Stapel6Gedraaid"];
            minAantalKaartenStapel7 = saveScript.intDict["Stapel7Gedraaid"];
            ZetBeginKaartenInStapel(false);
            DraaiVoorsteKaartOm(true);
            tijd = saveScript.floatDict["SolitaireTijd"];
            startTijdSolitaire = Time.time - tijd;
        }
        solitaireLayout.SetLayout();
    }

    // Update is called once per frame
    private void Update()
    {
        if (voltooid || uitlegActief)
        {
            if (uitlegActief) startTijdSolitaire = Time.time - tijd;
            return;
        }
        else
        {
            if (EindStapel1.Count >= 13 && EindStapel2.Count >= 13 && EindStapel3.Count >= 13 && EindStapel4.Count >= 13)
            {
                if (EindStapel1[^1].name.EndsWith('K') && EindStapel2[^1].name.EndsWith('K') && EindStapel3[^1].name.EndsWith('K') && EindStapel4[^1].name.EndsWith('K'))
                {
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
        if (eersteKeer)
        {
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
            for (int i = 0; i < saveScript.intDict["Stapel1Grootte"]; i++)
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

    public void DraaiVoorsteKaartOm(bool meerDraaienDoorGameResume)
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
        if (Stapel1.Count != 0)
        {
            if (Stapel1[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel1[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (Stapel2.Count != 0)
        {
            if (Stapel2[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel2[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (Stapel3.Count != 0)
        {
            if (Stapel3[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel3[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (Stapel4.Count != 0)
        {
            if (Stapel4[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel4[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (Stapel5.Count != 0)
        {
            if (Stapel5[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel5[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (Stapel6.Count != 0)
        {
            if (Stapel6[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel6[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (Stapel7.Count != 0)
        {
            if (Stapel7[^1].transform.localEulerAngles == new Vector3(0, 180, 0))
            {
                Stapel7[^1].transform.localEulerAngles = Vector3.zero;
            }
        }
        if (meerDraaienDoorGameResume)
        {
            for (int i = 0; i < Stapel1.Count - Mathf.Max(minAantalKaartenStapel1, 1); i++)
            {
                Stapel1[Stapel1.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < Stapel2.Count - Mathf.Max(minAantalKaartenStapel2, 1); i++)
            {
                Stapel2[Stapel2.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < Stapel3.Count - Mathf.Max(minAantalKaartenStapel3, 1); i++)
            {
                Stapel3[Stapel3.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < Stapel4.Count - Mathf.Max(minAantalKaartenStapel4, 1); i++)
            {
                Stapel4[Stapel4.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < Stapel5.Count - Mathf.Max(minAantalKaartenStapel5, 1); i++)
            {
                Stapel5[Stapel5.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < Stapel6.Count - Mathf.Max(minAantalKaartenStapel6, 1); i++)
            {
                Stapel6[Stapel6.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < Stapel7.Count - Mathf.Max(minAantalKaartenStapel7, 1); i++)
            {
                Stapel7[Stapel7.Count - i - 2].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < EindStapel1.Count; i++)
            {
                EindStapel1[i].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < EindStapel2.Count; i++)
            {
                EindStapel2[i].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < EindStapel3.Count; i++)
            {
                EindStapel3[i].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < EindStapel4.Count; i++)
            {
                EindStapel4[i].transform.localEulerAngles = Vector3.zero;
            }
            for (int i = 0; i < StapelRest.Count; i++)
            {
                StapelRest[i].transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            for (int i = 0; i < knoppenScript.OmgedraaideRest.Count; i++)
            {
                knoppenScript.OmgedraaideRest[i].transform.localEulerAngles = Vector3.zero;
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
