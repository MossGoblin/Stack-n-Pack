using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderMaster : MonoBehaviour
{
    // props
    List<Order> orderList;
    private int complexityLevel = 1;
    [SerializeField] private int orderAmount = 2;

    // refs

    void Start()
    {
        //grid = GameObject.FindObjectOfType<GridController>.GetConponent<Grid>();
        orderList = new List<Order>();

        // Init orders
        //InitOrders(complexityLevel);
    }

    void Update()
    {

    }
}