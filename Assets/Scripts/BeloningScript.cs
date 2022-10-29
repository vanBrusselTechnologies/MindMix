using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeloningScript : MonoBehaviour
{
    public static BeloningScript Instance;
    private int klaar;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    private bool isPaused;
    private bool wasPaused;
    private SaveScript saveScript;
    private int[] sudokuBeloningen = { 5, 11, 18, 27 };
    private int[] mvBeloningen = { 5, 10, 17, 25 };
    private int maxMunten2048 = 33;
    private int maxMuntenSolitaire = 35;
    private int laatstVerdiendeMunten;
    private TMP_Text laatsteDoelwitText;
    public RectTransform muntenObj;
    [SerializeField] private RectTransform muntenRect;
    [SerializeField] private TMP_Text muntenText;
    ShopScript shopScript;

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
        SceneManager.sceneLoaded += SceneLoaded;
        saveScript = SaveScript.Instance;
        DontDestroyOnLoad(this);
    }

    public int Beloning(Scene scene, int difficulty = 0, float score = 0, TMP_Text doelwitText = null)
    {
        laatsteDoelwitText = doelwitText;
        int munten = 0;
        switch (scene.name.ToLower())
        {
            case "sudoku":
                munten = sudokuBeloningen[difficulty];
                VoegMuntenToe(munten);
                break;
            case "2048":
                float max = Mathf.Pow(2, ((difficulty + 4) * (difficulty + 4)) - 2);
                munten = Mathf.CeilToInt(score / max * maxMunten2048);
                munten = Mathf.Max(1, munten);
                VoegMuntenToe(munten);
                break;
            case "solitaire":
                float minuten = Mathf.Max(1, score / 60);
                munten = Mathf.FloorToInt(maxMuntenSolitaire / minuten);
                munten = Mathf.Max(1, munten);
                VoegMuntenToe(munten);
                break;
            case "mijnenveger":
                munten = mvBeloningen[difficulty];
                VoegMuntenToe(munten);
                break;
        }
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventLevelEnd,
            new Parameter[]{
                new(FirebaseAnalytics.ParameterLevelName, scene.name),
                new(FirebaseAnalytics.ParameterSuccess, 1),
            }
        );
        laatstVerdiendeMunten = munten;
        return munten;
    }

    private void VoegMuntenToe(int coinsToAdd)
    {
        saveScript.intDict["munten"] += coinsToAdd;
        ShowHuidigAantalMunten();
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventEarnVirtualCurrency, new Parameter(FirebaseAnalytics.ParameterValue, coinsToAdd), new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Coin"), new Parameter(FirebaseAnalytics.ParameterCurrency, "EUR"));
    }

    public void GeefMuntenUit(int muntenToSpend)
    {
        saveScript.intDict["munten"] -= muntenToSpend;
        ShowHuidigAantalMunten();
        FirebaseAnalytics.LogEvent(
            FirebaseAnalytics.EventSpendVirtualCurrency, new Parameter(FirebaseAnalytics.ParameterItemName, shopScript.naam), new Parameter(FirebaseAnalytics.ParameterValue, muntenToSpend), new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Coin"));
    }

    public void VerdubbelCoins()
    {
        if (laatsteDoelwitText == null) return;
        int factor = 3;
        VoegMuntenToe(laatstVerdiendeMunten * (factor - 1));
        laatsteDoelwitText.text = (laatstVerdiendeMunten * factor).ToString();
        Transform parent = laatsteDoelwitText.transform.parent;
        parent.Find("verdubbel").gameObject.SetActive(false);
        float scaleFactor = Mathf.Min(Screen.safeArea.width * 0.85f / 500, Screen.safeArea.height * (5f / 30f) / 175);
        RectTransform rewardRect = parent.GetComponent<RectTransform>();
        rewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        rewardRect.sizeDelta = new Vector2(500, 175);
    }

    private void ShowHuidigAantalMunten()
    {
        ZetLocatieHuidigAantalMunten();
        muntenText.text = saveScript.intDict["munten"].ToString();
        muntenObj.gameObject.SetActive(true);
    }

    private void ZetLocatieHuidigAantalMunten()
    {
        Vector2 sizeDelta = Vector2.one * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f;
        sizeDelta.x *= 2.5f;
        muntenObj.anchoredPosition = new Vector2((Screen.width / 2f) - (Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f * 3f), (Screen.height / 2f) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        muntenObj.sizeDelta = sizeDelta;
        muntenRect.offsetMin = new Vector2(sizeDelta.y, 0);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (scene.name == "Shop")
        {
            shopScript = GameObject.Find("EventSystem").GetComponent<ShopScript>();
            ShowHuidigAantalMunten();
        }
        else
        {
            muntenObj.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isPaused && wasPaused)
        {
            ZetLocatieHuidigAantalMunten();
            wasPaused = isPaused;
            return;
        }
        wasPaused = isPaused;
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 3)
            {
                ZetLocatieHuidigAantalMunten();
            }
            return;
        }
        klaar = 0;
        ZetLocatieHuidigAantalMunten();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }
}