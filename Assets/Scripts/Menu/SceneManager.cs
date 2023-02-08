using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text loadingProgressText;

    public IEnumerator LoadSceneAsync(string scene)
    {
        float progress = 0;
        int checks = 0;

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            float progressNew = Mathf.Clamp01(asyncLoad.progress / .9f);
            if (checks == 0) checks++;
            if (checks == 1)
            {
                checks++;
                if (progressNew < 0.75f) loadingScreen.SetActive(true);
            }

            if (Math.Abs(progress - progressNew) > 0.005f)
            {
                Debug.Log($"Progress changed from {progress} to {progressNew}");
                progress = progressNew;
                progressBar.value = progress;
                loadingProgressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            }

            yield return null;
        }

        loadingScreen.SetActive(false);
        progressBar.value = 0;
    }

    public static void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static void LoadScene(int buildIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public static UnityEngine.SceneManagement.Scene GetActiveScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }
}