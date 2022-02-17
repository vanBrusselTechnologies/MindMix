using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KnoppenScript2048 : MonoBehaviour
{
    [SerializeField] private GameObject showMenuKnop;
    [SerializeField] private RectTransform showMenuKnopRect;
    [SerializeField] private RectTransform menuImageRect;
    [SerializeField] private RectTransform menuNieuweKnopRect;
    [SerializeField] private RectTransform menuGrootteRect;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private GameObject menu2048;
    [SerializeField] private RectTransform menu2048Rect;
    [SerializeField] private RectTransform uitlegTitelRect;
    [SerializeField] private RectTransform uitlegTekstRect;
    [SerializeField] private RectTransform uitlegSluitKnopRect;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private GameObject vak2048;
    [SerializeField] private GameObject overigCanvas;
    [SerializeField] private TMP_Dropdown grootteKeuze;
    private GegevensHouder gegevensScript;
    private SaveScript saveScript;
    private int gekozengrootte;

    // Use this for initialization
    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            SceneManager.LoadScene("LogoEnAppOpstart");
            return;
        }
        gegevensScript = gegevensHouder.GetComponent<GegevensHouder>();
        saveScript = gegevensHouder.GetComponent<SaveScript>();
        menu2048.SetActive(false);
    }

    public void terugNaarMenu()
    {
        SceneManager.LoadScene("SpellenOverzicht");
    }

    public void nieuwe2048()
    {
        TMP_Dropdown dropdown = grootteKeuze;
        gekozengrootte = dropdown.value;
        saveScript.intDict["grootte2048"] = gekozengrootte;
        Scene huidigeScene = SceneManager.GetActiveScene();
        gegevensScript.startNew2048 = true;
        SceneManager.LoadScene(huidigeScene.buildIndex);
    }

    public void OpenMenu()
    {
        bool verticaal = Screen.safeArea.width < Screen.safeArea.height;
        bool openen;
        if (showMenuKnop.transform.localEulerAngles == new Vector3(0, 0, 0) || showMenuKnop.transform.localEulerAngles == new Vector3(0, 0, 270))
        {
            showMenuKnop.transform.localEulerAngles += new Vector3(0, 0, 180);
            openen = false;
        }
        else
        {
            showMenuKnop.transform.localEulerAngles -= new Vector3(0, 0, 180);
            openen = true;
        }
        menu2048.SetActive(true);
        if (openen)
        {
            if (verticaal)
            {
                menu2048Rect.sizeDelta = new Vector2(Screen.width, Screen.safeArea.y + Screen.safeArea.height * 0.15f);
                menu2048Rect.anchoredPosition = new Vector2(0, -menu2048Rect.sizeDelta.y / 2f + Screen.safeArea.y);
                float schaal = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) / 1080f * 1.1f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) / 2520f * 1.1f);
                menuNieuweKnopRect.anchoredPosition = new Vector2(menu2048Rect.sizeDelta.x / 4f, Screen.safeArea.y / 2f);
                menuNieuweKnopRect.localScale = new Vector3(schaal, schaal, 1);
                menuGrootteRect.anchoredPosition = new Vector2(-menu2048Rect.sizeDelta.x / 4f, Screen.safeArea.y / 2f);
                menuGrootteRect.localScale = new Vector3(schaal, schaal, 1);
            }
            else
            {
                menu2048Rect.sizeDelta = new Vector2(Screen.safeArea.x + Screen.safeArea.width * 0.2f, Screen.height);
                menu2048Rect.anchoredPosition = new Vector2(Screen.safeArea.x - menu2048Rect.sizeDelta.x / 2f - (Screen.width / 2), Screen.height / 2f);
                float schaal = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) / 1080f * 1f, Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) / 2520f * 1f);
                terugNaarMenuKnopRect.transform.SetParent(menu2048.transform);
                terugNaarMenuKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 11, Screen.safeArea.height / 11);
                terugNaarMenuKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - menu2048Rect.sizeDelta.x / 2f + terugNaarMenuKnopRect.sizeDelta.x / 2f, -Screen.safeArea.y / 2f + Screen.height / 2f - terugNaarMenuKnopRect.sizeDelta.y / 2f);
                menuNieuweKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x / 2f, -Screen.safeArea.height / 8f);
                menuNieuweKnopRect.localScale = new Vector3(schaal, schaal, 1);
                menuGrootteRect.anchoredPosition = new Vector2(Screen.safeArea.x / 2f, Screen.safeArea.height / 8f);
                menuGrootteRect.localScale = new Vector3(schaal, schaal, 1);
            }
            grootteKeuze.value = saveScript.intDict["grootte2048"];
        }
        StartCoroutine(LaatMenuZien(openen, verticaal));
    }

    private IEnumerator LaatMenuZien(bool welLatenZien, bool verticaal)
    {
        float speed = 50f;
        menu2048.SetActive(true);
        if (verticaal)
        {
            if (welLatenZien)
            {
                showMenuKnop.transform.Translate(Vector3.up * speed);
                menu2048.transform.Translate(Vector3.up * speed);
                if (menu2048Rect.anchoredPosition.y > menu2048Rect.sizeDelta.y / 2f)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(0, menu2048Rect.sizeDelta.y + showMenuKnopRect.sizeDelta.y / 2f);
                    menu2048Rect.anchoredPosition = new Vector2(0, menu2048Rect.sizeDelta.y / 2f);
                    StopAllCoroutines();
                }
            }
            else
            {
                showMenuKnop.transform.Translate(1.5f * speed * Vector3.up);
                menu2048.transform.Translate(1.5f * speed * Vector3.down);
                if (menu2048Rect.anchoredPosition.y < -menu2048Rect.sizeDelta.y / 2f + Screen.safeArea.y)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + showMenuKnopRect.sizeDelta.y / 2f);
                    menu2048.SetActive(false);
                    StopAllCoroutines();
                }
            }
        }
        else
        {
            if (welLatenZien)
            {
                showMenuKnop.transform.Translate(Vector3.up * speed);
                menu2048.transform.Translate(Vector3.right * speed);
                if (menu2048Rect.anchoredPosition.x > menu2048Rect.sizeDelta.x / 2f - (Screen.width / 2))
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(menu2048Rect.sizeDelta.x - (Screen.width / 2f) + showMenuKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
                    menu2048Rect.anchoredPosition = new Vector2((menu2048Rect.sizeDelta.x / 2f) - (Screen.width / 2f), Screen.height / 2f);
                    StopAllCoroutines();
                }
            }
            else
            {
                showMenuKnop.transform.Translate(1.5f * speed * Vector3.up);
                menu2048.transform.Translate(1.5f * speed * Vector3.left);
                if (menu2048Rect.anchoredPosition.x < -menu2048Rect.sizeDelta.x / 2f - (Screen.width / 2) + Screen.safeArea.x)
                {
                    showMenuKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + showMenuKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
                    menu2048.SetActive(false);
                    StopAllCoroutines();
                }
            }
        }
        yield return gegevensScript.wachtHonderdste;
        StartCoroutine(LaatMenuZien(welLatenZien, verticaal));
    }

    public void OpenUitleg()
    {
        if (uitlegUI.activeSelf)
        {
            uitlegUI.SetActive(false);
            vak2048.SetActive(true);
            overigCanvas.SetActive(true);
        }
        else
        {
            uitlegUI.SetActive(true);
            uitlegTitelRect.anchoredPosition = new Vector2(0, Screen.safeArea.height * (1f / 3f));
            uitlegTitelRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.75f, Screen.safeArea.height);
            uitlegTekstRect.anchoredPosition = new Vector2(0, -Screen.safeArea.height / 8f);
            uitlegTekstRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.85f, Screen.safeArea.height / 2);
            uitlegSluitKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f * 0.6f));
            uitlegSluitKnopRect.localScale = new Vector2(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f, Mathf.Min(Screen.safeArea.height, Screen.safeArea.width) * 0.1f) / 108f;
            vak2048.SetActive(false);
            overigCanvas.SetActive(false);
            float kleinsteKant = Mathf.Min(Screen.safeArea.height, Screen.safeArea.width);
            float grootsteKant = Mathf.Max(Screen.safeArea.height, Screen.safeArea.width);
            if (kleinsteKant - 1440 > 0)
            {
                float factor = Mathf.Min(kleinsteKant / 1500f, grootsteKant / 2500f);
                uitlegTitelRect.localScale = Vector2.one * factor;
                uitlegTitelRect.sizeDelta /= factor;
            }
        }
    }
}
