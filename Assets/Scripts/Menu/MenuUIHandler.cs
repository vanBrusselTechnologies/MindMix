using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuUIHandler : BaseUIHandler
{
    public const float GameModeWheelRotationDivPerChild = 360f / 22f;

    private MenuLayout _menuLayout;

    [Header("Other scene specific")] [SerializeField]
    private SceneManager sceneManager;

    [SerializeField] private RectTransform gameWheel;
    [SerializeField] private RectTransform gameModeWheel;
    [SerializeField] private Collider gameWheelCollider;

    private int _currentSelectedGameIndex;
    private RectTransform _rotatingWheelRect;
    private float _touchMaxTotalDeltaPositionX;
    private bool _swipeOnButton;
    private Coroutine _coroutine;
    private Vector2 _startTouchPosition;
    private readonly List<TMP_Text> _gameWheelNames = new();
    private readonly List<TMP_Text> _gameModeWheelNames = new();
    private string _currentSelectedGame = "";
    private bool _startSetup = true;
    private Camera _camera;
    private TMP_Text _gameWheelName2Text;

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

        _menuLayout = (MenuLayout)baseLayout;
        _currentSelectedGameIndex = saveScript.IntDict["MenuSelectedGameIndex"];
        Transform gameModesParent = gameWheel.GetChild(_currentSelectedGameIndex).GetChild(2);
        if (gameModesParent.childCount > 0)
        {
            foreach (Transform gameMode in gameModesParent)
            {
                Instantiate(gameMode, gameModeWheel);
            }

            gameModeWheel.GetChild(0).GetChild(1).GetComponent<TMP_Text>().alpha = 1;
        }

        CreateClones();

        foreach (Transform child in gameWheel)
        {
            _gameWheelNames.Add(child.GetChild(1).GetComponent<TMP_Text>());
        }

        gameWheel.localEulerAngles =
            new Vector3(0, 0, 360f / gameWheel.childCount * _currentSelectedGameIndex);

        Transform selectedGameTransform = gameWheel.GetChild(_currentSelectedGameIndex);
        _currentSelectedGame = selectedGameTransform.name.Split("(")[0];
        if (selectedGameTransform.GetChild(2).childCount == 0)
        {
            selectedGameTransform.GetChild(1).GetComponent<TMP_Text>().alpha = 1;
        }
        else
        {
            gameModeWheel.localEulerAngles = new Vector3(0, 0,
                GameModeWheelRotationDivPerChild * saveScript.IntDict[$"{_currentSelectedGame}SelectedMode"]);
            foreach (Transform child in gameModeWheel)
            {
                _gameModeWheelNames.Add(child.GetChild(1).GetComponent<TMP_Text>());
            }

            TMP_Text gameWheelNameText = _gameWheelNames[_currentSelectedGameIndex];
            Transform gameParentTf = gameWheelNameText.transform.parent;
            _gameWheelName2Text = Instantiate(gameWheelNameText, gameParentTf);
            _gameWheelName2Text.alpha = 1;
            gameWheelNameText.gameObject.SetActive(false);
            RectTransform gameNameRect = (RectTransform)gameWheelNameText.transform;
            RectTransform gameName2Rect = (RectTransform)gameParentTf.GetChild(3);
            float y = gameNameRect.anchoredPosition.y + 100;
            gameName2Rect.anchoredPosition = new Vector2(gameName2Rect.anchoredPosition.x, y);
        }

        ChangeWheelItemSizes(gameWheel, true);
        ChangeWheelItemSizes(gameModeWheel, false);

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
                bool isGameWheelHit = gameWheelCollider.Raycast(ray, out RaycastHit _, Mathf.Infinity);
                bool hasNoGameModes = gameWheel.GetChild(_currentSelectedGameIndex).GetChild(2)
                    .childCount == 0;
                _rotatingWheelRect = isGameWheelHit || hasNoGameModes ? gameWheel : gameModeWheel;
                _touchMaxTotalDeltaPositionX = 0;
                break;
            case TouchPhase.Moved:
            {
                if (_swipeOnButton) break;
                float lastTouchMaxTotalDeltaPositionX = _touchMaxTotalDeltaPositionX;
                _touchMaxTotalDeltaPositionX = Mathf.Max(Mathf.Abs(touch.position.x - _startTouchPosition.x),
                    _touchMaxTotalDeltaPositionX);
                if (_touchMaxTotalDeltaPositionX < baseLayout.screenWidth * 0.01f) break;
                bool isInnerWheel = String.Equals(_rotatingWheelRect.name, gameWheel.name);
                if (isInnerWheel && lastTouchMaxTotalDeltaPositionX < baseLayout.screenWidth * 0.01f)
                    StartCoroutine(HideGameModeWheel());
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                float degreesRotationZ = touch.deltaPosition.x /
                    Mathf.Min(baseLayout.screenWidth, baseLayout.screenHeight) * 180f;
                float baseTargetEulerAngels = _rotatingWheelRect.localEulerAngles.z - degreesRotationZ;
                float targetEulerAngles = (baseTargetEulerAngels + 720f) % 180f;
                if (!isInnerWheel)
                {
                    targetEulerAngles = Mathf.Max(0, Mathf.Min(baseTargetEulerAngels,
                        (_rotatingWheelRect.childCount - 1f) * GameModeWheelRotationDivPerChild));
                }
                else if (targetEulerAngles > 180 - 360f / gameWheel.childCount / 2f) targetEulerAngles -= 180f;

                _rotatingWheelRect.localEulerAngles = new Vector3(0, 0, targetEulerAngles);
                ChangeWheelItemSizes(_rotatingWheelRect, isInnerWheel);
                break;
            }
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
            {
                if (_touchMaxTotalDeltaPositionX < baseLayout.screenWidth * 0.01f) return;
                bool isGameWheel = String.Equals(_rotatingWheelRect.name, gameWheel.name);
                int gameCount = _rotatingWheelRect.childCount;
                int origGameCount = 0;
                foreach (Transform child in _rotatingWheelRect)
                    if (!child.name.EndsWith("(Clone)"))
                        origGameCount += 1;
                if (origGameCount == 0) origGameCount = gameCount;
                float startRotation = _rotatingWheelRect.localEulerAngles.z;
                float rotationDivPerChild = isGameWheel ? 360f / gameCount : GameModeWheelRotationDivPerChild;
                float index = Mathf.Round(((startRotation + 360f) % 360f) / rotationDivPerChild);
                int selectedIndex = Mathf.RoundToInt(index % origGameCount);
                if (isGameWheel)
                {
                    _currentSelectedGameIndex = selectedIndex;
                    _currentSelectedGame = gameWheel.GetChild(selectedIndex).name.Split("(")[0];
                    if (gameWheel.GetChild(selectedIndex).GetChild(2).childCount != 0)
                    {
                        gameModeWheel.localEulerAngles = new Vector3(0, 0,
                            GameModeWheelRotationDivPerChild *
                            saveScript.IntDict[$"{_currentSelectedGame}SelectedMode"]);
                    }
                }

                float startAlpha = !isGameWheel
                    ? gameModeWheel.GetChild(selectedIndex).GetChild(1).GetComponent<TMP_Text>().alpha
                    : _gameWheelNames[selectedIndex].alpha;
                Transform secondWheel = isGameWheel ? gameModeWheel : gameWheel;
                float startLocalScale = secondWheel.localScale.y;
                _coroutine = StartCoroutine(TurnWheelToEndPosition(_rotatingWheelRect, selectedIndex, startRotation,
                    index * rotationDivPerChild, startAlpha, startLocalScale));
                _swipeOnButton = false;
                break;
            }
            case TouchPhase.Stationary:
            default: break;
        }
    }

    /// <summary>
    /// Creates clones for filling up the gameWheel
    /// </summary>
    private void CreateClones()
    {
        int gameCount = gameWheel.childCount;
        for (int i = 0; i < gameCount; i++)
        {
            Transform game = gameWheel.GetChild(i % gameCount);
            Instantiate(game.gameObject, gameWheel);
        }
    }

    private IEnumerator TurnWheelToEndPosition(Transform wheel, int index, float startRotation, float endRotation,
        float startAlpha, float startLocalScale, float t = 0)
    {
        bool isInnerWheel = String.Equals(wheel.name, gameWheel.name);
        float time = Mathf.Min(t * 4f, 1f);
        float zRotation = Mathf.Lerp(startRotation, endRotation, time);
        wheel.localEulerAngles = new Vector3(0, 0, zRotation);
        ChangeWheelItemSizes(wheel, isInnerWheel);
        if (isInnerWheel)
        {
            if (Mathf.Approximately(t, 0f))
            {
                if (gameModeWheel.childCount != 0)
                    foreach (Transform c in gameModeWheel)
                        Destroy(c.gameObject);
                yield return null;
            }

            if (wheel.GetChild(index).GetChild(2).childCount == 0)
                _gameWheelNames[index].alpha = Mathf.Lerp(startAlpha, 1, time);
            else
            {
                List<Transform> gameModes = new();
                foreach (Transform mode in wheel.GetChild(index).GetChild(2)) gameModes.Add(mode);
                ShowGameModeWheel(gameModes, index, startAlpha, startLocalScale, time);
            }
        }
        else
        {
            _gameModeWheelNames[index].alpha = Mathf.Lerp(startAlpha, 1, time);
        }

        if (time >= 1f) yield break;

        yield return new WaitForFixedUpdate();
        yield return TurnWheelToEndPosition(wheel, index, startRotation, endRotation, startAlpha, startLocalScale,
            t + Time.fixedDeltaTime);
    }

    private void ChangeWheelItemSizes(Transform wheel, bool isGameWheel)
    {
        List<TMP_Text> wheelNames = isGameWheel ? _gameWheelNames : _gameModeWheelNames;

        int childCount = wheel.childCount;
        if (childCount <= 1) return;
        float rotationDivPerChild = isGameWheel ? 360f / childCount : GameModeWheelRotationDivPerChild;
        float zRotation = Mathf.Round((wheel.localEulerAngles.z + 360f) % 360f * 1000f) / 1000f;

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
                wheelNames[childIndex].alpha = floorChild == childIndex ? 1f : 0f;
            }
            else
            {
                if (child.childCount == 4)
                {
                    child.GetChild(3).gameObject.SetActive(false);
                    child.GetChild(1).gameObject.SetActive(true);
                }

                if (floorChild == childIndex || ceilChild == childIndex)
                {
                    child.localScale = Vector3.one * (Mathf.Lerp(0.5f, 1f,
                        floorChild == childIndex ? 1 - partitionChildren : partitionChildren));
                    wheelNames[childIndex].alpha = Mathf.Lerp(0, 1f, floorChild == childIndex
                        ? Mathf.Pow(1 - partitionChildren, 2)
                        : Mathf.Pow(partitionChildren, 2));
                }
                else
                {
                    child.localScale = Vector3.one * 0.5f;
                    wheelNames[childIndex].alpha = 0;
                }
            }
        }
    }

    private void ShowGameModeWheel(List<Transform> gameModes, int gameIndex, float startAlpha = -1f,
        float startLocalScale = -1f, float t = 0f)
    {
        TMP_Text gameWheelNameText = _gameWheelNames[gameIndex];
        Transform gameParentTf = gameWheelNameText.transform.parent;

        if (Mathf.Approximately(t, 0f))
        {
            _gameModeWheelNames.Clear();
            foreach (Transform gameMode in gameModes)
            {
                GameObject obj = Instantiate(gameMode.gameObject, gameModeWheel, false);
                ((RectTransform)obj.transform).anchoredPosition = new Vector2(0, 100);
                _gameModeWheelNames.Add(obj.transform.GetChild(1).GetComponent<TMP_Text>());
                _menuLayout.SetLayoutGameWheels(gameModeWheel);
            }

            ChangeWheelItemSizes(gameModeWheel, false);
            if (gameParentTf.childCount < 4) _gameWheelName2Text = Instantiate(gameWheelNameText, gameParentTf);
        }

        if (gameParentTf.childCount == 4)
        {
            gameWheelNameText.gameObject.SetActive(false);
            _gameWheelName2Text.alpha = Mathf.Lerp(gameWheelNameText.alpha, 1, t);
            RectTransform gameNameRect = (RectTransform)gameWheelNameText.transform;
            RectTransform gameName2Rect = (RectTransform)gameParentTf.GetChild(3);
            float y = gameNameRect.anchoredPosition.y + Mathf.Lerp(0, 100, t);
            gameName2Rect.anchoredPosition = new Vector2(gameName2Rect.anchoredPosition.x, y);
            gameName2Rect.gameObject.SetActive(true);
        }
        
        int selectedGameModeIndex =
            Mathf.RoundToInt((gameModeWheel.localEulerAngles.z + 720f) % 360f / GameModeWheelRotationDivPerChild);
        _gameModeWheelNames[selectedGameModeIndex].alpha = Mathf.Lerp(1 - startAlpha, 1, t);

        float maxLocalScale = Mathf.Min(baseLayout.screenSafeAreaHeightInUnits / 450f,
            baseLayout.screenSafeAreaWidthInUnits / 250f) * 0.75f;
        gameModeWheel.localScale = Vector3.one * Mathf.Lerp(startLocalScale, maxLocalScale, t);
    }

    private IEnumerator HideGameModeWheel(float startAlpha = -1f, float startLocalScale = -1f, float t = 0f)
    {
        int childCount = gameWheel.childCount;
        if (childCount <= 1) yield break;
        float rotationDivPerChild = 360f / childCount;
        float zRotation = Mathf.Round(gameWheel.localEulerAngles.z * 1000f) / 1000f;

        int childIndex = Mathf.RoundToInt(zRotation / rotationDivPerChild) % childCount;
        if (Mathf.Approximately(startAlpha, -1f))
        {
            startAlpha = _gameWheelNames[childIndex].alpha;
        }

        if (Mathf.Approximately(startLocalScale, -1f)) startLocalScale = gameModeWheel.localScale.y;
        float time = Mathf.Min(t * 4f, 1f);
        TMP_Text gameWheelNameText = _gameWheelNames[childIndex];
        Transform gameParentTf = gameWheelNameText.transform.parent;
        if (gameParentTf.childCount == 4)
        {
            RectTransform gameNameRect = (RectTransform)gameWheelNameText.transform;
            RectTransform gameName2Rect = (RectTransform)_gameWheelName2Text.transform;
            float y = gameNameRect.anchoredPosition.y + Mathf.Lerp(100, 0, time);
            gameName2Rect.anchoredPosition = new Vector2(gameName2Rect.anchoredPosition.x, y);
            _gameWheelName2Text.alpha = _gameWheelNames[childIndex].alpha;
        }

        gameModeWheel.localScale = Vector3.one * Mathf.Lerp(startLocalScale, 0, time);
        if (time >= 1f)
        {
            if (gameParentTf.childCount != 4) yield break;
            gameParentTf.GetChild(3).gameObject.SetActive(false);
            gameWheelNameText.gameObject.SetActive(true);
            yield break;
        }

        yield return new WaitForFixedUpdate();
        yield return HideGameModeWheel(startAlpha, startLocalScale, t + Time.fixedDeltaTime);
    }

    public override void BackToMenu() => SceneManager.LoadScene("inlogEnVoorplaatApp");

    public override void OpenSettings() => SceneManager.LoadScene("Settings");

    public void OpenShop() => SceneManager.LoadScene("Shop");

    public void OpenSupport() => SceneManager.LoadScene("Support");

    public void PlayGame()
    {
        int gameMode = 0;
        if (gameModeWheel.childCount != 0)
        {
            gameMode = Mathf.RoundToInt(
                ((gameModeWheel.eulerAngles.z + 720f) % 360f) / GameModeWheelRotationDivPerChild);
            saveScript.IntDict[$"{_currentSelectedGame}SelectedMode"] = gameMode;
        }

        gegevensHouder.startNewGame = _currentSelectedGame switch
        {
            "Sudoku" => saveScript.StringDict["SudokuClues"].Trim().Equals(""),
            "Minesweeper" => saveScript.StringDict["MinesweeperMines"].Trim().Equals(""),
            "Solitaire" => saveScript.StringDict["SolitaireProgress"].Trim().Equals(""),
            "2048" => saveScript.StringDict[$"2048Mode{gameMode}Progress"].Trim().Equals(""),
            "ColorSort" => true,
            _ => gegevensHouder.startNewGame
        };
        saveScript.IntDict["MenuSelectedGameIndex"] = _currentSelectedGameIndex;
        if (_currentSelectedGame.Equals("Sudoku"))
            StartCoroutine(sceneManager.LoadSceneAsync(_currentSelectedGame));
        else SceneManager.LoadScene(_currentSelectedGame);
    }
}