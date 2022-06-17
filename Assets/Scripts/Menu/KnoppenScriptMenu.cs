using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KnoppenScriptMenu : MonoBehaviour
{
    private GegevensHouder gegevensHouder;
    private SaveScript saveScript;
    private MenuLayout menuLayout;
    [SerializeField] private GameObject ConfirmCanvas;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject generalCanvas;
    private string chosenGame;
    [SerializeField] private Transform knoppenHouder;
    [SerializeField] private TMP_Text confirmTitelText;
    [SerializeField] private TMP_Text Knop2048Text;
    [SerializeField] private TMP_Text KnopSudokuText;
    [SerializeField] private TMP_Text KnopSolitaireText;
    [SerializeField] private TMP_Text KnopMijnenvegerText;
    [SerializeField] private TMP_Text KnopColorSortText;

    // Use this for initialization
    private void Start()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        gegevensHouder = GegevensHouder.Instance;
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        saveScript = SaveScript.Instance;
        menuLayout = GetComponent<MenuLayout>();
    }

    public void OpenConfirmMenu(string sceneName)
    {
        chosenGame = sceneName;
        if (ConfirmCanvas.activeInHierarchy)
        {
            ConfirmCanvas.SetActive(false);
            menuCanvas.SetActive(true);
            generalCanvas.SetActive(true);
            chosenGame = "";
        }
        else
        {
            menuCanvas.SetActive(false);
            generalCanvas.SetActive(false);
            switch (sceneName.ToLower())
            {
                case "sudoku":
                    if (saveScript.intDict["kloppendCijferBijInt" + 1] == 0)
                    {
                        OpenGameScene(sceneName, true);
                        return;
                    }
                    confirmTitelText.text = KnopSudokuText.text;
                    break;
                case "solitaire":
                    if (saveScript.intDict["aanSolitaireBegonnen"] == 0)
                    {
                        OpenGameScene(sceneName, true);
                        return;
                    }
                    confirmTitelText.text = KnopSolitaireText.text;
                    break;
                case "2048":
                    if (saveScript.intDict["begonnenAan2048"] == 0)
                    {
                        OpenGameScene(sceneName, true);
                        return;
                    }
                    confirmTitelText.text = Knop2048Text.text;
                    break;
                case "mijnenveger":
                    if (saveScript.intDict["begonnenAanMV"] == 0)
                    {
                        OpenGameScene(sceneName, true);
                        return;
                    }
                    confirmTitelText.text = KnopMijnenvegerText.text;
                    break;
                case "colorsort":
                    if (true)
                    {
                        OpenGameScene(sceneName, true);
                        return;
                    }
                    confirmTitelText.text = KnopColorSortText.text;
                    break;
                default: break;
            }
            ConfirmCanvas.SetActive(true);
            menuLayout.SetLayoutConfirmCanvas();
        }
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

    public void PlayGame(string sceneName)
    {
        OpenConfirmMenu(sceneName);
    }

    private void OpenGameScene(string sceneName, bool startNewGame)
    {
        gegevensHouder.startNewGame = startNewGame;
        SceneManager.LoadScene(sceneName);
    }

    public void OpenGameScene(bool startNewGame)
    {
        OpenGameScene(chosenGame, startNewGame);
    }
}
