using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FireBaseSetup : MonoBehaviour
{
    [HideInInspector]
    public bool ready = false, offline = false;
    private bool eersteReadyFrame = true;

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this);
        FireBaseLogin();
    }

    // Update is called once per frame
    private void Update()
    {
        if (ready && eersteReadyFrame)
        {
            eersteReadyFrame = false;
            FireBaseSettings();
            GetComponent<PlayGamesSetup>().StartPlayGamesSetup();
            GetComponent<FireBaseDynamicLinks>().DynamicLinkSetup();
        }
    }

    private void FireBaseLogin()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                ready = true;
            }
            else
            {
                offline = true;
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void FireBaseSettings()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.LanguageCode = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
    }
}
