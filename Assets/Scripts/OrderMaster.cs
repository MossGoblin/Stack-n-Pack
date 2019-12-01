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

    // list of active matches
    bool[] activeMatches = new bool[10];

    private int seed;

    // refs
    public Storage grid;
    public Conductor master;
    [SerializeField] Transform orderHolder;
    [SerializeField] GameObject orderPrefab;

    [SerializeField] Text confirmationText;

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
        if (Input.GetKeyDown(KeyCode.Backspace)) // FIXME TBD Debug
        {
            CheckOrderGroupMatches();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            
        }

    }

    public void IssueOrder()
    {
        // generate OBJ
        seed = (int)UnityEngine.Random.Range(1, 100);
        int[] newContent = GenerateContent();
        Order newOrder = new Order(complexityLevel, newContent);
        orderList.Add(newOrder);
        // Debug.Log($"new order issued: level {complexityLevel}");
        // Debug.Log($"seed: {seed}");
        // Debug.Log($"order index = {newOrder.ContentIndex}");

        // Instantiate GO
        GameObject newOrderGO = Instantiate(orderPrefab, orderHolder);

        // crosslink objects
        newOrderGO.GetComponent<OrderGO>().orderData = newOrder;
        newOrder.SetGO(newOrderGO);

    }

    private void InitOrders(int complexityLevel)
    {
        // always 5 orders
        for (int count = 0; count < 5; count++)
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
                int newType = UnityEngine.Random.Range(0, 6);
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
        // reset match dicrionary
        matches.Clear();

        // collection of checked groups
        List<Group> matchedGroups = new List<Group>();

        // iterate orders
        foreach (var order in orderList)
        {
            // iterate groups
            foreach (Group group in master.groupMaster.GroupList())
            {
                if (!matchedGroups.Contains(group) && order.ContentIndex.SequenceEqual(group.Content))
                {
                    // record the match
                    if (!matches.ContainsKey(order))
                    {
                        matches.Add(order, new List<Group>());
                    }
                    matches[order].Add(group);
                    if (!matchedGroups.Contains(group))
                    {
                        // mark group as checked
                        matchedGroups.Add(group);
                    }
                }
            }
        }
        UpdateOrderGroupMatchVisualsAndTriggers();
        
    }

    // update match visuals

    // new plan:
    /*

    allow 10 matches - 2 per order
    iterate orders/group match dictionary
    for each of the orders check until all matches are found, up to 2
    if there are enough groups (1 or 2) - display the matches and record the matches as active
    mark the value of the digit for use ??
    */

    private void UpdateOrderGroupMatchVisualsAndTriggers()
    {
        // reset indicators
        foreach (var order in orderList)
        {
            OrderGO orderGO = order.GetOrderGO().GetComponent<OrderGO>();
            foreach (var display in orderGO.matchDisplay)
            {
                // clear order match visuals
                display.GetComponent<Image>().sprite = null;
                display.GetComponent<Image>().color = Color.clear;
                // clear active matches array
                for (int count = 0; count < 10; count++)
                {
                    activeMatches[count] = false;
                }
            }
        }

        int globalMatchNumber = 1;

        foreach (var order in matches)
        {
            for (int matchCount = 0; matchCount < 2; matchCount++)
            {
                if (globalMatchNumber <= 10 && (matchCount + 1) <= order.Value.Count) // we have a group in the list that is either number 1 or 2
                {
                    // get the color of that group
                    int groupColorIndex = master.groupMaster.groupToColorMap[order.Value[matchCount]];

                    // get the index of the orderGO in the layoutGroup
                    int indexInHolder = order.Key.GetOrderGO().transform.GetSiblingIndex();
                    int digitToUse = indexInHolder * 2 + matchCount + 1;


                    if (digitToUse == 10) // roll around 10 into 0 (to use the digit row of the keyboard)
                    {
                        digitToUse = 0;
                    }
                    // update order visual
                    OrderGO orderGO = order.Key.GetOrderGO().GetComponent<OrderGO>();
                    orderGO.matchDisplay[matchCount].GetComponent<Image>().sprite = orderGO.matchDigit[digitToUse];
                    orderGO.matchDisplay[matchCount].GetComponent<Image>().color = master.crateMaster.paletteArray[groupColorIndex];
                    // update active match list
                    int matchToActivate = digitToUse - 1;
                    if (digitToUse == -1)
                    {
                        matchToActivate = 10;
                    }
                    activeMatches[matchToActivate] = true;

                    globalMatchNumber++;
                }
            }
        }
    }

    public void RemoveGroupMatches(Group group)
    {
        List<Order> ordersToBeRemoved = new List<Order>();
        // iterate matches and remove groups
        foreach (var match in matches)
        {
            if (match.Value.Contains(group)) // if the match contains the removed group
            {
                match.Value.Remove(group); // remove teh group from the list of matches
                if (match.Value.Count == 0) // if there are no longer any matches for that order
                {
                    ordersToBeRemoved.Add(match.Key);
                }
            }
        }
        // clean up orders with no valid group matches
        foreach (var order in ordersToBeRemoved)
        {
            matches.Remove(order);
        }

        // TODO : HERE - trigger match update ?? Maybe ??
    }

    private bool DispatchOrder(Order order)
    {
        return true; // TODO HERE
    }

    public void IncreaseComplexity()
    {
        if (complexityLevel < 3)
        {
            complexityLevel++;
        }
    }
}