using GooglePlayGames;
using UnityEngine;

public class PlayGamesSetup : MonoBehaviour
{
    public void StartSetup()
    {
        Activate();
        GetComponent<PlayGamesLogin>().StartupLogin();
    }

    public void Activate()
    {
        PlayGamesPlatform.Activate();
    }
}