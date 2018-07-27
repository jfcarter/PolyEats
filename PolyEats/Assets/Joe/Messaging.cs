using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using Firebase.Auth;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;


public class Messaging : MonoBehaviour
{
    public GameObject messageLog;
    public InputField messageOut;
    private DatabaseReference reference;
    protected FirebaseAuth auth;
    private string user;
    private string displayname;

    public Text textPrefab;
    private string convo;


    void Start()
    {
        // Firebase init stuff
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        Scene m_Scene = SceneManager.GetActiveScene();
        if (m_Scene.name == "HungryConfirmation")
        {
            try // to get current user  (user is message subscription)
            {
                FirebaseUser u = auth.CurrentUser;

                displayname = u.DisplayName;
                user = u.UserId;
            }
            catch // UserID for debug purposes
            {
                user = "u8cnIVqGaEOcJiSLDAPkDvhp8CN2";
                displayname = "Hungry";
            }

            // clear out old messages
            reference.Child("MessageLogs/" + user).Child("msg").SetValueAsync("<Hungry Logged in>");

            //map will be updated every 1 second
            InvokeRepeating("ShowMessageLog", 1.0f, 1.0f);
        }
    }

    public void startMessageUpdate(string user)
    {
        // Firebase init stuff
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        try // to get current user  (user is message subscription)
        {
            FirebaseUser u = auth.CurrentUser;
            displayname = u.DisplayName;
            this.user = user;
        }
        catch // UserID for debug purposes
        {
            displayname = "Hustler";
            this.user = user;
        }
        Debug.Log(user);
        // clear out old messages
        reference.Child("MessageLogs/" + user).Child("msg").SetValueAsync("<Hustler Loggin in>");

        //map will be updated every 1 second
        InvokeRepeating("ShowMessageLog", 1.0f, 1.0f);
    }

    public void ShowMessageLog()
    {
        FirebaseDatabase.DefaultInstance.GetReference("MessageLogs").Child(user).GetValueAsync().ContinueWith(task =>
        {
            //If task is faulty display error
            if (task.IsFaulted) { Debug.LogError("Task is Faulted"); }


            //If task is complete get data
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                string s = snapshot.Child("msg").GetRawJsonValue();
                s = Regex.Replace(s, @"\s+", " ");
                // update only if message different
                if (s!=convo)
                {
                    /* 
                     * Adds messages into a scrollable box dynamically during runtime
                     */
                    //create a new item, name it, and set the parent
                    Text newItem = Instantiate(textPrefab) as Text;
                    newItem.name = gameObject.name;
                    newItem.text = s;
                    newItem.transform.parent = messageLog.transform;
                    newItem.transform.position = messageLog.transform.position;
                    newItem.transform.localScale = messageLog.transform.localScale;
                    newItem.transform.rotation = messageLog.transform.rotation;

                    // Forces message_log to auto scroll to newest message
                    Canvas.ForceUpdateCanvases();
                    messageLog.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;

                }

                convo = s;

            }
        });
    }


    // Overwrites most current message to conversation in Database
    // since async tasks are done in background thread, it looks like duplicate code but is not
    public void SendMessage()
    {
        Debug.Log(user);
        FirebaseDatabase.DefaultInstance.GetReference("MessageLogs").Child(user).GetValueAsync().ContinueWith(task =>
        {
            //If task is faulty display error
            if (task.IsFaulted) { Debug.LogError("Task is Faulted"); }


            //If task is complete get data
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Adds new message to current message log
                string new_message = snapshot.Child("msg").GetRawJsonValue().Replace('"', ' ');
                new_message = displayname + ": " + messageOut.text;

                // update text in database
                reference.Child("MessageLogs/" + user).Child("msg").SetValueAsync(new_message);

                // and clear out textbox
                messageOut.text = "";

            }
        });
    }


}
