package com.vanbrusselgames.mindmix.core.advertisement

import android.app.Activity
import android.content.Context
import android.net.ConnectivityManager
import android.net.Network
import android.os.Handler
import android.os.Looper
import androidx.compose.runtime.MutableState
import com.google.android.gms.ads.AdError
import com.google.android.gms.ads.AdRequest
import com.google.android.gms.ads.FullScreenContentCallback
import com.google.android.gms.ads.LoadAdError
import com.google.android.gms.ads.MobileAds
import com.google.android.gms.ads.rewarded.RewardedAd
import com.google.android.gms.ads.rewarded.RewardedAdLoadCallback
import com.google.android.ump.ConsentInformation
import com.google.android.ump.ConsentRequestParameters
import com.google.android.ump.UserMessagingPlatform
import com.vanbrusselgames.mindmix.core.common.NetworkMonitor
import com.vanbrusselgames.mindmix.core.logging.Logger
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.util.concurrent.atomic.AtomicBoolean
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AdManager @Inject constructor(
    @param:ApplicationContext private val ctx: Context,
    private val networkMonitor: NetworkMonitor
) {
    private lateinit var consentInformation: ConsentInformation
    private var loadingConsentInformation = false

    // Use an atomic boolean to initialize the Google Mobile Ads SDK and load ads once.
    private val isMobileAdsInitializeCalled = AtomicBoolean(false)

    private var rewardedAd: RewardedAd? = null

    private var loading = false
    private val adLoadedStates: MutableList<MutableState<Boolean>> = mutableListOf()

    private val mainHandler = Handler(Looper.getMainLooper())

    private var internetConnected = true

    private val networkCallback = object : ConnectivityManager.NetworkCallback() {
        override fun onAvailable(network: Network) {
            internetConnected = true
        }

        override fun onLost(network: Network) {
            internetConnected = false
        }
    }

    private var initialized = false

    private var sdkInitialized = false

    fun initialize(activity: Activity) {
        if (!initialized) {
            CoroutineScope(Dispatchers.Main).launch {
                initialized = true
                networkMonitor.registerNetworkCallback(networkCallback)
                requestConsent(activity)
                // Check if you can initialize the Google Mobile Ads SDK in parallel
                // while checking for new consent information. Consent obtained in
                // the previous session can be used to request ads.
                if (consentInformation.canRequestAds()) initializeMobileAdsSdk()
                loadAds()
            }
        }
    }

    private fun requestConsent(activity: Activity) {
        if (loadingConsentInformation) return
        loadingConsentInformation = true
        val params: ConsentRequestParameters = ConsentRequestParameters.Builder().build()
        consentInformation = UserMessagingPlatform.getConsentInformation(ctx)
        consentInformation.requestConsentInfoUpdate(activity, params, {
            UserMessagingPlatform.loadAndShowConsentFormIfRequired(activity) { loadAndShowError ->
                if (loadAndShowError != null) {
                    // Consent gathering failed.
                    Logger.w(
                        String.format(
                            "ConsentLoadAndShowError %s: %s",
                            loadAndShowError.errorCode,
                            loadAndShowError.message
                        )
                    )
                }
                loadingConsentInformation = false
            }
        }, { requestConsentError ->
            // Consent gathering failed.
            Logger.w(
                String.format(
                    "ConsentInfoUpdateError: %s: %s",
                    requestConsentError.errorCode,
                    requestConsentError.message
                )
            )
            loadingConsentInformation = false
        })
    }

    private fun initializeMobileAdsSdk() {
        if (isMobileAdsInitializeCalled.getAndSet(true)) return
        MobileAds.initialize(ctx) { initializationStatus ->
            sdkInitialized = true
            val statusMap = initializationStatus.adapterStatusMap
            for (adapterClass in statusMap.keys) {
                val status = statusMap[adapterClass]
                Logger.i(
                    "Adapter name: $adapterClass, Description: ${status!!.description}, Latency: ${status.latency}"
                )
            }
        }
    }

    private fun loadAds() {
        if (loading) return
        val adRequest = AdRequest.Builder().build()
        loading = true
        RewardedAd.load(
            ctx,
            "ca-app-pub-7048585956228368/6355537748",
            adRequest,
            object : RewardedAdLoadCallback() {
                override fun onAdFailedToLoad(adError: LoadAdError) {
                    rewardedAd = null
                    loading = false
                    Logger.w("AdLoadingError: $adError")
                }

                override fun onAdLoaded(ad: RewardedAd) {
                    Logger.i("Ad was loaded.")
                    rewardedAd = ad
                    loading = false
                    notifyStateAdLoaded()
                }
            })
    }

    private fun notifyStateAdLoaded() {
        adLoadedStates.forEach { it.value = true }
        adLoadedStates.clear()
    }

    fun checkAdLoaded(activity: Activity, adLoadedState: MutableState<Boolean>? = null) {
        if (adLoadedState != null) {
            if (rewardedAd != null) {
                adLoadedState.value = true
                return
            }
            adLoadedStates.add(adLoadedState)
        }
        if (consentInformation.canRequestAds()) {
            if (!sdkInitialized) initializeMobileAdsSdk()
            if (rewardedAd == null) loadAds()
        } else requestConsent(activity)

        if (adLoadedStates.isEmpty()) return

        mainHandler.postDelayed(object : Runnable {
            override fun run() {
                if (rewardedAd != null) return
                if (!internetConnected) return
                checkAdLoaded(activity)
            }
        }, 5000)
    }

    fun showAd(
        activity: Activity, adLoadedState: MutableState<Boolean>? = null, onReward: (Int) -> Unit
    ) {
        addAdCallbacks()

        rewardedAd?.let { ad ->
            ad.show(activity) { rewardItem ->
                // Handle the reward.
                val rewardAmount = rewardItem.amount
                val rewardType = rewardItem.type
                Logger.i("User earned the reward: ${rewardAmount}x $rewardType")
                onReward(rewardAmount)
                loadAds()
            }
        } ?: run {
            if (adLoadedState != null) adLoadedState.value = false
            Logger.w("The rewarded ad is not ready yet.")
            checkAdLoaded(activity, adLoadedState)
        }
    }

    private fun addAdCallbacks() {
        rewardedAd?.fullScreenContentCallback =
            object : FullScreenContentCallback() {
                override fun onAdClicked() {
                    // Called when a click is recorded for an ad.
                    Logger.i("Ad was clicked.")
                }

                override fun onAdDismissedFullScreenContent() {
                    // Called when ad is dismissed.
                    // Set the ad reference to null so you don't show the ad a second time.
                    Logger.i("Ad dismissed fullscreen content.")
                    rewardedAd = null
                    loadAds()
                }

                override fun onAdFailedToShowFullScreenContent(adError: AdError) {
                    // Called when ad fails to show.
                    Logger.e("Ad failed to show fullscreen content.")
                    rewardedAd = null
                    loadAds()
                }

                override fun onAdImpression() {
                    // Called when an impression is recorded for an ad.
                    Logger.i("Ad recorded an impression.")
                }

                override fun onAdShowedFullScreenContent() {
                    // Called when ad is shown.
                    Logger.i("Ad showed fullscreen content.")
                }
            }
    }
}
