using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SupportScript : MonoBehaviour
{
    [SerializeField] private RectTransform terugNaarMenuRect;
    private bool isPaused = false;
    private bool wasPaused = false;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private RectTransform SupportNameRect;
    [SerializeField] private RectTransform SupportUitlegRect;
    [SerializeField] private RectTransform mailKnopRect;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if(gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        SetLayout();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused && wasPaused)
        {
            SetLayout();
        }
        wasPaused = isPaused;
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 3)
            {
                SetLayout();
            }
            return;
        }
        klaar = 0;
        SetLayout();
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

    void SetLayout()
    {
        float safeZoneAntiY = (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y)) / 2f;
        float safeZoneAntiX = (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x)) / 2f;
        Vector2 sizeDelta = Vector2.one * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f;
        terugNaarMenuRect.sizeDelta = sizeDelta;
        terugNaarMenuRect.anchoredPosition = new Vector2((-Screen.width / 2f) + Screen.safeArea.x + (sizeDelta.x * 0.6f), (Screen.height / 2f) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (sizeDelta.y * 0.6f));
        SupportNameRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY + Screen.safeArea.height / 3f);
        SupportNameRect.sizeDelta = new Vector2(-Screen.safeArea.width * 0.15f, Screen.safeArea.height / 3f);
        SupportUitlegRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY);
        SupportUitlegRect.sizeDelta = new Vector2(-Screen.safeArea.width * 0.15f, Screen.safeArea.height / 3f);
        mailKnopRect.anchoredPosition = new Vector2(safeZoneAntiX, safeZoneAntiY - Screen.safeArea.height / 3f);
        mailKnopRect.sizeDelta = new Vector2(0, Screen.safeArea.height / 3f);
    }

    public void TerugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    [SerializeField] private string emailadres;
    [SerializeField] private LocalizedString beginInhoudLocalized;
    [SerializeField] private LocalizedString extraInformatieLocalized;


    public void StuurSupportEmail()
    {
        string onderwerp = "Support ticket";
        string inhoud;
        string beginInhoud = beginInhoudLocalized.GetLocalizedString() + "\n";
        string lijn = "\n---------------------------------------------------\n";
        string ruimteVoorInput = "\n\n\n\n\n";
        string persoonlijkeInfo = "\n" + extraInformatieLocalized.GetLocalizedString() + "\nuid=" + Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
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
