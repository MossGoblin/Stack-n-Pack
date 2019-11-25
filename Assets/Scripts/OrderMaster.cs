using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderMaster : MonoBehaviour
{
    // props
    public List<Order> orderList;
    private int complexityLevel = 1;
    // [SerializeField] private int orderAmount = 2;

    private int seed;

    // refs
    public Storage grid;
    public Conductor master;
    [SerializeField] Transform orderHolder;
    [SerializeField] GameObject orderPrefab;

    void Start()
    {
        grid = master.gridRef;

        orderList = new List<Order>();
        seed = (int)UnityEngine.Random.Range(1, 100);
        UnityEngine.Random.InitState(seed);
        // initial orders
        InitOrders(complexityLevel);
        
    }

    void Update()
    {
        // update order canvas layout spacing
        int newspacing = (6 - orderList.Count)*10;
        orderHolder.GetComponent<HorizontalLayoutGroup>().spacing = newspacing;
    }

    public void IssueOrder()
    {
        // generate OBJ
        seed = (int)UnityEngine.Random.Range(1, 100);
        int[] newContent = GenerateContent();
        Order newOrder = new Order(complexityLevel, newContent);
        orderList.Add(newOrder);
        Debug.Log($"new order issued: level {complexityLevel}");
        Debug.Log($"seed: {seed}");
        Debug.Log($"order index = {newOrder.ContentIndex}");

        // Instantiate GO
        GameObject newOrderGO = Instantiate(orderPrefab, orderHolder);

        // crosslink objects
        newOrderGO.GetComponent<OrderGO>().orderData = newOrder;
        newOrder.SetGO(newOrderGO);

    }

    private void InitOrders(int complexityLevel)
    {
        // Issue one order, for starters
        for (int count = 0; count < complexityLevel + 4; count++)
        {
            IssueOrder();
        }
    }

    private int[] GenerateContent() // is this breakdown still in use ??
    {
        int[] orderIndex = new int[6];
            /*
            types   1   2   3   4   5   6
            cmpl 1  3   3   3   3   3   3
            cmpl 2  4   4   4   4   4   4
            cmpl 3  5   5   5   5   5   5
            */
            int typeCount = complexityLevel + 2;

        // pick typeCount number of types from between minType and maxType
        List<int> selectedTypes = new List<int>();
        while(selectedTypes.Count < typeCount)
        {
            int newType = UnityEngine.Random.Range(0, 5);
            if (!selectedTypes.Contains(newType))
            {
                selectedTypes.Add(newType);
            }
        }

        // pick random number of crates (complexity-1 to complexity) for each of the selected type and calculate ContentIndex
        for (int count = 0; count < 6; count++)
        {
            if (selectedTypes.Contains(count))
            {
            int numberOfCrates = (int)UnityEngine.Random.Range(complexityLevel, typeCount);
            orderIndex[count] = numberOfCrates;
            }
        }

        return orderIndex;
    }

}