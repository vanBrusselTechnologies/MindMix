using UnityEngine;
using Firebase.Auth;

public class FireBaseAuth : MonoBehaviour
{
    private SaveScript saveScript;
    private FireBaseSetup fireBaseSetup;
    private FirebaseAuth auth;

    private void Start()
    {
        saveScript = GetComponent<SaveScript>();
        fireBaseSetup = GetComponent<FireBaseSetup>();
    }

    private bool eersteReadyFrame = true;
    private void Update()
    {
        if (fireBaseSetup.ready && eersteReadyFrame)
        {
            auth = FirebaseAuth.DefaultInstance;
            eersteReadyFrame = false;
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                RefreshLogin();
            }
        }
    }

    private void RefreshLogin()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;
        user.ReloadAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.InnerException.Message);
            }
            if (user != null)
            {
                saveScript.UpdateData();
            }
            else
            {
                eersteReadyFrame = true;
            }
        });
    }

    public void PlayGamesLogin(string authCode)
    {
        Debug.Log("Get Credential: PlayGamesAuthProvider.GetCredential(authCode)");
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.InnerException.Message);
            }
            Debug.Log("task succesfull");
            saveScript.intDict["laatsteXOffline"] = 0;
            saveScript.UpdateData();
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
