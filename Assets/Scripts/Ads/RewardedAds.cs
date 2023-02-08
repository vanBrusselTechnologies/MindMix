using TMPro;
using Unity.Services.Mediation;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour
{
    RewardHandler _rewardHandler;
    AdsInitializer _adsInitializer;
    [SerializeField] private Button rewardedAdButton;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _rewardHandler = RewardHandler.Instance;
        if (_rewardHandler == null) return;
        _adsInitializer = AdsInitializer.Instance;
        if (_adsInitializer == null) return;
        _adsInitializer.rewardedAds = this;
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd() //Set this as OnClickListener of button
    {
        if(_rewardHandler == null || _adsInitializer == null || _adsInitializer.rewardedAds != this) Init();
        if (_adsInitializer == null) return;
        rewardedAdButton.interactable = false;
        rewardedAdButton.transform.GetChild(0).GetComponentInChildren<TMP_Text>().SetText("Loading...");
        _adsInitializer.ShowRewardedAd();
    }

    public void OnUserRewarded(object sender, RewardEventArgs e)
    {
        Debug.Log($"Received reward: type:{e.Type}; amount:{e.Amount}");
        _rewardHandler.DoubleCoins();
    }
}