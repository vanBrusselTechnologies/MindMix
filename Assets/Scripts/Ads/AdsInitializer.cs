using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

public class AdsInitializer : MonoBehaviour
{
    public static AdsInitializer Instance;
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOsGameId;
    private string _gameId;
    private bool _isInitialized;
    private bool _waitingForAd;
    private bool _adLoaded;
    private int _adCountSinceAppOpen;
    private bool _callbackSet;
    private bool _waitingForNetwork;
    private bool _showAfterLoad;
    [HideInInspector] public RewardedAds rewardedAds;
    private IRewardedAd _rewardedAd;

    #region Initialization

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
        InitializeAds();
    }

    private static void InitAdMob(InitializationStatus initStatus)
    {
        Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
        foreach ((string className, AdapterStatus status) in map)
        {
            switch (status.InitializationState)
            {
                case AdapterState.NotReady:
                    // The adapter initialization did not complete.
                    Debug.LogWarning("Adapter: " + className + " not ready.");
                    break;
                case AdapterState.Ready:
                    // The adapter was successfully initialized.
                    Debug.Log("Adapter: " + className + " is initialized.");
                    break;
            }
        }

        Debug.Log("MobileAds initialized");
    }

    private async void InitUnityAds()
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

    private void InitializeAds()
    {
        MobileAds.Initialize(InitAdMob);
        InitUnityAds();
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
        StartCoroutine(WaitForNetwork(-1));
    }

    #endregion

    #region LoadAd

    private async void LoadRewardedAd()
    {
        if (!_isInitialized) return;
        if (_rewardedAd != null) return;
        _callbackSet = false;
        _adLoaded = false;
        string rewardedAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? "iOS_Rewarded"
            : "Android_Rewarded";
        IRewardedAd rewardedAd = MediationService.Instance.CreateRewardedAd(rewardedAdUnitId);
        _rewardedAd = rewardedAd;
        try
        {
            rewardedAd.OnLoaded += OnUnityAdsAdLoaded;
            rewardedAd.OnFailedLoad += OnUnityAdsFailedToLoad;
            rewardedAd.OnShowed += OnUnityAdsShowStart;
            rewardedAd.OnFailedShow += OnUnityAdsShowFailure;
            rewardedAd.OnClicked += OnUnityAdsShowClick;
            rewardedAd.OnClosed += OnUnityAdsClosed;
            rewardedAd.OnUserRewarded += OnUserRewarded;
            if (rewardedAds != null)
            {
                _callbackSet = true;
                rewardedAd.OnUserRewarded += rewardedAds.OnUserRewarded;
            }

            await rewardedAd.LoadAsync();
        }
        catch (LoadFailedException e)
        {
            Debug.Log($"LoadAd Exception: {e.LoadError}");
        }
    }

    #endregion

    #region ShowAd

    async Task WaitForAdLoaded()
    {
        await Task.Run(() =>
        {
            _waitingForAd = true;
            while (!_adLoaded)
            {
            }

            _waitingForAd = false;
        });
    }

    public async void ShowRewardedAd()
    {
        try
        {
            if (_rewardedAd.AdState == AdState.Showing) return;
            if (_rewardedAd.AdState != AdState.Loaded)
            {
                if (_rewardedAd.AdState == AdState.Unloaded) LoadRewardedAd();
                if (_waitingForAd) return;
                await WaitForAdLoaded();
            }

            if (rewardedAds == null) return;
            if (!_callbackSet)
            {
                _callbackSet = true;
                _rewardedAd.OnUserRewarded += rewardedAds.OnUserRewarded;
            }

            await _rewardedAd.ShowAsync();
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
        _adLoaded = true;
        if (_showAfterLoad) ShowRewardedAd();
        _showAfterLoad = false;
    }

    private void OnUnityAdsFailedToLoad(object sender, LoadErrorEventArgs e)
    {
        _adLoaded = false;
        _rewardedAd = null;
        switch (e.Error)
        {
            case LoadError.NoFill:
                LoadRewardedAd();
                break;
            case LoadError.NetworkError:
                StartCoroutine(WaitForNetwork(0));
                break;
            case LoadError.SdkNotInitialized:
                break;
            case LoadError.AdUnitLoading:
                _adLoaded = false;
                break;
            case LoadError.AdUnitShowing: break;
            case LoadError.MissingMandatoryMemberValues:
            case LoadError.TooManyLoadRequests:
            case LoadError.Unknown:
            default:
                Debug.Log($"{e.Error}:{e.Message}");
                break;
        }
    }

    private void OnUnityAdsShowFailure(object sender, ShowErrorEventArgs e)
    {
        switch (e.Error)
        {
            case ShowError.AdNotLoaded:
                _rewardedAd = null;
                LoadRewardedAd();
                break;
            case ShowError.AdNetworkError:
                StartCoroutine(WaitForNetwork(1));
                break;
            case ShowError.Unknown:
            case ShowError.InvalidActivity:
            default:
                Debug.Log($"Ad failed to show: {e.Error}:{e.Message}");
                break;
        }
    }

    void OnUnityAdsShowStart(object sender, EventArgs args)
    {
        _adLoaded = false;
    }

    private static void OnUnityAdsShowClick(object sender, EventArgs e)
    {
    }

    void OnUnityAdsClosed(object sender, EventArgs e)
    {
        _rewardedAd = null;
        LoadRewardedAd();
#if UNITY_ANDROID && !UNITY_EDITOR
        _adCountSinceAppOpen += 1;
        if (_adCountSinceAppOpen == 5)
            GameObject.Find("GoogleScriptsObj").GetComponent<InAppReview>().RequestInAppReview();
#endif
    }

    private static void OnUserRewarded(object sender, RewardEventArgs e)
    {
    }

    #endregion

    IEnumerator WaitForNetwork(int type)
    {
        if (_waitingForNetwork) yield break;
        _waitingForNetwork = true;
        yield return new WaitForSecondsRealtime(15f);
        _waitingForNetwork = false;
        if (Application.internetReachability == NetworkReachability.NotReachable)
            yield return StartCoroutine(WaitForNetwork(type));
        if (_isInitialized) yield break;
        switch (type)
        {
            case -1:
                Debug.Log("WaitForNetwork {-1}");
                InitializeAds();
                break;
            case 0:
                Debug.Log("WaitForNetwork {0}");
                LoadRewardedAd();
                break;
            case 1:
                Debug.Log("WaitForNetwork {1}");
                ShowRewardedAd();
                break;
        }
    }
}