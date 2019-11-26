using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderMaster : MonoBehaviour
{
    // props
    public List<Order> orderList;
    private int complexityLevel = 1;

    // dictionary of matches
    Dictionary<Order, List<Group>> matches = new Dictionary<Order, List<Group>>();

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
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            CheckOrderGroupMatches();
        }
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

        bool uniqueIndex = false;
        while (!uniqueIndex)
        {
            uniqueIndex = true;

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

            // check if this index is already used
            foreach (var order in orderList)
            {
                if (order.ContentIndex.SequenceEqual(orderIndex))
                {
                    uniqueIndex = false;
                    break;
                }
            }
        }
        return orderIndex;
    }

    public void CheckOrderGroupMatches()
    {
        // collection of checked groups
        List<Group> checkedGroups = new List<Group>();

        // iterate orders
        foreach (var order in orderList)
        {
            // iterate groups
            foreach (Group group in master.groupMaster.GroupList())
            {
                // mark group as checked
                checkedGroups.Add(group);

                if (!checkedGroups.Contains(group) && order.ContentIndex.SequenceEqual(group.Content))
                {
                    // record the match
                    if (!matches.ContainsKey(order))
                    {
                        matches.Add(order, new List<Group>());
                    }
                    matches[order].Add(group);
                }
            }
        }
        // FIXME :: UPDATE MATCHES WHEN REMOVING A GROUP!
        UpdateOrderGroupMatchVisuals();
        
    }

    // update match visuals
    private void UpdateOrderGroupMatchVisuals()
    {
        int numberOfMatches = 1;

        foreach (var order in matches)
        {
            for (int matchesPerGroup = 0; matchesPerGroup < 3; matchesPerGroup++)
            {
                // check of there is such a match in the group list for that order
                if (matchesPerGroup < order.Value.Count)
                {
                    // .. if there is - get the group color from the map
                    int groupColorIndex = master.groupMaster.groupToColorMap[order.Value[matchesPerGroup]];
                    // update the digit image and color it
                    OrderGO orderGO = order.Key.GetOrderGO().GetComponent<OrderGO>();
                    // digit to use
                    int digitToUse = numberOfMatches;
                    if (digitToUse == 10)
                    {
                        digitToUse = 0;
                    }

                    if (numberOfMatches < 10)
                    {
                        orderGO.matchDisplay[matchesPerGroup].GetComponent<Image>().sprite = orderGO.matchDigit[digitToUse];
                        orderGO.matchDisplay[matchesPerGroup].GetComponent<Image>().color = master.crateMaster.paletteArray[groupColorIndex];
                    }
                    else
                    {
                        orderGO.matchDisplay[matchesPerGroup].GetComponent<Image>().sprite = null;
                    }
                }
            }
        }

    }

}