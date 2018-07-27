using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendFood2Cart : MonoBehaviour {

    

    // update cart with food item
    public void SendFood(string food_price)
    {
        string[] fp = food_price.Split('$');
        string foodItem = fp[0];
        double price = Convert.ToDouble(fp[1]);

        GameObject.FindGameObjectWithTag("Cart").GetComponent<Cart>().UpdateCart(foodItem, price);

        //GameObject.FindGameObjectWithTag("Cart").GetComponent<Cart>().print();
    }
}
