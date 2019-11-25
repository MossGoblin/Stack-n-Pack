using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory
{
    // props
    public int PosGrid_X { get; private set; }
    public int PosGrid_Y { get; private set; }

    public GameObject FactoryGO { get; private set; }

    public int Content { get; private set; }

    // refs
    CrateMaster crateMaster;
    Storage gridRef;

    public Factory(int posX, int posY, Conductor master, GameObject factoryGO)
    {
        PosGrid_X = posX;
        PosGrid_Y = posY;
        this.gridRef = master.gridRef;
        this.crateMaster = master.crateMaster;
        FactoryGO = factoryGO;
        
        ReStock();
    }

    public void ReStock()
    {
        Content = crateMaster.RandomType();
        FactoryGO.GetComponentInChildren<SpriteRenderer>().color = crateMaster.colorPool[Content - 1];
    }
}
