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


    private int seed;

    // refs
    public Storage grid;
    public Conductor master;

    void Start()
    {
        grid = master.gridRef;

        orderList = new List<Order>();
        seed = (int)UnityEngine.Random.Range(1, 100);
        UnityEngine.Random.InitState(seed);
        
    }

    void Update()
    {
        // Init orders
        InitOrders(complexityLevel);
    }

    public void IssueOrder()
    {
        seed = (int)UnityEngine.Random.Range(1, 100);
        Order newOrder = new Order(complexityLevel, grid.Rarity, seed);
        orderList.Add(newOrder);
        Debug.Log($"new order issued: level {complexityLevel}");
        Debug.Log($"seed: {seed}");
        Debug.Log($"order index = {newOrder.ContentIndex}");
    }

    private void InitOrders(int complexityLevel)
    {
        // Issue one order, for starters
        // ONLY if we have already begun with the crates
        if (master.cratesStarted)
        {
            IssueOrder();
        }

        // FIXME : HERE
    }

}