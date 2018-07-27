using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterButton : MonoBehaviour
{

    // Use this for initialization
    public void Center()
    {
        //finds where player is
        GameObject map = GameObject.FindGameObjectWithTag("map");
        map.GetComponent<OnlineMaps>().SetPositionAndZoom(map.GetComponent<UserLocation>().longitude, map.GetComponent<UserLocation>().latitude, 16);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

