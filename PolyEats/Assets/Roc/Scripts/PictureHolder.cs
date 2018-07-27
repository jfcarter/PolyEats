using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Storage;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class PictureHolder : MonoBehaviour {

    static PictureHolder instance = null;
    public RawImage confirmationPic;
    private DatabaseReference reference;
    protected FirebaseAuth auth;
    private string user;


    // on creates
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // if duplicate -> remove
        }
        else
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }

    }

    public void setPicture(string userID)
    {
        Debug.Log("user ID is: " + userID);
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        user = userID;

        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.GetInstance("gs://broncobytes-35974.appspot.com");

        // Create a reference to user's storage
        StorageReference images_ref = storage.RootReference.Child(user);

        // Load image from database && set as confirmation Picture
        images_ref.GetBytesAsync(256000).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;
                Debug.Log("Finished downloading!");

                Texture2D temp = new Texture2D(480, 287);
                ImageConversion.LoadImage(temp, fileContents);
                confirmationPic.texture = temp;
            }
        });
    }

    // if cart scene, don't need to carry over anymore
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "HustlerRestaurantChooser")
        {
            Destroy(gameObject);
        }
    }
}
