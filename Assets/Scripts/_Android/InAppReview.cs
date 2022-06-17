#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
using System.Collections;
#endif
using UnityEngine;

public class InAppReview : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private ReviewManager reviewManager;

    private void Awake()
    {
        Debug.Log("Awake() inAppReview");
        reviewManager = new ReviewManager();
        RequestInAppReview();
    }

    public void RequestInAppReview()
    {
        Debug.Log("RequestInAppReview() inAppReview");
        StartCoroutine(RequestInAppReviewInfo());
    }

    private IEnumerator RequestInAppReviewInfo()
    {
        Debug.Log("RequestInAppReviewInfo() inAppReview");
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(requestFlowOperation.Error.ToString());
            yield break;
        }
        Debug.Log("RequestInAppReviewInfo() IsSuccessfull inAppReview");
        PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();
        yield return LaunchInAppReviewFlow(playReviewInfo);
    }

    private IEnumerator LaunchInAppReviewFlow(PlayReviewInfo playReviewInfo)
    {
        Debug.Log("LaunchInAppReviewFlow() inAppReview");
        Debug.Log("playReviewInfo: " + playReviewInfo);
        var launchOperationFlow = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchOperationFlow;
        playReviewInfo = null;
        if(launchOperationFlow.Error != ReviewErrorCode.NoError)
        {
            Debug.Log(launchOperationFlow.Error.ToString());
            yield break;
        }
    }
#endif
}