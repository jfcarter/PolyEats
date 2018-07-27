using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFoodText : MonoBehaviour {

    public Text textbox;

	// Use this for initialization
    // TODO: get order from Cart
	void Start () {
        textbox.text = GameObject.FindGameObjectWithTag("Cart").GetComponent<Cart>().TotalFood();
    }
	
	// Update is called once per frame
    // TODO: update textbox
	void Update () {

	}
}
