using UnityEngine;
using VBG.Extensions;

[DefaultExecutionOrder(-50)]
public abstract class BaseLayout : MonoBehaviour
{
    protected SaveScript saveScript;

    protected float screenWidth = 0;
    protected float screenHeight;
    protected float screenSafeAreaWidth;
    protected float screenSafeAreaHeight;
    protected float screenSafeAreaX;
    protected float screenSafeAreaY;
    protected float screenSafeAreaXRight;
    protected float screenSafeAreaYUp;

    protected float screenWidthInUnits;
    protected float screenHeightInUnits;
    protected float screenSafeAreaWidthInUnits;
    protected float screenSafeAreaHeightInUnits;
    protected float screenSafeAreaXInUnits;
    protected float screenSafeAreaYInUnits;
    protected float screenSafeAreaXRightInUnits;
    protected float screenSafeAreaYUpInUnits;

    protected ScreenOrientation screenOrientation;

    private int _framesToWaitAfterScreenRotation = 1;
    private int _framesAfterRotation;
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private float _lastSafezoneY;
    private float _lastSafezoneX;

    private bool isPaused;
    private bool wasPaused;

    [Header("Help UI")]
    [SerializeField] protected RectTransform helpUIOpenButtonRect;
    [SerializeField] protected GameObject helpUIObj;
    [SerializeField] protected RectTransform helpUITitleRect;
    [SerializeField] protected RectTransform helpUITextRect;
    [SerializeField] protected RectTransform helpUICloseButtonRect;

    [Header("Settings UI")]
    [SerializeField] protected RectTransform settingsUIOpenButtonRect;
    [SerializeField] protected GameObject settingsUIObj;
    [SerializeField] protected RectTransform settingsUIScrolldownRect;
    [SerializeField] protected RectTransform settingsUIScrolldownContentRect;
    [SerializeField] protected RectTransform settingsUICloseButtonRect;

    [Header("Finished game UI")]
    [SerializeField] protected GameObject finishedGameUIObj;
    [SerializeField] protected RectTransform finishedGameUITitleRect;
    [SerializeField] protected RectTransform finishedGameUITextRect;
    [SerializeField] protected RectTransform finishedGameUIRewardRect;
    [SerializeField] protected GameObject finishedGameUIRewardDoubleButtonObj;
    [SerializeField] protected RectTransform finishedGameUINewGameButtonRect;
    [SerializeField] protected RectTransform finishedGameUINewMoreDifficultGameButtonRect;
    [SerializeField] protected RectTransform finishedGameUIToMenuButtonRect;

    [Header("Menu UI")]
    [SerializeField] protected GameObject menuUIObj;
    [SerializeField] protected RectTransform menuUIOpenButtonRect;
    [SerializeField] protected RectTransform backToMenuButtonRect;
    [SerializeField] protected Transform backToMenuButtonCanvasTransform;

    protected virtual void Start()
    {
        saveScript = SaveScript.instance;
        if (saveScript == null)
        {
            return;
        }
        SetScreenValues();
        SetLayout();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (screenWidth == 0) return;
        if (!isPaused && wasPaused)
        {
            SetScreenValues();
            SetLayout();
        }
        wasPaused = isPaused;
        if (_lastScreenWidth == Screen.width && _lastScreenHeight == Screen.height && _lastSafezoneY == Screen.safeArea.y && _lastSafezoneX == Screen.safeArea.x)
        {
            if (_framesAfterRotation < _framesToWaitAfterScreenRotation)
            {
                _framesAfterRotation += 1;
                if (_framesAfterRotation == _framesToWaitAfterScreenRotation)
                {
                    SetScreenValues();
                    SetLayout();
                    return;
                }
            }
            return;
        }
        _framesAfterRotation = 0;
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;
        _lastSafezoneY = Screen.safeArea.y;
        _lastSafezoneX = Screen.safeArea.x;
    }

    void SetScreenValues()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Rect _safeArea = Screen.safeArea;
        screenSafeAreaWidth = _safeArea.width;
        screenSafeAreaHeight = _safeArea.height;
        screenSafeAreaX = _safeArea.x;
        screenSafeAreaY = _safeArea.y;
        screenSafeAreaXRight = screenWidth - screenSafeAreaWidth - screenSafeAreaX;
        screenSafeAreaYUp = screenHeight - screenSafeAreaHeight - screenSafeAreaY;

        screenWidthInUnits = ScreenExt.PixelsToUnits(screenWidth);
        screenHeightInUnits = ScreenExt.PixelsToUnits(screenHeight);
        screenSafeAreaWidthInUnits = ScreenExt.PixelsToUnits(screenSafeAreaWidth);
        screenSafeAreaHeightInUnits = ScreenExt.PixelsToUnits(screenSafeAreaHeight);
        screenSafeAreaXInUnits = ScreenExt.PixelsToUnits(screenSafeAreaX);
        screenSafeAreaYInUnits = ScreenExt.PixelsToUnits(screenSafeAreaY);
        screenSafeAreaXRightInUnits = ScreenExt.PixelsToUnits(screenSafeAreaXRight);
        screenSafeAreaYUpInUnits = ScreenExt.PixelsToUnits(screenSafeAreaYUp);

        screenOrientation = Screen.orientation;
    }

    public virtual void SetLayout()
    {
        if (screenWidth == 0) SetScreenValues();
        if (finishedGameUIObj != null && finishedGameUIObj.activeInHierarchy)
        {
            SetLayoutFinishedGameUI();
            return;
        }
        else if (helpUIObj != null && helpUIObj.activeInHierarchy)
        {
            SetLayoutHelpUI();
            return;
        }
        else if (settingsUIObj != null && settingsUIObj.activeInHierarchy)
        {
            SetLayoutSettingsUI();
            return;
        }
        SetLayoutBasic();
    }

    protected virtual void SetLayoutHelpUI()
    {
        helpUITitleRect.anchoredPosition = new Vector2(0, screenSafeAreaHeight * (1f / 3f));
        helpUITitleRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.75f, screenSafeAreaHeight);
        helpUITextRect.anchoredPosition = new Vector2(0, -screenSafeAreaHeight / 8f);
        helpUITextRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight / 2);
        helpUICloseButtonRect.anchoredPosition = new Vector2(-(screenWidth - screenSafeAreaWidth - screenSafeAreaX) - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f), -(screenHeight - screenSafeAreaHeight - screenSafeAreaY) - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f));
        helpUICloseButtonRect.localScale = new Vector2(Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f, Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f) / 108f;
        float smallestSide = Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth);
        float biggestSide = Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth);
        if (smallestSide - 1440 > 0)
        {
            float factor = Mathf.Min(smallestSide / 1500f, biggestSide / 2500f);
            helpUITitleRect.localScale = Vector2.one * factor;
            helpUITitleRect.sizeDelta /= factor;
        }
    }

    protected virtual void SetLayoutSettingsUI()
    {
        settingsUICloseButtonRect.anchoredPosition = new Vector2(-(screenWidth - screenSafeAreaWidth - screenSafeAreaX) - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f), -(screenHeight - screenSafeAreaHeight - screenSafeAreaY) - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f));
        settingsUICloseButtonRect.localScale = new Vector2(Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f, Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f) / 108f;
        Vector3 scrollDownScale = new Vector3(screenSafeAreaWidth * 0.98f / 2250f, screenSafeAreaHeight * 0.85f / 950f, 1);
        settingsUIScrolldownRect.localScale = scrollDownScale;
        float minScaleDeel = Mathf.Min(scrollDownScale.x, scrollDownScale.y);
        Vector3 scrollDownContentScale = new Vector3(minScaleDeel / scrollDownScale.x, minScaleDeel / scrollDownScale.y, 1);
        settingsUIScrolldownContentRect.localScale = scrollDownContentScale;
        Vector3 scrollDownPosition = new Vector3((screenSafeAreaX - screenSafeAreaXRight) / 2f, (screenSafeAreaY - screenSafeAreaYUp) / 2f + (screenSafeAreaHeight * 0.15f / -2f), 0);
        settingsUIScrolldownRect.anchoredPosition = scrollDownPosition;
    }

    protected virtual void SetLayoutFinishedGameUI()
    {
        finishedGameUITitleRect.anchoredPosition = new Vector2(screenSafeAreaXRight, screenSafeAreaYUp + (screenSafeAreaHeight * (25f / 30f)) - (screenHeight / 2f));
        finishedGameUITitleRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (10f / 30f));
        float smallestSide = Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth);
        float biggestSide = Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth);
        if (smallestSide - 1440 > 0)
        {
            float factor = Mathf.Min(smallestSide / 1500f, biggestSide / 2500f);
            finishedGameUITitleRect.localScale = Vector2.one * factor;
            finishedGameUITitleRect.sizeDelta /= factor;
        }
        finishedGameUITextRect.anchoredPosition = new Vector2(screenSafeAreaXRight, screenSafeAreaYUp + (screenSafeAreaHeight * (17f / 30f)) - (screenHeight / 2f));
        finishedGameUITextRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (8f / 30f));
        finishedGameUIRewardRect.anchoredPosition = new Vector2(screenSafeAreaXRight, screenSafeAreaYUp + (screenSafeAreaHeight * (10f / 30f)) - (screenHeight / 2f));
        if (Application.internetReachability == NetworkReachability.NotReachable || !finishedGameUIRewardDoubleButtonObj.activeInHierarchy)
        {
            finishedGameUIRewardDoubleButtonObj.SetActive(false);
            float scaleFactor = Mathf.Min(screenSafeAreaWidth * 0.85f / 500, screenSafeAreaHeight * (5f / 30f) / 175);
            finishedGameUIRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            finishedGameUIRewardRect.sizeDelta = new Vector2(500, 175);
        }
        else
        {
            float scaleFactor = Mathf.Min(screenSafeAreaWidth * 0.85f / 1000, screenSafeAreaHeight * (5f / 30f) / 175);
            finishedGameUIRewardRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            finishedGameUIRewardRect.sizeDelta = new Vector2(1000, 175);
        }
        float sizeX = screenSafeAreaWidth * 0.45f;
        float sizeY = screenSafeAreaHeight * (5f / 30f);
        float posY = screenSafeAreaYUp + (screenSafeAreaHeight * (3.5f / 30f)) - (screenHeight / 2f);
        if (finishedGameUINewMoreDifficultGameButtonRect == null)
        {
            finishedGameUINewGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaXRight - (screenSafeAreaWidth / 4f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = new Vector2(sizeX, sizeY);
            finishedGameUIToMenuButtonRect.anchoredPosition = new Vector2(screenSafeAreaXRight + (screenSafeAreaWidth / 4f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = new Vector2(sizeX, sizeY);
        }
        else
        {
            sizeX = screenSafeAreaWidth * 0.3f;
            finishedGameUINewGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaXRight - (screenSafeAreaWidth / 3f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = new Vector2(sizeX, sizeY);
            finishedGameUINewMoreDifficultGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaXRight, posY);
            finishedGameUINewMoreDifficultGameButtonRect.sizeDelta = new Vector2(sizeX, sizeY);
            finishedGameUIToMenuButtonRect.anchoredPosition = new Vector2(screenSafeAreaXRight + (screenSafeAreaWidth / 3f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }

    protected virtual void SetLayoutBasic()
    {
        if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            menuUIObj.SetActive(true);
            backToMenuButtonRect.transform.SetParent(backToMenuButtonCanvasTransform);
            backToMenuButtonRect.sizeDelta = new Vector2(screenSafeAreaWidth / 11, screenSafeAreaWidth / 11);
            backToMenuButtonRect.anchoredPosition = new Vector2((-screenSafeAreaWidth / 2) + (screenSafeAreaWidth / 11 * 0.6f), (screenHeight / 2) - (screenHeight - screenSafeAreaY - screenSafeAreaHeight) - (screenSafeAreaWidth / 11 * 0.6f));
            menuUIObj.SetActive(false);
            helpUIOpenButtonRect.sizeDelta = new Vector2(screenSafeAreaWidth / 12, screenSafeAreaWidth / 12);
            helpUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaWidth / 12 * 0.6f, -(screenHeight - screenSafeAreaHeight - screenSafeAreaY) - (screenSafeAreaWidth / 12 * 0.6f));
            settingsUIOpenButtonRect.sizeDelta = helpUIOpenButtonRect.sizeDelta;
            settingsUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaWidth / 12 * 0.6f, -(screenHeight - screenSafeAreaHeight - screenSafeAreaY) - (screenSafeAreaWidth / 12 * 0.6f) - (2f * screenSafeAreaWidth / 12f * 0.6f));
            menuUIOpenButtonRect.anchoredPosition = new Vector2(0, screenSafeAreaY + menuUIOpenButtonRect.sizeDelta.y / 2f);
            menuUIOpenButtonRect.sizeDelta = new Vector2(screenSafeAreaHeight * 0.1f, screenSafeAreaHeight * 0.04f);
            menuUIOpenButtonRect.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            backToMenuButtonRect.anchoredPosition = new Vector2(10000000, 1000000);
            menuUIObj.SetActive(false);
            helpUIOpenButtonRect.sizeDelta = new Vector2(screenSafeAreaHeight / 12, screenSafeAreaHeight / 12);
            helpUIOpenButtonRect.anchoredPosition = new Vector2(-(screenWidth - screenSafeAreaWidth - screenSafeAreaX) - (screenSafeAreaHeight / 12 * 0.6f), -(screenHeight - screenSafeAreaHeight - screenSafeAreaY) - (screenSafeAreaHeight / 12 * 0.6f));
            settingsUIOpenButtonRect.sizeDelta = helpUIOpenButtonRect.sizeDelta;
            settingsUIOpenButtonRect.anchoredPosition = new Vector2(-(screenWidth - screenSafeAreaWidth - screenSafeAreaX) - (screenSafeAreaHeight / 12 * 0.6f) - (2f * screenSafeAreaHeight / 12f * 0.6f), -(screenHeight - screenSafeAreaHeight - screenSafeAreaY) - (screenSafeAreaHeight / 12 * 0.6f));
            menuUIOpenButtonRect.anchoredPosition = new Vector2(screenSafeAreaX - (screenWidth / 2f) + menuUIOpenButtonRect.sizeDelta.y / 2f, screenHeight / 2f);
            menuUIOpenButtonRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.1f, screenSafeAreaWidth * 0.04f);
            menuUIOpenButtonRect.localEulerAngles = new Vector3(0, 0, 90);
        }
    }


    private void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }
}