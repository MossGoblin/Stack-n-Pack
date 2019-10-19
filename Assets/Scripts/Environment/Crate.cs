using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crate
{
    int type;
    int positionX;
    int positionY;
    Crate[,] grid;

    public GameObject SelfGO { get; set; }
    public int Group { get; set; }

    public Crate(GameObject crateGO, int posX, int posY, int type, Crate[,] grid)
    {
        SelfGO = crateGO;
        positionX = posX;
        positionY = posY;
        this.type = type;
        this.grid = grid;
        this.Group = 0;
    }

    // get type method - int

    public int GetCrateType()
    {
        return type;
    }

    // get coordinates method - vector 3
    public float[] GetWorldPosition()
    {
        return new float[2] { positionX, positionY };
    }

    public int[] GetGridPosition()
    {
        int gridPositionX = positionX + (grid.GetLength(0) / 2);
        int gridPositionY = positionY + (grid.GetLength(1) / 2);
        return new int[2] { gridPositionX, gridPositionY };
    }

    // set coordinates
    public void SetPosition(int posX, int posY)
    {
        positionX = posX;
        positionY = posY;
    }


    // get nbrs - Crate[]
    public Crate[] GetNbrs() // Is List okay as a return? - NO, NEED INPLICIT MAPPING!!
    {
        // prepare crate nbr grid to return
        Crate[] crateNbrList = new Crate[4];
        // for each direction
        int counter = 0;
        for (float theta = 0; theta <= Mathf.PI * 3 / 2; theta += Mathf.PI / 2) // top -- right -- bottom -- left
        {
            int stepY = (int)Mathf.Cos(theta);
            int stepX = (int)Mathf.Sin(theta);
            int nbrPosX = (positionX + stepX) - (grid.GetLength(0) / 2);
            int nbrPosY = (positionY + stepY) - (grid.GetLength(1) / 2);
            // check if out of borsers
            if (!IsWithinBorders(nbrPosX, 0) &&
                (!IsWithinBorders(nbrPosY, 1)))
            {
                break;
            }

            crateNbrList[counter] = null;
            // check if vacant
            if (grid[nbrPosX, nbrPosY] != null)
            {
                // Get the nbr crate at this position and add it to the nbr list
                crateNbrList[counter] = grid[nbrPosX, nbrPosY];
            }

            // TODO :: HERE Crate Class (what was that comment about?)
            counter++;
        }
        return crateNbrList;
    }

    private bool IsWithinBorders(float position, int dimention)
    {
        if (position > 1 && position < grid.GetLength(dimention) - 1)
        {
            return true;
        }
        return false;
    }
}