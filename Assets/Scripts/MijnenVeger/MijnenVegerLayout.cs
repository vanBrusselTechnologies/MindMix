using UnityEngine;

public class MijnenVegerLayout : MonoBehaviour
{
    private bool isPaused = false;
    private bool wasPaused = false;
    private KnoppenScriptMijnenVeger MVKnoppenScript;
    private MijnenVegerScript mvScript;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private GameObject mijnenVegerMenu;
    [SerializeField] private RectTransform menuShowKnopRect;
    [SerializeField] private GameObject raster;
    [SerializeField] private GameObject lijn;
    [SerializeField] private GameObject uitlegUI;
    [SerializeField] private RectTransform uitlegOpenKnopRect;
    [SerializeField] private Transform terugNaarMenuKnopCanvas;
    [SerializeField] private RectTransform terugNaarMenuKnopRect;
    [SerializeField] private RectTransform vlagOfSchepKnop;
    [SerializeField] private GameObject bommenTeGaanObj;
    [HideInInspector] public int langeKant = 22;
    [HideInInspector] public int korteKant = 16;
    [HideInInspector] public float breedteMvKnop = 0.0003726f;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject gegevensHouder = GameObject.Find("gegevensHouder");
        if (gegevensHouder == null)
        {
            return;
        }
        MVKnoppenScript = GetComponent<KnoppenScriptMijnenVeger>();
        mvScript = GetComponent<MijnenVegerScript>();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
        SetLayout();
    }

    private void SetLayout()
    {
        for (int i = 0; i < raster.transform.childCount; i++)
        {
            Destroy(raster.transform.GetChild(raster.transform.childCount - i - 1).gameObject);
        }
        mijnenVegerMenu.SetActive(false);
        klaar += 1;
        float schermWijdte = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.width);
        float schermHoogte = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.height);
        float buitenSafezoneLinks = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / Screen.safeArea.x);
        float buitenSafezoneOnder = Camera.main.orthographicSize * 2 / (Screen.height / Screen.safeArea.y);
        float buitenSafezoneRechts = Camera.main.orthographicSize * 2 * Screen.width / Screen.height / (Screen.width / (Screen.width - Screen.safeArea.width - Screen.safeArea.x));
        float buitenSafezoneBoven = Camera.main.orthographicSize * 2 / (Screen.height / (Screen.height - Screen.safeArea.height - Screen.safeArea.y));
        ScreenOrientation orientation = Screen.orientation;
        float uitlegKnopGrootte = Screen.safeArea.height / 12f / 1000f * 2f;
        Transform MVvak = raster.transform.parent.parent;
        float breedteMvVakje = Mathf.Min(Mathf.Min(schermHoogte, schermWijdte) * .95f / 16f, ((Mathf.Max(schermHoogte, schermWijdte) * 0.80f) - uitlegKnopGrootte) / 22f);
        Vector3 localScaleHorzLijn = new Vector3(100, 1f / (langeKant - 3) * 4f, 1);
        Vector3 localScaleVertLijn = new Vector3(1f / (langeKant - 3) * 4f, 100, 1);
        if (schermHoogte > schermWijdte)
        {
            korteKant = 22;
            langeKant = 16;
        }
        else
        {
            korteKant = 16;
            langeKant = 22;
        }
        for (int i = 1; i < Mathf.Max(langeKant, korteKant); i++)
        {
            if (i < korteKant)
            {
                GameObject templijnHorz = Instantiate(lijn);
                templijnHorz.transform.parent = raster.transform;
                templijnHorz.transform.localPosition = new Vector3(0, -50f + (i * 100f / korteKant), -2f) / 100f;
                templijnHorz.transform.localScale = localScaleHorzLijn / 100f;
            }
            if (i < langeKant)
            {
                GameObject templijnVert = Instantiate(lijn);
                templijnVert.transform.parent = raster.transform;
                templijnVert.transform.localPosition = new Vector3(-50f + (i * 100f / langeKant), 0, -2f) / 100f;
                templijnVert.transform.localScale = localScaleVertLijn / 100f;
            }
        }
        for (int i = 0; i < mvScript.buttons.Count; i++)
        {
            GameObject knop = mvScript.buttons[i].gameObject;
            int vakjesGehadNummer = int.Parse(knop.name);
            int kolom = vakjesGehadNummer % 100;
            int rij = (vakjesGehadNummer - kolom) / 100;
            if (korteKant > langeKant)
            {
                knop.transform.localPosition = new Vector3((breedteMvKnop * 75f) + (10.5f * langeKant * kolom * breedteMvKnop), (breedteMvKnop * 60f) + (korteKant * 5.55f * rij * breedteMvKnop), -0.45f);
            }
            else
            {
                knop.transform.localPosition = new Vector3((breedteMvKnop * 60f) + (langeKant * 5.55f * (21 - rij) * breedteMvKnop), (breedteMvKnop * 75f) + (10.5f * korteKant * kolom * breedteMvKnop), -0.45f);
            }
            knop.transform.localScale = new Vector3(breedteMvKnop * .5f, breedteMvKnop * .5f, 1f);
        }
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
            MVvak.transform.position = new Vector3((buitenSafezoneLinks - buitenSafezoneRechts) / 2f, ((buitenSafezoneOnder - buitenSafezoneBoven) / 2f) + (schermHoogte * .1f) - (uitlegKnopGrootte * 1.25f), -0.5f);
            MVvak.transform.localScale = new Vector3(breedteMvVakje * 16, breedteMvVakje * 22, 1);
            float menuutjeY = (schermHoogte * 0.1f) - (schermHoogte / 2f);
            vlagOfSchepKnop.transform.position = new Vector3((schermWijdte * 0.2f) + ((buitenSafezoneLinks - buitenSafezoneRechts) / 2f), ((buitenSafezoneOnder - buitenSafezoneBoven) / 2f) + menuutjeY, -5);
            bommenTeGaanObj.transform.position = new Vector3((schermWijdte * -0.2f) + ((buitenSafezoneLinks - buitenSafezoneRechts) / 2f), ((buitenSafezoneOnder - buitenSafezoneBoven) / 2f) + menuutjeY, 0);
        }
        else
        {
            uitlegOpenKnopRect.sizeDelta = new Vector2(Screen.safeArea.height / 12, Screen.safeArea.height / 12);
            uitlegOpenKnopRect.anchoredPosition = new Vector2(-(Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Screen.safeArea.height / 12 * 0.6f), -(Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Screen.safeArea.height / 12 * 0.6f));
            menuShowKnopRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + menuShowKnopRect.sizeDelta.y / 2f, Screen.height / 2f);
            menuShowKnopRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.1f, Screen.safeArea.width * 0.04f);
            menuShowKnopRect.localEulerAngles = new Vector3(0, 0, 90);
            terugNaarMenuKnopRect.anchoredPosition = new Vector2(10000000, 1000000);
            MVvak.transform.position = new Vector3((schermWijdte * 0.1f) - (uitlegKnopGrootte * 1.25f) + ((buitenSafezoneLinks - buitenSafezoneRechts) / 2f), (buitenSafezoneOnder - buitenSafezoneBoven) / 2f, -0.5f);
            MVvak.transform.localScale = new Vector3(breedteMvVakje * 22, breedteMvVakje * 16, 1);
            float menuutjeX = (schermWijdte * 0.1f) - (schermWijdte / 2f);
            vlagOfSchepKnop.transform.position = new Vector3(menuutjeX + ((buitenSafezoneLinks - buitenSafezoneRechts) / 2f), ((buitenSafezoneOnder - buitenSafezoneBoven) / 2f) + (schermHoogte * -0.2f), -5);
            bommenTeGaanObj.transform.position = new Vector3(menuutjeX + ((buitenSafezoneLinks - buitenSafezoneRechts) / 2f), ((buitenSafezoneOnder - buitenSafezoneBoven) / 2f) + (schermHoogte * 0.2f), 0);
        }
        float scale = Mathf.Min(Mathf.Max(schermHoogte, schermWijdte) * .5f, Mathf.Min(schermHoogte, schermWijdte)) * 0.025f * 0.9f;
        Vector3 lScale = new Vector3(scale, scale, 1);
        vlagOfSchepKnop.localScale = lScale;
        bommenTeGaanObj.transform.localScale = lScale / 2f;
        mvScript.mvSpeelveldLinks = MVvak.position.x - (breedteMvVakje * langeKant / 2);
        mvScript.mvSpeelveldRechts = MVvak.position.x + (breedteMvVakje * langeKant / 2);
        mvScript.mvSpeelveldBoven = MVvak.position.y + (breedteMvVakje * korteKant / 2);
        mvScript.mvSpeelveldOnder = MVvak.position.y - (breedteMvVakje * korteKant / 2);
    }

    // Update is called once per frame
    private void Update()
    {
        if (mvScript.gehaaldCanvas.activeInHierarchy)
        {
            if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
            {
                if (klaar < 3)
                {
                    mvScript.OpenGehaaldCanvas(!mvScript.GameOver);
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
                MVKnoppenScript.OpenUitleg();
                MVKnoppenScript.OpenUitleg();
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
                    MVKnoppenScript.OpenUitleg();
                    MVKnoppenScript.OpenUitleg();
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
