using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using GooglePlayGames;
using UnityEngine.UI;

public class OpenAppScript : MonoBehaviour
{

    [SerializeField] private GameObject naarMenuKnop;
    [SerializeField] private GameObject playGamesLoginObj;
    [SerializeField] private GameObject warningObj;
    [SerializeField] private Toggle dontShowLoginWarningToggle;

    private StartupAnimation voorplaatAnimatie;
    private FireBaseSetup fireBaseSetup;
    private FireBaseAuth fireBaseAuth;
    private SaveScript saveScript;
    private GegevensHouder gegevensScript;
    private StartupScreenLayout startupScreenLayout;

    bool playgamesLoggedIn = false;
    string authCode = "";
    bool inlogEnVoorplaatScene = false;

    // Start is called before the first frame update
    private void Start()
    {
        voorplaatAnimatie = GetComponent<StartupAnimation>();
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        fireBaseSetup = gegevensHouder.GetComponent<FireBaseSetup>();
        fireBaseAuth = gegevensHouder.GetComponent<FireBaseAuth>();
        saveScript = SaveScript.Instance;
        gegevensScript = GegevensHouder.Instance;
        if (SceneManager.GetActiveScene().name == "inlogEnVoorplaatApp")
        {
            startupScreenLayout = GetComponent<StartupScreenLayout>();
            inlogEnVoorplaatScene = true;
            if (voorplaatAnimatie.finished && FirebaseAuth.DefaultInstance.CurrentUser == null)
                playGamesLoginObj.SetActive(true);
            dontShowLoginWarningToggle.isOn = PlayerPrefs.GetInt("dontShowLoginWarning", 0) == 1;
        }
    }

    private bool firstFrame = true;
    private bool animatieWasKlaar = false;
    private void Update()
    {
        if (firstFrame && fireBaseSetup != null && (fireBaseSetup.ready || fireBaseSetup.offline) && saveScript.ready)
        {
            firstFrame = false;
            if (SceneManager.GetActiveScene().name.Equals("LogoEnAppOpstart"))
            {
                SceneManager.LoadScene("inlogEnVoorplaatApp");
                return;
            }
        }
        if (!inlogEnVoorplaatScene)
            return;
        if (playgamesLoggedIn && !authCode.Equals(""))
        {
            playgamesLoggedIn = false;
            fireBaseAuth.PlayGamesLogin(authCode);
            playGamesLoginObj.SetActive(false);
        }
        if (animatieWasKlaar == false && voorplaatAnimatie.finished)
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                playGamesLoginObj.SetActive(true);
            }
            animatieWasKlaar = true;
        }
    }

    public void GaNaarSpellenOverzicht()
    {
        if (!voorplaatAnimatie.finished) { voorplaatAnimatie.finished = true; return; }
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
            SceneManager.LoadScene("SpellenOverzicht");
        else if (gegevensScript.loginWarningGehad || PlayerPrefs.GetInt("dontShowLoginWarning", 0) == 1)
            ContinueWithoutLogin();
        else
            OpenNotLoggedInWarning();
    }

    public void ContinueWithoutLogin()
    {
        PlayerPrefs.SetInt("dontShowLoginWarning", dontShowLoginWarningToggle.isOn ? 1 : 0);
        gegevensScript.loginWarningGehad = true;
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void BackToLogin()
    {
        warningObj.SetActive(false);
    }

    public void PlayGamesLogin()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                playgamesLoggedIn = true;
                authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            }
        });
    }

    private void OpenNotLoggedInWarning()
    {
        warningObj.SetActive(true);
        startupScreenLayout.SetLayout();
    }
}