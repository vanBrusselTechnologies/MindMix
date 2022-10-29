using Unity.Services.Mediation;
using UnityEngine;

public class RewardedAds : MonoBehaviour
{
    BeloningScript beloningScript;
    AdsInitializer adsInitializer;

    private void Awake()
    {
        beloningScript = BeloningScript.Instance;
        if (beloningScript == null) return;
        adsInitializer = GameObject.Find("gegevensHouder").GetComponent<AdsInitializer>();
        adsInitializer.m_AD.OnUserRewarded += OnUserRewarded;
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd() //Set this as OnClickListener of button
    {
        adsInitializer.ShowRewardedAd();
    }
    
    void OnUserRewarded(object sender, RewardEventArgs e)
    {
        Debug.Log($"Received reward: type:{e.Type}; amount:{e.Amount}");
        beloningScript.VerdubbelCoins();
    }
}