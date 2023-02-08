using System;
using UnityEngine;
using VBG.Extensions;

[DefaultExecutionOrder(-50)]
public class BaseLayout : MonoBehaviour
{
    protected SaveScript saveScript;

    private Camera _camera;

    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;
    [HideInInspector] public float screenSafeAreaWidth;
    [HideInInspector] public float screenSafeAreaHeight;
    [HideInInspector] public float screenSafeAreaX;
    [HideInInspector] public float screenSafeAreaY;
    [HideInInspector] public float screenSafeAreaXRight;
    [HideInInspector] public float screenSafeAreaYUp;
    [HideInInspector] public float screenSafeAreaCenterX;
    [HideInInspector] public float screenSafeAreaCenterY;

    [HideInInspector] public float screenWidthInUnits;
    [HideInInspector] public float screenHeightInUnits;
    [HideInInspector] public float screenSafeAreaWidthInUnits;
    [HideInInspector] public float screenSafeAreaHeightInUnits;
    [HideInInspector] public float screenSafeAreaXInUnits;
    [HideInInspector] public float screenSafeAreaYInUnits;
    [HideInInspector] public float screenSafeAreaXRightInUnits;
    [HideInInspector] public float screenSafeAreaYUpInUnits;
    [HideInInspector] public float screenSafeAreaCenterXInUnits;
    [HideInInspector] public float screenSafeAreaCenterYInUnits;

    protected ScreenOrientation screenOrientation;

    private const int FramesToWaitAfterScreenRotation = 1;
    private int _framesAfterRotation;
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private float _lastSafeZoneY;
    private float _lastSafeZoneX;

    private bool _isPaused;
    private bool _wasPaused;

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
        saveScript = SaveScript.Instance;
        if (saveScript == null) return;
        _camera = Camera.main;
        SetScreenValues();
        SetLayout();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (screenWidth == 0) return;
        if (!_isPaused && _wasPaused)
        {
            SetScreenValues();
            SetLayout();
        }
        _wasPaused = _isPaused;
        if (_lastScreenWidth == Screen.width && _lastScreenHeight == Screen.height && Math.Abs(_lastSafeZoneY - Screen.safeArea.y) < 0.0001f && Math.Abs(_lastSafeZoneX - Screen.safeArea.x) < 0.0001f)
        {
            if (_framesAfterRotation < FramesToWaitAfterScreenRotation)
            {
                _framesAfterRotation += 1;
                if (_framesAfterRotation == FramesToWaitAfterScreenRotation)
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
        _lastSafeZoneY = Screen.safeArea.y;
        _lastSafeZoneX = Screen.safeArea.x;
    }

    private void SetScreenValues()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Rect safeArea = Screen.safeArea;
        screenSafeAreaWidth = safeArea.width;
        screenSafeAreaHeight = safeArea.height;
        screenSafeAreaX = safeArea.x;
        screenSafeAreaY = safeArea.y;
        screenSafeAreaXRight = screenWidth - screenSafeAreaWidth - screenSafeAreaX;
        screenSafeAreaYUp = screenHeight - screenSafeAreaHeight - screenSafeAreaY;
        screenSafeAreaCenterX = (screenSafeAreaX - screenSafeAreaXRight) / 2f;
        screenSafeAreaCenterY = (screenSafeAreaY - screenSafeAreaYUp) / 2f;

        screenWidthInUnits = ScreenExt.PixelsToUnits(screenWidth, _camera);
        screenHeightInUnits = ScreenExt.PixelsToUnits(screenHeight, _camera);
        screenSafeAreaWidthInUnits = ScreenExt.PixelsToUnits(screenSafeAreaWidth, _camera);
        screenSafeAreaHeightInUnits = ScreenExt.PixelsToUnits(screenSafeAreaHeight, _camera);
        screenSafeAreaXInUnits = ScreenExt.PixelsToUnits(screenSafeAreaX, _camera);
        screenSafeAreaYInUnits = ScreenExt.PixelsToUnits(screenSafeAreaY, _camera);
        screenSafeAreaXRightInUnits = ScreenExt.PixelsToUnits(screenSafeAreaXRight, _camera);
        screenSafeAreaYUpInUnits = ScreenExt.PixelsToUnits(screenSafeAreaYUp, _camera);
        screenSafeAreaCenterXInUnits = (screenSafeAreaXInUnits - screenSafeAreaXRightInUnits) / 2f;
        screenSafeAreaCenterYInUnits = (screenSafeAreaYInUnits - screenSafeAreaYUpInUnits) / 2f;

        screenOrientation = Screen.orientation;

    }

    public virtual void SetLayout()
    {
        if (screenWidth == 0) SetScreenValues();
        if (!finishedGameUIObj.Equals(null) && finishedGameUIObj.activeInHierarchy)
        {
            SetLayoutFinishedGameUI();
            return;
        }

        if (!helpUIObj.Equals(null) && helpUIObj.activeInHierarchy)
        {
            SetLayoutHelpUI();
            return;
        }

        if (!settingsUIObj.Equals(null) && settingsUIObj.activeInHierarchy)
        {
            SetLayoutSettingsUI();
            return;
        }
        SetLayoutBasic();
    }

    protected virtual void SetLayoutHelpUI()
    {
        helpUITitleRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + screenSafeAreaHeight * (1f / 3f));
        helpUITitleRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.75f, screenSafeAreaHeight);
        helpUITextRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY - screenSafeAreaHeight / 8f);
        helpUITextRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight / 2);
        helpUICloseButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f), -screenSafeAreaYUp - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f));
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
        settingsUICloseButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f), -screenSafeAreaYUp - (Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f * 0.6f));
        settingsUICloseButtonRect.localScale = new Vector2(Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f, Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth) * 0.1f) / 108f;
        Vector3 scrollDownScale = new(screenSafeAreaWidth * 0.98f / 2250f, screenSafeAreaHeight * 0.85f / 950f, 1);
        settingsUIScrolldownRect.localScale = scrollDownScale;
        float minScaleValue = Mathf.Min(scrollDownScale.x, scrollDownScale.y);
        Vector3 scrollDownContentScale = new(minScaleValue / scrollDownScale.x, minScaleValue / scrollDownScale.y, 1);
        settingsUIScrolldownContentRect.localScale = scrollDownContentScale;
        Vector3 scrollDownPosition = new(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * 0.15f / -2f), 0);
        settingsUIScrolldownRect.anchoredPosition = scrollDownPosition;
    }

    protected virtual void SetLayoutFinishedGameUI()
    {
        finishedGameUITitleRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * (25f / 30f)) - (screenHeight / 2f));
        finishedGameUITitleRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (10f / 30f));
        float smallestSide = Mathf.Min(screenSafeAreaHeight, screenSafeAreaWidth);
        float biggestSide = Mathf.Max(screenSafeAreaHeight, screenSafeAreaWidth);
        if (smallestSide - 1440 > 0)
        {
            float factor = Mathf.Min(smallestSide / 1500f, biggestSide / 2500f);
            finishedGameUITitleRect.localScale = Vector2.one * factor;
            finishedGameUITitleRect.sizeDelta /= factor;
        }
        finishedGameUITextRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * (17f / 30f)) - (screenHeight / 2f));
        finishedGameUITextRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.85f, screenSafeAreaHeight * (8f / 30f));
        if (finishedGameUIRewardRect != null)
        {
            finishedGameUIRewardRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, screenSafeAreaCenterY + (screenSafeAreaHeight * (10f / 30f)) - (screenHeight / 2f));
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
        }
        Vector2 size = new(screenSafeAreaWidth * 0.45f, screenSafeAreaHeight * (5f / 30f));
        float posY = screenSafeAreaCenterY + screenSafeAreaYUp + (screenSafeAreaHeight * (3.5f / 30f)) - (screenHeight / 2f);
        if (finishedGameUINewMoreDifficultGameButtonRect == null)
        {
            finishedGameUINewGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX - (screenSafeAreaWidth / 4f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = size;
            finishedGameUIToMenuButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX + (screenSafeAreaWidth / 4f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = size;
        }
        else
        {
            size.x = screenSafeAreaWidth * 0.3f;
            finishedGameUINewGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX - (screenSafeAreaWidth / 3f), posY);
            finishedGameUINewGameButtonRect.sizeDelta = size;
            finishedGameUINewMoreDifficultGameButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX, posY);
            finishedGameUINewMoreDifficultGameButtonRect.sizeDelta = size;
            finishedGameUIToMenuButtonRect.anchoredPosition = new Vector2(screenSafeAreaCenterX + (screenSafeAreaWidth / 3f), posY);
            finishedGameUIToMenuButtonRect.sizeDelta = size;
        }
    }

    protected virtual void SetLayoutBasic()
    {
        float size = Mathf.Min(Mathf.Max(screenSafeAreaWidth, screenSafeAreaHeight) / 12f, Mathf.Min(screenSafeAreaWidth, screenSafeAreaHeight) / 10f);
        if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            menuUIObj.SetActive(true);
            backToMenuButtonRect.transform.SetParent(backToMenuButtonCanvasTransform);
            backToMenuButtonRect.sizeDelta = Vector2.one * size;
            backToMenuButtonRect.anchoredPosition = new Vector2((size * 0.6f) - (screenSafeAreaWidth / 2f), (screenHeight / 2f) - screenSafeAreaYUp - (size * 0.6f));
            settingsUIOpenButtonRect.anchoredPosition = new Vector2(-size * 0.6f, -screenSafeAreaYUp - (size * 0.6f) - size);
            menuUIOpenButtonRect.anchoredPosition = new Vector2(0, screenSafeAreaY + menuUIOpenButtonRect.sizeDelta.y / 2f);
            menuUIOpenButtonRect.sizeDelta = new Vector2(screenSafeAreaHeight * 0.1f, screenSafeAreaHeight * 0.04f);
            menuUIOpenButtonRect.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            backToMenuButtonRect.anchoredPosition = new Vector2(10000000, 1000000);
            settingsUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f) - size, -screenSafeAreaYUp - (size * 0.6f));
            menuUIOpenButtonRect.anchoredPosition = new Vector2(screenSafeAreaX - (screenWidth / 2f) + menuUIOpenButtonRect.sizeDelta.y / 2f, screenHeight / 2f);
            menuUIOpenButtonRect.sizeDelta = new Vector2(screenSafeAreaWidth * 0.1f, screenSafeAreaWidth * 0.04f);
            menuUIOpenButtonRect.localEulerAngles = new Vector3(0, 0, 90);
        }
        menuUIObj.SetActive(false);
        settingsUIOpenButtonRect.sizeDelta = Vector2.one * size;
        helpUIOpenButtonRect.sizeDelta = Vector2.one * size;
        helpUIOpenButtonRect.anchoredPosition = new Vector2(-screenSafeAreaXRight - (size * 0.6f), -screenSafeAreaYUp - (size * 0.6f));
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _isPaused = !hasFocus;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        _isPaused = pauseStatus;
    }
}