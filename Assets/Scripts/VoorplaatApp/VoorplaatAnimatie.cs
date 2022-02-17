using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoorplaatAnimatie : MonoBehaviour
{
    [SerializeField] private Image vpSpelNaamImg;
    [SerializeField] private RectTransform vpSpelNaamRect;
    [SerializeField] private RectTransform vpSpellenHouderRect;
    [SerializeField] private RectTransform vp2048Rect;
    [SerializeField] private RectTransform vpMijnenvegerRect;
    [SerializeField] private RectTransform vpSolitaireRect;
    [SerializeField] private RectTransform vpSudokuRect;
    [SerializeField] private TMP_Text vp2048Text;
    [SerializeField] private TMP_Text vpMijnenvegerText;
    [SerializeField] private TMP_Text vpSolitaireText;
    [SerializeField] private TMP_Text vpSudokuText;
    [SerializeField] private List<Vector3> startPosRot;
    [SerializeField] private List<Vector3> eindPosRot2048;
    [SerializeField] private List<Vector3> eindPosRotMijnenveger;
    [SerializeField] private List<Vector3> eindPosRotSolitaire;
    [SerializeField] private List<Vector3> eindPosRotSudoku;
    [SerializeField] private RectTransform loginKnopScalerRect;
    [SerializeField] private RectTransform googlePlayGamesLoginRect;

    private float tijdTeWachten = 0.025f;
    private WaitForSecondsRealtime wachtff;
    [HideInInspector] public bool finished = false;

    private int klaar = 0;
    private float vorigeScreenWidth;
    private float vorigeSafezoneY;
    private float vorigeSafezoneX;

    void Start()
    {
        finished = PlayerPrefs.GetInt("voorplaatFilmpjeGehad", 0) == 1;
        vpSpellenHouderRect.localScale = Vector3.one * Mathf.Min(Screen.safeArea.height / 350f, Screen.safeArea.width / 350f);
        vp2048Rect.anchoredPosition = startPosRot[0];
        vp2048Rect.localEulerAngles = startPosRot[1];
        vpMijnenvegerRect.anchoredPosition = startPosRot[0];
        vpMijnenvegerRect.localEulerAngles = startPosRot[1];
        vpSolitaireRect.anchoredPosition = startPosRot[0];
        vpSolitaireRect.localEulerAngles = startPosRot[1];
        vpSudokuRect.anchoredPosition = startPosRot[0];
        vpSudokuRect.localEulerAngles = startPosRot[1];
        vpSpelNaamRect.anchoredPosition = new Vector3(0, (Screen.safeArea.height * 0.25f) + (Screen.safeArea.x / 2f), 0);
        vpSpelNaamRect.localEulerAngles = Vector3.zero;
        vpSpelNaamRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.25f, Screen.safeArea.height / 2.05f);
        Color naamKleur = Color.red;
        naamKleur.a = 0;
        vpSpelNaamImg.color = naamKleur;
        wachtff = new WaitForSecondsRealtime(tijdTeWachten);
        if (finished)
        {
            vp2048Rect.gameObject.SetActive(true);
            vpMijnenvegerRect.gameObject.SetActive(true);
            vpSolitaireRect.gameObject.SetActive(true);
            vpSudokuRect.gameObject.SetActive(true);
            vpSpelNaamRect.gameObject.SetActive(true);
            i = float.MaxValue;
        }
        else
        {
            if (Screen.safeArea.width < Screen.safeArea.height)
            {
                Screen.autorotateToLandscapeRight = false;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = true;
            }
            else
            {
                Screen.autorotateToLandscapeRight = true;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
            }
        }
        StartCoroutine(speelAnimatie());
    }

    private void Update()
    {
        if (!finished) return;
        if (vorigeScreenWidth == Screen.width && vorigeSafezoneY == Screen.safeArea.y && vorigeSafezoneX == Screen.safeArea.x)
        {
            if (klaar < 2)
            {
                klaar += 1;
                if (klaar == 1)
                {
                    return;
                }
                i = float.MaxValue;
                StartCoroutine(speelAnimatie());
            }
            return;
        }
        klaar = 0;
        vorigeScreenWidth = Screen.width;
        vorigeSafezoneY = Screen.safeArea.y;
        vorigeSafezoneX = Screen.safeArea.x;
    }

    float i = 0f;
    float secPerAnim = .75f;
    bool isActive = false;
    IEnumerator speelAnimatie()
    {
        float tmpI = i;
        if (i <= secPerAnim)
        {
            if (!isActive)
            {
                vp2048Rect.gameObject.SetActive(true);
                isActive = true;
            }
            vp2048Rect.localScale = Vector3.one * (tmpI / secPerAnim);
        }            //2048
        else if (i <= secPerAnim * 2)
        {
            tmpI -= secPerAnim;
            isActive = false;
            vp2048Rect.anchoredPosition = new Vector2(((eindPosRot2048[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((eindPosRot2048[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vp2048Rect.localEulerAngles = new Vector3(eindPosRot2048[1].x * (tmpI / secPerAnim), eindPosRot2048[1].y * (tmpI / secPerAnim), eindPosRot2048[1].z * (tmpI / secPerAnim));
            vp2048Text.alpha = 1 - (tmpI / secPerAnim);
        }
        else if (i <= secPerAnim * 3)
        {
            tmpI -= secPerAnim * 2;
            if (!isActive)
            {
                vpMijnenvegerRect.gameObject.SetActive(true);
                isActive = true;
            }
            vpMijnenvegerRect.localScale = Vector3.one * (tmpI / secPerAnim);
        }   //Mijnenveger
        else if (i <= secPerAnim * 4)
        {
            tmpI -= secPerAnim * 3;
            isActive = false;
            vpMijnenvegerRect.anchoredPosition = new Vector2(((eindPosRotMijnenveger[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((eindPosRotMijnenveger[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vpMijnenvegerRect.localEulerAngles = new Vector3(eindPosRotMijnenveger[1].x * (tmpI / secPerAnim), eindPosRotMijnenveger[1].y * (tmpI / secPerAnim), eindPosRotMijnenveger[1].z * (tmpI / secPerAnim));
            vpMijnenvegerText.alpha = 1 - (tmpI / secPerAnim);
        }
        else if (i <= secPerAnim * 5)
        {
            tmpI -= secPerAnim * 4;
            if (!isActive)
            {
                vpSolitaireRect.gameObject.SetActive(true);
                isActive = true;
            }
            vpSolitaireRect.localScale = Vector3.one * (tmpI / secPerAnim);
        }   //Solitaire
        else if (i <= secPerAnim * 6)
        {
            tmpI -= secPerAnim * 5;
            isActive = false;
            vpSolitaireRect.anchoredPosition = new Vector2(((eindPosRotSolitaire[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((eindPosRotSolitaire[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vpSolitaireRect.localEulerAngles = new Vector3(eindPosRotSolitaire[1].x * (tmpI / secPerAnim), eindPosRotSolitaire[1].y * (tmpI / secPerAnim), eindPosRotSolitaire[1].z * (tmpI / secPerAnim));
            vpSolitaireText.alpha = 1 - (tmpI / secPerAnim);
        }
        else if (i <= secPerAnim * 7)
        {
            tmpI -= secPerAnim * 6;
            if (!isActive)
            {
                vpSudokuRect.gameObject.SetActive(true);
                isActive = true;
            }
            vpSudokuRect.localScale = Vector3.one * (tmpI / secPerAnim);
        }   //Sudoku
        else if (i <= secPerAnim * 8)
        {
            tmpI -= secPerAnim * 7;
            isActive = false;
            vpSudokuRect.anchoredPosition = new Vector2(((eindPosRotSudoku[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((eindPosRotSudoku[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vpSudokuRect.localEulerAngles = new Vector3(eindPosRotSudoku[1].x * (tmpI / secPerAnim), eindPosRotSudoku[1].y * (tmpI / secPerAnim), eindPosRotSudoku[1].z * (tmpI / secPerAnim));
            vpSudokuText.alpha = 1 - (tmpI / secPerAnim);
        }
        else if (i <= secPerAnim * 9)
        {
            tmpI -= secPerAnim * 8;
            if (!isActive)
            {
                vpSpelNaamRect.gameObject.SetActive(true);
                isActive = true;
            }
            Color naamKleur = Color.red;
            naamKleur.a = tmpI / secPerAnim;
            vpSpelNaamImg.color = naamKleur;
        }   //SpelNaam
        else
        {
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            finished = true;
            vp2048Rect.gameObject.SetActive(true);
            vpMijnenvegerRect.gameObject.SetActive(true);
            vpSolitaireRect.gameObject.SetActive(true);
            vpSudokuRect.gameObject.SetActive(true);
            vpSpelNaamRect.gameObject.SetActive(true);
            vpSpellenHouderRect.localScale = Vector3.one * Mathf.Min(Screen.safeArea.height / 350f, Screen.safeArea.width / 350f);
            vp2048Rect.anchoredPosition = eindPosRot2048[0];
            vp2048Rect.localEulerAngles = eindPosRot2048[1];
            vp2048Text.alpha = 0;
            vp2048Rect.localScale = Vector3.one;
            vpMijnenvegerRect.anchoredPosition = eindPosRotMijnenveger[0];
            vpMijnenvegerRect.localEulerAngles = eindPosRotMijnenveger[1];
            vpMijnenvegerText.alpha = 0;
            vpMijnenvegerRect.localScale = Vector3.one;
            vpSolitaireRect.anchoredPosition = eindPosRotSolitaire[0];
            vpSolitaireRect.localEulerAngles = eindPosRotSolitaire[1];
            vpSolitaireText.alpha = 0;
            vpSolitaireRect.localScale = Vector3.one;
            vpSudokuRect.anchoredPosition = eindPosRotSudoku[0];
            vpSudokuRect.localEulerAngles = eindPosRotSudoku[1];
            vpSudokuText.alpha = 0;
            vpSudokuRect.localScale = Vector3.one;
            vpSpelNaamRect.anchoredPosition = new Vector3(0, (Screen.safeArea.height * 0.25f) + (Screen.safeArea.x / 2f), 0);
            vpSpelNaamRect.localEulerAngles = Vector3.zero;
            vpSpelNaamRect.sizeDelta = new Vector2(Screen.safeArea.width * 0.25f, Screen.safeArea.height / 2.05f);
            vpSpelNaamImg.color = Color.red;
            isActive = false;
            loginKnopScalerRect.localScale = vpSpellenHouderRect.localScale;
            googlePlayGamesLoginRect.localScale = Vector3.one * (0.5f / loginKnopScalerRect.localScale.x);
            googlePlayGamesLoginRect.anchoredPosition = new Vector2(0, eindPosRot2048[0].y - (200f * googlePlayGamesLoginRect.localScale.y * 1.5f));
            PlayerPrefs.SetInt("voorplaatFilmpjeGehad", 1);
            StopAllCoroutines();
        }
        yield return wachtff;
        i += tijdTeWachten;
        i = Mathf.Round(i * 100f) / 100f;
        yield return speelAnimatie();
    }
}
