using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string _adUnitId;
    private bool adLoaded = false;
    BeloningScript beloningScript;

    private void Awake()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if(gegevensHouder == null)
        {
            return;
        }
        beloningScript = gegevensHouder.GetComponent<BeloningScript>();
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? "iOS_Rewarded"
            : "Android_Rewarded";
        LoadAd();
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        Advertisement.Load(_adUnitId, this);
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd() //Set this as OnClickListener of button
    {
        if (!adLoaded)
        {
            Advertisement.Load(_adUnitId, this);
        }
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            beloningScript.VerdubbelCoins();
            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
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
        adLoaded = true;
    }
}