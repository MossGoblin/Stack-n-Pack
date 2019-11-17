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
        orderList = new List<Order>();
        seed = (int)UnityEngine.Random.Range(1, 100);
        UnityEngine.Random.InitState(seed);
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
        seed = (int)UnityEngine.Random.Range(1, 100);
        Order newOrder = new Order(complexityLevel, rarityGrid, seed);
        orderList.Add(newOrder);
        Debug.Log($"new order issued: level {complexityLevel}");
        Debug.Log($"seed: {seed}");
        Debug.Log($"order index = {newOrder.ContentIndex}");
    }
}