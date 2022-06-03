#if UNITY_ANDROID && !UNITY_EDITOR
using System.Collections;
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif
using UnityEngine;

public class InAppUpdate : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    AppUpdateManager appUpdateManager;

    private void Awake()
    {
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdates());
    }

    private IEnumerator CheckForUpdates()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
        yield return appUpdateInfoOperation;
        if (appUpdateInfoOperation.IsSuccessful)
        {
            AppUpdateInfo appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable) yield break;
            AppUpdateOptions appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true);
            appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
        }
        else Debug.Log(appUpdateInfoOperation.Error);
    }
#endif
}
