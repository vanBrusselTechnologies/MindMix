using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using VBG.Extensions;

public class ShareScreenshot : MonoBehaviour
{
    [SerializeField] private LocalizedString shareSubject;
    [SerializeField] private LocalizedString shareMessage;
    [SerializeField] private LocalizedString sharePlatformChooserHelpText;

    private bool isFocus;
    private string m_shareSubject, m_shareMessage, m_sharePlatformChooserHelpText;
    private bool isProcessing;
    private string screenshotName = "MindMix_highscore.png";


    void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }

    public void OnShareButtonClick()
    {
        m_shareSubject = shareSubject.GetLocalizedString();
        m_shareMessage = shareMessage.GetLocalizedString();
        m_sharePlatformChooserHelpText = sharePlatformChooserHelpText.GetLocalizedString();
        ShareScreenshotPerOperationSystem();
    }


    private void ShareScreenshotPerOperationSystem()
    {

#if UNITY_ANDROID
        if (!isProcessing)
        {
            StartCoroutine(ShareScreenshotInAndroid());
        }
#else
        Debug.Log("No sharing set up for this platform.");
#endif
    }



#if UNITY_ANDROID
    private IEnumerator ShareScreenshotInAndroid()
    {
        isProcessing = true;
        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        string screenShotPath = Application.persistentDataPath + "/" + screenshotName;
        ScreenCapture.CaptureScreenshot(screenshotName, 1);
        yield return new WaitForSeconds(0.5f);

        if (!Application.isEditor)
        {
            //current activity context
            AndroidJavaClass unity = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            //Create intent for action send
            AndroidJavaClass intentClass = new("android.content.Intent");
            AndroidJavaObject intentObject = new("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            //create file object of the screenshot captured
            AndroidJavaObject fileObject = new("java.io.File", screenShotPath);

            //create FileProvider class object
            AndroidJavaClass fileProviderClass = new("androidx.core.content.FileProvider");

            object[] providerParams = new object[3];
            providerParams[0] = currentActivity;
            providerParams[1] = "com.vanBrusselGames.MindMix.provider";
            providerParams[2] = fileObject;

            //instead of parsing the uri, will get the uri from file using FileProvider
            AndroidJavaObject uriObject = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", providerParams);

            //put image and string extra
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), m_shareSubject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), m_shareMessage);

            //additionally grant permission to read the uri
            intentObject.Call<AndroidJavaObject>("addFlags", intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));

            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, m_sharePlatformChooserHelpText);
            currentActivity.Call("startActivity", chooser);
        }

        yield return new WaitUntil(() => isFocus);
        isProcessing = false;
    }
#endif

    private IEnumerable CreateScreenshotOfObject(GameObject objToCapture, GameObject objToRenderOn)
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture(1);
        Transform tf = objToCapture.transform;
        Vector3 scale = tf.localScale;
        float scaleXInPixels = ScreenExt.UnitsToPixels(scale.x);
        float scaleYInPixels = ScreenExt.UnitsToPixels(scale.y);
        Vector3 pos = tf.position;
        float posXInPixels = ScreenExt.UnitsToPixels(pos.x + ScreenExt.PixelsToUnits(Screen.width) / 2f);
        float posYInPixels = ScreenExt.UnitsToPixels(pos.y + ScreenExt.PixelsToUnits(Screen.height) / 2f);
        Rect rect = new(posXInPixels - (scaleXInPixels / 2f), posYInPixels - (scaleYInPixels / 2f), scaleXInPixels, scaleYInPixels);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.one / 2f);
        objToRenderOn.GetComponent<Image>().sprite = sprite;
    }
}