using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KnoppenScriptMenu : MonoBehaviour
{
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    private FontHouder myFonts;
    [SerializeField] private GameObject ConfirmCanvas;
    [SerializeField] private GameObject menuCanvas;
    private string gekozenGame;
    [SerializeField] private Transform knoppenHouder;
    [SerializeField] private RectTransform confirmTitelRect;
    [SerializeField] private TMP_Text confirmTitelText;
    [SerializeField] private RectTransform confirmTekstRect;
    [SerializeField] private TMP_Text confirmTekstText;
    [SerializeField] private RectTransform confirmStartNieuweKnopRect;
    [SerializeField] private TMP_Text confirmStartNieuweKnopText;
    [SerializeField] private RectTransform confirmGaVerderKnopRect;
    [SerializeField] private TMP_Text confirmGaVerderKnopText;
    [SerializeField] private RectTransform confirmCancelKnopRect;
    [SerializeField] private TMP_Text confirmCancelKnopText;
    [SerializeField] private TMP_Text Knop2048Text;
    [SerializeField] private TMP_Text KnopSudokuText;
    [SerializeField] private TMP_Text KnopSolitaireText;
    [SerializeField] private TMP_Text KnopMijnenvegerText;

    // Use this for initialization
    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        myFonts = gegevensHouder.GetComponent<FontHouder>();
    }

    public void OpenConfirmMenu(string game)
    {
        gekozenGame = game;
        if (ConfirmCanvas.activeSelf)
        {
            ConfirmCanvas.SetActive(false);
            menuCanvas.SetActive(true);
            gekozenGame = "";
        }
        else
        {
            menuCanvas.SetActive(false);
            switch (game.ToLower())
            {
                case "sudoku":
                    if (saveScript.intDict["kloppendCijferBijInt" + 1] == 0)
                    {
                        OpenSudoku(true);
                        return;
                    }
                    confirmTitelText.text = KnopSudokuText.text;
                    break;
                case "solitaire":
                    if (saveScript.intDict["aanSolitaireBegonnen"] == 0)
                    {
                        OpenSolitaire(true);
                        return;
                    }
                    confirmTitelText.text = KnopSolitaireText.text;
                    break;
                case "2048":
                    if (saveScript.intDict["begonnenAan2048"] == 0)
                    {
                        Open2048(true);
                        return;
                    }
                    confirmTitelText.text = Knop2048Text.text;
                    break;
                case "mijnenveger":
                    if (saveScript.intDict["begonnenAanMV"] == 0)
                    {
                        OpenMijnenveger(true);
                        return;
                    }
                    confirmTitelText.text = KnopMijnenvegerText.text;
                    break;
                default: break;
            }
            ConfirmCanvas.SetActive(true);
            confirmTitelRect.anchoredPosition = new Vector2(0, Screen.safeArea.height * (1f / 3f));
            confirmTitelRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.75f, Screen.safeArea.height);
            float kleinsteKant = Mathf.Min(Screen.safeArea.height, Screen.safeArea.width);
            float grootsteKant = Mathf.Max(Screen.safeArea.height, Screen.safeArea.width);
            if (kleinsteKant - 1440 > 0)
            {
                float factor = Mathf.Min(kleinsteKant / 1500f, grootsteKant / 2500f);
                confirmTitelRect.localScale = Vector2.one * factor;
                confirmTitelRect.sizeDelta /= factor;
            }
            confirmTekstRect.anchoredPosition = new Vector2(0, 0);
            confirmTekstRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height / 4);
            confirmStartNieuweKnopRect.anchoredPosition = new Vector2(-Screen.safeArea.width / 3f, -Screen.safeArea.height / 3);
            confirmStartNieuweKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.3f, Screen.safeArea.height / 5);
            confirmGaVerderKnopRect.anchoredPosition = new Vector2(0, -Screen.safeArea.height / 3);
            confirmGaVerderKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.3f, Screen.safeArea.height / 5);
            confirmCancelKnopRect.anchoredPosition = new Vector2(Screen.safeArea.width / 3f, -Screen.safeArea.height / 3);
            confirmCancelKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.3f, Screen.safeArea.height / 5);
            confirmTitelText.font = myFonts.font;
            confirmTitelText.fontMaterial = myFonts.fontMaterialTitel;
            confirmTekstText.font = myFonts.font;
            confirmTekstText.fontMaterial = myFonts.fontMaterialTekst;
            confirmStartNieuweKnopText.font = myFonts.font;
            confirmStartNieuweKnopText.fontMaterial = myFonts.fontMaterialKnop;
            confirmGaVerderKnopText.font = myFonts.font;
            confirmGaVerderKnopText.fontMaterial = myFonts.fontMaterialKnop;
            confirmCancelKnopText.font = myFonts.font;
            confirmCancelKnopText.fontMaterial = myFonts.fontMaterialKnop;
        }
    }

    public void NaarSudoku()
    {
        OpenConfirmMenu("Sudoku");
    }

    public void OpenGame(bool nieuwe)
    {
        switch (gekozenGame.ToLower())
        {
            case "sudoku":
                OpenSudoku(nieuwe);
                break;
            case "solitaire":
                OpenSolitaire(nieuwe);
                break;
            case "2048":
                Open2048(nieuwe);
                break;
            case "mijnenveger":
                OpenMijnenveger(nieuwe);
                break;
        }
    }

    public void OpenSudoku(bool nieuwe)
    {
        gegevensScript.startNewSudoku = nieuwe;
        SceneManager.LoadScene("Sudoku");
    }

    public void NaarSolitaire()
    {
        OpenConfirmMenu("Solitaire");
    }

    public void OpenSolitaire(bool nieuwe)
    {
        gegevensScript.startNewSolitaire = nieuwe;
        SceneManager.LoadScene("Solitaire");
    }

    public void Naar2048()
    {
        OpenConfirmMenu("2048");
    }

    public void Open2048(bool nieuwe)
    {
        gegevensScript.startNew2048 = nieuwe;
        SceneManager.LoadScene("2048");
    }

    public void NaarTetris()
    {
        SceneManager.LoadScene("Tetris");
    }

    public void NaarMijnenveger()
    {
        OpenConfirmMenu("Mijnenveger");
    }

    public void OpenMijnenveger(bool nieuwe)
    {
        gegevensScript.startNewMV = nieuwe;
        SceneManager.LoadScene("Mijnenveger");
    }

    public void TerugNaarVoorplaat()
    {
        SceneManager.LoadScene("inlogEnVoorplaatApp");
    }

    public void OpenInstellingen()
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
}
