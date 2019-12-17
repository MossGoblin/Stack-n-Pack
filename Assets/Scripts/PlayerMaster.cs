﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaster : MonoBehaviour
{
    // state
    public int Content;
    public bool clampsOpen;

    // selfrefs
    Transform transform;

    // score/energy variables
    [SerializeField] int[] zapBounds = new int[] {150, 300, 600}; 
    int totalZap;
    [SerializeField] public int currentZap = 100;
    [SerializeField] int zapToPickUp = 1;
    [SerializeField] int zapToPlace = 1;

    // refs
    public Conductor master;
    private Storage gridRef;
    private CrateMaster crateMaster;

    // Start is called before the first frame update
    void Start()
    {
        totalZap = currentZap;
        Content = 0;
        transform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        crateMaster = master.crateMaster;
        this.gridRef = master.gridRef;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentZap <= 0)
        {
            // FIXME : Call a PopUp;
            Debug.Log("No more ZAP!");
            Application.Quit();
        }
        HandleInput();
    }

    private void HandleInput()
    {
        if(!master.gamePaused)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                clampsOpen = true;
            }
            else
            {
                clampsOpen = false;
            }
    
            (int xDX, int yDY) posD = (0, 0);
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                posD = (0, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                posD = (0, -1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                posD = (-1, 0);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                posD = (1, 0);
            }
            if (posD.xDX != 0 || posD.yDY != 0)
            {
                MovePlayer(posD);
            }
    
            // picking up content from a factory
            if (Input.GetKeyDown(KeyCode.Return))
            {
                TryLoadCrate();
            }
        }
    }

    private bool MovePlayer((int xDX, int yDY) posD)
    {
        bool pickedUp = false;
        bool dropped = false;
        // 1. check if the new location is within borders
        // 2. check if there is a crate to be released
        // 3. check if there is a crate to be picked up AND can be picked up
        // 4. release a crate, if any
        // 5. move player
        // 6. pick up a crate, if any

        (int oldX, int oldY) oldPos = ((int)transform.position.x, (int)transform.position.y);
        (int newX, int newY) newPos = ((posD.xDX + oldPos.oldX), (posD.yDY + oldPos.oldY));

        // 1. if new position outside of borders - return false
        if (!gridRef.WithinBorders(newPos))
        {
            return false;
        }

        // 2. check if there is a crate to be released
        // bool oldContent = (Content != 0); // deprecated

        // 3. check if there is a crate to be picked up
        int newContent = 0;
        if (gridRef.storageGrid[newPos.newX, newPos.newY] != null)
        {
            newContent = gridRef.storageGrid[newPos.newX, newPos.newY].Content;
        }

        bool positionViableForRelease = gridRef.WithinStorage(oldPos);

        // are we releasing crate?
        if (clampsOpen && positionViableForRelease && Content != 0 && currentZap >= zapToPlace)
        {
            // release crate:
            // create crate
            crateMaster.PlaceCrate(Content, oldPos);
            Content = 0;
            dropped = true;
            LoseZap(zapToPlace);
        }

        // are we picking up a crate
        if (Content == 0 && newContent != 0 && clampsOpen && currentZap >= zapToPickUp)
        {
            // pick up crate
            // - get crate from position
            Crate crateToBePickedUp = gridRef.storageGrid[newPos.newX, newPos.newY];
            // - update player content
            Content = crateToBePickedUp.Content + 1;
            // - remove crate from grid and list
            crateMaster.RemoveCrate(crateToBePickedUp);
            // -- remove crate grom group
            master.groupMaster.RemoveCrateFromGroup(crateToBePickedUp);
            newContent = 0;
            pickedUp = true;
            LoseZap(zapToPickUp);
        }

        // can we move
        bool allowMovement = false;
        if ((newContent == 0) ||
            (Content == 0 && newContent != 0 && clampsOpen))
        {
            allowMovement = true;
        }

        // move, if possible
        if (allowMovement)
        {
            Vector3 newPosition = new Vector3(newPos.newX, newPos.newY);
            transform.position = newPosition;
        }

        // check if we have picked up or dropped a crate
        if (pickedUp || dropped)
        {
            // trigger each group to rebuild content
            master.groupMaster.RecheckAllGroupsContent();
        }

        // TODO ? Temp return
        return true;
    }

    private bool TryLoadCrate()
    {
        // check if there is space for the new content
        if (Content != 0)
        {
            return false;
        }

        // check if in position
        Factory workingFactory = null;
        foreach (Factory factory in gridRef.factoryGrid)
        {
            if (factory.PosGrid_X == transform.position.x && factory.PosGrid_Y == transform.position.y)
            {
                workingFactory = factory;
            }
        }

        // are we in position ??
        if (workingFactory == null)
        {
            return false;
        }

        // load content
        Content = workingFactory.Content;
        workingFactory.ReStock();
        return true;
    }

    public void GainZap(int zapGain)
    {
        totalZap += zapGain;
        currentZap += zapGain;
        if (currentZap >= 999)
        {
            master.UIMaster.PopUpWin();
        }

        if (totalZap > zapBounds[master.orderMaster.GetComplexity()- 1])
        {
            master.orderMaster.IncreaseComplexity();
        }
        Debug.Log($"Gained {zapGain} zap");
    }

    private void LoseZap(int zapLoss)
    {
        currentZap -= zapLoss;
        if (currentZap <= 0)
        {
            master.UIMaster.PopUpLose();
        }
        Debug.Log($"Lost {zapLoss} zap");
    }

}
