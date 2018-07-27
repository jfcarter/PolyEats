using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System.Threading.Tasks;

public class DatabasePuller : MonoBehaviour
{

    private DatabaseReference reference;
    GameObject temp;
    GameObject map;
    GameObject restaurantChoice;

    // Use this for initialization
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://broncobytes-35974.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        temp = GameObject.FindGameObjectWithTag("Map_Controller");
        map = GameObject.FindGameObjectWithTag("map");
        restaurantChoice = GameObject.FindGameObjectWithTag("Choice");
        //RetrieveOrder("oscarzhanfdsf");
    }

    public void RetrieveOrders()
    {
        //Hide all markers before creating new ones
        map.GetComponent<OnlineMaps>().hideMarkers();
        Order targetOrder = null;
        //Task<DataSnapshot> dataSnapshot = FirebaseDatabase.DefaultInstance.GetReference("Orders").GetValueAsync();

        FirebaseDatabase.DefaultInstance.GetReference("Orders").GetValueAsync().ContinueWith(task =>
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
                    Debug.Log(obj.Key);
                    Debug.Log("Loading database");
                    targetOrder = JsonUtility.FromJson<Order>(obj.GetRawJsonValue());
                    string replace = obj.Key.Replace('?', '.');
                    Debug.Log("placing marker on building " + targetOrder.buildingNumber);
                    //RE-ADD CODE
                    if (targetOrder.orderFrom == restaurantChoice.GetComponent<RestaurantChooser>().restaurant) {
                        map.GetComponent<OnlineMaps>().ShowMarker(targetOrder.user, targetOrder.buildingNumber, targetOrder.foodList, targetOrder.totalPrice, targetOrder.roomNumber);
                    }
                }
            }
        });
    }


}
