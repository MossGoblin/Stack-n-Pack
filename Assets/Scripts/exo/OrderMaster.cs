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
    public Storage grid;
    public Conductor master;

    void Start()
    {
        orderList = new List<Order>();

        // Init orders
        //InitOrders(complexityLevel);
    }

    void Update()
    {

    }

    public void IssueOrder()
    {
        grid = master.gridRef;
        int[] rarityGrid = grid.Rarity;
        Order newOrder = new Order(complexityLevel, rarityGrid);
        orderList.Add(newOrder);
        Debug.Log($"new order issued: level {complexityLevel}");
        Debug.Log($"order index = {orderList[0].ContentIndex}");
    }
}