using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOsGameId;
    private bool _testMode = false;
    private string _gameId;

    private void Start()
    {
        InitializeAds();
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

    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }
}