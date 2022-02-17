using System.Collections.Generic;
using UnityEngine;

public class SolitaireLayout : MonoBehaviour
{
    [SerializeField] Transform solitaire;
    private bool isPaused = false;
    private bool wasPaused = false;
    private KnoppenScriptSolitaire solitaireKnoppenScript;
    private SolitaireScript solitaireScript;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private GameObject solitaireMenu;
    [SerializeField] private RectTransform menuShowKnopRect;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private Transform maakAfKnopHouder;
    [SerializeField] private RectTransform uitlegOpenKnopRect;
    [SerializeField] private Transform terugNaarMenuKnopCanvas;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private List<Transform> Stapels = new List<Transform>();
    [SerializeField] private List<Transform> EindStapels = new List<Transform>();
    [SerializeField] private Transform RestStapel;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        solitaireKnoppenScript = GetComponent<KnoppenScriptSolitaire>();
        solitaireScript = GetComponent<SolitaireScript>();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
        StartLayout();
        solitaireScript.ZetKaartenOpGoedePlek(false);
    }

    public void StartLayout()
    {
        SetLayout(true);
    }

    private void SetLayout(bool metKaartScale)
    {
        solitaireMenu.SetActive(false);
        klaar += 1;
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        float buitenSafezoneOnder = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.y);
        float buitenSafezoneBoven = Camera.main.orthographicSize * 2 / (Screen.height / (Screen.height - Screen.safeArea.height - Screen.safeArea.y));
        ScreenOrientation orientation = Screen.orientation;
        float maakAfKnopYPos = (-schermHoogte / 2f) - ((buitenSafezoneBoven - buitenSafezoneOnder) / 2f) + (schermHoogte * 0.1f);
        maakAfKnopHouder.position = new Vector3(0f, maakAfKnopYPos, 0f);
        if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
        {
            float solitaireGrootte;
            solitaireGrootte = Mathf.Min(Mathf.Min(schermHoogte, schermWijdte), Mathf.Max(schermHoogte, schermWijdte) * 0.80f);
            solitaireMenu.SetActive(true);
            terugNaarMenuKnopRect.transform.SetParent(terugNaarMenuKnopCanvas);
            terugNaarMenuKnopRect.sizeDelta = new Vector2(Screen.safeArea.width / 11, Screen.safeArea.width / 11);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2((-Screen.safeArea.width / 2) + (Screen.safeArea.width / 11 * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.y - Screen.safeArea.height) - (Screen.safeArea.width / 11 * 0.6f));
            solitaireMenu.SetActive(false);
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.width / 12, Screen.safeArea.width / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-Screen.safeArea.width / 12 * 0.6f, -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.width / 12 * 0.6f));
            menuShowKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + menuShowKnopRect.sizeDelta.y / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.height * 0.1f, Screen.safeArea.height * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 180);
            solitaire.localScale = new Vector3(solitaireGrootte / 10, solitaireGrootte / 10, 1);
            if (metKaartScale)
            {
                menuShowKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + (Screen.safeArea.height / 50f));
                solitaireMenu.SetActive(false);
                schermWijdte = Mathf.Min(schermWijdte, schermHoogte * (8f / 4.5f));
                for (int i = 0; i < solitaireScript.kaarten.Count; i++)
                {
                    solitaireScript.kaarten[i].transform.localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10) / 0.18f) * schermWijdte / 81f * 10 * 0.18f;
                    if (i < 7)
                    {
                        Stapels[i].localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
                        if (i < 4)
                        {
                            EindStapels[i].localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
                        }
                    }
                }
                RestStapel.localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
                solitaireScript.ZetKaartenOpGoedePlek(true);
            }
            return;
        }
        else
        {
            float solitaireGrootte;
            solitaireGrootte = Mathf.Min(Mathf.Min(schermHoogte, schermWijdte), Mathf.Max(schermHoogte, schermWijdte) * 0.80f);
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 12, Screen.safeArea.height / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Screen.safeArea.height / 12 * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.height / 12 * 0.6f));
            menuShowKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + menuShowKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.1f, Screen.safeArea.width * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 90);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2(10000000, 1000000);
            solitaire.transform.localScale = new Vector3(solitaireGrootte / 10, solitaireGrootte / 10, 1);
            if (metKaartScale)
            {
                menuShowKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x + (Screen.safeArea.width / 50f) - (Screen.width / 2), Screen.safeArea.y + (Screen.safeArea.height / 2));
                solitaireMenu.SetActive(false);
                schermWijdte = Mathf.Min(schermWijdte, schermHoogte * (8f / 4.5f));
                for (int i = 0; i < solitaireScript.kaarten.Count; i++)
                {
                    solitaireScript.kaarten[i].transform.localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10) / 0.18f) * schermWijdte / 81f * 10 * 0.18f;
                    if (i < 7)
                    {
                        Stapels[i].localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
                        if (i < 4)
                        {
                            EindStapels[i].localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
                        }
                    }
                }
                RestStapel.localScale = new Vector3(1, 1, 1 / (schermWijdte / 81f * 10)) * schermWijdte / 81f * 10;
                solitaireScript.ZetKaartenOpGoedePlek(true);
            }
            return;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (solitaireKnoppenScript.gehaaldCanvas.activeInHierarchy)
        {
            if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
            {
                if (klaar < 3)
                {
                    solitaireKnoppenScript.MaakSolitaireAf();
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
            if (uitlegUI != null)
            {
                solitaireKnoppenScript.OpenUitleg();
                solitaireKnoppenScript.OpenUitleg();
            }
            else
            {
                SetLayout(true);
            }
        }
        wasPaused = isPaused;
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 3)
            {
                if (uitlegUI != null)
                {
                    solitaireKnoppenScript.OpenUitleg();
                    solitaireKnoppenScript.OpenUitleg();
                }
                else
                {
                    SetLayout(true);
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
