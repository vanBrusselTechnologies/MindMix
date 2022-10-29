using Firebase.Messaging;
using UnityEngine;

public class FireBaseMessages : MonoBehaviour
{
    private FireBaseSetup fireBaseSetup;

    private void Start()
    {
        fireBaseSetup = GetComponent<FireBaseSetup>();
    }

    private bool eersteReadyFrame = true;
    private void Update()
    {
        if (fireBaseSetup.ready && eersteReadyFrame)
        {
            eersteReadyFrame = false;
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        //Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        //Debug.Log("Received a new message from: " + e.Message.From);
    }
}
