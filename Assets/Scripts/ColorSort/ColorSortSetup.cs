using System.Collections.Generic;
using UnityEngine;
using VBG.Extensions;

public class ColorSortSetup : BaseLayout
{
    [SerializeField] List<GameObject> ringPrefabs;
    [SerializeField] GameObject leegObjectPrefab;
    [SerializeField] GameObject ringhouderObj;
    int difficulty = 3;
    private int[] ringstapels = new int[] { 5, 6, 8, 9 };
    private int[] ringenPerStapel = new int[] { 4, 4, 4, 5 };
    private float hoogteRing = 0.5f;
    private float breedteRing = 2.5f;
    private float ruimteTussenStapels = 0.25f;
    [SerializeField] Transform stapelsHouder;
    [SerializeField] List<GameObject> stapels;

    private void InstantiateRingStack()
    {
        for (int i = 0; i < ringstapels[difficulty] + 1; i++)
        {
            GameObject stapel = Instantiate(leegObjectPrefab, stapelsHouder);
            stapel.name = "Stapel" + i;
            stapels.Add(stapel);
        }
        Instantiate(ringhouderObj, stapels[ringstapels[difficulty]].transform);
    }

    private void InstantiateRings()
    {
        List<int> ringPrefabIndexes = new List<int>();
        for (int i = 0; i < ringPrefabs.Count; i++)
        {
            ringPrefabIndexes.Add(i);
        }
        List<GameObject> ringsInGame = new List<GameObject>();
        for (int i = 0; i < ringstapels[difficulty]; i++)
        {
            int rand = Random.Range(0, ringPrefabIndexes.Count);
            ringsInGame.Add(ringPrefabs[ringPrefabIndexes[rand]]);
            ringPrefabIndexes.RemoveAt(rand);
            Instantiate(ringhouderObj, stapels[i].transform);
            for (int a = 0; a < ringenPerStapel[difficulty]; a++)
            {
                Instantiate(ringsInGame[i], Vector3.zero, Quaternion.Euler(90, 0, 0), stapels[i].transform);
            }
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InstantiateRingStack();
        InstantiateRings();
    }

    public override void SetLayout()
    {
        stapelsHouder.localPosition = Vector3.zero;
        stapelsHouder.localScale = Vector3.one;
        int kolommenMogelijk = Mathf.FloorToInt(screenSafeAreaWidthInUnits / (2f * ruimteTussenStapels + breedteRing)) + Mathf.FloorToInt(screenSafeAreaWidthInUnits % (2f * ruimteTussenStapels + breedteRing) / breedteRing);
        int columnsToUse = Mathf.CeilToInt((ringstapels[difficulty] + 1f) / Mathf.CeilToInt((ringstapels[difficulty] + 1f) / kolommenMogelijk));
        float hoogteStapel = hoogteRing * (ringenPerStapel[difficulty] + 1);
        int rijenMogelijk = Mathf.FloorToInt(screenSafeAreaHeightInUnits / (ruimteTussenStapels * 2f + hoogteStapel)) + Mathf.FloorToInt(screenSafeAreaHeightInUnits % (ruimteTussenStapels * 2f + hoogteStapel) / hoogteStapel);
        int benodigdeRijen = 1;
        if (ringstapels[difficulty] + 1 > columnsToUse)
        {
            benodigdeRijen = Mathf.CeilToInt((ringstapels[difficulty] + 1f) / columnsToUse);
        }
        float scale = 1f;
        if(ScreenExt.aspect > 2f)
        {
            scale = ScreenExt.aspect / 2f;
        }
        while (benodigdeRijen > rijenMogelijk)
        {
            scale -= 0.01f;
            kolommenMogelijk = Mathf.FloorToInt(screenSafeAreaWidthInUnits / (scale * (2f * ruimteTussenStapels + breedteRing))) + Mathf.FloorToInt(screenSafeAreaWidthInUnits % (scale * (2f * ruimteTussenStapels + breedteRing)) / (scale * breedteRing));
            columnsToUse = Mathf.CeilToInt((ringstapels[difficulty] + 1f) / Mathf.CeilToInt((ringstapels[difficulty] + 1f) / kolommenMogelijk));
            hoogteStapel = hoogteRing * (ringenPerStapel[difficulty] + 1);
            rijenMogelijk = Mathf.FloorToInt(screenSafeAreaHeightInUnits / (scale * (ruimteTussenStapels * 2f + hoogteStapel))) + Mathf.FloorToInt(screenSafeAreaHeightInUnits % (scale * (ruimteTussenStapels * 2f + hoogteStapel)) / (scale * hoogteStapel));
            benodigdeRijen = 1;
            if (ringstapels[difficulty] + 1 > columnsToUse)
            {
                benodigdeRijen = Mathf.CeilToInt((ringstapels[difficulty] + 1f) / columnsToUse);
            }
        }
        Transform prefabInLaatsteKolom = null;
        for (int i = 0; i <= ringstapels[difficulty]; i++)
        {
            Transform stapelTf = stapels[i].transform;
            int xi = i;
            int yi = 0;
            if (i >= columnsToUse)
            {
                yi = Mathf.FloorToInt(i / columnsToUse);
                xi = i - yi * columnsToUse;
            }
            float posX = -screenWidthInUnits / 2f + xi * (ruimteTussenStapels * 2f + breedteRing) + breedteRing / 2f;
            float posY = screenHeightInUnits / 2f - (yi * ((ruimteTussenStapels * 2f) + hoogteStapel)) - hoogteStapel + hoogteRing / 2f;
            stapelTf.position = new Vector3(posX, posY, 0);
            for (int a = 0; a < stapelTf.childCount; a++)
            {
                Transform obj = stapelTf.GetChild(a);
                obj.localPosition = a * hoogteRing * Vector3.up;
            }
            if (xi == columnsToUse - 1)
            {
                prefabInLaatsteKolom = stapelTf;
            }
        }
        float spelBreedte = prefabInLaatsteKolom.position.x - stapels[0].transform.position.x + breedteRing;
        float spelHoogte = stapels[0].transform.position.y - stapels[^1].transform.position.y + hoogteStapel;
        Vector3 positionChange = new Vector3((screenWidthInUnits - spelBreedte) / 2f, -(screenHeightInUnits - spelHoogte) / 2f, 0);
        foreach (Transform child in stapelsHouder)
        {
            child.position += positionChange;
        }
        scale *= 0.95f;
        stapelsHouder.localScale = (Vector3.right + Vector3.up) * scale + Vector3.forward;
        stapelsHouder.position = new Vector3(ScreenExt.PixelsToUnits(ScreenExt.LeftOutsideSafezone - ScreenExt.RightOutsideSafezone) / 2f, ScreenExt.PixelsToUnits(ScreenExt.BottomOutsideSafezone - ScreenExt.TopOutsideSafezone) / 2f, 0);
    }
}
