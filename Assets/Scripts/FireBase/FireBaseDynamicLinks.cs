using System;
using Firebase.DynamicLinks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireBaseDynamicLinks : MonoBehaviour
{
    public void DynamicLinkSetup()
    {
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
    }

    // Display the dynamic link received by the application.
    void OnDynamicLink(object sender, EventArgs args)
    {
        ReceivedDynamicLinkEventArgs dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
        string url = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString;
        string link = GetLinkFromUrl(url);
        if (link == "") return;
        Debug.LogFormat("Received dynamic link {0}", link);
        if (link.StartsWith("scene")) OpenScene(link);
    }

    private string GetLinkFromUrl(string url)
    {
        if (url.StartsWith("https://play.google.com/store/apps/details?link="))
        {
            url = url[48..];
            if (url.EndsWith("&id=com.vanBrusselGames.MindMix"))
            {
                url = url[..^31].Trim();
                return url;
            }
        }
        return "";
    }

    private void OpenScene(string link)
    {
        link = link[5..];
        string scene;
        if (link.Contains('\\'))
        {
            scene = link.Split('\\')[0];
            SceneManager.LoadScene(scene);
        }
        else
        {
            scene = link;
            SceneManager.LoadScene(scene);
        }
    }
}