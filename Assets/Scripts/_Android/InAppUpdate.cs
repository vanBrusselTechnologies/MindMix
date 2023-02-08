using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections;
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

public class InAppUpdate : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    AppUpdateManager _appUpdateManager;
    float _downloadProgress;

    private void Awake()
    {
        try
        {
            _appUpdateManager = new AppUpdateManager();
            StartCoroutine(CheckForUpdates());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private IEnumerator CheckForUpdates()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            _appUpdateManager.GetAppUpdateInfo();
        yield return appUpdateInfoOperation;
        if (appUpdateInfoOperation.IsSuccessful)
        {
            AppUpdateInfo appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable) yield break;
            AppUpdateOptions appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true);
            AppUpdateRequest startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
            yield return startUpdateRequest;
            
        }
        else Debug.LogWarning(appUpdateInfoOperation.Error, this);
    }
#endif
}