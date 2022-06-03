using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VBG.Extensions;

public class KaartSleepScript : MonoBehaviour
{
    private SolitaireScript solitaireScript;
    private KnoppenScriptSolitaire knoppenScript;
    private SolitaireLayout solitaireLayout;
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
        saveScript = SaveScript.Instance;
        if (saveScript == null) return;
        solitaireScript = eventSystem.GetComponent<SolitaireScript>();
        knoppenScript = eventSystem.GetComponent<KnoppenScriptSolitaire>();
        solitaireLayout = eventSystem.GetComponent<SolitaireLayout>();
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

    private IEnumerator OnMouseDown()
    {
        if (tf.parent.localEulerAngles == achterkantboven || Input.touchCount == 0) yield break;
        yield return new WaitForEndOfFrame();
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null) yield break;
        Vector3 muisPositie = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        positieMuis = muisPositie;
        deTeVerplaatsenKaarten.Clear();
        int stapelInt = KrijgStapelBijPositie(muisPositie);
        aantalKaartenTeVerplaatsen = 1;
        if (stapelInt.IsBetween(1, 7))
        {
            GameObject parent = tf.parent.gameObject;
            List<GameObject> stapel = stapelInt == 1 ? solitaireScript.Stapel1 : stapelInt == 2 ? solitaireScript.Stapel2 : stapelInt == 3 ? solitaireScript.Stapel3 : stapelInt == 4 ? solitaireScript.Stapel4 : stapelInt == 5 ? solitaireScript.Stapel5 : stapelInt == 6 ? solitaireScript.Stapel6 : stapelInt == 7 ? solitaireScript.Stapel7 : null;
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
                List<GameObject> stapel = stapelInt == 1 ? solitaireScript.Stapel1 : stapelInt == 2 ? solitaireScript.Stapel2 : stapelInt == 3 ? solitaireScript.Stapel3 : stapelInt == 4 ? solitaireScript.Stapel4 : stapelInt == 5 ? solitaireScript.Stapel5 : stapelInt == 6 ? solitaireScript.Stapel6 : stapelInt == 7 ? solitaireScript.Stapel7 : null;
                if(stapel.IndexOf(mogelijk[i]) != -1)
                {
                    GameObject parent = tf.parent.gameObject;
                    HaalUitStapels(parent);
                    stapel.Add(parent);
                    saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = solitaireScript.kaarten.IndexOf(parent);
                    klaar = true;
                    if (mogelijk[i].name == "leeg_leeg")
                    {
                        stapel.RemoveAt(0);
                    }
                    for (int a = 0; a < deTeVerplaatsenKaarten.Count; a++)
                    {
                        HaalUitStapels(deTeVerplaatsenKaarten[a]);
                        stapel.Add(deTeVerplaatsenKaarten[a]);
                        saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = solitaireScript.kaarten.IndexOf(deTeVerplaatsenKaarten[a]);
                    }
                    saveScript.intDict["Stapel" + stapelInt + "Grootte"] = stapel.Count;
                    solitaireScript.DraaiVoorsteKaartOm(false);
                    solitaireLayout.PositionCards();
                }
            }
            if (!klaar && deTeVerplaatsenKaarten.Count == 0 && KanOpEindStapel())
            {
                GameObject parent = tf.parent.gameObject;
                HaalUitStapels(parent);
                int eindStapelInt = kaartsoort_kaartX[0].Equals("Klaver") ? 1 : kaartsoort_kaartX[0].Equals("Ruiten") ? 2 : kaartsoort_kaartX[0].Equals("Harten") ? 3 : kaartsoort_kaartX[0].Equals("Schoppe") ? 4 : 0;
                List<GameObject> eindStapel = eindStapelInt == 1 ? solitaireScript.EindStapel1 : eindStapelInt == 2 ? solitaireScript.EindStapel2 : eindStapelInt == 3 ? solitaireScript.EindStapel3 : eindStapelInt == 4 ? solitaireScript.EindStapel4 : null;
                eindStapel.Add(parent);
                saveScript.intDict["Eindstapel" + eindStapelInt + ":" + (eindStapel.Count - 1)] = solitaireScript.kaarten.IndexOf(parent);
                klaar = true;
                saveScript.intDict["Eindstapel" + eindStapelInt + "Grootte"] = eindStapel.Count;
                solitaireScript.DraaiVoorsteKaartOm(false);
                solitaireLayout.PositionCards();
            }
        }
        if (!klaar)
        {
            int stapelInt = deI[0] + 1;
            if (mogelijk.Count != 0 && stapelInt.IsBetween(1, 7))
            {
                List<GameObject> stapel = stapelInt == 1 ? solitaireScript.Stapel1 : stapelInt == 2 ? solitaireScript.Stapel2 : stapelInt == 3 ? solitaireScript.Stapel3 : stapelInt == 4 ? solitaireScript.Stapel4 : stapelInt == 5 ? solitaireScript.Stapel5 : stapelInt == 6 ? solitaireScript.Stapel6 : stapelInt == 7 ? solitaireScript.Stapel7 : null;
                GameObject parent = tf.parent.gameObject;
                HaalUitStapels(parent);
                stapel.Add(parent);
                saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = solitaireScript.kaarten.IndexOf(parent);
                if (stapel[0].name == "leeg_leeg")
                {
                    stapel.RemoveAt(0);
                }
                for (int a = 0; a < deTeVerplaatsenKaarten.Count; a++)
                {
                    HaalUitStapels(deTeVerplaatsenKaarten[a]);
                    stapel.Add(deTeVerplaatsenKaarten[a]);
                    saveScript.intDict["Stapel" + stapelInt + ":" + (stapel.Count - 1)] = solitaireScript.kaarten.IndexOf(deTeVerplaatsenKaarten[a]);
                }
                saveScript.intDict["Stapel" + stapelInt + "Grootte"] = stapel.Count;
                solitaireScript.DraaiVoorsteKaartOm(false);
                solitaireLayout.PositionCards();
            }
            else
            {
                solitaireScript.TijdStraf();
                solitaireLayout.PositionCards();
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
            List<GameObject> stapel = i == 1 ? solitaireScript.Stapel1 : i == 2 ? solitaireScript.Stapel2 : i == 3 ? solitaireScript.Stapel3 : i == 4 ? solitaireScript.Stapel4 : i == 5 ? solitaireScript.Stapel5 : i == 6 ? solitaireScript.Stapel6 : i == 7 ? solitaireScript.Stapel7 : null;
            if (stapel.Count > 0) voorsteKaarten.Add(stapel[^1]);
            else voorsteKaarten.Add(empty);
        }
        return voorsteKaarten;
    }

    public int KrijgStapelBijPositie(Vector3 positie)
    {
        float _screenSafeAreaWidthInUnits = Mathf.Min(solitaireLayout.screenSafeAreaWidthInUnits, solitaireLayout.screenSafeAreaHeightInUnits * (8f / 4.5f));
        float halfCardsScale = _screenSafeAreaWidthInUnits / 81f * 10f * 0.5f;
        List<float> xPositions = new List<float>();
        float _screenWidthInUnits = Mathf.Min(solitaireLayout.screenWidthInUnits, solitaireLayout.screenHeightInUnits * (8f / 4.5f));
        float baseY = solitaireLayout.screenSafeAreaCenterYInUnits + (solitaireLayout.screenHeightInUnits * (-1f / 3f)) + (solitaireLayout.screenHeightInUnits / 35f / 1.5f * (25f / 1.5f));
        float baseYFoundation = solitaireLayout.screenSafeAreaCenterYInUnits + (solitaireLayout.screenHeightInUnits * (-1f / 3f)) + (solitaireLayout.screenHeightInUnits / 35f / 1.5f * (34f + (4f / 9f)));
        for (int i = 0; i < 7; i++)
        {
            float xPos = solitaireLayout.screenSafeAreaCenterXInUnits + (_screenWidthInUnits / 81f * (-33f + (i * 11f)));
            if (positie.x.IsBetween(xPos - halfCardsScale, xPos + halfCardsScale))
            {
                if (positie.y <= baseY + (halfCardsScale * 1.5f))
                {
                    return (i + 1);
                }
            }
            xPositions.Add(xPos);
        }
        if (positie.x.IsBetween(xPositions[0] - halfCardsScale, xPositions[3] + halfCardsScale))
        {
            if (positie.y.IsBetween(baseYFoundation + halfCardsScale * 1.5f, baseYFoundation - halfCardsScale * 1.5f))
            {
                return 10;
            }
        }
        return 0;
    }

    public void HaalUitStapels(GameObject kaartDieWegMoet)
    {
        if (solitaireScript.Stapel1.Remove(kaartDieWegMoet))
        {
            for(int i = 0; i < solitaireScript.Stapel1.Count; i++)
            {
                saveScript.intDict["Stapel1:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel1[i]);
            }
            saveScript.intDict["Stapel1Grootte"] = solitaireScript.Stapel1.Count;
        }
        if (solitaireScript.Stapel2.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.Stapel2.Count; i++)
            {
                saveScript.intDict["Stapel2:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel2[i]);
            }
            saveScript.intDict["Stapel2Grootte"] = solitaireScript.Stapel2.Count;
        }
        if (solitaireScript.Stapel3.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.Stapel3.Count; i++)
            {
                saveScript.intDict["Stapel3:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel3[i]);
            }
            saveScript.intDict["Stapel3Grootte"] = solitaireScript.Stapel3.Count;
        }
        if (solitaireScript.Stapel4.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.Stapel4.Count; i++)
            {
                saveScript.intDict["Stapel4:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel4[i]);
            }
            saveScript.intDict["Stapel4Grootte"] = solitaireScript.Stapel4.Count;
        }
        if (solitaireScript.Stapel5.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.Stapel5.Count; i++)
            {
                saveScript.intDict["Stapel5:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel5[i]);
            }
            saveScript.intDict["Stapel5Grootte"] = solitaireScript.Stapel5.Count;
        }
        if (solitaireScript.Stapel6.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.Stapel6.Count; i++)
            {
                saveScript.intDict["Stapel6:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel6[i]);
            }
            saveScript.intDict["Stapel6Grootte"] = solitaireScript.Stapel6.Count;
        }
        if (solitaireScript.Stapel7.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.Stapel7.Count; i++)
            {
                saveScript.intDict["Stapel7:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.Stapel7[i]);
            }
            saveScript.intDict["Stapel7Grootte"] = solitaireScript.Stapel7.Count;
        }
        if (solitaireScript.EindStapel1.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.EindStapel1.Count; i++)
            {
                saveScript.intDict["Eindstapel1:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.EindStapel1[i]);
            }
            saveScript.intDict["Eindstapel1Grootte"] = solitaireScript.EindStapel1.Count;
        }
        if (solitaireScript.EindStapel2.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.EindStapel2.Count; i++)
            {
                saveScript.intDict["Eindstapel2:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.EindStapel2[i]);
            }
            saveScript.intDict["Eindstapel2Grootte"] = solitaireScript.EindStapel2.Count;
        }
        if (solitaireScript.EindStapel3.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.EindStapel3.Count; i++)
            {
                saveScript.intDict["Eindstapel3:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.EindStapel3[i]);
            }
            saveScript.intDict["Eindstapel3Grootte"] = solitaireScript.EindStapel3.Count;
        }
        if (solitaireScript.EindStapel4.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < solitaireScript.EindStapel4.Count; i++)
            {
                saveScript.intDict["Eindstapel4:" + i] = solitaireScript.kaarten.IndexOf(solitaireScript.EindStapel4[i]);
            }
            saveScript.intDict["Eindstapel4Grootte"] = solitaireScript.EindStapel4.Count;
        }
        if (knoppenScript.OmgedraaideRest.Remove(kaartDieWegMoet))
        {
            for (int i = 0; i < knoppenScript.OmgedraaideRest.Count; i++)
            {
                saveScript.intDict["ReststapelOmgekeerd:" + i] = solitaireScript.kaarten.IndexOf(knoppenScript.OmgedraaideRest[i]);
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
                if (solitaireScript.EindStapel1.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel1[^1].name.Split('_')[1] == "A";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (solitaireScript.EindStapel2.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel2[^1].name.Split('_')[1] == "A";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (solitaireScript.EindStapel3.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel3[^1].name.Split('_')[1] == "A";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (solitaireScript.EindStapel4.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel4[^1].name.Split('_')[1] == "A";
                }
            }
        }
        else if (kaartX <= 10 && kaartX > 2)
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (solitaireScript.EindStapel1.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel1[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (solitaireScript.EindStapel2.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel2[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (solitaireScript.EindStapel3.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel3[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (solitaireScript.EindStapel4.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel4[^1].name.Split('_')[1] == (kaartX - 1).ToString();
                }
            }
        }
        else if (kaartsoort_kaartX[1] == "J")
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (solitaireScript.EindStapel1.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel1[^1].name.Split('_')[1] == "10";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (solitaireScript.EindStapel2.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel2[^1].name.Split('_')[1] == "10";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (solitaireScript.EindStapel3.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel3[^1].name.Split('_')[1] == "10";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (solitaireScript.EindStapel4.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel4[^1].name.Split('_')[1] == "10";
                }
            }
        }
        else if (kaartsoort_kaartX[1] == "Q")
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (solitaireScript.EindStapel1.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel1[^1].name.Split('_')[1] == "J";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (solitaireScript.EindStapel2.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel2[^1].name.Split('_')[1] == "J";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (solitaireScript.EindStapel3.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel3[^1].name.Split('_')[1] == "J";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (solitaireScript.EindStapel4.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel4[^1].name.Split('_')[1] == "J";
                }
            }
        }
        else if (kaartsoort_kaartX[1] == "K")
        {
            if (kaartsoort_kaartX[0] == "Klaver")
            {
                if (solitaireScript.EindStapel1.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel1[^1].name.Split('_')[1] == "Q";
                }
            }
            else if (kaartsoort_kaartX[0] == "Ruiten")
            {
                if (solitaireScript.EindStapel2.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel2[^1].name.Split('_')[1] == "Q";
                }
            }
            else if (kaartsoort_kaartX[0] == "Harten")
            {
                if (solitaireScript.EindStapel3.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel3[^1].name.Split('_')[1] == "Q";
                }
            }
            else if (kaartsoort_kaartX[0] == "Schoppe")
            {
                if (solitaireScript.EindStapel4.Count != 0)
                {
                    HetKan = solitaireScript.EindStapel4[^1].name.Split('_')[1] == "Q";
                }
            }
        }
        return HetKan;
    }
}
