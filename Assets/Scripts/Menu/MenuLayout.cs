using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuLayout : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> spelKnoppenText;
    [SerializeField] private List<RectTransform> spelKnoppenRect;
    private List<RectTransform> knoppenRectSorted = new List<RectTransform>(0);
    private bool isPaused = false;
    private bool wasPaused = false;
    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;
    [SerializeField] private RectTransform instellingenKnopRect;
    [SerializeField] private RectTransform terugNaarOpenAppKnopRect;
    [SerializeField] private RectTransform supportKnopRect;
    [SerializeField] private RectTransform shopKnopRect;
    [SerializeField] private GameObject ConfirmCanvas;
    [SerializeField] private GameObject menuCanvas;

    // Start is called before the first frame update
    private void Start()
    {
        List<string> knopNamen = new List<string>();
        List<string> knopNamenNietSorted = new List<string>();
        for (int i = 0; i < spelKnoppenText.Count; i++)
        {
            string knopNaam = spelKnoppenText[i].text;
            knopNamen.Add(knopNaam);
            knopNamenNietSorted.Add(knopNaam);
        }
        knopNamen.Sort();
        for (int i = 0; i < knopNamen.Count; i++)
        {
            int index = knopNamenNietSorted.IndexOf(knopNamen[i]);
            knoppenRectSorted.Add(spelKnoppenRect[index]);
        }
        SetLayout();
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    private void SetLayout()
    {
        klaar += 1;
        menuCanvas.SetActive(true);
        ConfirmCanvas.SetActive(false);
        for(int i = 0; i < knoppenRectSorted.Count; i++)
        {
            knoppenRectSorted[i].sizeDelta = new Vector2(Screen.safeArea.width * 0.9f, Screen.safeArea.height * 0.9f * 0.9f / knoppenRectSorted.Count);
            float x = 0.5f * (Screen.safeArea.x - (Screen.width - Screen.safeArea.width - Screen.safeArea.x));
            float y = (Screen.safeArea.height * 0.75f / knoppenRectSorted.Count * (knoppenRectSorted.Count / 2)) - (Screen.safeArea.height * 0.85f / knoppenRectSorted.Count * i) + (.5f * (Screen.safeArea.y - (Screen.height - Screen.safeArea.height - Screen.safeArea.y))) - (Screen.safeArea.height * 0.1f);
            knoppenRectSorted[i].anchoredPosition = new Vector2(x, y);
        }
        Vector2 sizeDelta = Vector2.one * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f;
        instellingenKnopRect.sizeDelta = sizeDelta;
        instellingenKnopRect.anchoredPosition = new Vector2((Screen.width / 2) - (Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        shopKnopRect.sizeDelta = sizeDelta;
        shopKnopRect.anchoredPosition = new Vector2((Screen.width / 2) - (Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f) - (2f * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        supportKnopRect.sizeDelta = sizeDelta;
        supportKnopRect.anchoredPosition = new Vector2((Screen.width / 2) - (Screen.width - Screen.safeArea.width - Screen.safeArea.x) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f) - (4f * Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11f * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (Mathf.Min(Screen.safeArea.width, Screen.safeArea.height) / 11 * 0.6f));
        terugNaarOpenAppKnopRect.sizeDelta = sizeDelta;
        terugNaarOpenAppKnopRect.anchoredPosition = new Vector2((-Screen.width / 2) + Screen.safeArea.x + (sizeDelta.x * 0.6f), (Screen.height / 2) - (Screen.height - Screen.safeArea.height - Screen.safeArea.y) - (sizeDelta.y * 0.6f));
    }

    // Update is called once per frame
    private void Update()
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
}
