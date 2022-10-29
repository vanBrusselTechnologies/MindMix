
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
#if UNITY_ANDROID && UNITY_EDITOR
using System;
using System.Collections;
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

public class InAppUpdate : MonoBehaviour
{
    [SerializeField] private GameObject updateHandlerBannerCanvas;
    [SerializeField] private RectTransform updateHandlerBannerRect;
    [SerializeField] private Button completeUpdateButton;
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private LocalizedString finishedLocalizedString;
#if UNITY_ANDROID && UNITY_EDITOR
    AppUpdateManager _appUpdateManager;
    float _downloadProgress;

    private void Awake()
    {
        try
        {
            _appUpdateManager = new AppUpdateManager();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
        return;
        DontDestroyOnLoad(updateHandlerBannerCanvas);
        
        StartCoroutine(CheckForUpdates());
    }

    private IEnumerator CheckForUpdates()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();
        yield return appUpdateInfoOperation;
        if (appUpdateInfoOperation.IsSuccessful)
        {
            AppUpdateInfo appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable) yield break;
            AppUpdateOptions appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions(allowAssetPackDeletion: true);
            AppUpdateRequest startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
            while (!startUpdateRequest.IsDone && (
                startUpdateRequest.Status != AppUpdateStatus.Downloading || 
                startUpdateRequest.Status != AppUpdateStatus.Downloaded))
            {
                if (startUpdateRequest.Status == AppUpdateStatus.Failed || startUpdateRequest.Status == AppUpdateStatus.Canceled)
                    yield break;
                yield return null;
            }
            ShowUpdateHandlerBanner();
            while (!startUpdateRequest.IsDone)
            {
                if (startUpdateRequest.Status == AppUpdateStatus.Failed)
                {
                    updateHandlerBannerCanvas.SetActive(false);
                    yield break;
                }
                float _downloadProgress = this._downloadProgress;
                this._downloadProgress = Mathf.Floor(startUpdateRequest.DownloadProgress * 100f) / 100f;
                if (this._downloadProgress != _downloadProgress)
                {
                    percentageText.text = this._downloadProgress + "%";
                }
                yield return null;
            }
            percentageText.text = finishedLocalizedString.GetLocalizedString();
            float width = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.95f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) * 0.3f);
            updateHandlerBannerRect.anchoredPosition = new Vector2(0, width / 7f);
            completeUpdateButton.gameObject.SetActive(true);
            completeUpdateButton.onClick.AddListener(OnClickCompleteUpdate);
        }
        else Debug.Log(appUpdateInfoOperation.Error);
    }
    
    private void ShowUpdateHandlerBanner()
    {
        updateHandlerBannerCanvas.SetActive(true);
        float width = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.95f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) * 0.3f);
        updateHandlerBannerRect.sizeDelta = new Vector2(width, width / 3.5f);
    }

    private void OnClickCompleteUpdate()
    {
        updateHandlerBannerCanvas.SetActive(false);
        StartCoroutine(CompleteUpdate());
    }

    private IEnumerator CompleteUpdate()
    {
        PlayAsyncOperation<VoidResult, AppUpdateErrorCode> result = _appUpdateManager.CompleteUpdate();
        yield return result;
        if (result.Error != AppUpdateErrorCode.NoError)
            Debug.Log(result.Error);
    }
#endif
}
