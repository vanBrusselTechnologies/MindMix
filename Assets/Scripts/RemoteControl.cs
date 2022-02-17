using UnityEngine;
using Unity.RemoteConfig;

public class RemoteControl : MonoBehaviour
{

    public struct userAttributes { }
    public struct appAttributes { }

    string nieuwsteVersie;

    private GegevensHouder gegevensHouder;

    private void Start()
    {
        gegevensHouder = GetComponent<GegevensHouder>();
        ConfigManager.FetchCompleted += ApplyRemoteSettings;
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
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
                nieuwsteVersie = ConfigManager.appConfig.GetString("nieuwsteVersie");
                break;
        }
        if (Application.version.StartsWith(nieuwsteVersie))
        {
            Debug.Log("Nieuwste versie van app");
            Debug.Log("Hier ook nog even naar kijken");
        }
        else
        {
            gegevensHouder.ShowUpdateWindow(nieuwsteVersie);
        }
    }
}