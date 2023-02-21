using System;
using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardHandler : MonoBehaviour
{
    public static RewardHandler Instance;
    private int _finished;
    private float _lastScreenWidth;
    private float _lastSafeZoneY;
    private float _lastSafeZoneX;
    private bool _isPaused;
    private bool _wasPaused;
    private SaveScript _saveScript;
    private readonly int[] _sudokuRewards = { 5, 11, 18, 27 };
    private readonly int[] _minesweeperRewards = { 5, 10, 17, 25 };
    private const int MaximumReward2048 = 33;
    private const int MaximumRewardSolitaire = 35;
    private int _lastEarnedCoinAmount;
    private TMP_Text _lastTargetText;
    public RectTransform muntenObj;
    [SerializeField] private RectTransform muntenRect;
    [SerializeField] private TMP_Text muntenText;
    ShopScript _shopScript;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
        _saveScript = SaveScript.Instance;
        DontDestroyOnLoad(this);
    }

    public int GetReward(Scene scene, int difficulty = 0, float score = 0, TMP_Text targetText = null)
    {
        _lastTargetText = targetText;
        int coins = 0;
        switch (scene.name.ToLower())
        {
            case "sudoku":
                coins = _sudokuRewards[difficulty];
                ReceiveCoins(coins);
                break;
            case "2048":
                float max = Mathf.Pow(2, ((difficulty + 4) * (difficulty + 4)) - 2);
                coins = Mathf.CeilToInt(score / max * MaximumReward2048);
                coins = Mathf.Max(1, coins);
                ReceiveCoins(coins);
                break;
            case "solitaire":
                float minutes = Mathf.Max(1, score / 60f);
                coins = Mathf.FloorToInt(MaximumRewardSolitaire / minutes);
                coins = Mathf.Max(1, coins);
                ReceiveCoins(coins);
                break;
            case "minesweeper":
                coins = _minesweeperRewards[difficulty];
                ReceiveCoins(coins);
                break;
        }

        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventLevelEnd,
            new Parameter[]
            {
                new(FirebaseAnalytics.ParameterLevelName, scene.name),
                new(FirebaseAnalytics.ParameterSuccess, 1),
            }
        );
        _lastEarnedCoinAmount = coins;
        return coins;
    }

    private void ReceiveCoins(int coins)
    {
        _saveScript.IntDict["munten"] += coins;
        ShowCurrentCoins();
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventEarnVirtualCurrency, new Parameter(FirebaseAnalytics.ParameterValue, coins),
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Coin"),
            new Parameter(FirebaseAnalytics.ParameterCurrency, "EUR"));
    }

    public void SpendCoins(int coins)
    {
        _saveScript.IntDict["munten"] -= coins;
        ShowCurrentCoins();
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventSpendVirtualCurrency,
            new Parameter(FirebaseAnalytics.ParameterItemName, _shopScript.naam),
            new Parameter(FirebaseAnalytics.ParameterValue, coins),
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Coin"));
    }

    public void DoubleCoins()
    {
        if (_lastTargetText == null) return;
        const int factor = 3;
        ReceiveCoins(_lastEarnedCoinAmount * (factor - 1));
        _lastTargetText.text = (_lastEarnedCoinAmount * factor).ToString();
        Transform parent = _lastTargetText.transform.parent;
        parent.Find("verdubbel").gameObject.SetActive(false);
        float scaleFactor = Mathf.Min(Screen.safeArea.width * 0.85f / 500, Screen.safeArea.height * (5f / 30f) / 175);
        RectTransform rewardRect = parent.GetComponent<RectTransform>();
        rewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        rewardRect.sizeDelta = new Vector2(500, 175);
    }

    private void ShowCurrentCoins()
    {
        SetPositionCurrentCoins();
        muntenText.text = _saveScript.IntDict["munten"].ToString();
        muntenObj.gameObject.SetActive(true);
    }

    private void SetPositionCurrentCoins()
    {
        Vector2 sizeDelta = Vector2.one * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f;
        sizeDelta.x *= 2.5f;
        muntenObj.anchoredPosition = new Vector2(
            (Screen.width / 2f) - (Screen.width - Screen.safeArea.width - Screen.safeArea.x) -
            (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f * 3f),
            (Screen.height / 2f) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) -
            (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        muntenObj.sizeDelta = sizeDelta;
        muntenRect.offsetMin = new Vector2(sizeDelta.y, 0);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (scene.name.Equals("Shop"))
        {
            _shopScript = GameObject.Find("EventSystem").GetComponent<ShopScript>();
            ShowCurrentCoins();
        }
        else
        {
            muntenObj.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_isPaused && _wasPaused)
        {
            SetPositionCurrentCoins();
            _wasPaused = _isPaused;
            return;
        }

        _wasPaused = _isPaused;
        if (Math.Abs(_lastScreenWidth - Screen.width) < 0.0001f && 
            Math.Abs(_lastSafeZoneY - Screen.safeArea.y) < 0.0001f &&
            Math.Abs(_lastSafeZoneX - Screen.safeArea.x) < 0.0001f)
        {
            if (_finished < 3)
            {
                SetPositionCurrentCoins();
            }

            return;
        }

        _finished = 0;
        SetPositionCurrentCoins();
        _lastScreenWidth = Screen.width;
        _lastSafeZoneY = Screen.safeArea.y;
        _lastSafeZoneX = Screen.safeArea.x;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _isPaused = !hasFocus;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        _isPaused = pauseStatus;
    }
}