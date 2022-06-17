using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOsGameId;
    private bool _testMode = false;
    private string _gameId;
    private bool _isInitialized = false;
    [HideInInspector] public bool adLoaded;

    private void Start()
    {
        InitializeAds();
        StartCoroutine(ReInitialize());
    }

    public void InitializeAds()
    {
        MetaData metaData = new MetaData("privacy");
        metaData.Set("mode", "none");
        Advertisement.SetMetaData(metaData);

        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    IEnumerator ReInitialize()
    {
        yield return new WaitForSecondsRealtime(30f);
        if (_isInitialized) yield break;
        InitializeAds();
        StartCoroutine(ReInitialize());
    }

    public void OnInitializationComplete()
    {
        _isInitialized = true;
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }
}