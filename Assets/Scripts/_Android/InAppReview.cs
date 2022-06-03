#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
using System.Collections;
#endif
using UnityEngine;

public class InAppReview: MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private ReviewManager reviewManager;

    private void Awake()
    {
        reviewManager = new ReviewManager();
        RequestInAppReview();
    }

    public void RequestInAppReview()
    {
        StartCoroutine(RequestInAppReviewInfo());
    }

    private IEnumerator RequestInAppReviewInfo()
    {
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(requestFlowOperation.Error.ToString());
            yield break;
        }
        PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();
        yield return LaunchInAppReviewFlow(playReviewInfo);
    }

    private IEnumerator LaunchInAppReviewFlow(PlayReviewInfo playReviewInfo)
    {
        var launchOperationFlow = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchOperationFlow;
        if(launchOperationFlow.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(launchOperationFlow.Error.ToString());
            yield break;
        }
    }
#else
    private void Awake()
    {
        Debug.LogWarning("In app Review en Update moeten aangepast en vooral getest worden vóór Release");
    }
#endif
}