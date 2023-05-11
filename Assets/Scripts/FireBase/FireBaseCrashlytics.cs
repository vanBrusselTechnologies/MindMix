using Firebase.Crashlytics;
using UnityEngine;

public class FireBaseCrashlytics : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public void OnFirebaseReady()
    {
        Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        Crashlytics.IsCrashlyticsCollectionEnabled = true;
    }
/*Force CrashTest
    private int _updatesBeforeException;
    // Update is called once per frame
    void Update()
    {
        // Call the exception-throwing method here so that it's run
        // every frame update
        ThrowExceptionEvery60Updates();
    }

    // A method that tests your Crashlytics implementation by throwing an
    // exception every 60 frame updates. You should see reports in the
    // Firebase console a few minutes after running your app with this method.
    void ThrowExceptionEvery60Updates()
    {
        if (_updatesBeforeException > 0)
        {
            _updatesBeforeException--;
        }
        else
        {
            // Set the counter to 60 updates
            _updatesBeforeException = 60;

            // Throw an exception to test your Crashlytics implementation
            throw new System.Exception("test exception please ignore");
        }
    }
    */
#endif
}