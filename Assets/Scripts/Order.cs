using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    // props
    public int[] ContentIndex { get; private set; }
    private int complexity;
    private int[] rarityGrid;

    // refs

    private GameObject orderGO;

    // private Storage gridRef;

    public Order (int complexity, int[] contentIndex)
    {
        this.complexity = complexity;
        this.ContentIndex = contentIndex;
    }
    public void SetGO(GameObject orderGO)
    {
        this.orderGO = orderGO;
    }
}
