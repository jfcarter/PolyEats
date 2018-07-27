using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using Firebase.Auth;

public class LoadQR : MonoBehaviour {

    private DatabaseReference reference;
    protected FirebaseAuth auth;
    string email;


    // Use this for initialization
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        //string email = GameObject.FindGameObjectWithTag("yolo").GetComponent<UserName>().username.email;

        try
        {
            FirebaseUser user = auth.CurrentUser;
            email = user.ToString();
        }
        catch { }


        // TODO:  instead of pulling orderID from Order-Database, 
        //          pull user's profile pic from Profile-Database && set as image 
        //              TODO: (will need to create img placeholder in UI)
        FirebaseDatabase.DefaultInstance.GetReference("Orders").GetValueAsync().ContinueWith(task =>
        {
            Order targetOrder = null;

            //If task is faulty display error
            if (task.IsFaulted)
            {
                Debug.LogError("Task is Faulted");
            }
            //If task is complete get data
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string n = "";

                //Iterate through data's children
                foreach (DataSnapshot obj in snapshot.Children)
                {
                    

                    //Check if key is email we are looking for
                    if (obj.Key.Equals(email))
                    {
                        Debug.Log("Email found");
                        targetOrder = JsonUtility.FromJson<Order>(obj.GetRawJsonValue());
                        //n = targetOrder.imageNumber;

                        //string path = Application.dataPath + "/Editor/Vuforia/ImageTargetTextures/PolyEatsImages/";
                        //string filename = path + "IR-pic" + n + "_scaled.jpg";

                        Debug.Log("Image Number: " + n);

                        
                        for(int i=1; i<9; ++i)
                        {

                            if ( i != int.Parse(n) )
                            {
                                string tag = "i_" + i;
                                GameObject.FindGameObjectWithTag(tag).SetActive(false);
                            }
                        }

                    }
                    
                }
            }
        });
    }
	
}
