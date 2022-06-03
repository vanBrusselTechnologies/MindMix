using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KnoppenScriptSolitaire : BaseUIHandler
{
    [Header("Other scene specific")]
    [SerializeField] private TMP_Text beloningText;
    [SerializeField] private GameObject maakAfKnop;
    [SerializeField] private GameObject restStapelOmdraaiKnop;

    private List<GameObject> StapelRest = new List<GameObject>();
    [HideInInspector] public List<GameObject> OmgedraaideRest = new List<GameObject>();

    private bool canBeFinished = false;
    private bool omdraaiKnopGedeactiveerd = false;

    private SolitaireScript solitaireScript;
    private BeloningScript beloningScript;

    protected override void Start()
    {
        baseLayout = GetComponent<SolitaireLayout>();
        base.Start();
        if (saveScript == null) return;
        solitaireScript = GetComponent<SolitaireScript>();
        StapelRest = solitaireScript.StapelRest;
        beloningScript = BeloningScript.Instance;
        if (gegevensHouder.startNewGame)
        {
            WisOudeGegevens();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (finishedGameUIObj.activeInHierarchy)
        {
            return;
        }
        if (!omdraaiKnopGedeactiveerd && (StapelRest.Count + OmgedraaideRest.Count == 0 || (OmgedraaideRest.Count == 1 && StapelRest.Count == 0)))
        {
            omdraaiKnopGedeactiveerd = true;
            restStapelOmdraaiKnop.SetActive(false);
        }
        if (canBeFinished) return;
        bool een = false;
        bool twee = false;
        bool drie = false;
        bool vier = false;
        bool vijf = false;
        bool zes = false;
        bool zeven = false;
        if (solitaireScript.Stapel1.Count == 0 || solitaireScript.Stapel1[0].name == "leeg_leeg" || solitaireScript.Stapel1[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            een = true;
        }
        if (solitaireScript.Stapel2.Count == 0 || solitaireScript.Stapel2[0].name == "leeg_leeg" || solitaireScript.Stapel2[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            twee = true;
        }
        if (solitaireScript.Stapel3.Count == 0 || solitaireScript.Stapel3[0].name == "leeg_leeg" || solitaireScript.Stapel3[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            drie = true;
        }
        if (solitaireScript.Stapel4.Count == 0 || solitaireScript.Stapel4[0].name == "leeg_leeg" || solitaireScript.Stapel4[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            vier = true;
        }
        if (solitaireScript.Stapel5.Count == 0 || solitaireScript.Stapel5[0].name == "leeg_leeg" || solitaireScript.Stapel5[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            vijf = true;
        }
        if (solitaireScript.Stapel6.Count == 0 || solitaireScript.Stapel6[0].name == "leeg_leeg" || solitaireScript.Stapel6[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            zes = true;
        }
        if (solitaireScript.Stapel7.Count == 0 || solitaireScript.Stapel7[0].name == "leeg_leeg" || solitaireScript.Stapel7[0].transform.localEulerAngles != new Vector3(0, 180, 0))
        {
            zeven = true;
        }
        if (een && twee && drie && vier && vijf && zes && zeven)
        {
            maakAfKnop.SetActive(true);
            canBeFinished = true;
        }
    }

    public void OnClickButtonStapelRest()
    {
        if (StapelRest.Count != 0)
        {
            OmgedraaideRest.Add(StapelRest[^1]);
            StapelRest.RemoveAt(StapelRest.Count - 1);
            for (int i = 0; i < OmgedraaideRest.Count; i++)
            {
                saveScript.intDict["ReststapelOmgekeerd:" + i] = solitaireScript.kaarten.IndexOf(OmgedraaideRest[i]);
            }
            OmgedraaideRest[^1].transform.localEulerAngles = Vector3.zero;
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
                saveScript.intDict["Reststapel:" + i] = solitaireScript.kaarten.IndexOf(OmgedraaideRest[totI - (i + 1)]);
                OmgedraaideRest.RemoveAt(totI - (i + 1));
                saveScript.intDict["ReststapelOmgekeerd:" + i] = 0;
                StapelRest[i].transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            saveScript.intDict["ReststapelGrootte"] = StapelRest.Count;
            saveScript.intDict["ReststapelOmgekeerdGrootte"] = OmgedraaideRest.Count;
        }
        baseLayout.SetLayout();
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
        solitaireScript.voltooid = true;
        for (int i = 0; i < solitaireScript.kaarten.Count; i++)
        {
            if (solitaireScript.kaarten[i].name.Split('_')[1] != "K")
            {
                solitaireScript.kaarten[i].transform.position = new Vector3(0, 0, 10);
            }
            else if (solitaireScript.kaarten[i].name.Split('_')[0] == "Klaver")
            {
                solitaireScript.kaarten[i].transform.SetPositionAndRotation(new Vector3(-3f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
            else if (solitaireScript.kaarten[i].name.Split('_')[0] == "Ruiten")
            {
                solitaireScript.kaarten[i].transform.SetPositionAndRotation(new Vector3(-1.25f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
            else if (solitaireScript.kaarten[i].name.Split('_')[0] == "Harten")
            {
                solitaireScript.kaarten[i].transform.SetPositionAndRotation(new Vector3(0.5f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
            else if (solitaireScript.kaarten[i].name.Split('_')[0] == "Schoppe")
            {
                solitaireScript.kaarten[i].transform.SetPositionAndRotation(new Vector3(2.25f, 3f, -2f), new Quaternion(0, 0, 0, 1));
            }
        }
        maakAfKnop.SetActive(false);
        finishedGameUIObj.SetActive(true);
        gameSpecificRootObj.SetActive(false);
        generalCanvasObj.SetActive(false);
        menuCanvasObj.SetActive(false);
        helpUICanvasObj.SetActive(false);
        settingsCanvasObj.SetActive(false);
        baseLayout.SetLayout();
        WisOudeGegevens();
        saveScript.intDict["aanSolitaireBegonnen"] = 0;
    }

    public override void OpenHelpUI()
    {
        base.OpenHelpUI();
        solitaireScript.uitlegActief = helpUICanvasObj.activeSelf;
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

    public override void OpenSettings()
    {
        base.OpenSettings();
        solitaireScript.uitlegActief = settingsCanvasObj.activeSelf;
    }
}
