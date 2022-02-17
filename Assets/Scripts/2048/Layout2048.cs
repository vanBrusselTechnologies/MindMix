using UnityEngine;

public class Layout2048 : MonoBehaviour
{
    private bool isPaused = false;
    private bool wasPaused = false;
    private KnoppenScript2048 knoppenScript2048;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private GameObject menu2048;
    [SerializeField] private RectTransform menuShowKnopRect;
    [SerializeField] private GameObject gehaaldCanvas;
    [SerializeField] private GameObject vak2048;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private RectTransform uitlegOpenKnopRect;
    [SerializeField] private Transform terugNaarMenuKnopCanvas;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private RectTransform scoreRect;
    private Script2048 script2048;

    // Start is called before the first frame update
    private void Start()
    {
        script2048 = GetComponent<Script2048>();
        knoppenScript2048 = GetComponent<KnoppenScript2048>();
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            Destroy(gameObject);
            return;
        }
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
        SetLayout();
    }

    private void SetLayout()
    {
        menu2048.SetActive(false);
        klaar += 1;
        float buitenSafezoneLinks = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.x);
        float buitenSafezoneOnder = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.y);
        float buitenSafezoneRechts = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / (Screen.width - Screen.safeArea.width - Screen.safeArea.x));
        float buitenSafezoneBoven = Camera.main.orthographicSize * 2 / (Screen.height / (Screen.height - Screen.safeArea.height - Screen.safeArea.y));
        ScreenOrientation orientation = Screen.orientation;
        float schaal2048 = Mathf.Min(Mathf.Min(Screen.safeArea.height, Screen.safeArea.width), Mathf.Max(Screen.safeArea.height, Screen.safeArea.width) * 0.75f) * 0.95f;
        if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
        {
            terugNaarMenuKnopRect.transform.SetParent(terugNaarMenuKnopCanvas);
            terugNaarMenuKnopRect.sizeDelta = new Vector2(Screen.safeArea.width / 11, Screen.safeArea.width / 11);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2((-Screen.safeArea.width / 2) + (Screen.safeArea.width / 11 * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.y - Screen.safeArea.height) - (Screen.safeArea.width / 11 * 0.6f));
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.width / 12, Screen.safeArea.width / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-Screen.safeArea.width / 12 * 0.6f, -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.width / 12 * 0.6f));
            menuShowKnopRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + menuShowKnopRect.sizeDelta.y / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.height * 0.1f, Screen.safeArea.height * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 180);
            schaal2048 = schaal2048 / Screen.height * 10f;
            vak2048.transform.localScale = new Vector3(schaal2048, schaal2048, 1);
            float vijfProcentSchermOmhoog = Camera.main.orthographicSize * 2f / Screen.height * Screen.safeArea.height * 0.05f;
            vak2048.transform.localPosition = new Vector3((buitenSafezoneLinks - buitenSafezoneRechts) / 2f, (buitenSafezoneOnder - buitenSafezoneBoven) / 2f + vijfProcentSchermOmhoog, -0.1f);
            scoreRect.anchoredPosition = new Vector2(0, Screen.safeArea.height * 0.1f + Screen.safeArea.y - Screen.height / 2f);
        }
        else
        {
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 12, Screen.safeArea.height / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Screen.safeArea.height / 12 * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.height / 12 * 0.6f));
            menuShowKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + menuShowKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.1f, Screen.safeArea.width * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 90);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2(10000000, 1000000);
            schaal2048 = schaal2048 / Screen.height * 10f;
            vak2048.transform.localScale = new Vector3(schaal2048, schaal2048, 1);
            float vijfProcentSchermOmhoog = Camera.main.orthographicSize * 2f / Screen.height * Screen.safeArea.width * 0.05f;
            vak2048.transform.localPosition = new Vector3((buitenSafezoneLinks - buitenSafezoneRechts) / 2f + vijfProcentSchermOmhoog, (buitenSafezoneOnder - buitenSafezoneBoven) / 2f, -0.1f);
            scoreRect.anchoredPosition = new Vector2(Screen.safeArea.width * 0.1f + Screen.safeArea.x - Screen.width / 2f, 0);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gehaaldCanvas.activeInHierarchy) {
            if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
            {
                if (klaar < 3)
                {
                    script2048.OpenGehaaldCanvas();
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
                knoppenScript2048.OpenUitleg();
                knoppenScript2048.OpenUitleg();
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
                    knoppenScript2048.OpenUitleg();
                    knoppenScript2048.OpenUitleg();
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
