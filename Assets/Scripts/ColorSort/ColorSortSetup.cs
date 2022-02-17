using System.Collections.Generic;
using UnityEngine;

public class ColorSortSetup : MonoBehaviour
{
    [SerializeField] List<GameObject> ringPrefabs;
    [SerializeField] GameObject leegObjectPrefab;
    int difficulty = 3;
    private int[] ringstapels = new int[] { 5, 6, 8, 9 };
    private int[] ringenPerStapel = new int[] { 4, 4, 4, 5 };
    private float hoogteRing = 0.5f;
    private float breedteRing = 2.5f;
    private float ruimteTussenStapels = 0.25f;
    [SerializeField] GameObject stapelsHouder;
    [SerializeField] List<GameObject> stapels;

    // Start is called before the first frame update
    void Start()
    {
        List<int> ringPrefabIndexes = new List<int>();
        for (int i = 0; i < ringPrefabs.Count; i++)
        {
            ringPrefabIndexes.Add(i);
        }
        for (int i = 0; i < ringstapels[difficulty] + 1; i++)
        {
            GameObject stapel = Instantiate(leegObjectPrefab, stapelsHouder.transform);
            stapel.name = "Stapel" + i;
            stapels.Add(stapel);
        }
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        int kolommenMogelijk = Mathf.FloorToInt(schermWijdte / (2f * ruimteTussenStapels + breedteRing)) + Mathf.FloorToInt(schermWijdte % (2f * ruimteTussenStapels + breedteRing) / breedteRing);
        float hoogteStapel = hoogteRing * ringenPerStapel[difficulty];
        int rijenMogelijk = Mathf.FloorToInt(schermHoogte / (ruimteTussenStapels * 2f + hoogteStapel)) + Mathf.FloorToInt(schermHoogte % (ruimteTussenStapels * 2f + hoogteStapel) / hoogteStapel);
        int benodigdeRijen = 1;
        if (ringstapels[difficulty] + 1 > kolommenMogelijk)
        {
            benodigdeRijen = Mathf.CeilToInt((ringstapels[difficulty] + 1f) / kolommenMogelijk);
            Debug.Log(benodigdeRijen);
        }
        if (benodigdeRijen > rijenMogelijk)
        {
            Debug.Log("moet worden gescaled");
        }
        List<GameObject> ringsInGame = new List<GameObject>();
        GameObject prefabInLaatsteKolom = null;
        GameObject prefabInLaatsteRij = null;
        for (int i = 0; i < ringstapels[difficulty]; i++)
        {
            int rand = Random.Range(0, ringPrefabIndexes.Count);
            ringsInGame.Add(ringPrefabs[ringPrefabIndexes[rand]]);
            ringPrefabIndexes.RemoveAt(rand);
            int xi = i;
            int yi = 0;
            if (i >= kolommenMogelijk)
            {
                yi = Mathf.FloorToInt(i / kolommenMogelijk);
                xi = i - yi * kolommenMogelijk;
            }
            GameObject prefab = null;
            for (int a = 0; a < ringenPerStapel[difficulty]; a++)
            {
                prefab = Instantiate(ringsInGame[i], new Vector3((breedteRing - schermWijdte) / 2f + xi * (ruimteTussenStapels * 2f + breedteRing), (schermHoogte + hoogteRing) / 2f - hoogteStapel - yi * (hoogteStapel + 2f * ruimteTussenStapels) + a * hoogteRing, 0), Quaternion.identity, stapels[i].transform);
                prefab.transform.eulerAngles = new Vector3(90, 0, 0);
            }
            if (xi == kolommenMogelijk - 1)
            {
                prefabInLaatsteKolom = prefab;
            }
            if(yi == rijenMogelijk - 1)
            {
                prefabInLaatsteRij = prefab;
            }
            //0.5 X 2.5
        }
        stapelsHouder.transform.position = new Vector3((schermWijdte / 2f - prefabInLaatsteKolom.transform.position.x - breedteRing / 2f) / 2f, (prefabInLaatsteRij.transform.position.y - hoogteRing / 2f) / 2f, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
