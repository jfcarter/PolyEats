//The purpose of this script is to grab the database information and
//use it appropriately

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUpdater : MonoBehaviour {

    GameObject map;
    GameObject orderList;
	// Use this for initialization
	void Start () {
        map = GameObject.FindGameObjectWithTag("map");
        orderList = GameObject.FindGameObjectWithTag("OrderController");
        map.GetComponent<OnlineMaps>().hideMarkers();
        //map will be updated every 4 seconds
        InvokeRepeating("UpdateMap", 1.0f, 3.0f);
    }
	public void setOrderInfo(string orderInfo)
    {
        Debug.Log("Setting order info");
        Debug.Log(orderInfo);
    }

    public void stopMapUpdate()
    {
        //stop updating map
        CancelInvoke();
    }

    public void UpdateMap () {
        Debug.Log("Checking database for new orders");
        //use database to create markers on map
        orderList.GetComponent<DatabasePuller>().RetrieveOrders();
        Debug.Log("Displaying new orders");
        //display the markers (data is sorted oldest to newest so if you see a repeat number do not take)
        //map.GetComponent<OnlineMaps>().ShowMarker("1"); //create a for loop that goes through the orders and enters building number for "1"
        
        
        /*
        //If marker is clicked
        if () {
            //Cancel the invoke
            CancelInvoke();
            //display the markers order and begin route

            //Open messenger to Hungry for updates
        }
        */
    }

    public void showOrder()
    {

    }
}
