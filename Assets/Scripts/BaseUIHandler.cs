using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-49)]
public abstract class BaseUIHandler : MonoBehaviour
{
    protected GegevensHouder gegevensHouder;
    protected SaveScript saveScript;

    [SerializeField] protected BaseLayout baseLayout;

    [Header("Menu UI")]
    [SerializeField] protected GameObject menuUIObj;
    [SerializeField] protected Transform menuUITransform;
    [SerializeField] protected RectTransform menuUIRect;
    [SerializeField] protected Transform showMenuButtonTransform;
    [SerializeField] protected RectTransform showMenuButtonRect;
    [SerializeField] protected RectTransform backToMenuButtonRect;
    [SerializeField] protected RectTransform menuNewGameButtonRect;
    [SerializeField] protected RectTransform menuNewGameOptionRect;

    [Header("Canvases")]
    [SerializeField] protected GameObject helpUICanvasObj;
    [SerializeField] protected GameObject generalCanvasObj;
    [SerializeField] protected GameObject menuCanvasObj;
    [SerializeField] protected GameObject gameSpecificRootObj;
    [SerializeField] protected GameObject settingsCanvasObj;
    [SerializeField] protected GameObject finishedGameUIObj;

    protected virtual void Start()
    {
        saveScript = SaveScript.Instance;
        if (saveScript == null) return;
        gegevensHouder = GegevensHouder.Instance;
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsCanvasObj != null && settingsCanvasObj.activeInHierarchy) OpenSettings();
            else if (helpUICanvasObj != null && helpUICanvasObj.activeInHierarchy) OpenHelpUI();
            else BackToMenu();
        }
    }

    public virtual void BackToMenu()
    {
        SceneManager.LoadScene("GameChoiceMenu");
    }

    public void OpenMenu()
    {
        bool vertical = baseLayout.screenSafeAreaWidth < baseLayout.screenSafeAreaHeight;
        bool appear;
        if (showMenuButtonTransform.localEulerAngles == Vector3.zero || showMenuButtonTransform.localEulerAngles == new Vector3(0, 0, 270))
        {
            showMenuButtonTransform.localEulerAngles += new Vector3(0, 0, 180);
            appear = false;
        }
        else
        {
            showMenuButtonTransform.localEulerAngles -= new Vector3(0, 0, 180);
            appear = true;
        }
        if (appear)
        {
            menuUIObj.SetActive(true);
            float scale = Mathf.Min(Mathf.Min(baseLayout.screenSafeAreaHeight, baseLayout.screenSafeAreaWidth) / 1080f, Mathf.Max(baseLayout.screenSafeAreaHeight, baseLayout.screenSafeAreaWidth) / 2520f);
            if (vertical)
            {
                scale *= 1.1f;
                float menuUIRectSizeDeltaX = baseLayout.screenWidth;
                float menuUIRectSizeDeltaY = baseLayout.screenSafeAreaY + baseLayout.screenSafeAreaHeight * 0.15f;
                menuUIRect.sizeDelta = new Vector2(menuUIRectSizeDeltaX, menuUIRectSizeDeltaY);
                menuUIRect.anchoredPosition = new Vector2(0, -menuUIRectSizeDeltaY / 2f + baseLayout.screenSafeAreaY);
                float xNewGameButton = 0;
                if (menuNewGameOptionRect != null)
                {
                    xNewGameButton = menuUIRectSizeDeltaX / 4f;
                    menuNewGameOptionRect.anchoredPosition = new Vector2(-xNewGameButton, baseLayout.screenSafeAreaY / 2f);
                }
                menuNewGameButtonRect.anchoredPosition = new Vector2(xNewGameButton, baseLayout.screenSafeAreaY / 2f);
            }
            else
            {
                menuUIRect.sizeDelta = new Vector2(baseLayout.screenSafeAreaX + baseLayout.screenSafeAreaWidth * 0.2f, baseLayout.screenHeight);
                menuUIRect.anchoredPosition = new Vector2(baseLayout.screenSafeAreaX - menuUIRect.sizeDelta.x / 2f - (baseLayout.screenWidth / 2f), baseLayout.screenHeight / 2f);
                backToMenuButtonRect.transform.SetParent(menuUITransform);
                float size = Mathf.Min(Mathf.Max(baseLayout.screenSafeAreaWidth, baseLayout.screenSafeAreaHeight) / 12f, Mathf.Min(baseLayout.screenSafeAreaWidth, baseLayout.screenSafeAreaHeight) / 10f);
                backToMenuButtonRect.sizeDelta = Vector2.one * size;
                backToMenuButtonRect.anchoredPosition = new Vector2((size * 0.6f) - (menuUIRect.sizeDelta.x / 2f) + baseLayout.screenSafeAreaX, (baseLayout.screenHeight / 2f) - (baseLayout.screenSafeAreaY / 2f) - (size * 0.6f));
                float yNewGameButton = 0;
                if (menuNewGameOptionRect != null)
                {
                    yNewGameButton = -baseLayout.screenSafeAreaHeight / 8f;
                    menuNewGameOptionRect.anchoredPosition = new Vector2(baseLayout.screenSafeAreaX / 2f, baseLayout.screenSafeAreaHeight / 8f);
                }
                menuNewGameButtonRect.anchoredPosition = new Vector2(baseLayout.screenSafeAreaX / 2f, yNewGameButton);
            }
            menuNewGameButtonRect.localScale = new Vector3(scale, scale, 1);
            if (menuNewGameOptionRect != null)
                menuNewGameOptionRect.localScale = new Vector3(scale, scale, 1);
        }
        StartCoroutine(ShowMenu(appear, vertical));
    }

    private IEnumerator ShowMenu(bool appear, bool vertical)
    {
        const float speed = 50f;
        menuUIObj.SetActive(true);
        if (vertical)
        {
            if (appear)
            {
                showMenuButtonTransform.Translate(Vector3.up * speed);
                menuUITransform.Translate(Vector3.up * speed);
                if (menuUIRect.anchoredPosition.y > menuUIRect.sizeDelta.y / 2f)
                {
                    float sizeDeltaY = menuUIRect.sizeDelta.y;
                    showMenuButtonRect.anchoredPosition = new Vector2(0, sizeDeltaY + showMenuButtonRect.sizeDelta.y / 2f);
                    menuUIRect.anchoredPosition = new Vector2(0, sizeDeltaY / 2f);
                    StopAllCoroutines();
                    yield break;
                }
            }
            else
            {
                showMenuButtonTransform.Translate(1.5f * speed * Vector3.up);
                menuUITransform.Translate(1.5f * speed * Vector3.down);
                if (menuUIRect.anchoredPosition.y < -menuUIRect.sizeDelta.y / 2f + Screen.safeArea.y)
                {
                    showMenuButtonRect.anchoredPosition = new Vector2(0, Screen.safeArea.y + showMenuButtonRect.sizeDelta.y / 2f);
                    menuUIObj.SetActive(false);
                    StopAllCoroutines();
                    yield break;
                }
            }
        }
        else
        {
            if (appear)
            {
                showMenuButtonTransform.Translate(Vector3.up * speed);
                menuUITransform.Translate(Vector3.right * speed);
                if (menuUIRect.anchoredPosition.x > menuUIRect.sizeDelta.x / 2f - (Screen.width / 2f))
                {
                    float sizeDeltaX = menuUIRect.sizeDelta.x;
                    showMenuButtonRect.anchoredPosition = new Vector2(sizeDeltaX - (Screen.width / 2f) + showMenuButtonRect.sizeDelta.y / 2f, Screen.height / 2f);
                    menuUIRect.anchoredPosition = new Vector2((sizeDeltaX / 2f) - (Screen.width / 2f), Screen.height / 2f);
                    StopAllCoroutines();
                    yield break;
                }
            }
            else
            {
                showMenuButtonTransform.Translate(1.5f * speed * Vector3.up);
                menuUITransform.Translate(1.5f * speed * Vector3.left);
                if (menuUIRect.anchoredPosition.x < -menuUIRect.sizeDelta.x / 2f - (Screen.width / 2f) + Screen.safeArea.x)
                {
                    showMenuButtonRect.anchoredPosition = new Vector2(Screen.safeArea.x - (Screen.width / 2f) + showMenuButtonRect.sizeDelta.y / 2f, Screen.height / 2f);
                    menuUIObj.SetActive(false);
                    StopAllCoroutines();
                    yield break;
                }
            }
        }
        yield return gegevensHouder.wachtHonderdste;
        StartCoroutine(ShowMenu(appear, vertical));
    }

    public virtual void OpenHelpUI()
    {
        bool helpUIActive = helpUICanvasObj.activeSelf;
        helpUICanvasObj.SetActive(!helpUIActive);
        gameSpecificRootObj.SetActive(helpUIActive);
        generalCanvasObj.SetActive(helpUIActive);
        menuCanvasObj.SetActive(helpUIActive);
        baseLayout.SetLayout();
    }

    public virtual void OpenSettings()
    {
        bool settingObjActive = settingsCanvasObj.activeSelf;
        settingsCanvasObj.SetActive(!settingObjActive);
        gameSpecificRootObj.SetActive(settingObjActive);
        generalCanvasObj.SetActive(settingObjActive);
        menuCanvasObj.SetActive(settingObjActive);
        baseLayout.SetLayout();
    }

    public virtual void StartNewGame()
    {
        gegevensHouder.startNewGame = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) return;
        if (helpUICanvasObj != null && gameSpecificRootObj.activeInHierarchy) OpenHelpUI();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) return;
        if (helpUICanvasObj != null && gameSpecificRootObj.activeInHierarchy) OpenHelpUI();
    }
}