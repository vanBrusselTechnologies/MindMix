using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartupAnimation : MonoBehaviour
{
    [SerializeField] private Image vpGameLogoImg;
    [SerializeField] private RectTransform vpGameLogoRect;
    [SerializeField] private RectTransform vpSpellenHouderRect;
    [SerializeField] private RectTransform vp2048Rect;
    [SerializeField] private RectTransform vpMinesweeperRect;
    [SerializeField] private RectTransform vpSolitaireRect;
    [SerializeField] private RectTransform vpSudokuRect;
    [SerializeField] private TMP_Text vp2048Text;
    [SerializeField] private TMP_Text vpMinesweeperText;
    [SerializeField] private TMP_Text vpSolitaireText;
    [SerializeField] private TMP_Text vpSudokuText;
    [SerializeField] private List<Vector3> startPosRot;
    public List<Vector3> endPosRot2048;
    public List<Vector3> endPosRotMijnenveger;
    public List<Vector3> endPosRotSolitaire;
    public List<Vector3> endPosRotSudoku;

    private const float TimeToWait = 0.025f;
    private WaitForSecondsRealtime _wachtff;
    private bool _animPartSwitch;
    [HideInInspector] public bool finished;

    StartupScreenLayout _screenLayout;

    void Start()
    {
        Debug.Log("Dit moet weg of veranderd worden!");
        _screenLayout = GetComponent<StartupScreenLayout>();
        finished = PlayerPrefs.GetInt("voorplaatFilmpjeGehad", 0) == 1;
        if (!finished)
        {
            vpSpellenHouderRect.localScale = Vector3.one * Mathf.Min(_screenLayout.screenSafeAreaHeightInUnits / 350f,
                _screenLayout.screenSafeAreaWidthInUnits / 350f);
            vp2048Rect.anchoredPosition = startPosRot[0];
            vp2048Rect.localEulerAngles = startPosRot[1];
            vpMinesweeperRect.anchoredPosition = startPosRot[0];
            vpMinesweeperRect.localEulerAngles = startPosRot[1];
            vpSolitaireRect.anchoredPosition = startPosRot[0];
            vpSolitaireRect.localEulerAngles = startPosRot[1];
            vpSudokuRect.anchoredPosition = startPosRot[0];
            vpSudokuRect.localEulerAngles = startPosRot[1];
            vpGameLogoRect.anchoredPosition = new Vector3(0, (_screenLayout.screenSafeAreaXInUnits / 2f), 0);
            vpGameLogoRect.localEulerAngles = Vector3.zero;
            vpGameLogoRect.sizeDelta = new Vector2(_screenLayout.screenSafeAreaWidthInUnits * 0.75f,
                _screenLayout.screenSafeAreaHeightInUnits / 2.05f);
            Color nameColor = Color.red;
            nameColor.a = 0;
            vpGameLogoImg.color = nameColor;
            _wachtff = new WaitForSecondsRealtime(TimeToWait);
            bool portraitMode = _screenLayout.screenSafeAreaWidth < _screenLayout.screenSafeAreaHeight;
            Screen.autorotateToLandscapeRight = !portraitMode;
            Screen.autorotateToLandscapeLeft = !portraitMode;
            Screen.autorotateToPortrait = portraitMode;
            Screen.autorotateToPortraitUpsideDown = portraitMode;
        }

        StartCoroutine(PlayAnimation());
    }

    float i;
    float secPerAnim = .75f;
    bool isActive;

    IEnumerator PlayAnimation()
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

            float progress = tmpI / secPerAnim;
            vp2048Text.alpha = progress;
            vp2048Rect.localScale = Vector3.one * progress;
        } //2048
        else if (i <= secPerAnim * 1.5f)
        {
            if (!_animPartSwitch)
            {
                vp2048Rect.localScale = Vector3.one;
                _animPartSwitch = true;
            }

            tmpI -= secPerAnim;
            isActive = false;
            float progress = 2 * (tmpI / secPerAnim);
            vp2048Rect.anchoredPosition =
                new Vector2(((endPosRot2048[0].x - startPosRot[0].x) * progress) + startPosRot[0].x,
                    ((endPosRot2048[0].y - startPosRot[0].y) * progress) + startPosRot[0].y);
            vp2048Rect.localEulerAngles = new Vector3(endPosRot2048[1].x * progress, endPosRot2048[1].y * progress,
                endPosRot2048[1].z * progress);
            vp2048Text.alpha = 1 - progress;
        }
        else if (i <= secPerAnim * 2.5f)
        {
            if (_animPartSwitch)
            {
                vp2048Rect.anchoredPosition = endPosRot2048[0];
                vp2048Rect.localEulerAngles = endPosRot2048[1];
                vp2048Text.alpha = 0;
                _animPartSwitch = false;
            }

            tmpI -= secPerAnim * 1.5f;
            float progress = tmpI / secPerAnim;
            if (!isActive)
            {
                vpMinesweeperRect.gameObject.SetActive(true);
                isActive = true;
            }

            vpMinesweeperText.alpha = progress;
            vpMinesweeperRect.localScale = Vector3.one * progress;
        } //Mijnenveger
        else if (i <= secPerAnim * 3f)
        {
            if (!_animPartSwitch)
            {
                vpMinesweeperRect.localScale = Vector3.one;
                _animPartSwitch = true;
            }

            tmpI -= secPerAnim * 2.5f;
            float progress = 2 * (tmpI / secPerAnim);
            isActive = false;
            vpMinesweeperRect.anchoredPosition =
                new Vector2(((endPosRotMijnenveger[0].x - startPosRot[0].x) * progress) + startPosRot[0].x,
                    ((endPosRotMijnenveger[0].y - startPosRot[0].y) * progress) + startPosRot[0].y);
            vpMinesweeperRect.localEulerAngles = new Vector3(endPosRotMijnenveger[1].x * progress,
                endPosRotMijnenveger[1].y * progress, endPosRotMijnenveger[1].z * progress);
            vpMinesweeperText.alpha = 1 - progress;
        }
        else if (i <= secPerAnim * 4f)
        {
            if (_animPartSwitch)
            {
                vpMinesweeperRect.anchoredPosition = endPosRotMijnenveger[0];
                vpMinesweeperRect.localEulerAngles = endPosRotMijnenveger[1];
                vpMinesweeperText.alpha = 0;
                _animPartSwitch = false;
            }

            tmpI -= secPerAnim * 3f;
            float progress = tmpI / secPerAnim;
            if (!isActive)
            {
                vpSolitaireRect.gameObject.SetActive(true);
                isActive = true;
            }

            vpSolitaireText.alpha = progress;
            vpSolitaireRect.localScale = Vector3.one * progress;
        } //Solitaire
        else if (i <= secPerAnim * 4.5f)
        {
            if (!_animPartSwitch)
            {
                vpSolitaireRect.localScale = Vector3.one;
                _animPartSwitch = true;
            }

            vpSolitaireRect.localScale = Vector3.one;
            tmpI -= secPerAnim * 4;
            float progress = 2 * (tmpI / secPerAnim);
            isActive = false;
            vpSolitaireRect.anchoredPosition =
                new Vector2(((endPosRotSolitaire[0].x - startPosRot[0].x) * progress) + startPosRot[0].x,
                    ((endPosRotSolitaire[0].y - startPosRot[0].y) * progress) + startPosRot[0].y);
            vpSolitaireRect.localEulerAngles = new Vector3(endPosRotSolitaire[1].x * progress,
                endPosRotSolitaire[1].y * progress, endPosRotSolitaire[1].z * progress);
            vpSolitaireText.alpha = 1 - 2f * progress;
        }
        else if (i <= secPerAnim * 5.5f)
        {
            if (_animPartSwitch)
            {
                vpSolitaireRect.anchoredPosition = endPosRotSolitaire[0];
                vpSolitaireRect.localEulerAngles = endPosRotSolitaire[1];
                vpSolitaireText.alpha = 0;
                _animPartSwitch = false;
            }

            tmpI -= secPerAnim * 4.5f;
            float progress = tmpI / secPerAnim;
            if (!isActive)
            {
                vpSudokuRect.gameObject.SetActive(true);
                isActive = true;
            }

            vpSudokuText.alpha = progress;
            vpSudokuRect.localScale = Vector3.one * progress;
        } //Sudoku
        else if (i <= secPerAnim * 6f)
        {
            if (!_animPartSwitch)
            {
                vpSudokuRect.localScale = Vector3.one;
                _animPartSwitch = true;
            }

            vpSudokuRect.localScale = Vector3.one;
            tmpI -= secPerAnim * 5.5f;
            float progress = 2 * (tmpI / secPerAnim);
            isActive = false;
            vpSudokuRect.anchoredPosition =
                new Vector2(((endPosRotSudoku[0].x - startPosRot[0].x) * progress) + startPosRot[0].x,
                    ((endPosRotSudoku[0].y - startPosRot[0].y) * progress) + startPosRot[0].y);
            vpSudokuRect.localEulerAngles = new Vector3(endPosRotSudoku[1].x * progress,
                endPosRotSudoku[1].y * progress, endPosRotSudoku[1].z * progress);
            vpSudokuText.alpha = 1 - progress;
        }
        else if (i <= secPerAnim * 7f)
        {
            if (_animPartSwitch)
            {
                vpSudokuRect.anchoredPosition = endPosRotSudoku[0];
                vpSudokuRect.localEulerAngles = endPosRotSudoku[1];
                vpSudokuText.alpha = 0;
                _animPartSwitch = false;
            }

            tmpI -= secPerAnim * 6f;
            float progress = tmpI / secPerAnim;
            if (!isActive)
            {
                vpGameLogoRect.gameObject.SetActive(true);
                isActive = true;
            }

            Color nameColor = Color.red;
            nameColor.a = progress;
            vpGameLogoImg.color = nameColor;
        } //GameLogo
        else
        {
            if (!_animPartSwitch)
            {
                vpGameLogoImg.color = Color.red;
                _animPartSwitch = true;
            }

            FinishAnimation();
            PlayerPrefs.SetInt("voorplaatFilmpjeGehad", 1);
            yield break;
        }

        yield return _wachtff;
        i += Mathf.Min(0.499f, Mathf.Max(TimeToWait, Time.deltaTime));
        i = Mathf.Round(i * 100f) / 100f;
        yield return PlayAnimation();
    }

    private void FinishAnimation()
    {
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        finished = true;
        vp2048Rect.gameObject.SetActive(true);
        vpMinesweeperRect.gameObject.SetActive(true);
        vpSolitaireRect.gameObject.SetActive(true);
        vpSudokuRect.gameObject.SetActive(true);
        vpGameLogoRect.gameObject.SetActive(true);
        vp2048Text.alpha = 0;
        vpMinesweeperText.alpha = 0;
        vpSolitaireText.alpha = 0;
        vpSudokuText.alpha = 0;
        vpGameLogoImg.color = Color.red;
        isActive = false;
        _screenLayout.SetLayout();
    }
}