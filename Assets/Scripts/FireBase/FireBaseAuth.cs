using Firebase.Auth;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class FireBaseAuth : MonoBehaviour
{
    public FirebaseUser CurrentUser => _auth?.CurrentUser;
    public bool Connected => CurrentUser != null;

    private SaveScript _saveScript;
    private FirebaseAuth _auth;

    private bool _waitingForNetwork;

    public void OnFirebaseReady()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _auth.LanguageCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        if (Connected) RefreshLogin();
    }

    private void RefreshLogin()
    {
        CurrentUser.ReloadAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception?.InnerException?.Message);
                return;
            }

            _saveScript = SaveScript.Instance;
            Debug.LogWarning("MUST BE CHANGED");
            _saveScript.IntDict["laatsteXOffline"] = 0;
            _saveScript.UpdateData();
        });
    }

    public void PlayGamesLogin(string authCode)
    {
        if (Connected && !CurrentUser.IsAnonymous) return;
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception?.InnerException?.Message);
                return;
            }

            _saveScript = SaveScript.Instance;
            Debug.LogWarning("MUST BE CHANGED");
            _saveScript.IntDict["laatsteXOffline"] = 0;
            _saveScript.UpdateData();
        });
    }
}