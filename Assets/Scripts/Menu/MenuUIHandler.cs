using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuUIHandler : BaseUIHandler
{
    private SceneManager _sceneManager;
    
    [Header("Other scene specific")] [SerializeField]
    private RectTransform gameWheel;

    [SerializeField] private RectTransform gameModeWheel;
    [SerializeField] private Collider gameWheelCollider;

    private RectTransform _rotatingWheelRect;
    private float _touchMaxTotalDeltaPositionX;
    private bool _swipeOnButton;
    private Coroutine _coroutine;
    private Vector2 _startTouchPosition;
    private readonly List<TMP_Text> _gameWheelNames = new();
    private string _currentSelectedGame = "";
    private bool _startSetup = true;
    private Camera _camera;

    private void Awake() => _camera = Camera.main;

    protected override void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        base.Start();
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }

        baseLayout = GetComponent<MenuLayout>();
        _sceneManager = GetComponent<SceneManager>();
        foreach (Transform child in gameWheel)
        {
            _gameWheelNames.Add(child.GetChild(1).GetComponent<TMP_Text>());
        }

        gameWheel.localEulerAngles =
            new Vector3(0, 0, 360f / gameWheel.childCount * gegevensHouder.currentSelectedGameWheelIndex);

        ChangeWheelItemSizes(gameWheel, true);
        ChangeWheelItemSizes(gameModeWheel, false);
        Transform selectedGameTransform = gameWheel.GetChild(gegevensHouder.currentSelectedGameWheelIndex);
        _currentSelectedGame = selectedGameTransform.name.Split("(")[0];
        if (selectedGameTransform.GetChild(2).childCount == 0)
            selectedGameTransform.GetChild(1).GetComponent<TMP_Text>().alpha = 1;
        _startSetup = false;
    }

    protected override void Update()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _swipeOnButton = EventSystem.current.currentSelectedGameObject != null;
                _startTouchPosition = touch.position;
                Ray ray = _camera.ScreenPointToRay(_startTouchPosition);
                if (gameWheelCollider.Raycast(ray, out RaycastHit _, Mathf.Infinity) ||
                    gameWheel.GetChild(gegevensHouder.currentSelectedGameWheelIndex).GetChild(2).childCount == 0)
                {
                    _rotatingWheelRect = gameWheel;
                }
                else
                {
                    _rotatingWheelRect = gameModeWheel;
                    if (gameWheel.GetChild(gegevensHouder.currentSelectedGameWheelIndex).GetChild(2).childCount == 0)
                    {
                        _swipeOnButton = true;
                    }
                }

                _touchMaxTotalDeltaPositionX = 0;
                break;
            case TouchPhase.Moved:
            {
                if (_swipeOnButton) break;
                bool hadMovedEnough = _touchMaxTotalDeltaPositionX >= baseLayout.screenWidth * 0.01f;
                _touchMaxTotalDeltaPositionX = Mathf.Max(Mathf.Abs(touch.position.x - _startTouchPosition.x),
                    _touchMaxTotalDeltaPositionX);
                if (_touchMaxTotalDeltaPositionX < baseLayout.screenWidth * 0.01f) break;
                bool isInnerWheel = String.Equals(_rotatingWheelRect.name, gameWheel.name);
                if (!hadMovedEnough && isInnerWheel) StartCoroutine(HideSecondWheel(_rotatingWheelRect));
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                float degreesRotationZ = touch.deltaPosition.x /
                    Mathf.Min(baseLayout.screenWidth, baseLayout.screenHeight) * 180f;
                _rotatingWheelRect.localEulerAngles =
                    new Vector3(0, 0, _rotatingWheelRect.localEulerAngles.z - degreesRotationZ);
                ChangeWheelItemSizes(_rotatingWheelRect, isInnerWheel);
                break;
            }
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
            {
                if (_touchMaxTotalDeltaPositionX < baseLayout.screenWidth * 0.01f) return;
                int gameCount = _rotatingWheelRect.childCount;
                int origGameCount = 0;
                foreach (Transform child in _rotatingWheelRect)
                    if (!child.name.EndsWith("(Clone)"))
                        origGameCount += 1;
                float startRotation = _rotatingWheelRect.localEulerAngles.z;
                float rotationDivPerChild = 360f / gameCount;
                float endRotation = Mathf.Round((startRotation % 360f) / rotationDivPerChild) * rotationDivPerChild;
                int selectedGameIndex = Mathf.RoundToInt((endRotation / rotationDivPerChild) % origGameCount);
                gegevensHouder.currentSelectedGameWheelIndex = selectedGameIndex;
                _currentSelectedGame = _rotatingWheelRect.GetChild(selectedGameIndex).name.Split("(")[0];
                Transform secondWheel =
                    String.Equals(_rotatingWheelRect.name, gameWheel.name) ? gameModeWheel : gameWheel;
                List<TMP_Text> wheelNames =
                    String.Equals(_rotatingWheelRect.name, gameWheel.name) ? _gameWheelNames : null;
                float startAlpha = wheelNames == null
                    ? _rotatingWheelRect.GetChild(selectedGameIndex).GetChild(1).GetComponent<TMP_Text>().alpha
                    : wheelNames[selectedGameIndex].alpha;
                float startLocalScale = secondWheel.localScale.y;
                _coroutine = StartCoroutine(TurnWheelToEndPosition(_rotatingWheelRect, selectedGameIndex, startRotation,
                    endRotation, startAlpha, startLocalScale));
                _swipeOnButton = false;
                break;
            }
            case TouchPhase.Stationary:
            default: break;
        }
    }

    private IEnumerator TurnWheelToEndPosition(Transform wheel, int gameIndex, float startRotation, float endRotation,
        float startAlpha, float startLocalScale, float t = 0)
    {
        bool isInnerWheel = String.Equals(wheel.name, gameWheel.name);
        float time = Mathf.Min(t * 4f, 1f);
        float zRotation = Mathf.Lerp(startRotation, endRotation, time);
        wheel.localEulerAngles = new Vector3(0, 0, zRotation);
        ChangeWheelItemSizes(wheel, isInnerWheel);
        if (isInnerWheel)
        {
            if (wheel.GetChild(gameIndex).GetChild(2).childCount == 0)
                _gameWheelNames[gameIndex].alpha = Mathf.Lerp(startAlpha, 1, time);
            else
                ShowGameModeWheel(gameIndex, startAlpha, startLocalScale, time);
        }
        //else ; //Hide GameModeNames();

        if (time >= 1f) yield break;
        yield return new WaitForFixedUpdate();
        yield return TurnWheelToEndPosition(wheel, gameIndex, startRotation, endRotation, startAlpha, startLocalScale,
            t + Time.fixedDeltaTime);
    }

    private void ChangeWheelItemSizes(Transform wheel, bool isGameWheel)
    {
        List<TMP_Text> wheelNames = isGameWheel ? _gameWheelNames : null;

        int childCount = wheel.childCount;
        if (childCount <= 1) return;
        float rotationDivPerChild = 360f / childCount;
        float zRotation = Mathf.Round(wheel.localEulerAngles.z * 1000f) / 1000f;

        int ceilChild = Mathf.CeilToInt(zRotation / rotationDivPerChild) % childCount;
        int floorChild = Mathf.FloorToInt(zRotation / rotationDivPerChild) % childCount;
        float partitionChildren = (zRotation % rotationDivPerChild) / rotationDivPerChild;
        for (int childIndex = 0; childIndex < childCount; childIndex++)
        {
            Transform child = wheel.GetChild(childIndex);
            if (ceilChild == floorChild)
            {
                child.localScale = Vector3.one * (floorChild == childIndex ? 1f : 0.5f);
                if (_startSetup) continue;
                if (wheelNames != null)
                    wheelNames[childIndex].alpha = floorChild == childIndex ? 1f : 0f;
            }
            else
            {
                if (floorChild == childIndex || ceilChild == childIndex)
                {
                    child.localScale = Vector3.one *
                                       (Mathf.Lerp(0.5f, 1f,
                                           floorChild == childIndex ? 1 - partitionChildren : partitionChildren));
                    if (wheelNames != null)
                    {
                        wheelNames[childIndex].alpha = Mathf.Lerp(0, 1f,
                            floorChild == childIndex
                                ? Mathf.Pow(1 - partitionChildren, 2)
                                : Mathf.Pow(partitionChildren, 2));
                    }
                }
                else if (childIndex == (floorChild - 1) % childCount || childIndex == (ceilChild + 1) % childCount)
                {
                    child.localScale = Vector3.one * 0.5f;
                    if (wheelNames != null)
                    {
                        wheelNames[childIndex].alpha = 0;
                    }
                }
            }
        }
    }

    private void ShowGameModeWheel(int gameIndex, float startAlpha = -1f, float startLocalScale = -1f, float t = 0f)
    {
        _gameWheelNames[gameIndex].alpha = Mathf.Lerp(startAlpha, 0, t);

        float maxLocalScale = Mathf.Min(baseLayout.screenSafeAreaHeightInUnits / 450f,
            baseLayout.screenSafeAreaWidthInUnits / 250f);
        gameModeWheel.localScale = Vector3.one * (Mathf.Lerp(startLocalScale, maxLocalScale, t) * 0.75f);
    }

    private IEnumerator HideSecondWheel(Transform wheel, float startAlpha = -1f, float startLocalScale = -1f,
        float t = 0f)
    {
        Transform secondWheel = String.Equals(wheel.name, gameWheel.name) ? gameModeWheel : gameWheel;
        List<TMP_Text> wheelNames = String.Equals(wheel.name, gameWheel.name) ? _gameWheelNames : null;
        int childCount = wheel.childCount;
        if (childCount <= 1) yield break;
        float rotationDivPerChild = 360f / childCount;
        float zRotation = Mathf.Round(wheel.localEulerAngles.z * 1000f) / 1000f;

        int childIndex = Mathf.RoundToInt(zRotation / rotationDivPerChild) % childCount;
        if (Mathf.Approximately(startAlpha, -1f))
        {
            startAlpha = wheelNames == null
                ? wheel.GetChild(childIndex).GetChild(1).GetComponent<TMP_Text>().alpha
                : wheelNames[childIndex].alpha;
        }

        if (Mathf.Approximately(startLocalScale, -1f)) startLocalScale = secondWheel.localScale.y;
        float time = Mathf.Min(t / 0.25f, 1f);
        if (wheelNames == null)
            ; //wheel.GetChild(childIndex).GetChild(1).GetComponent<TMP_Text>().alpha = Mathf.Lerp(startAlpha, 1, time);
        else
            wheelNames[childIndex].alpha = Mathf.Lerp(startAlpha, 1, time);
        secondWheel.localScale = Vector3.one * Mathf.Lerp(startLocalScale, 0, time);
        if (time >= 1f)
            yield break;
        yield return new WaitForFixedUpdate();
        yield return HideSecondWheel(wheel, startAlpha, startLocalScale, t + Time.fixedDeltaTime);
    }

    public override void BackToMenu()
    {
        SceneManager.LoadScene("inlogEnVoorplaatApp");
    }

    public override void OpenSettings()
    {
        SceneManager.LoadScene("Instellingen");
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void OpenSupport()
    {
        SceneManager.LoadScene("Support");
    }

    public void PlayGame()
    {
        gegevensHouder.startNewGame = _currentSelectedGame.ToLower() switch
        {
            "sudoku" => saveScript.StringDict["SudokuClues"].Equals(""),
            "minesweeper" => saveScript.StringDict["MinesweeperMines"].Equals(""),
            "solitaire" => saveScript.StringDict["SolitaireProgress"].Equals(""),
            "2048" => saveScript.IntDict["begonnenAan2048"] == 0,
            "colorsort" => true,
            _ => gegevensHouder.startNewGame
        };
        StartCoroutine(_sceneManager.LoadSceneAsync(_currentSelectedGame));
    }
    
    
}