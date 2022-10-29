using Firebase.Auth;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SupportScript : MonoBehaviour
{
    [SerializeField] private string emailadres;
    [SerializeField] private LocalizedString beginInhoudLocalized;
    [SerializeField] private LocalizedString extraInformatieLocalized;

    // Start is called before the first frame update
    void Start()
    {
        if (GegevensHouder.Instance == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
        }
    }

    public void TerugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void StuurSupportEmail()
    {
        string onderwerp = "Support ticket";
        string inhoud;
        string beginInhoud = beginInhoudLocalized.GetLocalizedString() + "\n";
        string lijn = "\n---------------------------------------------------\n";
        string ruimteVoorInput = "\n\n\n\n\n";
        string persoonlijkeInfo = "\n" + extraInformatieLocalized.GetLocalizedString() + "\nuid=" + FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        inhoud = beginInhoud + lijn + ruimteVoorInput + lijn + persoonlijkeInfo;
        string email = emailadres;
        string subject = EscapeURLFunction(onderwerp);
        string body = EscapeURLFunction(inhoud);
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private string EscapeURLFunction(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/G2HeJb52Y3");
    }
}
