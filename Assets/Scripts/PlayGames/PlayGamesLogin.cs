using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using VBG.UnityAndroid;

public class PlayGamesLogin : MonoBehaviour
{
    [SerializeField] private FireBaseAuth fireBaseAuth;
    private bool _waitingForNetwork;

    private enum AuthenticationMethod
    {
        Automatic,
        Manual
    }

    private AuthenticationMethod _method;

    public void StartupLogin()
    {
        _method = AuthenticationMethod.Automatic;
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ManualLogin()
    {
        _method = AuthenticationMethod.Manual;
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }


    private void ProcessAuthentication(SignInStatus status)
    {
        if (status != SignInStatus.Success)
        {
            if (_method == AuthenticationMethod.Automatic && fireBaseAuth.CurrentUser == null) ManualLogin();
            return;
        }

        PlayGamesPlatform.Instance.RequestServerSideAccess(false,
            code =>
            {
                if (code == null)
                {
                    StartCoroutine(WaitForNetwork());
                    new AndroidToast().MakeText("MindMix is offline", AndroidToast.DurationType.LengthLong).Show();
                }
                else
                {
                    fireBaseAuth.PlayGamesLogin(code);
                }
            }
        );
    }

    System.Collections.IEnumerator WaitForNetwork()
    {
        if (_waitingForNetwork) yield break;
        _waitingForNetwork = true;
        while (true)
        {
            yield return new WaitForSecondsRealtime(10f);
            if (Application.internetReachability == NetworkReachability.NotReachable) yield return null;
            else break;
        }

        if (_method == AuthenticationMethod.Automatic) StartupLogin();
        else ManualLogin();
        _waitingForNetwork = false;
    }
}