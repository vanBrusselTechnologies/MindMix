using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string _adUnitId;
    BeloningScript beloningScript;
    AdsInitializer adsInitializer;

    private void Awake()
    {
        beloningScript = BeloningScript.Instance;
        if (beloningScript == null) return;
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? "iOS_Rewarded"
            : "Android_Rewarded";
        adsInitializer = GameObject.Find("gegevensHouder").GetComponent<AdsInitializer>();
        if (Advertisement.isInitialized)
            LoadAd();
        else StartCoroutine(WaitForInit());
    }

    IEnumerator WaitForInit()
    {
        yield return new WaitUntil(() => Advertisement.isInitialized);
        LoadAd();
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        if (!adsInitializer.adLoaded)
            Advertisement.Load(_adUnitId, this);
    }

    IEnumerator WaitForAdLoaded()
    {
        yield return new WaitUntil(() => adsInitializer.adLoaded);
        ShowAd();
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd() //Set this as OnClickListener of button
    {
        if (!adsInitializer.adLoaded)
        {
            LoadAd();
            StartCoroutine(WaitForAdLoaded());
            return;
        }
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        adsInitializer.adLoaded = false;
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            beloningScript.VerdubbelCoins();
        }
        // Load another ad:
        LoadAd();
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
        if (error != UnityAdsLoadError.INITIALIZE_FAILED)
        {
            adsInitializer.adLoaded = false;
            LoadAd();
        }
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        adsInitializer.adLoaded = false;
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
        LoadAd();
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        //Debug.Log("showStart: " + adUnitId); 
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        //Debug.Log("ShowClick: " + adUnitId);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        adsInitializer.adLoaded = true;
    }
}