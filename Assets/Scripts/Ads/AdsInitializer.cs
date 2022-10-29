using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

public class AdsInitializer : MonoBehaviour
{
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOsGameId;
    private string _gameId;
    private bool _isInitialized;
    [HideInInspector] public bool adLoaded;
    public IRewardedAd m_AD { get; private set; }

    #region Initialization

    private void Start()
    {
        InitializeAds();
        StartCoroutine(ReInitialize());
    }

    private async void InitializeAds()
    {
        try
        {
            _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? iOsGameId
                : androidGameId;
            InitializationOptions opt = new();
            opt.SetGameId(_gameId);
            await UnityServices.InitializeAsync(opt);
            OnInitializationComplete();
        }
        catch (Exception e)
        {
            OnInitializationFailed(e);
        }
    }

    IEnumerator ReInitialize()
    {
        yield return new WaitForSecondsRealtime(30f);
        if (_isInitialized) yield break;
        InitializeAds();
        StartCoroutine(ReInitialize());
    }

    private void OnInitializationComplete()
    {
        _isInitialized = true;
        LoadRewardedAd();
    }

    private void OnInitializationFailed(Exception e)
    {
        SdkInitializationError initializationError = SdkInitializationError.Unknown;
        if (e is InitializeFailedException initializeFailedException)
        {
            initializationError = initializeFailedException.initializationError;
        }

        Debug.Log($"UnityAds InitializationError: {initializationError}: {e.Message}");
    }

    #endregion

    public async void LoadRewardedAd()
    {
        string rewardedAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? "iOS_Rewarded"
            : "Android_Rewarded";
        IRewardedAd rewardedAd = MediationService.Instance.CreateRewardedAd(rewardedAdUnitId);
        m_AD = rewardedAd;
        try
        {
            rewardedAd.OnLoaded += OnUnityAdsAdLoaded;
            rewardedAd.OnFailedLoad += OnUnityAdsFailedToLoad;
            rewardedAd.OnShowed += OnUnityAdsShowStart;
            rewardedAd.OnFailedShow += OnUnityAdsShowFailure;
            rewardedAd.OnClicked += OnUnityAdsShowClick;
            rewardedAd.OnClosed += OnUnityAdsClosed;
            rewardedAd.OnUserRewarded += OnUserRewarded;
            await rewardedAd.LoadAsync();
        }
        catch (LoadFailedException e)
        {
            Debug.Log($"LoadAd Exception: {e.LoadError}");
        }
    }

    #region ShowAd

    async Task WaitForAdLoaded()
    {
        await Task.Run(() =>
        {
            while (!adLoaded)
            {
            }
        });
    }

    public async void ShowRewardedAd()
    {
        if (m_AD.AdState != AdState.Loaded)
        {
            if (m_AD.AdState == AdState.Unloaded) LoadRewardedAd();
            await WaitForAdLoaded();
        }

        try
        {
            await m_AD.ShowAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    #endregion

    #region callbacks

    void OnUnityAdsAdLoaded(object sender, EventArgs e)
    {
        Debug.Log("Ad Load Success");
        adLoaded = true;
    }

    void OnUnityAdsFailedToLoad(object sender, LoadErrorEventArgs e)
    {
        Debug.Log($"{e.Error}:{e.Message}");
    }

    void OnUnityAdsShowFailure(object sender, ShowErrorEventArgs args)
    {
        Debug.Log($"Ad failed to show: {args.Error}");
    }

    void OnUnityAdsShowStart(object sender, EventArgs args)
    {
        Debug.Log("Ad shown successfully.");
        adLoaded = false;
    }

    void OnUnityAdsShowClick(object sender, EventArgs e)
    {
        Debug.Log("Ad show clicked");
    }

    void OnUnityAdsClosed(object sender, EventArgs e)
    {
        Debug.Log("Ad Closed");
        LoadRewardedAd();
    }

    void OnUserRewarded(object sender, RewardEventArgs e)
    {
        Debug.Log("Ad - User rewarded");
    }

    #endregion
}