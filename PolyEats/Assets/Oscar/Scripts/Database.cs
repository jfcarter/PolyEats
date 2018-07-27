using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;

public class Database {

    private DatabaseReference reference;

    // Use this for initialization
    void Start () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public Database()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    //Adds new user accont to database
	public void AddNewUser(User user)
    {
        DatabaseReference userRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Users");
        userRef.Child(user.getEmail()).SetRawJsonValueAsync(JsonUtility.ToJson(user));
    }

    //Adds new order to database
    public void AddNewOrder(Order order, User user)
    {
        DatabaseReference userRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Orders");
        userRef.Child(user.getEmail()).SetRawJsonValueAsync(JsonUtility.ToJson(order));
        //string email = FirebaseAuth.DefaultInstance.CurrentUser.Email;
    }

    public void RemoveOrder(string email)
    {
        DatabaseReference userRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Orders");
        userRef.Child(email).SetRawJsonValueAsync(null);
    }
    

}
