using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageAreaCreator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SpriteTileMode floorTile;
    [SerializeField] public int storageAreaW;
    [SerializeField] public int storageAreaH;
    [SerializeField] GameObject concreteTile;
    [SerializeField] Transform tilesParent;
    [SerializeField] Transform player;
    public int storageAreaOriginW;
    public int storageAreaOriginH;
    public int storageAreaEndPointW;
    public int storageAreaEndPointH;

    private bool[,] vacancyGrid;


    void Awake()
    {
        // init vacancy grid
        vacancyGrid = new bool[storageAreaW, storageAreaH];

         for (int countWidth = 0; countWidth < storageAreaW; countWidth++)
        {
            for (int countHeight = 0; countHeight < storageAreaH; countHeight++)
            {
                MarkVacancyGrid(countWidth, countHeight, true);
            }
        }
    }

    internal bool HasSpace()
    {
        int numberOfSpaces = 0;
        for (int countX = 0; countX < storageAreaW; countX++)
        {
            for (int countY = 0; countY < storageAreaH; countY++)
            {
                if (vacancyGrid[countX, countY])
                {
                    numberOfSpaces++;
                }
            }
        }
        if (numberOfSpaces >= 2)
        {
            return true;
        }
        Debug.Log("Out Of Space!");
        return false;
    }

    // Update is called once per frame
    public bool CreateFloor()
    {
        storageAreaOriginW = (storageAreaW / 2) * -1;
        storageAreaOriginH = (storageAreaH / 2) * -1;
        storageAreaEndPointW = storageAreaW + storageAreaOriginW - 1;
        storageAreaEndPointH = storageAreaH + storageAreaOriginH - 1;

        for (int countH = 0; countH < storageAreaH; countH++)
        {
            for (int countW = 0; countW < storageAreaW; countW++)
            {
                float positionX = countW + storageAreaOriginW;
                float positionY = countH + storageAreaOriginH;
                GameObject newTile = Instantiate(concreteTile, new Vector3(positionX, positionY), Quaternion.identity);
                newTile.transform.SetParent(tilesParent);
                newTile.name = "crate " + positionX + " / " + positionY;
                // Debug.Log("placed a tile at: " + countX + " / " +  countY);
            }
        }
        return true;
    }

    public bool IsTileAvailableForCrate(int coordW, int coordH)
    {
        int playerWidth = (int)(player.transform.position.x - storageAreaOriginW);
        int playerHeight = (int)(player.transform.position.y - storageAreaOriginH);
        if ((coordW != playerWidth) || (coordH != playerHeight))
        {
            bool result = vacancyGrid[coordW, coordH];
            return vacancyGrid[coordW, coordH];
        }
        else
        {
            return false;
        }
        //Debug.Log("[" + coordW + " / " + coordH + "] : " + vacancyGrid[coordW, coordH]);
    }
    public bool IsTileVacant(int coordX, int coordY)
    {
        return vacancyGrid[coordX, coordY];
    }

    public void MarkVacancyGrid(int coordW, int coordH, bool free)
    {
        vacancyGrid[coordW, coordH] = free;
    }

    public int GetAbsFromRelW(int relW)
    {
        return relW - storageAreaOriginW;
    }

    public int GetAbsFromRelH(int relH)
    {
        return relH - storageAreaOriginH;
    }

    //public int GetAbsFromDeltaW(int x, int currentW)
    //{
    //    return currentW + x - storageAreaOriginW;
    //}

    //public int GetAbsFromDeltaH(int y, int currentH)
    //{
    //    return y + currentH - storageAreaOriginH;
    //}
}
