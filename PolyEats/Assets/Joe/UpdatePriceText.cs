using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class UpdatePriceText : MonoBehaviour
{

    public Text textbox;

    // Use this for initialization
    void Start()
    {
        // since Update is called once per frame, no need to initialize
    }


    // updates textbox with current price
    void Update()
    {
        double price = GameObject.FindGameObjectWithTag("Cart").GetComponent<Cart>().TotalPrice();

        textbox.text = "$" + price.ToString("#.00", CultureInfo.InvariantCulture);

    }
}