using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate
{
    // props and fields
    public int Content { get; private set; }
    public GameObject CrateGO { get; private set; }
    public int Group { get; private set; }
    public int PositionX_Grid { get; set; }
    public int PositionY_Grid { get; set; }

    // refs
    private Storage gridRef;
    CrateMaster crateMaster;

    public Crate(int posX, int posY, Conductor master, int content)
    {
        PositionX_Grid = posX;
        PositionY_Grid = posY;
        this.Content = content;
        this.gridRef = master.gridRef;
        this.crateMaster = master.crateMaster;
        RegisterCrate();
    }

    private void RegisterCrate()
    {
        gridRef.storageGrid[PositionX_Grid, PositionY_Grid] = this;
    }

    // set up GO
    public void SetUpGO(GameObject crateGO)
    {
        CrateGO = crateGO;
        SpriteRenderer contentMarker = CrateGO.transform.GetChild(0).GetComponent<SpriteRenderer>();
        contentMarker.color = crateMaster.colorPool[Content];
    }

    // Set up group
    public void SetGroup(int groupIndex)
    {
        Group = groupIndex;
    }

    // get nbrs
    public Crate[] Neighbours()// always top..left
    {
        // get adjacency
        int[,] adjacency = Adjacency();
        Crate[] nbrs = new Crate[4];
        for (int count = 0; count < 4; count++) // iterate nbrs
        {
            int nbrPosX = PositionX_Grid + adjacency[count, 0];
            int nbrPosY = PositionY_Grid + adjacency[count, 1];

            if (gridRef.PosIsValid(nbrPosX, nbrPosY)) // the nbr is within the grid
            {
                nbrs[count] = gridRef.storageGrid[nbrPosX, nbrPosY];
            }
            else
            {
                nbrs[count] = null;
            }
        }
        return nbrs;
    }

    public int[,] Adjacency()
    {
        return new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } }; // relative grid coordinates for the neighbours of a cell - top to left;
    }

}
