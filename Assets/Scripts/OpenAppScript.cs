using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using GooglePlayGames;
using System.Net;

public class OpenAppScript : MonoBehaviour
{
    private VoorplaatAnimatie voorplaatAnimatie;
    [SerializeField] private GameObject naarMenuKnop;
    [SerializeField] private GameObject playGamesLoginObj;
    private FireBaseSetup fireBaseSetup;
    private FireBaseAuth fireBaseAuth;
    private SaveScript saveScript;
    [SerializeField] private GameObject warningObj;
    private GegevensHouder gegevensScript;

    bool playgamesLoggedIn = false;
    string authCode = "";
    bool inlogEnVoorplaatScene = false;

    // Start is called before the first frame update
    private void Start()
    {
        voorplaatAnimatie = GetComponent<VoorplaatAnimatie>();
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
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        if (SceneManager.GetActiveScene().name == "inlogEnVoorplaatApp")
        {
            inlogEnVoorplaatScene = true;
            if (voorplaatAnimatie.finished && FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                playGamesLoginObj.SetActive(true);
            }
            return;
        }
    }

    private bool eersteFrame = true;
    private bool animatieWasKlaar = false;
    private void Update()
    {
        if (eersteFrame && (fireBaseSetup.ready || fireBaseSetup.offline) && saveScript.ready)
        {
            eersteFrame = false;
            if (SceneManager.GetActiveScene().name == "LogoEnAppOpstart")
            {
                SceneManager.LoadScene("inlogEnVoorplaatApp");
                return;
            }
        }
        if (!inlogEnVoorplaatScene)
        {
            return;
        }
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
        {
            SceneManager.LoadScene("SpellenOverzicht");
        }
        else
        {
            if (gegevensScript.loginWarningGehad)
            {
                VerderZonderLogin();
            }
            else
            {
                warningObj.SetActive(true);
            }
        }
    }

    public void VerderZonderLogin()
    {
        gegevensScript.loginWarningGehad = true;
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void terugVoorLogin()
    {
        warningObj.SetActive(false);
    }

    public void PlayGamesLogin()
    {
        Debug.Log("Start authentication: Social.localUser.Authenticate()");
        Debug.Log("is already authenticated: "+Social.localUser.authenticated);
        Social.localUser.Authenticate((bool success) =>
        {
            Debug.Log("authentication succesfull?: " + success);
            if (success)
            {
                playgamesLoggedIn = true;
                authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                Debug.Log("authCode: " + authCode);
            }
        });
    }
}