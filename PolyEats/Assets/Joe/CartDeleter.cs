using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartDeleter : MonoBehaviour {

    // deletes cart when switching back to order screen
    public void DeleteCart()
    {
        GameObject tempCart = GameObject.FindGameObjectWithTag("Cart");
        Destroy(tempCart);
    }
}
