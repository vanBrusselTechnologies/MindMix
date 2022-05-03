using System.Collections.Generic;
using UnityEngine;
using VBG.Extensions;

public class SolitaireLayout : BaseLayout
{
    [Header("Other scene specific")]
    [SerializeField] Transform solitaire;
    private SolitaireScript solitaireScript;
    [SerializeField] private Transform finishGameButtonTransform;
    [SerializeField] private List<Transform> cardColumns = new List<Transform>();
    [SerializeField] private List<Transform> foundations = new List<Transform>();
    [SerializeField] private Transform stockPileTransform;
    [SerializeField] private Transform stockPileTurnButtonTransform;

    // Start is called before the first frame update
    protected override void Start()
    {
        saveScript = SaveScript.instance;
        if (saveScript == null)
        {
            return;
        }
        solitaireScript = GetComponent<SolitaireScript>();
    }

    public override void SetLayout()
    {
        base.SetLayout();
        float finishGameButtonYPos = (-screenSafeAreaHeightInUnits / 2f) - ((screenSafeAreaYUpInUnits - screenSafeAreaYInUnits) / 2f) + (screenSafeAreaHeightInUnits * 0.1f);
        finishGameButtonTransform.position = new Vector3(0f, finishGameButtonYPos, 0f);
        ScaleCards();
        PositionCards();
    }

    private void ScaleCards()
    {
        screenSafeAreaWidthInUnits = Mathf.Min(screenSafeAreaWidthInUnits, screenSafeAreaHeightInUnits * (8f / 4.5f));
        Vector3 cardsLocalScale = new Vector3(1, 1, 1 / (screenSafeAreaWidthInUnits / 81f * 10) / 0.18f) * screenSafeAreaWidthInUnits / 81f * 10 * 0.18f;
        Vector3 stockLocalScale = new Vector3(1, 1, 1 / (screenSafeAreaWidthInUnits / 81f * 10)) * screenSafeAreaWidthInUnits / 81f * 10;
        for (int i = 0; i < solitaireScript.kaarten.Count; i++)
        {
            solitaireScript.kaarten[i].transform.localScale = cardsLocalScale;
            if (i < 7)
            {
                cardColumns[i].localScale = stockLocalScale;
                if (i < 4)
                {
                    foundations[i].localScale = stockLocalScale;
                }
            }
        }
        stockPileTransform.localScale = stockLocalScale;
    }

    public void PositionCards()
    {
        float _screenSafeAreaCenter = (screenSafeAreaXInUnits - screenSafeAreaXRightInUnits) / 2f;
        List<float> xPositions = new List<float>();
        float _screenWidth = Mathf.Min(screenWidthInUnits, screenHeightInUnits * (8f / 4.5f));
        for (int i = 0; i < 7; i++)
        {
            xPositions.Add(_screenSafeAreaCenter + _screenWidth / 81f * (-33f + i * 11f));
        }
        float xRestStapelUI = (screenSafeAreaX - screenSafeAreaXRight) / 2f + screenWidth / 81f * 33f + screenWidth / 2f;
        float baseY = screenSafeAreaYInUnits - screenSafeAreaYUpInUnits + (screenHeightInUnits * -1f / 2f / 1.5f) + (screenHeightInUnits / 35f / 1.5f * ((2f + (3f / 6f)) * 10f / 1.5f));
        float baseYFoundation = screenSafeAreaYInUnits - screenSafeAreaYUpInUnits + (screenHeightInUnits * -1f / 2f / 1.5f) + (screenHeightInUnits / 35f / 1.5f * ((5f + (1f / 6f)) * 10f / 1.5f));
        float restStapelBasisYUI = screenSafeAreaY - screenSafeAreaYUp + (screenHeight * -1f / 2f / 1.5f) + (screenHeight / 35f / 1.5f * ((5f + (1f / 6f)) * 10f / 1.5f)) + screenHeight / 2f;
        float spaceBetweenCardsFactor = saveScript.floatDict["spaceBetweenCardsFactor"];
        if (spaceBetweenCardsFactor == 0) spaceBetweenCardsFactor = 1;
        float verschilY = 0.3f * spaceBetweenCardsFactor;
        float baseZ = -2f;
        float verschilZ = 0.1f;
        for(int i = 0; i < 7; i++)
        {
            cardColumns[i].position = new Vector3(xPositions[i], baseY, -1f);
            if (i < 4)
            {
                foundations[i].position = new Vector3(xPositions[i], baseYFoundation, -1f);
            }
        }
        stockPileTurnButtonTransform.position = new Vector3(xRestStapelUI, restStapelBasisYUI, -1f);
        stockPileTurnButtonTransform.localScale = new Vector3(1, 1, 1 / Mathf.Min(screenSafeAreaWidth / 81f / 10, screenSafeAreaHeight / 81f / 10 * (8f / 4.5f))) * Mathf.Min(screenSafeAreaWidth / 81f / 10, screenSafeAreaHeight / 81f / 10 * (8f / 4.5f));
        List<List<GameObject>> CardsStocks = new List<List<GameObject>>();
        CardsStocks.AddRange(solitaireScript.Stapel1, solitaireScript.Stapel2, solitaireScript.Stapel3, solitaireScript.Stapel4, solitaireScript.Stapel5, solitaireScript.Stapel6, solitaireScript.Stapel7);
        for (int i = 0; i < CardsStocks.Count; i++)
        {
            for(int ii = 0; ii < CardsStocks[i].Count; ii++)
            {
                CardsStocks[i][ii].transform.position = new Vector3(xPositions[i], baseY - (ii * verschilY), baseZ - (verschilZ * ii));
            }
        }
        List<List<GameObject>> CardsFoundations = new List<List<GameObject>>();
        CardsFoundations.AddRange(solitaireScript.EindStapel1, solitaireScript.EindStapel2, solitaireScript.EindStapel3, solitaireScript.EindStapel4);
        for (int i = 0; i < CardsFoundations.Count; i++)
        {
            for (int ii = 0; ii < CardsFoundations[i].Count; ii++)
            {
                CardsFoundations[i][ii].transform.position = new Vector3(xPositions[i], baseYFoundation, baseZ - (verschilZ * ii));
            }
        }
        if (solitaireScript.knoppenScript.OmgedraaideRest.Count != 0)
        {
            for (int i = 0; i < solitaireScript.knoppenScript.OmgedraaideRest.Count; i++)
            {
                solitaireScript.knoppenScript.OmgedraaideRest[i].transform.position = new Vector3(xPositions[5], baseYFoundation, baseZ - (verschilZ * i));
            }
        }
        for (int i = 0; i < solitaireScript.StapelRest.Count; i++)
        {
            solitaireScript.StapelRest[i].transform.position = new Vector3(xPositions[6], baseYFoundation, baseZ - (verschilZ * i));
        }
    }
}