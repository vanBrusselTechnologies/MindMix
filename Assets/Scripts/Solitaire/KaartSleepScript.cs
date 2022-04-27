using System.Collections.Generic;
using UnityEngine;
using VBG.Extensions;

public class KaartSleepScript : MonoBehaviour
{
    private SolitaireScript ScriptSolitaire;
    private KnoppenScriptSolitaire knoppenScript;
    private float distance;
    private bool dragging;
    private string[] kaartsoort_kaartX = new string[2];
    private List<GameObject> voorsteKaarten = new List<GameObject>();
    private List<GameObject> mogelijk = new List<GameObject>();
    private int[] deI = { -1, -1, -1, -1, -1, -1, -1 };
    private GameObject empty;
    private int aantalKaartenTeVerplaatsen;
    private List<GameObject> deTeVerplaatsenKaarten = new List<GameObject>();
    private Vector3 positieMuis;
    private SaveScript saveScript;
    [SerializeField] private GameObject eventSystem;
    private Vector3 achterkantboven = new Vector3(0, 180, 0);
    Transform tf;

    // Use this for initialization
    private void Start()
    {
        ScriptSolitaire = eventSystem.GetComponent<SolitaireScript>();
        knoppenScript = eventSystem.GetComponent<KnoppenScriptSolitaire>();
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        tf = transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            rayPoint.z = -7f;
            tf.parent.gameObject.transform.position = rayPoint;
            if (deTeVerplaatsenKaarten.Count != 0 && KrijgStapelBijPositie(positieMuis) >= 1 && KrijgStapelBijPositie(positieMuis) <= 7)
            {
                for (int i = 0; i < deTeVerplaatsenKaarten.Count; i++)
                {
                    int g = deTeVerplaatsenKaarten.Count - aantalKaartenTeVerplaatsen;
                    Vector3 rayPointE = rayPoint;
                    rayPointE.y = rayPoint.y - ((i + 1 - g) * 0.3f);
                    rayPointE.z = rayPoint.z - ((i + 1 - g) * 0.1f);
                    deTeVerplaatsenKaarten[i].transform.position = rayPointE;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (tf.parent.localEulerAngles == achterkantboven || Input.touchCount == 0) return;
        Vector3 muisPositie = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        positieMuis = muisPositie;
        deTeVerplaatsenKaarten.Clear();
        int stapelInt = KrijgStapelBijPositie(muisPositie);
        aantalKaartenTeVerplaatsen = 1;
        if (stapelInt.IsBetween(1, 7))
        {
            GameObject parent = tf.parent.gameObject;
            List<GameObject> stapel = stapelInt == 1 ? ScriptSolitaire.Stapel1 : stapelInt == 2 ? ScriptSolitaire.Stapel2 : stapelInt == 3 ? ScriptSolitaire.Stapel3 : stapelInt == 4 ? ScriptSolitaire.Stapel4 : stapelInt == 5 ? ScriptSolitaire.Stapel5 : stapelInt == 6 ? ScriptSolitaire.Stapel6 : stapelInt == 7 ? ScriptSolitaire.Stapel7 : null;
            int parentIndex = stapel.IndexOf(parent);
            aantalKaartenTeVerplaatsen = stapel.Count - parentIndex - 1;
            if (aantalKaartenTeVerplaatsen != 0)
            {
                for (int i = 0; i < aantalKaartenTeVerplaatsen; i++)
                {
                    GameObject kaart = stapel[parentIndex + i + 1];
                    deTeVerplaatsenKaarten.Add(kaart);
                }
            }
        }
        kaartsoort_kaartX = tf.parent.gameObject.name.Split('_');
        voorsteKaarten = krijgVoorsteKaartvanStapels();
        if (kaartsoort_kaartX[0] == "Schoppe" || kaartsoort_kaartX[0] == "Klaver")
        {
            for (int i = 0; i < voorsteKaarten.Count; i++)
            {
                if (voorsteKaarten[i].name.Split('_')[0] == "Ruiten" || voorsteKaarten[i].name.Split('_')[0] == "Harten")
                {
                    mogelijk.Add(voorsteKaarten[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < voorsteKaarten.Count; i++)
            {
                if (voorsteKaarten[i].name.Split('_')[0] == "Schoppe" || voorsteKaarten[i].name.Split('_')[0] == "Klaver")
                {
                    mogelijk.Add(voorsteKaarten[i]);
                }
            }
        }
        string huidigKaartX = kaartsoort_kaartX[1];
        if (huidigKaartX.Equals("K"))
        {
            for (int i = 0; i < voorsteKaarten.Count; i++)
            {
                if (voorsteKaarten[i].name == empty.name)
                {
                    mogelijk.Add(voorsteKaarten[i]);
                    mogelijk.Add(voorsteKaarten[i]);
                }
            }
        }
        else
        {
            string benodigdeKaart = huidigKaartX.Equals("A") ? "2" : huidigKaartX.Equals("10") ? "J" : huidigKaartX.Equals("J") ? "Q" : huidigKaartX.Equals("Q") ? "K" : (int.Parse(huidigKaartX) + 1).ToString();
            for (int i = 0; i < voorsteKaarten.Count; i++)
            {
                if (voorsteKaarten[i].name.Split('_')[1] == benodigdeKaart)
                {
                    mogelijk.Add(voorsteKaarten[i]);
                }
            }
        }
        if (KanOpEindStapel())
        {
            GameObject a = new GameObject("leeg");
            mogelijk.Add(a);
            mogelijk.Add(a);
            Destroy(a);
        }
        distance = Vector3.Distance(tf.position, Camera.main.transform.position);
        dragging = true;
        mogelijk = mogelijk.GetDuplicates();
        for (int i = 0; i < mogelijk.Count; i++)
        {
            deI[i] = voorsteKaarten.IndexOf(mogelijk[i]);
        }
        GameObject[] leeg_leeg = GameObject.FindGameObjectsWithTag("helemaalLeeg");
        for (int i = 0; i < leeg_leeg.Length; i++)
        {
            Destroy(leeg_leeg[i]);
        }
    }

    private void OnMouseUp()
    {
        if (!dragging) return;
        Vector3 muisPositie = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        bool klaar = false;
        dragging = false;
        for (int i = 0; i < mogelijk.Count; i++)
        {
            int stapelInt = KrijgStapelBijPositie(muisPositie);
            if (stapelInt.IsBetween(1, 7))
            {
                List<GameObject> stapel = stapelInt == 1 ? ScriptSolitaire.Stapel1 : stapelInt == 2 ? ScriptSolitaire.Stapel2 : stapelInt == 3 ? ScriptSolitaire.Stapel3 : stapelInt == 4 ? ScriptSolitaire.Stapel4 : stapelInt == 5 ? ScriptSolitaire.Stapel5 : stapelInt == 6 ? ScriptSolitaire.Stapel6 : stapelInt == 7 ? ScriptSolitaire.Stapel7 : null;
                if(stapel.IndexOf(mogelijk[i]) != -1)
                {
                    GameObject parent = tf.parent.gameObject;
                    HaalUitStapels(parent);
                    stapel.Add(parent);
                    saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = ScriptSolitaire.kaarten.IndexOf(parent);
                    klaar = true;
                    if (mogelijk[i].name == "leeg_leeg")
                    {
                        stapel.RemoveAt(0);
                    }
                    for (int a = 0; a < deTeVerplaatsenKaarten.Count; a++)
                    {
                        HaalUitStapels(deTeVerplaatsenKaarten[a]);
                        stapel.Add(deTeVerplaatsenKaarten[a]);
                        saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = ScriptSolitaire.kaarten.IndexOf(deTeVerplaatsenKaarten[a]);
                    }
                    saveScript.intDict["Stapel" + stapelInt + "Grootte"] = stapel.Count;
                    ScriptSolitaire.ZetKaartenOpGoedePlek(false);
                }
            }
            if (!klaar && deTeVerplaatsenKaarten.Count == 0 && KanOpEindStapel())
            {
                GameObject parent = tf.parent.gameObject;
                HaalUitStapels(parent);
                int eindStapelInt = kaartsoort_kaartX[0].Equals("Klaver") ? 1 : kaartsoort_kaartX[0].Equals("Ruiten") ? 2 : kaartsoort_kaartX[0].Equals("Harten") ? 3 : kaartsoort_kaartX[0].Equals("Schoppe") ? 4 : 0;
                List<GameObject> eindStapel = eindStapelInt == 1 ? ScriptSolitaire.EindStapel1 : eindStapelInt == 2 ? ScriptSolitaire.EindStapel2 : eindStapelInt == 3 ? ScriptSolitaire.EindStapel3 : eindStapelInt == 4 ? ScriptSolitaire.EindStapel4 : null;
                eindStapel.Add(parent);
                saveScript.intDict["Eindstapel" + eindStapelInt + ":" + (eindStapel.Count - 1)] = ScriptSolitaire.kaarten.IndexOf(parent);
                klaar = true;
                saveScript.intDict["Eindstapel" + eindStapelInt + "Grootte"] = eindStapel.Count;
                ScriptSolitaire.ZetKaartenOpGoedePlek(false);
            }
        }
        if (!klaar)
        {
            int stapelInt = deI[0] + 1;
            if (mogelijk.Count != 0 && stapelInt.IsBetween(1, 7))
            {
                List<GameObject> stapel = stapelInt == 1 ? ScriptSolitaire.Stapel1 : stapelInt == 2 ? ScriptSolitaire.Stapel2 : stapelInt == 3 ? ScriptSolitaire.Stapel3 : stapelInt == 4 ? ScriptSolitaire.Stapel4 : stapelInt == 5 ? ScriptSolitaire.Stapel5 : stapelInt == 6 ? ScriptSolitaire.Stapel6 : stapelInt == 7 ? ScriptSolitaire.Stapel7 : null;
                GameObject parent = tf.parent.gameObject;
                HaalUitStapels(parent);
                stapel.Add(parent);
                saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = ScriptSolitaire.kaarten.IndexOf(parent);
                if (stapel[0].name == "leeg_leeg")
                {
                    stapel.RemoveAt(0);
                }
                for (int a = 0; a < deTeVerplaatsenKaarten.Count; a++)
                {
                    HaalUitStapels(deTeVerplaatsenKaarten[a]);
                    stapel.Add(deTeVerplaatsenKaarten[a]);
                    saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = ScriptSolitaire.kaarten.IndexOf(deTeVerplaatsenKaarten[a]);
                }
                saveScript.intDict["Stapel" + stapelInt + "Grootte"] = stapel.Count;
                ScriptSolitaire.ZetKaartenOpGoedePlek(false);
            }
            else
            {
                ScriptSolitaire.TijdStraf();
                ScriptSolitaire.ZetKaartenOpGoedePlek(false);
            }
        }
    }

    public List<GameObject> krijgVoorsteKaartvanStapels()
    {
        empty = new GameObject("leeg_leeg")
        {
            tag = "helemaalLeeg"
        };
        List<GameObject> voorsteKaarten = new List<GameObject>();
        for (int i = 1; i <= 7; i++)
        {
            List<GameObject> stapel = i == 1 ? ScriptSolitaire.Stapel1 : i == 2 ? ScriptSolitaire.Stapel2 : i == 3 ? ScriptSolitaire.Stapel3 : i == 4 ? ScriptSolitaire.Stapel4 : i == 5 ? ScriptSolitaire.Stapel5 : i == 6 ? ScriptSolitaire.Stapel6 : i == 7 ? ScriptSolitaire.Stapel7 : null;
            if (stapel.Count > 0) voorsteKaarten.Add(stapel[^1]);
            else voorsteKaarten.Add(empty);
        }
        return voorsteKaarten;
    }

    public int KrijgStapelBijPositie(Vector3 positie)
    {
        int stapelX = 0;
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
        float basisY = saveZoneOnder - saveZoneBoven + (schermHoogte * -1f / 2f / 1.5f) + (schermHoogte / 35f / 1.5f * ((2f + (3f / 6f)) * 10f / 1.5f));
        float basisYeind = saveZoneOnder - saveZoneBoven + (schermHoogte / 1.5f / 2f * -1f) + (schermHoogte / 35f / 1.5f * ((5f + (1f / 6f)) * 10f / 1.5f));
        float verschilY = 0.3f;
        float schaalX = ScriptSolitaire.kaarten[0].transform.localScale.x / 0.18f;
        float schaalY = ScriptSolitaire.kaarten[0].transform.localScale.y * 1.5f / 0.18f;
        if (positie.x < (xStapel1 + (schaalX / 2)) && positie.x > (xStapel1 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel1.Count - 1))))
        {
            stapelX = 1;
        }
        if (positie.x < (xStapel2 + (schaalX / 2)) && positie.x > (xStapel2 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel2.Count - 1))))
        {
            stapelX = 2;
        }
        if (positie.x < (xStapel3 + (schaalX / 2)) && positie.x > (xStapel3 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel3.Count - 1))))
        {
            stapelX = 3;
        }
        if (positie.x < (xStapel4 + (schaalX / 2)) && positie.x > (xStapel4 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel4.Count - 1))))
        {
            stapelX = 4;
        }
        if (positie.x < (xStapel5 + (schaalX / 2)) && positie.x > (xStapel5 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel5.Count - 1))))
        {
            stapelX = 5;
        }
        if (positie.x < (xStapel6 + (schaalX / 2)) && positie.x > (xStapel6 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel6.Count - 1))))
        {
            stapelX = 6;
        }
        if (positie.x < (xStapel7 + (schaalX / 2)) && positie.x > (xStapel7 - (schaalX / 2)) && positie.y < basisY + (schaalY / 2) && positie.y > (basisY - (schaalY / 2) - (verschilY * (ScriptSolitaire.Stapel7.Count - 1))))
        {
            stapelX = 7;
        }
        if (positie.x < (xStapel4 + (schaalX / 2)) && positie.x > (xStapel1 - (schaalX / 2)) && positie.y < basisYeind + (schaalY / 2) && positie.y > (basisYeind - (schaalY / 2)))
        {
            stapelX = 10;
        }
        return stapelX;
    }

    public void HaalUitStapels(GameObject kaartDieWegMoet)
    {
        if (ScriptSolitaire.Stapel1.Remove(kaartDieWegMoet))
        {
            for(int i = 0; i < ScriptSolitaire.Stapel1.Count; i++)
            {
                saveScript.intDict["Stapel1:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel1[i]);
            }
            saveScript.intDict["Stapel1Grootte"] = ScriptSolitaire.Stapel1.Count;
        }
        if (ScriptSolitaire.Stapel2.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.Stapel2.Count; i++)
            {
                saveScript.intDict["Stapel2:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel2[i]);
            }
            saveScript.intDict["Stapel2Grootte"] = ScriptSolitaire.Stapel2.Count;
        }
        if (ScriptSolitaire.Stapel3.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.Stapel3.Count; i++)
            {
                saveScript.intDict["Stapel3:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel3[i]);
            }
            saveScript.intDict["Stapel3Grootte"] = ScriptSolitaire.Stapel3.Count;
        }
        if (ScriptSolitaire.Stapel4.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.Stapel4.Count; i++)
            {
                saveScript.intDict["Stapel4:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel4[i]);
            }
            saveScript.intDict["Stapel4Grootte"] = ScriptSolitaire.Stapel4.Count;
        }
        if (ScriptSolitaire.Stapel5.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.Stapel5.Count; i++)
            {
                saveScript.intDict["Stapel5:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel5[i]);
            }
            saveScript.intDict["Stapel5Grootte"] = ScriptSolitaire.Stapel5.Count;
        }
        if (ScriptSolitaire.Stapel6.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.Stapel6.Count; i++)
            {
                saveScript.intDict["Stapel6:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel6[i]);
            }
            saveScript.intDict["Stapel6Grootte"] = ScriptSolitaire.Stapel6.Count;
        }
        if (ScriptSolitaire.Stapel7.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.Stapel7.Count; i++)
            {
                saveScript.intDict["Stapel7:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.Stapel7[i]);
            }
            saveScript.intDict["Stapel7Grootte"] = ScriptSolitaire.Stapel7.Count;
        }
        if (ScriptSolitaire.EindStapel1.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.EindStapel1.Count; i++)
            {
                saveScript.intDict["Eindstapel1:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.EindStapel1[i]);
            }
            saveScript.intDict["Eindstapel1Grootte"] = ScriptSolitaire.EindStapel1.Count;
        }
        if (ScriptSolitaire.EindStapel2.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.EindStapel2.Count; i++)
            {
                saveScript.intDict["Eindstapel2:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.EindStapel2[i]);
            }
            saveScript.intDict["Eindstapel2Grootte"] = ScriptSolitaire.EindStapel2.Count;
        }
        if (ScriptSolitaire.EindStapel3.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.EindStapel3.Count; i++)
            {
                saveScript.intDict["Eindstapel3:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.EindStapel3[i]);
            }
            saveScript.intDict["Eindstapel3Grootte"] = ScriptSolitaire.EindStapel3.Count;
        }
        if (ScriptSolitaire.EindStapel4.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < ScriptSolitaire.EindStapel4.Count; i++)
            {
                saveScript.intDict["Eindstapel4:" + i] = ScriptSolitaire.kaarten.IndexOf(ScriptSolitaire.EindStapel4[i]);
            }
            saveScript.intDict["Eindstapel4Grootte"] = ScriptSolitaire.EindStapel4.Count;
        }
        if (knoppenScript.OmgedraaideRest.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < knoppenScript.OmgedraaideRest.Count; i++)
            {
                saveScript.intDict["ReststapelOmgekeerd:" + i] = ScriptSolitaire.kaarten.IndexOf(knoppenScript.OmgedraaideRest[i]);
            }
            saveScript.intDict["ReststapelOmgekeerdGrootte"] = knoppenScript.OmgedraaideRest.Count;
        }
    }

    public bool KanOpEindStapel()
    {
        bool HetKan = false;
        if (kaartsoort_kaartX[1] == "A") return true;
        int.TryParse(kaartsoort_kaartX[1], out int kaartX);
        if (kaartX == 2)
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (ScriptSolitaire.EindStapel1.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel1[^1].name.Split('_')[1] == "A";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (ScriptSolitaire.EindStapel2.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel2[^1].name.Split('_')[1] == "A";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (ScriptSolitaire.EindStapel3.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel3[^1].name.Split('_')[1] == "A";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (ScriptSolitaire.EindStapel4.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel4[^1].name.Split('_')[1] == "A";
                }
            }
        }
        else if (kaartX <= 10 && kaartX > 2)
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (ScriptSolitaire.EindStapel1.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel1[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (ScriptSolitaire.EindStapel2.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel2[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (ScriptSolitaire.EindStapel3.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel3[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (ScriptSolitaire.EindStapel4.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel4[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
        }
        else if (kaartsoort_kaartX[1] == "J")
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (ScriptSolitaire.EindStapel1.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel1[^1].name.Split('_')[1] == "10";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (ScriptSolitaire.EindStapel2.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel2[^1].name.Split('_')[1] == "10";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (ScriptSolitaire.EindStapel3.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel3[^1].name.Split('_')[1] == "10";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (ScriptSolitaire.EindStapel4.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel4[^1].name.Split('_')[1] == "10";
                }
            }
        }
        else if (kaartsoort_kaartX[1] == "Q")
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (ScriptSolitaire.EindStapel1.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel1[^1].name.Split('_')[1] == "J";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (ScriptSolitaire.EindStapel2.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel2[^1].name.Split('_')[1] == "J";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (ScriptSolitaire.EindStapel3.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel3[^1].name.Split('_')[1] == "J";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (ScriptSolitaire.EindStapel4.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel4[^1].name.Split('_')[1] == "J";
                }
            }
        }
        else if (kaartsoort_kaartX[1] == "K")
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (ScriptSolitaire.EindStapel1.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel1[^1].name.Split('_')[1] == "Q";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (ScriptSolitaire.EindStapel2.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel2[^1].name.Split('_')[1] == "Q";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (ScriptSolitaire.EindStapel3.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel3[^1].name.Split('_')[1] == "Q";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (ScriptSolitaire.EindStapel4.Count != 0)
                {
                    HetKan = ScriptSolitaire.EindStapel4[^1].name.Split('_')[1] == "Q";
                }
            }
        }
        return HetKan;
    }
}
