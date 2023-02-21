using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VBG.Extensions;
using Color = UnityEngine.Color;

public class Achtergrond : MonoBehaviour
{
    [SerializeField] private Sprite voorplaat;
    [SerializeField] private RectTransform achtergrondRect;
    [SerializeField] private Image achtergrondImg;
    [SerializeField] private GameObject lightObj;
    [SerializeField] private GameObject cameraObj;
    [SerializeField] private GameObject backgroundCanvasObj;
    private Color _color = Color.white;
    private int _ready;
    private bool _isPaused;
    private bool _wasPaused;
    private float _lastScreenWidth;
    private float _lastSafeZoneY;
    private float _lastSafeZoneX;
    private GegevensHouder _gegevensScript;
    [HideInInspector] public List<Color> colorList = new();
    private bool _isChangingColor;
    [HideInInspector] public bool isChangedColor;
    private SaveScript _saveScript;
    [HideInInspector] public List<TMP_Dropdown.OptionData> colorOptionData = new();
    private readonly List<int> _availableColors = new();
    [HideInInspector] public List<TMP_Dropdown.OptionData> boughtColorOptionData = new();
    [HideInInspector] public List<TMP_Dropdown.OptionData> imageOptionData = new();
    private readonly List<int> _availableImages = new();
    [HideInInspector] public List<TMP_Dropdown.OptionData> boughtImageOptionData = new();

    private void Awake()
    {
        _gegevensScript = GetComponent<GegevensHouder>();
        _saveScript = GetComponent<SaveScript>();
        bool isColor = false;
        foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
        {
            System.Drawing.Color col = System.Drawing.Color.FromKnownColor(knownColor);
            if (!isColor && col.Name.Equals("AliceBlue"))
            {
                isColor = true;
            }

            if (!isColor) continue;
            colorOptionData.Add(new TMP_Dropdown.OptionData(col.Name));
            Color color = new(col.R / 256f, col.G / 256f, col.B / 256f, col.A / 256f);
            colorList.Add(color);
            if (col.Name.Equals("YellowGreen"))
            {
                isColor = false;
            }
        }
        foreach (Sprite image in _gegevensScript.achtergronden)
        {
            imageOptionData.Add(new TMP_Dropdown.OptionData(image));
        }
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneLoaded;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(backgroundCanvasObj);
        DontDestroyOnLoad(cameraObj);
        DontDestroyOnLoad(lightObj);
        _lastScreenWidth = Screen.width;
        _lastSafeZoneY = Screen.safeArea.y;
        _lastSafeZoneX = Screen.safeArea.x;
    }

    public void StartValues()
    {
        _availableColors.Clear();
        _availableImages.Clear();
        for (int i = -1; i < 140; i++)
        {
            if (_saveScript.IntDict["kleur" + i + "gekocht"] == 1)
            {
                _availableColors.Add(i);
            }
        }
        for (int i = 0; i < 50; i++)
        {
            if (_saveScript.IntDict["afbeelding" + i + "gekocht"] == 1)
            {
                _availableImages.Add(i);
            }
        }
        _availableColors.Sort();
        _availableImages.Sort();
        boughtColorOptionData.Clear();
        boughtImageOptionData.Clear();
        for (int ii = 0; ii < _availableColors.Count; ii++)
        {
            boughtColorOptionData.Add(_availableColors[ii] == -1
                ? new TMP_Dropdown.OptionData("Changing Color")
                : colorOptionData[_availableColors[ii]]);
            if (ii < _availableImages.Count)
            {
                boughtImageOptionData.Add(imageOptionData[_availableImages[ii]]);
            }
        }
        SetBackground();
    }

    private void OnSceneLoaded(Scene scene1, Scene scene2)
    {
        SetBackground();
    }

    private void FixedUpdate()
    {
        if (!_isPaused && _wasPaused)
        {
            SetBackground();
        }
        _wasPaused = _isPaused;
        if (isChangedColor)
        {
            isChangedColor = false;
            SetBackground();
        }
        if (_isChangingColor)
        {
            SetBackground();
        }
        if (Math.Abs(_lastScreenWidth - Screen.width) < 0.0001f && Math.Abs(_lastSafeZoneY - Screen.safeArea.y) < 0.0001f && Math.Abs(_lastSafeZoneX - Screen.safeArea.x) < 0.0001f)
        {
            if (_ready >= 3) return;
            _ready += 1;
            SetBackground();
            return;
        }
        _ready = 0;
        _lastScreenWidth = Screen.width;
        _lastSafeZoneY = Screen.safeArea.y;
        _lastSafeZoneX = Screen.safeArea.x;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        _isPaused = pauseStatus;
    }

    private void SetBackground()
    {
        List<int> bgList = _gegevensScript.GetBackgroundList();
        _isChangingColor = false;
        if (bgList.Count < 2) return;
        int backgroundType = bgList[0];
        if (!achtergrondImg) return;
        switch (backgroundType)
        {
            case 0:
            {
                int colorIndex = bgList[1];
                achtergrondImg.sprite = _gegevensScript.spriteWit;
                if (colorIndex == -1)
                {
                    _isChangingColor = true;
                    ChangeColor(achtergrondImg, _color);
                }
                else
                {
                    achtergrondImg.color = colorList[colorIndex];
                }
                int width = Screen.width;
                int height = Screen.height;
                int sizeFactor = width > height ? width : height;
                achtergrondRect.sizeDelta = Vector2.one * sizeFactor;
                return;
            }
            case 1:
            {
                int afbInt = bgList[1];
                achtergrondImg.color = Color.white;
                float width = Screen.width;
                float height = Screen.height;
                Rect imgRect;
                if (afbInt == -1)
                {
                    achtergrondImg.sprite = voorplaat;
                    imgRect = achtergrondImg.sprite.rect;
                }
                else
                {
                    achtergrondImg.sprite = _gegevensScript.achtergronden[afbInt];
                    imgRect = achtergrondImg.sprite.rect;
                }
                if (imgRect.width / width < imgRect.height / height)
                {
                    height = 1000 * width;
                }
                else
                {
                    width = 1000 * height;
                }
                achtergrondRect.sizeDelta = new Vector2(width, height);
                return;
            }
        }
    }

    private void ChangeColor(Graphic backgroundImage, Color oldColor)
    {
        if (oldColor.Equals(Color.white))
        {
            float tmpR = _saveScript.FloatDict["color.r"];
            float tmpG = _saveScript.FloatDict["color.g"];
            float tmpB = _saveScript.FloatDict["color.b"];
            if (tmpR == 0 && tmpG == 0 && tmpB == 0)
            {
                oldColor = Color.red;
                oldColor.a = 1f;
            }
            else
            {
                oldColor = new Color(tmpR, tmpG, tmpB, 1f);
            }
        }
        Color nextColor = oldColor;
        if (Mathf.Approximately(nextColor.r, 1f) && !Mathf.Approximately(nextColor.g, 1f) && Mathf.Approximately(nextColor.b, 0f))
        {
            nextColor.g += 1f / 255f;
            nextColor.g = Mathf.Min(nextColor.g, 1f);
        }
        else if (nextColor.r.IsBetween(0, 1, false) && Mathf.Approximately(nextColor.g, 1f))
        {
            nextColor.r -= 1f / 255f;
            nextColor.r = Mathf.Max(nextColor.r, 0f);
        }
        else if (Mathf.Approximately(nextColor.r, 0f) && Mathf.Approximately(nextColor.g, 1f) && !Mathf.Approximately(nextColor.b, 1f))
        {
            nextColor.b += 1f / 255f;
            nextColor.b = Mathf.Min(nextColor.b, 1f);
        }
        else if (nextColor.g.IsBetween(0, 1, false) && Mathf.Approximately(nextColor.b, 1f))
        {
            nextColor.g -= 1f / 255f;
            nextColor.g = Mathf.Max(nextColor.g, 0f);
        }
        else if (Mathf.Approximately(nextColor.b, 1f) && !Mathf.Approximately(nextColor.r, 1f))
        {
            nextColor.r += 1f / 255f;
            nextColor.r = Mathf.Min(nextColor.r, 1f);
        }
        else if (nextColor.b.IsBetween(0, 1, false) && Mathf.Approximately(nextColor.r, 1f))
        {
            nextColor.b -= 1f / 255f;
            nextColor.b = Mathf.Max(nextColor.b, 0f);
        }
        backgroundImage.color = nextColor;
        _color = nextColor;
        _saveScript.FloatDict["color.r"] = nextColor.r;
        _saveScript.FloatDict["color.g"] = nextColor.g;
        _saveScript.FloatDict["color.b"] = nextColor.b;
    }

    public void ColorBought(int colorIndex)
    {
        _availableColors.Add(colorIndex);
        _availableColors.Sort();
        boughtColorOptionData.Clear();
        foreach (var color in _availableColors)
        {
            boughtColorOptionData.Add(color == -1
                ? new TMP_Dropdown.OptionData("Changing Color")
                : colorOptionData[color]);
        }
    }

    public void ImageBought(int colorIndex)
    {
        _availableImages.Add(colorIndex);
        _availableImages.Sort();
        boughtImageOptionData.Clear();
        foreach (var img in _availableImages)
        {
            boughtImageOptionData.Add(imageOptionData[img]);
        }
    }
}
