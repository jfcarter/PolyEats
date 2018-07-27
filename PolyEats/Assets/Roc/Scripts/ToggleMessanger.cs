using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMessanger : MonoBehaviour {

    public GameObject toggleButton;
    public GameObject messanger;
    public GameObject orderList;
    private bool switcher = false;

	// Use this for initialization
	void Start () {
        messanger.SetActive(false);
	}
	
    public void TurnOnToggleButton()
    {
        toggleButton.SetActive(true);
    }

    public void TurnOffToggleButton()
    {
        toggleButton.SetActive(false);
    }

    // Update is called once per frame
    public void Toggle () {
        if (!switcher)
        {
            messanger.SetActive(true);
            orderList.SetActive(false);
            switcher = true;
        }
        else if (switcher)
        {
            messanger.SetActive(false);
            orderList.SetActive(true);
            switcher = false;
        }
	}
}
