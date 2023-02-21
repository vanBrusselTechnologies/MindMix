using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SolitaireGameHandler : MonoBehaviour
{
    public const int SpaceBetweenCards = 75;

    private GegevensHouder _gegevensHouder;
    private SaveScript _saveScript;
    private RewardHandler _rewardHandler;
    [SerializeField] private SolitaireLayout solitaireLayout;
    [SerializeField] private SolitaireUIHandler solitaireUIHandler;
    [SerializeField] private SolitaireTouchHandler solitaireTouchHandler;

    public List<RectTransform> playingCards;

    [SerializeField] private TMP_Text playingTimeText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject finishGameButtonObj;

    private readonly List<RectTransform> _shuffledPlayingCards = new();

    [HideInInspector] public float spaceBetweenCardsFactor = 1f;
    [HideInInspector] public bool solitaireInactive;
    [HideInInspector] public bool finished;
    private float _currentPlayingTime;
    private float _startPlayTime;
    private float _totalTimePenalty;
    private const float TimePenalty = 5f;

    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        _gegevensHouder = GegevensHouder.Instance;
        if (_gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }

        _saveScript = SaveScript.Instance;
        _rewardHandler = RewardHandler.Instance;
        
        if (_gegevensHouder.startNewGame)
        {
            ClearProgress();
            ShufflePlayingCards();
            AddCardsToStack();
            _currentPlayingTime = 0f;
            _startPlayTime = Time.time;
            SaveProgress();
        }
        else
        {
            LoadProgress();
            _currentPlayingTime = _saveScript.FloatDict["SolitaireTime"];
            _startPlayTime = Time.time - _currentPlayingTime;
        }

        solitaireLayout.SetLayout();
    }

    private void Update()
    {
        if (finished) return;
        if (solitaireInactive)
        {
            _startPlayTime = Time.time - _currentPlayingTime;
            return;
        }

        ShowCurrentPlayingTime();
    }

    private void ShufflePlayingCards()
    {
        List<RectTransform> temp = new();
        for (int i = 0; i < 52; i++)
        {
            temp.Add(playingCards[i]);
        }

        for (int i = 0; i < temp.Count;)
        {
            int rand = Random.Range(0, temp.Count);
            _shuffledPlayingCards.Add(temp[rand]);
            temp.RemoveAt(rand);
        }
    }

    private void AddCardsToStack()
    {
        List<Transform> stackTfs = solitaireTouchHandler.stackTfs;
        int[] cardsPerStack = { 1, 2, 3, 4, 5, 6, 7, 24 };
        foreach (int cardCount in cardsPerStack)
        {
            for (int i = 0; i < cardCount; i++)
            {
                RectTransform card = _shuffledPlayingCards[0];
                if (cardCount <= 7)
                {
                    card.SetParent(stackTfs[6 + cardCount]);
                    card.offsetMax = Vector2.up;
                    card.offsetMin = Vector2.down;
                    card.anchoredPosition = new Vector2(0, i * -SpaceBetweenCards * spaceBetweenCardsFactor);
                    if (i != cardCount - 1) card.GetChild(0).SetSiblingIndex(1);
                }
                else
                {
                    card.SetParent(stackTfs[6]);
                    card.SetSiblingIndex(card.parent.childCount - 2);
                    card.offsetMax = Vector2.up;
                    card.offsetMin = Vector2.down;
                    card.GetChild(0).SetSiblingIndex(1);
                }

                _shuffledPlayingCards.RemoveAt(0);
            }
        }
    }

    private void LoadProgress()
    {
        string progress = _saveScript.StringDict["SolitaireProgress"];
        string[] cardsInStack = progress.Split("999");
        List<Transform> stackTfs = solitaireTouchHandler.stackTfs;
        for (int i = 0; i < cardsInStack.Length; i++)
        {
            string cardsString = cardsInStack[i];
            string[] cards = cardsString.Trim().Split(' ');
            for (int j = 0; j < cards.Length; j++)
            {
                string cardName = cards[j];
                if (cardName.Equals("")) continue;
                int _ = Mathf.Abs(int.Parse(cardName));
                int index = Mathf.FloorToInt(_ / 100f) * 13 + _ % 100;

                RectTransform card = playingCards[index];
                card.SetParent(stackTfs[i]);
                card.offsetMax = Vector2.up;
                card.offsetMin = Vector2.down;
                if (i >= 7)
                    card.anchoredPosition = new Vector2(0, j * -SpaceBetweenCards * spaceBetweenCardsFactor);
                if (cardName[..1].Equals("-"))
                    card.GetChild(0).SetSiblingIndex(1);
            }

            if (i == 6) stackTfs[i].GetChild(0).SetSiblingIndex(stackTfs[i].childCount - 1);
        }

        if (stackTfs[5].childCount <= 1 && stackTfs[6].childCount == 1)
            Destroy(stackTfs[6].GetChild(0).gameObject);

        solitaireTouchHandler.CheckIfAllCardsTurned();
    }

    public void CorrectPositions()
    {
        List<Transform> stackTfs = solitaireTouchHandler.stackTfs.GetRange(7, 7);
        foreach (Transform stackTf in stackTfs)
        {
            for (int i = 0; i < stackTf.childCount; i++)
            {
                string cardName = stackTf.GetChild(i).name;
                if (cardName.Equals("")) continue;
                int _ = Mathf.Abs(int.Parse(cardName));
                int index = Mathf.FloorToInt(_ / 100f) * 13 + _ % 100;

                RectTransform card = playingCards[index];
                card.offsetMax = Vector2.up;
                card.offsetMin = Vector2.down;
                card.anchoredPosition = new Vector2(0, i * -SpaceBetweenCards * spaceBetweenCardsFactor);
            }
        }
    }

    private void ShowCurrentPlayingTime()
    {
        _currentPlayingTime = Time.time - _startPlayTime + _totalTimePenalty;
        _saveScript.FloatDict["SolitaireTime"] = _currentPlayingTime;
        int totalSeconds = Mathf.FloorToInt(_currentPlayingTime);
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = totalSeconds - (minutes * 60);
        int milliseconds = Mathf.FloorToInt((_currentPlayingTime - totalSeconds) * 1000);
        string currentPlayingTimeString = $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
        playingTimeText.text = currentPlayingTimeString;
    }

    public void AddTimePenalty()
    {
        _totalTimePenalty += TimePenalty;
    }
    
    public void FinishGame()
    {
        finished = true;
        finishGameButtonObj.SetActive(false);
        Scene scene = SceneManager.GetActiveScene();
        float usedPlayingTime = _saveScript.FloatDict["SolitaireTime"];
        rewardText.text = _rewardHandler.GetReward(scene: scene, score: usedPlayingTime, targetText: rewardText)
            .ToString();

        foreach (var card in playingCards)
        {
            int cardIndex = int.Parse(card.name);
            int cardType = Mathf.FloorToInt(cardIndex / 100f);
            card.SetParent(solitaireTouchHandler.stackTfs[cardType]);
            card.anchoredPosition = Vector2.zero;
        }

        HideFinishGameButton();
        solitaireUIHandler.ShowFinishedCanvas();
        ClearProgress();
    }

    public void ShowFinishGameButton()
    {
        finishGameButtonObj.SetActive(true);
    }

    private void HideFinishGameButton()
    {
        finishGameButtonObj.SetActive(true);
    }

    private void ClearProgress()
    {
        _saveScript.StringDict["SolitaireProgress"] = "";
        _saveScript.FloatDict["SolitaireTime"] = 0;
    }

    public void SaveProgress()
    {
        string[] cards = new string[66];
        int i = 0;
        foreach (Transform stackTf in solitaireTouchHandler.stackTfs)
        {
            foreach (Transform card in stackTf)
            {
                if (card.name.Equals("StockButton")) continue;
                if (card.GetChild(0).name.Equals("front"))
                    cards[i] = $"-{card.name}";
                else
                    cards[i] = card.name;
                i++;
            }

            cards[i] = "999";
            i++;
        }

        _saveScript.StringDict["SolitaireProgress"] = SaveScript.StringifyArray(cards, " ");
    }
}