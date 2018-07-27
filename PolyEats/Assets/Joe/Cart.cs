using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Cart : MonoBehaviour {

    static Cart instance = null;
    public ArrayList cart;

    // on creates
    private void Awake()
    {
        if(instance != null )
        {
            Destroy(gameObject); // if duplicate -> remove
        }
        else
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }

    }

    // Use this for initialization
    void Start () {

        this.cart = new ArrayList();

    }
    



    // adds food item to cart
    public void UpdateCart(string food_item, double price)
    {
        this.cart.Add(Tuple.Create(food_item, price));
    }

    public ArrayList GetCart()
    {
        return this.cart;
    }

    // view current contents of cart
    public void print()
    {
        foreach (Tuple<string, double> t in this.cart)
        {
            Debug.Log(t.Item1 + " " + t.Item2);
        }
    }

    // sum prices
    public double TotalPrice()
    {
        double sum = 0;

        foreach (Tuple<string, double> t in this.cart)
        {
            sum += t.Item2;
        }

        return sum;
    }

    // sum prices
    public string TotalFood()
    {
        string food = "";

        foreach (Tuple<string, double> t in this.cart)
        {
            food += t.Item1 + "\n";
        }

        return food;
    }
}
