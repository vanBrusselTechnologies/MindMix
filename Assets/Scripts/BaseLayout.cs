using UnityEngine;
using UnityEngine.Events;
using VBG.Extensions;

[DefaultExecutionOrder(-50)]
public class BaseLayout : MonoBehaviour
{
    public UnityEvent ScreenRotated;
    public float ScreenWidthInUnits { get { if (_screenWidth == 0) { SetScreenValues(); return _screenWidth; } else { return _screenWidth; } } }
    private float _screenWidth;
    public float ScreenHeightInUnits { get { if (_screenHeight == 0) { SetScreenValues(); return _screenHeight; } else { return _screenHeight; } } }
    private float _screenHeight;
    public float ScreenSafeAreaWidthInUnits { get { if (_screenSafeAreaWidth == 0) { SetScreenValues(); return _screenSafeAreaWidth; } else { return _screenSafeAreaWidth; } } }
    private float _screenSafeAreaWidth;
    public float ScreenSafeAreaHeightInUnits { get { if (_screenSafeAreaHeight == 0) { SetScreenValues(); return _screenSafeAreaHeight; } else { return _screenSafeAreaHeight; } } }
    private float _screenSafeAreaHeight;

    // Start is called before the first frame update
    void Awake()
    {
        if (ScreenRotated == null)
            ScreenRotated = new UnityEvent();
        else
            ScreenRotated.RemoveAllListeners();
    }

    private int _framesToWaitAfterScreenRotation = 3;
    private int _framesAfterRotation;
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private float _lastSafezoneY;
    private float _lastSafezoneX;

    // Update is called once per frame
    void Update()
    {
        if (_lastScreenWidth == Screen.width && _lastScreenHeight == Screen.height && _lastSafezoneY == Screen.safeArea.y && _lastSafezoneX == Screen.safeArea.x)
        {
            if (_framesAfterRotation < _framesToWaitAfterScreenRotation)
            {
                _framesAfterRotation += 1;
                if (_framesAfterRotation == _framesToWaitAfterScreenRotation)
                {
                    SetScreenValues();
                    ScreenRotated.Invoke();
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
        _screenWidth = ScreenExt.WidthInUnits;
        _screenHeight = ScreenExt.HeightInUnits;
        _screenSafeAreaWidth = ScreenExt.SafeAreaWidthInUnits;
        _screenSafeAreaHeight = ScreenExt.SafeAreaHeightInUnits;
    }
}
