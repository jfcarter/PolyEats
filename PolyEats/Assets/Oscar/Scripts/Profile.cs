using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class Profile : MonoBehaviour {

    protected Firebase.Auth.FirebaseAuth auth;
    protected DatabaseReference reference;

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    // Use this for initialization
    void Start () {
        //sets up database
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });


    }

    public UnityEngine.UI.Text displayName;
    public UnityEngine.UI.Text email;
    public UnityEngine.UI.Image profilePic;

    void LoadProfileBasics()
    {
        displayName.text = auth.CurrentUser.DisplayName;
        email.text = auth.CurrentUser.Email;

        string targetUser = auth.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWith(task =>
        {
            //If task is faulty display error
            if (task.IsFaulted)
            {
                Debug.LogError("Task is Faulted");
            }
            //If task is complete get data
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;

                //Iterate through data's children
                foreach (DataSnapshot obj in snapshot.Children)
                {

                    if(obj.Key.ToString() == targetUser)
                    {
                        //TO BE DONE
                        //CONVERT STRING TO BYTE ARRAY AND INTO PICTURE
                        string picString = obj.Child("img").Value.ToString();

                        //Then convert to Image type

                        //profilePic.sprite = (Image source here)
                    }
                }
            }
        });

    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
	
    void ChangeUserName(string newName)
    {
        if (auth.CurrentUser != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = newName,

            };
            auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWith(task2 =>
            {
                if (task2.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task2.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task2.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }

    }

	// Update is called once per frame
	void Update () {
		
	}
}
