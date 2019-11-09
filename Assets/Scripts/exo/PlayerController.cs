using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // state
    public int Content;
    public bool clampsOpen;

    // selfrefs
    Transform transform;

    // refs
    public Conductor master;
    private Storage gridRef;
    private CrateMaster crateMaster;

    // Start is called before the first frame update
    void Start()
    {
        Content = 0;
        transform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        crateMaster = master.crateMaster;
        this.gridRef = master.gridRef;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
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
    }

    private bool MovePlayer((int xDX, int yDY) posD)
    {
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
        bool oldContent = (Content != 0);

        // 3. check if there is a crate to be picked up
        int newContent = 0;
        if (gridRef.storageGrid[newPos.newX, newPos.newY] != null)
        {
            newContent = gridRef.storageGrid[newPos.newX, newPos.newY].Content;
        }

        // TODO :: HERE - RELEASING CRATE
        bool positionViableForRelease = gridRef.WithinStorage(oldPos);

        // going through the motions
        // are we releasing crate?
        if (clampsOpen && positionViableForRelease && Content != 0)
        {
            // release crate:
            // create crate
            crateMaster.PlaceCrate(Content, oldPos);
            Content = 0;
        }

        // are we picking up a crate
        if (Content == 0 && newContent != 0)
        {
            // move player
            // pick up crate
        }


        // TODO : TEMP Move Player
        Vector3 newPosition = new Vector3(newPos.newX, newPos.newY);
        transform.position = newPosition;
        // TODO :: Temp return
        return true;
    }
}
