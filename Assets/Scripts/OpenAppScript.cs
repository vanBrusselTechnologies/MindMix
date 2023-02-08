using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class OpenAppScript : MonoBehaviour
{
    [SerializeField] private GameObject playGamesLoginObj;
    [SerializeField] private GameObject warningObj;
    [SerializeField] private Toggle dontShowLoginWarningToggle;

    private StartupAnimation _voorplaatAnimatie;
    private FireBaseSetup _fireBaseSetup;
    private PlayGamesLogin _playGamesLogin;
    private SaveScript _saveScript;
    private GegevensHouder _gegevensScript;
    private StartupScreenLayout _startupScreenLayout;

    bool _inlogEnVoorplaatScene;

    // Start is called before the first frame update
    private void Start()
    {
        _voorplaatAnimatie = GetComponent<StartupAnimation>();
        Physics.autoSimulation = false;
        Physics.Simulate(1000000f);
        GameObject googleScriptsObj = GameObject.Find("GoogleScriptsObj");
        if (googleScriptsObj == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        _fireBaseSetup = googleScriptsObj.GetComponent<FireBaseSetup>();
        _playGamesLogin = googleScriptsObj.GetComponent<PlayGamesLogin>();
        _playGamesLogin.PlayGamesLoginObject = playGamesLoginObj;
        _saveScript = SaveScript.Instance;
        _gegevensScript = GegevensHouder.Instance;
        if (SceneManager.GetActiveScene().name == "inlogEnVoorplaatApp")
        {
            _startupScreenLayout = GetComponent<StartupScreenLayout>();
            _inlogEnVoorplaatScene = true;
            if (_voorplaatAnimatie.finished && FirebaseAuth.DefaultInstance.CurrentUser == null)
                playGamesLoginObj.SetActive(true);
            dontShowLoginWarningToggle.isOn = PlayerPrefs.GetInt("dontShowLoginWarning", 0) == 1;
        }
    }

    private bool _firstFrame = true;
    private bool _animatieWasKlaar;

    private void Update()
    {
        if (_firstFrame && _fireBaseSetup != null && (_fireBaseSetup.ready || _fireBaseSetup.offline) && _saveScript.ready)
        {
            _firstFrame = false;
            if (SceneManager.GetActiveScene().name.Equals("LogoEnAppOpstart"))
            {
                SceneManager.LoadScene("inlogEnVoorplaatApp");
                return;
            }
        }

        if (!_inlogEnVoorplaatScene) return;

        if (!_animatieWasKlaar && _voorplaatAnimatie.finished)
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                playGamesLoginObj.SetActive(true);
#endif
            }

            _animatieWasKlaar = true;
        }
    }

    public void GaNaarSpellenOverzicht()
    {
        if (!_voorplaatAnimatie.finished)
        {
            _voorplaatAnimatie.finished = true;
            return;
        }

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
            SceneManager.LoadScene("SpellenOverzicht");
        else if (_gegevensScript.isLoginWarned || PlayerPrefs.GetInt("dontShowLoginWarning", 0) == 1)
            ContinueWithoutLogin();
        else
            OpenNotLoggedInWarning();
    }

    public void ContinueWithoutLogin()
    {
        PlayerPrefs.SetInt("dontShowLoginWarning", dontShowLoginWarningToggle.isOn ? 1 : 0);
        _gegevensScript.isLoginWarned = true;
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void BackToLogin()
    {
        warningObj.SetActive(false);
    }

    public void PlayGamesLogin()
    {
        _playGamesLogin.ManualLogin();
    }

    private void OpenNotLoggedInWarning()
    {
        warningObj.SetActive(true);
        _startupScreenLayout.SetLayout();
    }
}