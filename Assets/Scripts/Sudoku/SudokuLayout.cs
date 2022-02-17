using System.Collections.Generic;
using UnityEngine;

public class SudokuLayout : MonoBehaviour
{
    private bool isPaused = false;
    private bool wasPaused = false;
    private KnoppenScript SudokuKnoppenScript;
    private AfScript afScript;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private GameObject sudoku;
    [SerializeField] private GameObject sudokuMenu;
    [SerializeField] private RectTransform menuShowKnopRect;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private RectTransform uitlegOpenKnopRect;
    [SerializeField] private Transform terugNaarMenuKnopCanvas;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private RectTransform onScreenNumpadRect;
    [SerializeField] private List<RectTransform> numpadDelenRect;
    [SerializeField] private RectTransform rasterEnNumpadRect;
    [SerializeField] private RectTransform instellingenOpenKnopRect;
    [SerializeField] private GameObject instellingenObj;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
        SudokuKnoppenScript = GetComponent<KnoppenScript>();
        afScript = GetComponent<AfScript>();
        SetLayout();
    }

    private void SetLayout()
    {
        sudokuMenu.SetActive(false);
        klaar += 1;
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        float buitenSafezoneLinks = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.x);
        float buitenSafezoneOnder = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.y);
        float buitenSafezoneRechts = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / (Screen.width - Screen.safeArea.width - Screen.safeArea.x));
        float buitenSafezoneBoven = Camera.main.orthographicSize * 2 / (Screen.height / (Screen.height - Screen.safeArea.height - Screen.safeArea.y));
        ScreenOrientation orientation = Screen.orientation;
        float sudokuGrootte = Mathf.Min(Mathf.Min(schermHoogte, schermWijdte), Mathf.Max(schermHoogte, schermWijdte) * 0.70f);
        if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
        {
            sudokuMenu.SetActive(true);
            terugNaarMenuKnopRect.transform.SetParent(terugNaarMenuKnopCanvas);
            terugNaarMenuKnopRect.sizeDelta = new Vector2(Screen.safeArea.width / 11, Screen.safeArea.width / 11);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2((-Screen.safeArea.width / 2) + (Screen.safeArea.width / 11 * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.y - Screen.safeArea.height) - (Screen.safeArea.width / 11 * 0.6f));
            sudokuMenu.SetActive(false);
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.width / 12, Screen.safeArea.width / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-Screen.safeArea.width / 12 * 0.6f, -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.width / 12 * 0.6f));
            instellingenOpenKnopRect.sizeDelta = uitlegOpenKnopRect.sizeDelta;
            instellingenOpenKnopRect.anchoredPosition = new Vector2(-Screen.safeArea.width / 12 * 0.6f, -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.width / 12 * 0.6f) - (2f * Screen.safeArea.width / 12f * 0.6f));
            rasterEnNumpadRect.anchoredPosition = new Vector2(0.5f * (buitenSafezoneLinks - buitenSafezoneRechts), (sudokuGrootte / 20f / 18f * 76) + (0.5f * (buitenSafezoneOnder - buitenSafezoneBoven)));
            menuShowKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + menuShowKnopRect.sizeDelta.y / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.height * 0.1f, Screen.safeArea.height * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 180);
            onScreenNumpadRect.anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < onScreenNumpadRect.transform.childCount; i++)
            {
                numpadDelenRect[i].localScale = new Vector3(sudokuGrootte / 6, sudokuGrootte / 6, 1);
                float rijLagerY = 0;
                int getalInRij;
                if (numpadDelenRect[i].name.Length >= 10)
                {
                    getalInRij = 10;
                }
                else
                {
                    getalInRij = int.Parse(numpadDelenRect[i].name);
                }
                if (getalInRij > 5)
                {
                    getalInRij -= 5;
                    rijLagerY = sudokuGrootte / 2f / 18f * 76;
                }
                numpadDelenRect[i].anchoredPosition = new Vector2((-sudokuGrootte / 18f * 76) + (sudokuGrootte / 2f / 18f * 76 * (getalInRij - 1)), (-sudokuGrootte / 18f * 76) - (sudokuGrootte / 2f / 18f * 76) - rijLagerY);
            }
            sudoku.transform.localScale = new Vector3(sudokuGrootte / 10, sudokuGrootte / 10, 1);
        }
        else
        {
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 12, Screen.safeArea.height / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Screen.safeArea.height / 12 * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.height / 12 * 0.6f));
            instellingenOpenKnopRect.sizeDelta = uitlegOpenKnopRect.sizeDelta;
            instellingenOpenKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Screen.safeArea.height / 12 * 0.6f) - (2f * Screen.safeArea.height / 12f * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.height / 12 * 0.6f));
            rasterEnNumpadRect.anchoredPosition = new Vector2((-sudokuGrootte / 20f / 18f * 76) + (sudokuGrootte / 50) + (0.5f * (buitenSafezoneLinks - buitenSafezoneRechts)), 0.5f * (buitenSafezoneOnder - buitenSafezoneBoven));
            menuShowKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + menuShowKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.1f, Screen.safeArea.width * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 90);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2(10000000, 1000000);
            onScreenNumpadRect.anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < onScreenNumpadRect.transform.childCount; i++)
            {
                numpadDelenRect[i].localScale = new Vector3(sudokuGrootte / 6, sudokuGrootte / 6, 1);
                float rijLagerY = 0;
                int getalInRij;
                if (numpadDelenRect[i].name == "normaalOfNotitieHouder")
                {
                    getalInRij = 10;
                }
                else
                {
                    getalInRij = int.Parse(numpadDelenRect[i].name);
                }
                if (getalInRij % 2 == 1)
                {
                    getalInRij = Mathf.FloorToInt(getalInRij / 2) + 1;
                }
                else
                {
                    getalInRij /= 2;
                    rijLagerY = sudokuGrootte / 2f / 18f * 76;
                }
                numpadDelenRect[i].anchoredPosition = new Vector2((sudokuGrootte / 18f * 76) + (sudokuGrootte / 2f / 18f * 76) + rijLagerY, (sudokuGrootte / 18f * 76) - (sudokuGrootte / 2f / 18f * 76 * (getalInRij - 1)));
            }
            sudoku.transform.localScale = new Vector3(sudokuGrootte / 10, sudokuGrootte / 10, 1);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (afScript.GehaaldCanvas.activeInHierarchy)
        {
            if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
            {
                if (klaar < 3)
                {
                    afScript.OpenGehaaldCanvas();
                    klaar += 1;
                }
                return;
            }
            klaar = 0;
            vorigeScreenWidth = Screen.width;
            vorigeSafezoneY = Screen.safeArea.y;
            vorigeSafezoneX = Screen.safeArea.x;
            return;
        }
        if (!isPaused && wasPaused)
        {
            if (uitlegUI.activeInHierarchy)
            {
                SudokuKnoppenScript.OpenUitleg();
                SudokuKnoppenScript.OpenUitleg();
            }
            else if (instellingenObj.activeInHierarchy)
            {
                SudokuKnoppenScript.OpenSudokuSettings();
                SudokuKnoppenScript.OpenSudokuSettings();
            }
            else
            {
                SetLayout();
            }
        }
        wasPaused = isPaused;
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 3)
            {
                if (uitlegUI.activeInHierarchy)
                {
                    SudokuKnoppenScript.OpenUitleg();
                    SudokuKnoppenScript.OpenUitleg();
                }
                else if (instellingenObj.activeInHierarchy)
                {
                    SudokuKnoppenScript.OpenSudokuSettings();
                    SudokuKnoppenScript.OpenSudokuSettings();
                }
                else
                {
                    SetLayout();
                }
            }
            return;
        }
        klaar = 0;
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
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
