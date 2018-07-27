using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    public string buildingNumber;
    public string roomNumber;
    public List<string> foodList;
    public string totalPrice;
    public string orderFrom;
    public string user;

    public Order(string user, List<string> foodList, string buildingNumber, string roomNumber, string totalPrice, string orderFrom)
    {
        this.user = user;
        this.foodList = foodList;
        this.buildingNumber = buildingNumber;
        this.roomNumber = roomNumber;
        this.totalPrice = totalPrice;
        this.orderFrom = orderFrom;
    }

    public Order(Order order)
    {
        this.foodList = order.foodList;
        this.buildingNumber = order.buildingNumber;
        this.roomNumber = order.roomNumber;
    }

    public string getBuildingNumber()
    {
        return this.buildingNumber;
    }

    public string getRoomNumber()
    {
        return this.roomNumber;
    }

    public List<string> getFoodList()
    {
        return this.foodList;
    }

    public string getOrderFrom()
    {
        return this.orderFrom;
    }
}
