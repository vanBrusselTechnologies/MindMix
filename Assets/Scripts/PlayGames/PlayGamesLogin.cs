using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class PlayGamesLogin : MonoBehaviour
{
    [SerializeField] private FireBaseAuth fireBaseAuth;
    private GameObject _playGamesLoginObject;
    private bool _playGamesLoginObjectSet;

    public GameObject PlayGamesLoginObject
    {
        set
        {
            _playGamesLoginObject = value;
            _playGamesLoginObjectSet = true;
        }
    }

    public void StartupLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ManualLogin()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }


    private void ProcessAuthentication(SignInStatus status)
    {
        if (status != SignInStatus.Success) return;
        PlayGamesPlatform.Instance.RequestServerSideAccess(false,
            code =>
            {
                fireBaseAuth.PlayGamesLogin(code);
                if(_playGamesLoginObjectSet)
                    _playGamesLoginObject.SetActive(false);
            });
    }
}