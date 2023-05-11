using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

public class PlayGamesSetup : MonoBehaviour
{
    [SerializeField] private PlayGamesLogin playGamesLogin;

    public void StartSetup()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.DebugLogEnabled = Debug.unityLogger.logEnabled;
        PlayGamesPlatform.Activate();
        playGamesLogin.StartupLogin();
#endif
    }
}