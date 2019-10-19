using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // player holding space
    public int crateOnHold;
    public GameObject incomingCrate;

    // player gameobject transform
    private Transform playerTransform;

    // Crate controller ref
    [SerializeField] CrateController crateMaster;

    // storage area master link
    [SerializeField] StorageController storageMaster;

    // release trigger
    public bool releaseClampFlag;
    // Start is called before the first frame update
    void Start()
    {
        // build refs
        crateMaster = FindObjectOfType<CrateController>();
        storageMaster = FindObjectOfType<StorageController>();
        playerTransform = GetComponentInParent<Transform>();
        releaseClampFlag = false;
        crateOnHold = 0;
        incomingCrate = null;
    }

    // Update is called once per frame
    void Update()
    {
        ManageInput();
    }

    public bool ManageInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //Debug.Log("Up");
            MovePlayer(1,0);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("Left");
            MovePlayer(0,-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Debug.Log("Right");
            MovePlayer(0,1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //Debug.Log("Down");
            MovePlayer(-1,0);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            releaseClampFlag = true;
        }
        else
        {
            releaseClampFlag = false;
        }

        if (Input.GetKeyDown(KeyCode.Return)) // pick up new crate
        {
            // First check if no crate on hold
            if (crateOnHold == 0)
            {

                // check if on the proper space
                float posX = playerTransform.position.x;
                float posY = playerTransform.position.y;
                if (posX == storageMaster.storageAreaOriginW && posY == storageMaster.storageAreaOriginH + 1) // bottom left
                {
                    crateOnHold = storageMaster.factoryMap[0];
                }
                else if (posX == storageMaster.storageAreaOriginW && posY == storageMaster.storageAreaEndPointH - 1) // bottom right
                {
                    crateOnHold = storageMaster.factoryMap[1];
                }
                else if (posX == storageMaster.storageAreaEndPointW && posY == storageMaster.storageAreaOriginH + 1) // top left
                {
                    crateOnHold = storageMaster.factoryMap[2];
                }
                else if (posX == storageMaster.storageAreaEndPointW && posY == storageMaster.storageAreaEndPointH - 1) // top right
                {
                    crateOnHold = storageMaster.factoryMap[3];
                }
            }
        }

        return true;
    }

    void MovePlayer(float directionY, float directionX)
    {
        // validation flags
        bool destinationWithinBorders = false;
        bool holdingCrate = false;
        bool crateIsReleased = false;
        bool crateToBePickedUp = false;
        bool didMove = false;
        bool withinServiceArea = false;

        // current coordinates (int)
        int currentW = (int)playerTransform.position.x; // world view
        int currentH = (int)playerTransform.position.y; // world view

        int currentX= storageMaster.GetAbsFromRelW(currentW); // grid view
        int currentY = storageMaster.GetAbsFromRelH(currentH); // grid view

        int destinationW = (int)(currentW + directionX); // world view
        int destinationH = (int)(currentH + directionY); // world view

        int destinationX = storageMaster.GetAbsFromRelW(destinationW); // grid view
        int destinationY = storageMaster.GetAbsFromRelH(destinationH); // grid view

        // 01 check if the DESTINATION space is inside the board
        if (IsWithinBorders(destinationX, destinationY))
        {
            // within borders
            destinationWithinBorders = true;
        }

        // 01.1 - check if CURRENT space is in the service area
        if (currentX == 0 ||
            currentX == storageMaster.storageAreaW - 1 ||
            currentY == 0 ||
            currentY == storageMaster.storageAreaH - 1)
        {
            withinServiceArea = true;
        }

        // 02 check if there is a crate on hold
        if (this.crateOnHold != 0)
        {
            holdingCrate = true;
        }

        // 03 check if the crate will be released - there is a crate + clamp released + not in the service lane
        if (holdingCrate && releaseClampFlag && !withinServiceArea)
        {
            crateIsReleased = true;
        }

        // 04 check if there is crate to be picked up
        if (destinationWithinBorders && !(storageMaster.IsTileEmpty(destinationX, destinationY)))
        {
            crateToBePickedUp = true;
        }

        // 05 start evaluating and moving - case by case
        if (destinationWithinBorders)
        {

            // 05.1 dump old crate, if any
            if (crateIsReleased)
            {
                crateMaster.GetComponent<CrateController>().SpawnCrateByType(this.crateOnHold, transform.position.x, transform.position.y); // TODO :: FOLLOW
                this.crateOnHold = 0;
            }

            // 05.02 move to new location if OK
            if ((crateToBePickedUp && this.crateOnHold == 0) || !crateToBePickedUp)
            {
                playerTransform.Translate(new Vector3(directionX, directionY)); // DEV : could introduce lerping
                didMove = true;

            }

            // 05.03 pick up new crate if any
            if (didMove && crateToBePickedUp)
            {
                Crate incomingCrate = GetCrateFromCoordinates(destinationX, destinationY);
                GameObject incomingCrateGO = incomingCrate.SelfGO;
                this.crateOnHold = incomingCrate.GetCrateType();

                // TODO :: CHECK if Erase function is objectified
                crateMaster.EraseCrate(incomingCrate);
                Destroy(incomingCrateGO);
            }
        }

        //Debug.Log("moved to: " + playerTransform.position.x + " / " + playerTransform.position.y);
    }



    private Crate GetCrateFromCoordinates(int coordX, int coordY)
    {
        return crateMaster.crateGrid[coordX, coordY];
    }

    private bool IsWithinBorders(int posX, int posY)
    {
        return storageMaster.IsWithinBorders(posX, posY);
    }
}
