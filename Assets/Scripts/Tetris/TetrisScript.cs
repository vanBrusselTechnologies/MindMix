using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TetrisScript : MonoBehaviour
{
    public GameObject vorm1;
    public GameObject vorm2;
    public GameObject vorm3;
    public GameObject vorm4;
    public GameObject vorm5;
    public GameObject vorm6;
    public GameObject vorm7;
    private List<GameObject> vormen = new();
    private GameObject vallendeVorm;
    private GameObject volgendeVorm;
    public bool valtGeenVormMeer = true;
    private float valSnelheid = 5f;
    private Vector3 startpositie = new(0, 12, -1);
    private Vector3 positie;
    private Vector3 volgendeVormPositie = new(-20, 0, 0);
    private bool Eenkomendepositie;
    private bool stop;
    private GameObject a;
    private bool lijnMinder;
    private TMP_Text scoreText;
    private int extraScore;
    private List<Collider> tempList = new();
    private int snelheidverhogingbijpunten = 250;

    // Use this for initialization
    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        valSnelheid = 5f;
        vormen.Add(vorm1);
        vormen.Add(vorm2);
        vormen.Add(vorm3);
        vormen.Add(vorm4);
        vormen.Add(vorm5);
        vormen.Add(vorm6);
        vormen.Add(vorm7);
        volgendeVorm = vormen[Random.Range(0, vormen.Count)];
        positie = startpositie;
        a = new GameObject("controleKubus");
        a.transform.localScale = new Vector3(15, 1, 1);
        a.AddComponent<BoxCollider>();
        a.GetComponent<BoxCollider>().isTrigger = true;
        a.tag = "check";
        scoreText = GameObject.Find("ScorePunten").GetComponent<TMP_Text>();
        scoreText.text = "0";
    }

    // Update is called once per frame
    private void Update()
    {
        if (int.Parse(scoreText.text) > snelheidverhogingbijpunten)
        {
            valSnelheid += 0.5f;
            snelheidverhogingbijpunten *= 3;
        }
        if (!valtGeenVormMeer)
        {
            welkeBotsingenMogelijkPerVormOmlaag();
        }
        else if (valtGeenVormMeer && !stop)
        {
            haalVolleRijenWeg();
            Transform[] tiles = FindObjectsOfType<Transform>();
            vallendeVorm = volgendeVorm;
            volgendeVorm = vormen[Random.Range(0, vormen.Count)];
            valtGeenVormMeer = false;
            for (int a = 0; a < tiles.Length; a++)
            {
                if (Vector3.Distance(tiles[a].position, startpositie) <= 2 && !tiles[a].gameObject.CompareTag("check"))
                {
                    stop = true;
                    //laad: je bent af scherm met score en vergelijking met highscore
                }
            }
            vallendeVorm = Instantiate(vallendeVorm, startpositie, Quaternion.identity);
            for (int a = 0; a < tiles.Length; a++)
            {
                if (Vector3.Distance(tiles[a].position, volgendeVormPositie) <= 1)
                {
                    Destroy(tiles[a].gameObject);
                }
            }
            Instantiate(volgendeVorm, volgendeVormPositie, Quaternion.identity);
        }
        if (!stop)
        {
            extraScore = 0;
            if (lijnMinder)
            {
                if (extraScore == 0)
                {
                    extraScore = 1;
                }
                tempList.Clear();
                haalVolleRijenWeg();
                if (lijnMinder)
                {
                    tempList.Clear();
                    haalVolleRijenWeg();
                    if (lijnMinder)
                    {
                        tempList.Clear();
                        haalVolleRijenWeg();
                    }
                }
                lijnMinder = false;
            }
            if (extraScore == 1)
            {
                extraScore = 25;
            }
            else if (extraScore == 2)
            {
                extraScore = 50;
            }
            else if (extraScore == 3)
            {
                extraScore = 100;
            }
            else if (extraScore == 4)
            {
                extraScore = 200;
            }
            scoreText.text = (int.Parse(scoreText.text) + extraScore).ToString();
            if (magDraaien())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    vallendeVorm.transform.Rotate(new Vector3(0, 0, 90));
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                valSnelheid *= 2;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                valSnelheid /= 2;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
            {
                welkeBotsingenMogelijkPerVormRechts();
            }
            else if (!Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                welkeBotsingenMogelijkPerVormLinks();
            }
        }
    }

    public bool KanVerplaatsen(Vector3 positie)
    {
        bool kanVerplaatsen = true;
        Collider[] hitColliders = Physics.OverlapSphere(positie, 0.01f);
        List<Collider> temp = new();
        for (int i = 0; i < hitColliders.Length; i++)
        {
            temp.Add(hitColliders[i]);
        }
        for (int i = temp.Count - 1; i >= 0; i--)
        {
            if (temp[i].gameObject.CompareTag("check"))
            {
                temp.RemoveAt(i);
            }
            else if (temp[i].gameObject.transform.parent == null)
            {
                ;
            }
            else if (temp[i].gameObject.transform.parent.gameObject == vallendeVorm)
            {
                temp.RemoveAt(i);
            }
        }
        if (temp.Count > 0)
        {
            kanVerplaatsen = false;
        }
        temp.Clear();
        return kanVerplaatsen;
    }

    public void welkeBotsingenMogelijkPerVormOmlaag()
    {
        positie = vallendeVorm.transform.position;
        Vector3 komendePositie = positie;
        List<Vector3> komendePosities = new();
        if (vallendeVorm.name == vorm1.name + "(Clone)")
        {
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePositie = vallendeVorm.transform.Find("Onder").gameObject.transform.position;
                komendePositie.y -= 0.5f;
                Eenkomendepositie = true;
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePositie = vallendeVorm.transform.Find("Boven").gameObject.transform.position;
                komendePositie.y -= 0.5f;
                Eenkomendepositie = true;
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                for (int i = 0; i < vallendeVorm.transform.childCount; i++)
                {
                    komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.position);
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
                Eenkomendepositie = false;
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                for (int i = 0; i < vallendeVorm.transform.childCount; i++)
                {
                    komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.position);
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
                Eenkomendepositie = false;
            }
        }
        else if (vallendeVorm.name == vorm2.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm3.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm4.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm5.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm6.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm7.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksOnder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsBoven").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("RechtsBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.y -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        if (Eenkomendepositie)
        {
            if (KanVerplaatsen(komendePositie))
            {
                positie.y -= valSnelheid * Time.deltaTime;
                vallendeVorm.transform.position = positie;
            }
            else
            {
                valtGeenVormMeer = true;
                positie.y = Mathf.RoundToInt(positie.y);
                vallendeVorm.transform.position = positie;
            }
        }
        else
        {
            bool temp = true;
            for (int i = 0; i < komendePosities.Count; i++)
            {
                if (!KanVerplaatsen(komendePosities[i]))
                {
                    temp = false;
                    i = 10000;
                }
            }
            if (temp)
            {
                positie.y -= valSnelheid * Time.deltaTime;
                vallendeVorm.transform.position = positie;
            }
            else
            {
                valtGeenVormMeer = true;
                positie.y = Mathf.RoundToInt(positie.y);
                vallendeVorm.transform.position = positie;
            }
            komendePosities.Clear();
        }
    }

    public void welkeBotsingenMogelijkPerVormRechts()
    {
        positie = vallendeVorm.transform.position;
        Vector3 komendePositie = positie;
        List<Vector3> komendePosities = new();
        if (vallendeVorm.name == vorm1.name + "(Clone)")
        {
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePositie = vallendeVorm.transform.Find("Onder").gameObject.transform.position;
                komendePositie.x += 0.5f;
                Eenkomendepositie = true;
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePositie = vallendeVorm.transform.Find("Boven").gameObject.transform.position;
                komendePositie.x += 0.5f;
                Eenkomendepositie = true;
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                for (int i = 0; i < vallendeVorm.transform.childCount; i++)
                {
                    komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.position);
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
                Eenkomendepositie = false;
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                for (int i = 0; i < vallendeVorm.transform.childCount; i++)
                {
                    komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.position);
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
                Eenkomendepositie = false;
            }
        }
        else if (vallendeVorm.name == vorm2.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm3.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm4.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm5.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm6.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm7.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksOnder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsBoven").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("RechtsBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x += 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }

        Vector3 tempPos = vallendeVorm.transform.position;
        tempPos.x += 1;
        if (Eenkomendepositie)
        {
            if (KanVerplaatsen(komendePositie))
            {
                vallendeVorm.transform.position = tempPos;
            }
        }
        else
        {
            bool temp = true;
            for (int i = 0; i < komendePosities.Count; i++)
            {
                if (!KanVerplaatsen(komendePosities[i]))
                {
                    temp = false;
                    i = 10000;
                }
            }
            if (temp)
            {
                vallendeVorm.transform.position = tempPos;
            }
            komendePosities.Clear();
        }
    }

    public void welkeBotsingenMogelijkPerVormLinks()
    {
        positie = vallendeVorm.transform.position;
        Vector3 komendePositie = positie;
        List<Vector3> komendePosities = new();
        if (vallendeVorm.name == vorm1.name + "(Clone)")
        {
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePositie = vallendeVorm.transform.Find("Onder").gameObject.transform.position;
                komendePositie.x -= 0.5f;
                Eenkomendepositie = true;
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePositie = vallendeVorm.transform.Find("Boven").gameObject.transform.position;
                komendePositie.x -= 0.5f;
                Eenkomendepositie = true;
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                for (int i = 0; i < vallendeVorm.transform.childCount; i++)
                {
                    komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.position);
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
                Eenkomendepositie = false;
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                for (int i = 0; i < vallendeVorm.transform.childCount; i++)
                {
                    komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.position);
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
                Eenkomendepositie = false;
            }
        }
        else if (vallendeVorm.name == vorm2.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm3.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksMidden").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm4.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsMidden").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm5.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm6.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Onder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Links").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("Rechts").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("Boven").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        else if (vallendeVorm.name == vorm7.name + "(Clone)")
        {
            Eenkomendepositie = false;
            if (vallendeVorm.transform.rotation.eulerAngles.z == 270)
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksOnder").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation.eulerAngles.z == 90)
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsBoven").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, 1) || vallendeVorm.transform.rotation == new Quaternion(0, 0, 0, -1))
            {
                komendePosities.Add(vallendeVorm.transform.Find("LinksBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("LinksOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
            else if (vallendeVorm.transform.rotation == new Quaternion(0, 0, 1, 0) || vallendeVorm.transform.rotation == new Quaternion(0, 0, -1, 0))
            {
                komendePosities.Add(vallendeVorm.transform.Find("RechtsBoven").gameObject.transform.position);
                komendePosities.Add(vallendeVorm.transform.Find("RechtsOnder").gameObject.transform.position);
                for (int i = 0; i < komendePosities.Count; i++)
                {
                    Vector3 temp = komendePosities[i];
                    temp.x -= 0.5f;
                    komendePosities[i] = temp;
                }
            }
        }
        Vector3 tempPos = vallendeVorm.transform.position;
        tempPos.x -= 1;
        if (Eenkomendepositie)
        {
            if (KanVerplaatsen(komendePositie))
            {
                vallendeVorm.transform.position = tempPos;
            }
        }
        else
        {
            bool temp = true;
            for (int i = 0; i < komendePosities.Count; i++)
            {
                if (!KanVerplaatsen(komendePosities[i]))
                {

                    temp = false;
                    i = 10000;
                }
            }
            if (temp)
            {
                vallendeVorm.transform.position = tempPos;
            }
            komendePosities.Clear();
        }
    }

    public bool magDraaien()
    {
        float rotatieZ = vallendeVorm.transform.rotation.eulerAngles.z;
        positie = vallendeVorm.transform.position;
        List<Vector3> komendePosities = new();
        float tempX;
        for (int i = 0; i < vallendeVorm.transform.childCount; i++)
        {
            komendePosities.Add(vallendeVorm.transform.GetChild(i).gameObject.transform.localPosition);
            Vector3 temp;
            if (rotatieZ == 0)
            {
                temp = komendePosities[i];
            }
            else if (rotatieZ == 180)
            {
                temp = -1 * komendePosities[i];
            }
            else if (rotatieZ == 90)
            {
                temp = komendePosities[i];
                tempX = temp.x;
                temp.x = -temp.y;
                temp.y = tempX;
            }
            else
            {
                temp = komendePosities[i];
                tempX = temp.x;
                temp.x = temp.y;
                temp.y = -tempX;
            }
            tempX = temp.x;
            temp.x = -temp.y;
            temp.y = tempX;
            temp += vallendeVorm.transform.position;
            komendePosities[i] = temp;
        }
        bool hetMag = true;
        bool tempbool = true;
        for (int i = 0; i < komendePosities.Count; i++)
        {
            if (!KanVerplaatsen(komendePosities[i]))
            {
                tempbool = false;
                i = 10000;
            }
        }
        if (!tempbool)
        {
            hetMag = false;
        }
        komendePosities.Clear();
        return hetMag;
    }

    public void haalVolleRijenWeg()
    {
        lijnMinder = false;
        for (int i = 0; i < 23; i++)
        {
            for (int o = tempList.Count - 1; o >= 0; o--)
            {
                tempList.RemoveAt(o);
            }
            tempList.Clear();
            a.transform.position = new Vector3(0, i - 11, -1);
            for (int x = 0; x < 15; x++)
            {
                Collider[] hitColliders = Physics.OverlapSphere(new Vector3(-7 + x, i - 11, -1), 0);
                for (int u = 0; u < hitColliders.Length; u++)
                {
                    tempList.Add(hitColliders[u]);
                }
            }
            for (int o = tempList.Count - 1; o >= 0; o--)
            {
                if (tempList[o].gameObject.CompareTag("check") || tempList[o].gameObject.name == "vorm1" || tempList[o].gameObject.name == "vorm2" || tempList[o].gameObject.name == "vorm3" || tempList[o].gameObject.name == "vorm4" || tempList[o].gameObject.name == "vorm5" || tempList[o].gameObject.name == "vorm6" || tempList[o].gameObject.name == "vorm7")
                {
                    tempList.RemoveAt(o);
                }
            }
            if (tempList.Count > 1 && tempList.Count % 15 == 0)
            {
                for (int x = 0; x < tempList.Count; x++)
                {
                    Destroy(tempList[x].gameObject.transform.gameObject);
                }
                Transform[] obj = FindObjectsOfType<Transform>();
                for (int x = 0; x < obj.Length; x++)
                {
                    bool isGeenParent = true;
                    for (int u = 1; u < 8; u++)
                    {
                        if (obj[x].gameObject.name.Equals("vorm" + u + "(Clone)"))
                        {
                            isGeenParent = false;
                            if (obj[x].gameObject.transform.childCount == 0)
                            {
                                Destroy(obj[x].gameObject.transform.gameObject);
                            }
                        }
                    }
                    if (isGeenParent)
                    {
                        if (obj[x].position.y > i - 11 && obj[x].position.x > -8 && obj[x].position.x < 8 && obj[x].position.z == -1)
                        {
                            Vector3 tempPos = obj[x].position;
                            tempPos.y -= 1;
                            obj[x].position = tempPos;
                        }
                    }
                }
                extraScore += 1;
                lijnMinder = true;
                i = 100000;
            }
        }
    }
}
