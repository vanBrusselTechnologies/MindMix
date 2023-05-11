using Firebase;
using Firebase.Extensions;
using Firebase.AppCheck;
using UnityEngine;

public class FireBaseSetup : MonoBehaviour
{
    [SerializeField] private PlayGamesSetup playGamesSetup;
    [SerializeField] private FireBaseDynamicLinks fireBaseDynamicLinks;
    [SerializeField] private FireBaseAuth fireBaseAuth;
    [SerializeField] private FireBaseMessages fireBaseMessages;
    [SerializeField] private FireBaseCrashlytics fireBaseCrashlytics;

#if UNITY_ANDROID && !UNITY_EDITOR
    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this);
        FireBaseLogin();
    }

    private void FireBaseLogin()
    {
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
        FirebaseAppCheck.SetAppCheckProviderFactory(
            PlayIntegrityProviderFactory.Instance);
        fireBaseCrashlytics.OnFirebaseReady();
        fireBaseAuth.OnFirebaseReady();
        fireBaseMessages.OnFirebaseReady();
        fireBaseDynamicLinks.OnFirebaseReady();
        playGamesSetup.StartSetup();
    }
#endif
}