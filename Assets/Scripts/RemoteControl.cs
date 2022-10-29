using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class RemoteControl : MonoBehaviour
{

    private struct UserAttributes { }
    private struct AppAttributes { }

    string nieuwsteVersie;

    private async void Awake()
    {
        if (!Utilities.CheckForInternetConnection()) return;
        await InitializeRemoteConfigAsync();
        RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
    }

    async Task InitializeRemoteConfigAsync()
    {
        // initialize handlers for unity game services
        await UnityServices.InitializeAsync();

        // options can be passed in the initializer, e.g if you want to set analytics-user-id or an environment-name use the lines from below:
        // var options = new InitializationOptions()
        //   .SetOption("com.unity.services.core.analytics-user-id", "my-user-id-1234")
        //   .SetOption("com.unity.services.core.environment-name", "production");
        // await UnityServices.InitializeAsync(options);

        // remote config requires authentication for managing environment information
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                //Debug.Log("No settings loaded this session; using default values.");
                break;
            case ConfigOrigin.Cached:
                //Debug.Log("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                //Debug.Log("New settings loaded this session; update values accordingly.");
                nieuwsteVersie = RemoteConfigService.Instance.appConfig.GetString("nieuwsteVersie");
                break;
        }
        if (Application.version.StartsWith(nieuwsteVersie)) Debug.Log("Nieuwste versie van app");
    }
}