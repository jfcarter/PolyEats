using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateGPSText : MonoBehaviour {

    public Text coordinates;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        coordinates.text = "Lat: " + UserLocation.Instance.latitude.ToString() + "\nLong: " + UserLocation.Instance.longitude.ToString();
    }
}
