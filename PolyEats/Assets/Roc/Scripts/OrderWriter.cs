//The purpose of this script is to place the 
//Hungry persons order on the hustlers screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderWriter : MonoBehaviour {

    public Text textbox;
	
    public void WriteOrder(string order)
    {
        textbox.text = order;
    }
}
