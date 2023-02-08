using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
using System.Collections;
#endif

public class InAppReview : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private ReviewManager _reviewManager;

    private void Awake()
    {
        _reviewManager = new ReviewManager();
    }

    public void RequestInAppReview()
    {
        StartCoroutine(RequestInAppReviewInfo());
    }

    private IEnumerator RequestInAppReviewInfo()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
        PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();
        yield return LaunchInAppReviewFlow(playReviewInfo);
    }

    private IEnumerator LaunchInAppReviewFlow(PlayReviewInfo playReviewInfo)
    {
        var launchOperationFlow = _reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchOperationFlow;
        playReviewInfo = null;
        if(launchOperationFlow.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
#endif
}