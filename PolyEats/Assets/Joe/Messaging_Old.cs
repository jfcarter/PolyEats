using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;

public class Messaging_Old : MonoBehaviour
{
    //private DatabaseReference reference;

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public Text messageLog;
    public InputField messageOut;

    private string topic = "/MessageLogs/"; // folder in database that stores all message logs
    private string orderID;



    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    void Start()
    {
        this.orderID = "<some value>"; // maybe should be UserID 

        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        //reference = FirebaseDatabase.DefaultInstance.RootReference;

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.Messaging.FirebaseMessaging.Subscribe(topic + orderID); // subscribing to token
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                Debug.Log(topic + orderID);
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

    }


    // If you want to target this specific device for messages.
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    // If you want to be able to receive incoming messages.
    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);

        foreach (KeyValuePair<string, string> iter in e.Message.Data)
        {
            Debug.Log("  " + iter.Key + ": " + iter.Value);
        }

        messageLog.text = e.Message.Data.Values.ToString();
    }


    // End our messaging session when the program exits.
    public void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }



    // send message to Firebase Messenger
    public void SendMessage()
    {
        Firebase.Messaging.FirebaseMessage message = new Firebase.Messaging.FirebaseMessage();

        // this is apparently the crazy way to intialize a Map with only 1 k,v pair
        IDictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("outgoing message", messageOut.text);

        // message data must be of Dictionary type
        message.Data = dict;
        message.To = topic + orderID;
        


        // for debugging: output message data
        foreach(string v in message.Data.Values)
        {
            Debug.Log(v);
        }


        // TODO: how to check where message sending to?
        Firebase.Messaging.FirebaseMessaging.Send(message);



        // and clear out textbox
        messageOut.text = "";


    }


}
