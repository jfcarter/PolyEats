using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Firebase.Storage;

public class Authenticator : MonoBehaviour
{
    public InputField email;
    public InputField password;
    public InputField firstName;
    public InputField lastName;
    //text for picture button
    Text text;
    //which camera user is on
    int currentCamIndex = 0;
    //camera screen
    WebCamTexture tex;
    //saved picture
    public RawImage currentPicture;
    private byte[] pictureData;
    private Color32[] data;
    bool pictureTaken = false;
    //database
    private DatabaseReference reference;
    protected FirebaseAuth auth;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    private string uid;


    private void Start()
    {
        try
        {
            //Turns on camera
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);
            tex.Play();
            currentPicture.texture = tex;
            //gets button
            text = GameObject.FindGameObjectWithTag("Photo_Button").GetComponentInChildren<Text>();
        }
        catch { }

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

    void InitializeFirebase()
    {

        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("auth: "+ auth);
    }

    public void Signup()
    {
        if (pictureTaken)
        {
            //continuously pulls from database
            auth.CreateUserWithEmailAndPasswordAsync(email.text.ToString(), password.text.ToString()).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                FirebaseUser newUser = task.Result;
                newUser.SendEmailVerificationAsync();


                if (newUser != null)
                {
                    Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
                    {
                        DisplayName = firstName + " " + lastName,

                    };
                    newUser.UpdateUserProfileAsync(profile).ContinueWith(task2 =>
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

                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);

                LoadDatabase();
            });
        }
        else
        {
            print("picture not yet taken");
        }
    }

    public void LoadDatabase()
    {
        //Debug.Log("pic data: "+ pictureData);
        Debug.Log("userID: " + auth.CurrentUser.UserId);
        //reference.Child("Users").Child(auth.CurrentUser.UserId).Child("img").SetValueAsync(pictureData);

        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.GetInstance("gs://broncobytes-35974.appspot.com");

        // Create a reference to user's storage && input picture (as Byte array encoded as JPG)
        StorageReference images_ref = storage.RootReference.Child(auth.CurrentUser.UserId);
        images_ref.PutBytesAsync(pictureData);

        SceneManager.LoadScene("LoginPage");
    }

    public void TakePhoto()
    {
        Texture2D temp = new Texture2D(tex.width, tex.height);
        temp.SetPixels32(data);
        temp.Apply();
        //converts texture2D to byte[]
        pictureData = temp.EncodeToJPG();



    }

    public void Login()
    {
        Firebase.Auth.Credential credential =
            Firebase.Auth.EmailAuthProvider.GetCredential(email.text.ToString(), password.text.ToString());
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            if (newUser != null)
            {
                Debug.Log("Credentials obtained");
                SceneManager.LoadSceneAsync("HustlerOrHungry");
            }
            else
            {
                Debug.Log("Credentials Null");
            }
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
        

        // TODO: SOMEWHERE ADD THAT E-MAIL LINK SENT HAS BEEN VERIFIED


        //Not sure why below code is needed to authenticate and login
        /*  
         *  That's cus it's not needed ... :/
         * /
        //User targetUser = null;
        //FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWith(task =>
        //{
        //    //If task is faulty display error
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("Task is Faulted");
        //    }
        //    //If task is complete get data
        //    else if (task.IsCompleted)
        //    {

        //        DataSnapshot snapshot = task.Result;

        //        //Iterate through data's children
        //        foreach (DataSnapshot obj in snapshot.Children)
        //        {


        //            //Just to let you know, we use Unique User Id from authenticator to label information not email
        //            //Below is code you are looking for
        //            /*if (obj.Key.Equals(auth.CurrentUser.UserId))
        //            {
        //                //Do what you need to do
        //            }*/
        //            if (obj.Key.Equals(email.text.Replace('.' , '!')))
        //            {
        //                Debug.Log("Passed");
        //                targetUser = JsonUtility.FromJson<User>(obj.GetRawJsonValue());
        //                if (targetUser.password.Equals(password.text))
        //                {
        //                    GameObject.FindGameObjectWithTag("yolo").GetComponent<UserName>().SetUsername(targetUser);
        //                    SceneManager.LoadSceneAsync("HustlerOrHungry");
        //                    Debug.Log("Passed2");
        //                }
        //            }
        //        }
        //    }
        //});
    }

    public void SwapCam_Clicked()
    {
        //if device has multiple cameras only use one
        if (WebCamTexture.devices.Length > 0)
        {
            tex.Stop();
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;
            tex.Play();
        }
    }

    public void StartStopCam()
    {
        if (!pictureTaken)
        {
            pictureTaken = true;
            //gets and sets picture data
            data = new Color32[tex.width * tex.height];
            tex.GetPixels32(data);
            //stops camera
            tex.Stop();
            //change button text
            text.text = "Successful";

            TakePhoto();
        }
        else if (pictureTaken)
        {
            pictureTaken = false;
            tex.Play();
            text.text = "Press To Take Payment photo";
        }
    }

    private void OnDestroy()
    {
        tex.Stop();
    }
}
	
