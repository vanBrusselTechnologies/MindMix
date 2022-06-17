#if UNITY_ANDROID && !UNITY_EDITOR
using System.Collections;
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class InAppUpdate : MonoBehaviour
{
    [SerializeField] private GameObject updateHandlerBannerCanvas;
    [SerializeField] private RectTransform updateHandlerBannerRect;
    [SerializeField] private Button completeUpdateButton;
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private LocalizedString finishedLocalizedString;
#if UNITY_ANDROID && !UNITY_EDITOR
    AppUpdateManager appUpdateManager;
    float downloadProgress = 0;

    private void Awake()
    {
        DontDestroyOnLoad(updateHandlerBannerCanvas);
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
            AppUpdateOptions appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions(allowAssetPackDeletion: true);
            AppUpdateRequest startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
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
                float _downloadProgress = downloadProgress;
                downloadProgress = Mathf.Floor(startUpdateRequest.DownloadProgress * 100f) / 100f;
                if (downloadProgress != _downloadProgress)
                {
                    percentageText.text = downloadProgress + "%";
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
        PlayAsyncOperation<VoidResult, AppUpdateErrorCode> result = appUpdateManager.CompleteUpdate();
        yield return result;
        Debug.Log(result.Error);
    }
#endif
}
