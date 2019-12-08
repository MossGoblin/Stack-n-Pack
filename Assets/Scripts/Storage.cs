using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage
{
    //props n fields
    public int StorageWidth { get; private set; }
    public int StorageHeight { get; private set; }
    public Crate[,] storageGrid;
    public Factory[] factoryGrid;
    public GameObject[,] tileGrid;
    public int gameProgress = 1;
    public Storage(int width, int hight)
    {
        StorageWidth = width;
        StorageHeight = hight;
        storageGrid = new Crate[StorageWidth, StorageHeight];
        factoryGrid = new Factory[4];
        tileGrid = new GameObject[StorageWidth, StorageHeight];

        //Debug.Log("Grid Initated");
    }

    public bool PosIsValid(int posX, int posY)
    {
        if (posX < 0 &&
        posX >= StorageWidth &&
        posY < 0 &&
        posY >= StorageHeight)
        {
            return false;
        }
        return true;
    }

    internal bool WithinBorders((int xDX, int yDY) pos)
    {
        if (pos.xDX >= 0 &&
            pos.xDX < StorageWidth &&
            pos.yDY >= 0 &&
            pos.yDY < StorageHeight)
        {
            return true;
        }
        return false;
    }

    internal bool WithinStorage((int xDX, int yDY) pos)
    {
        if (pos.xDX >= 1 &&
            pos.xDX < StorageWidth-1 &&
            pos.yDY >= 1 &&
            pos.yDY < StorageHeight-1)
        {
            return true;
        }
        return false;
    }
}