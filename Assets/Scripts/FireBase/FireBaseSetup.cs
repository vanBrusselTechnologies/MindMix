using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FireBaseSetup : MonoBehaviour
{
    [SerializeField] private PlayGamesSetup playGamesSetup;
    [SerializeField] private FireBaseDynamicLinks fireBaseDynamicLinks;
    [SerializeField] private FireBaseAuth fireBaseAuth;
    [SerializeField] private FireBaseMessages fireBaseMessages;

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this);
        FireBaseLogin();
    }

    private void FireBaseLogin()
    {
        FirebaseApp.LogLevel = LogLevel.Warning;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
                EmitOnFirebaseReady();
            else
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
        });
    }

    private void EmitOnFirebaseReady()
    {
        fireBaseAuth.OnFirebaseReady();
        fireBaseMessages.OnFirebaseReady();
        fireBaseDynamicLinks.DynamicLinkSetup();
        playGamesSetup.StartSetup();
    }
}