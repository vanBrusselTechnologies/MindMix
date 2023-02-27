using Firebase.Messaging;
using UnityEngine;

public class FireBaseMessages : MonoBehaviour
{
    public void OnFirebaseReady()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log($"Received Registration Token: {token.Token}");
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log($"Received a new message {e.Message.Notification.Body} from: {e.Message.From}");
    }
}
