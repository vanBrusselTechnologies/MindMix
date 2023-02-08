using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class FireBaseSetup : MonoBehaviour
{
    [HideInInspector]
    public bool ready, offline;
    private bool _firstReadyFrame = true;

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this);
        FireBaseLogin();
    }

    // Update is called once per frame
    private void Update()
    {
        if (ready && _firstReadyFrame)
        {
            _firstReadyFrame = false;
            FireBaseSettings();
            GetComponent<PlayGamesSetup>().StartSetup();
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
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private static void FireBaseSettings()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.LanguageCode = LocalizationSettings.SelectedLocale.Identifier.Code;
    }
}
