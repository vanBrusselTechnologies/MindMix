using Firebase.Auth;
using UnityEngine;

public class FireBaseAuth : MonoBehaviour
{
    private SaveScript _saveScript;
    private FireBaseSetup _fireBaseSetup;
    private FirebaseAuth _auth;

    private void Start()
    {
        _saveScript = SaveScript.Instance;
        _fireBaseSetup = GetComponent<FireBaseSetup>();
    }

    private bool _firstReadyFrame = true;
    private void Update()
    {
        if (_fireBaseSetup.ready && _firstReadyFrame)
        {
            _auth = FirebaseAuth.DefaultInstance;
            _firstReadyFrame = false;
            FirebaseUser user = _auth.CurrentUser;
            if (user != null)
            {
                RefreshLogin();
            }
        }
    }

    private void RefreshLogin()
    {
        FirebaseUser user = _auth.CurrentUser;
        user.ReloadAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception?.InnerException?.Message);
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (user != null)
            {
                _saveScript.UpdateData();
            }
            else
            {
                _firstReadyFrame = true;
            }
        });
    }

    public void PlayGamesLogin(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception?.InnerException?.Message);
            }
            _saveScript.IntDict["laatsteXOffline"] = 0;
            _saveScript.UpdateData();
        });
    }

    /*public void LogUit()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        RefreshLogin();
    }*/

    /*public void UnlinkPlayGames(InstellingenScript script = null)
    {
        auth.CurrentUser.UnlinkAsync(PlayGamesAuthProvider.ProviderId).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.InnerException.Message);
            }
        });
    }*/
}
