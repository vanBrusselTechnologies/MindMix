using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartupAnimation : MonoBehaviour
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
    public List<Vector3> endPosRot2048;
    public List<Vector3> endPosRotMijnenveger;
    public List<Vector3> endPosRotSolitaire;
    public List<Vector3> endPosRotSudoku;

    private float tijdTeWachten = 0.025f;
    private WaitForSecondsRealtime wachtff;
    [HideInInspector] public bool finished = false;

    StartupScreenLayout screenLayout;

    void Start()
    {
        screenLayout = GetComponent<StartupScreenLayout>();
        finished = PlayerPrefs.GetInt("voorplaatFilmpjeGehad", 0) == 1;
        if (!finished)
        {
            vpSpellenHouderRect.localScale = Vector3.one * Mathf.Min(screenLayout.screenSafeAreaHeight / 350f, screenLayout.screenSafeAreaWidth / 350f);
            vp2048Rect.anchoredPosition = startPosRot[0];
            vp2048Rect.localEulerAngles = startPosRot[1];
            vpMijnenvegerRect.anchoredPosition = startPosRot[0];
            vpMijnenvegerRect.localEulerAngles = startPosRot[1];
            vpSolitaireRect.anchoredPosition = startPosRot[0];
            vpSolitaireRect.localEulerAngles = startPosRot[1];
            vpSudokuRect.anchoredPosition = startPosRot[0];
            vpSudokuRect.localEulerAngles = startPosRot[1];
            vpSpelNaamRect.anchoredPosition = new Vector3(0, (screenLayout.screenSafeAreaHeight * 0.25f) + (screenLayout.screenSafeAreaX / 2f), 0);
            vpSpelNaamRect.localEulerAngles = Vector3.zero;
            vpSpelNaamRect.sizeDelta = new Vector2(screenLayout.screenSafeAreaWidth * 0.25f, screenLayout.screenSafeAreaHeight / 2.05f);
            Color naamKleur = Color.red;
            naamKleur.a = 0;
            vpSpelNaamImg.color = naamKleur;
            wachtff = new WaitForSecondsRealtime(tijdTeWachten);
            bool portaitMode = screenLayout.screenSafeAreaWidth < screenLayout.screenSafeAreaHeight;
            Screen.autorotateToLandscapeRight = !portaitMode;
            Screen.autorotateToLandscapeLeft = !portaitMode;
            Screen.autorotateToPortrait = portaitMode;
            Screen.autorotateToPortraitUpsideDown = portaitMode;
        }
        StartCoroutine(playAnimation());
    }

    float i = 0f;
    float secPerAnim = .75f;
    bool isActive = false;
    IEnumerator playAnimation()
    {
        if (finished) i = float.MaxValue;
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
            vp2048Rect.anchoredPosition = new Vector2(((endPosRot2048[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((endPosRot2048[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vp2048Rect.localEulerAngles = new Vector3(endPosRot2048[1].x * (tmpI / secPerAnim), endPosRot2048[1].y * (tmpI / secPerAnim), endPosRot2048[1].z * (tmpI / secPerAnim));
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
            vpMijnenvegerRect.anchoredPosition = new Vector2(((endPosRotMijnenveger[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((endPosRotMijnenveger[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vpMijnenvegerRect.localEulerAngles = new Vector3(endPosRotMijnenveger[1].x * (tmpI / secPerAnim), endPosRotMijnenveger[1].y * (tmpI / secPerAnim), endPosRotMijnenveger[1].z * (tmpI / secPerAnim));
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
            vpSolitaireRect.anchoredPosition = new Vector2(((endPosRotSolitaire[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((endPosRotSolitaire[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vpSolitaireRect.localEulerAngles = new Vector3(endPosRotSolitaire[1].x * (tmpI / secPerAnim), endPosRotSolitaire[1].y * (tmpI / secPerAnim), endPosRotSolitaire[1].z * (tmpI / secPerAnim));
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
            vpSudokuRect.anchoredPosition = new Vector2(((endPosRotSudoku[0].x - startPosRot[0].x) * (tmpI / secPerAnim)) + startPosRot[0].x, ((endPosRotSudoku[0].y - startPosRot[0].y) * (tmpI / secPerAnim)) + startPosRot[0].y);
            vpSudokuRect.localEulerAngles = new Vector3(endPosRotSudoku[1].x * (tmpI / secPerAnim), endPosRotSudoku[1].y * (tmpI / secPerAnim), endPosRotSudoku[1].z * (tmpI / secPerAnim));
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
            FinishAnimation();
            PlayerPrefs.SetInt("voorplaatFilmpjeGehad", 1);
            yield break;
        }
        yield return wachtff;
        i += tijdTeWachten;
        i = Mathf.Round(i * 100f) / 100f;
        yield return playAnimation();
    }

    private void FinishAnimation()
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
        vp2048Text.alpha = 0;
        vpMijnenvegerText.alpha = 0;
        vpSolitaireText.alpha = 0;
        vpSudokuText.alpha = 0;
        vpSpelNaamImg.color = Color.red;
        isActive = false;
        screenLayout.SetLayout();
    }
}
