using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System;
using Firebase.Auth;
using Firebase.Storage;
using System.Threading.Tasks;

public class PushOrder : MonoBehaviour {

    public InputField bldgnum;
    public InputField roomnum;
    public Text food;
    public Text price;

    private DatabaseReference reference;
    protected FirebaseAuth auth;
    private string user;


    // Use this for initialization
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    public void CreateOrderTicket()
    {
        // Get food items from previous screen
        string[] s = food.text.Split('\n');
        List<string> l = new List<string>();
        foreach (string f in s)
        {
            if(f.Length > 0)
            {
                l.Add(f);
            }
        }


        try // to get current user  (user is message subscription)
        {
            FirebaseUser u = auth.CurrentUser;
            user = u.UserId;
        }
        catch // UserID for debug purposes
        {
            user = "u8cnIVqGaEOcJiSLDAPkDvhp8CN2";
        }

        AddNewOrder(new Order(user, l, bldgnum.text, roomnum.text, price.text, "Dennys"));
    }

    //Adds new order to database
    public void AddNewOrder(Order order)
    {
        DatabaseReference userRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Orders");
        userRef.Child(user).SetRawJsonValueAsync(JsonUtility.ToJson(order));
        //string email = FirebaseAuth.DefaultInstance.CurrentUser.Email;
    }

}
