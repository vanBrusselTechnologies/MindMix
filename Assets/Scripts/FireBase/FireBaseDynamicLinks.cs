using System;
using Firebase.DynamicLinks;
using UnityEngine;

public class FireBaseDynamicLinks : MonoBehaviour
{
    public void OnFirebaseReady()
    {
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
    }

    // Display the dynamic link received by the application.
    static void OnDynamicLink(object sender, ReceivedDynamicLinkEventArgs args)
    {
        string url = args.ReceivedDynamicLink.Url.OriginalString;
        string link = GetLinkFromUrl(url);
        if (link.Equals("")) return;
        Debug.LogFormat("Received dynamic link {0}", link);
        if (link.StartsWith("scene")) OpenScene(link);
    }

    private static string GetLinkFromUrl(string url)
    {
        if (!url.StartsWith("https://play.google.com/store/apps/details?link=")) return "";
        url = url[48..];
        if (!url.EndsWith("&id=com.vanBrusselGames.MindMix")) return "";
        url = url[..^31].Trim();
        return url;
    }

    private static void OpenScene(string link)
    {
        link = link[5..];
        string scene = link.Contains('\\') ? link.Split('\\')[0] : link;
        SceneManager.LoadScene(scene);
    }
}