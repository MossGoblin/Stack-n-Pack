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
            // TODO :: BUG - pick up new crate - CRATE TYPE ON HOLD DOES NOT MATCH CRATE DELIVERED
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

                // TODO :: HERE If a new crate is picked up - reset the factory
            }
        }

        return true;
    }

    // Receives a direction; checks if the move is valid
    void MovePlayer(float directionY, float directionX)
    {
        // validation flags
        bool withinBorders = false;
        bool crateToBeReleased = false;
        bool crateIsReleased = false;
        bool crateToBePickedUp = false;
        bool moved = false;
        bool withinServiceArea = false;

        // current coordinates in int
        int currentW = (int)playerTransform.position.x;
        int currentH = (int)playerTransform.position.y;

        int currentX= storageMaster.GetAbsFromRelW(currentW);
        int currentY = storageMaster.GetAbsFromRelH(currentH);

        // check for storage boundaries
        int desiredW = (int)(currentW + directionX);
        int desiredH = (int)(currentH + directionY);

        int gridX = storageMaster.GetAbsFromRelW(desiredW);
        int gridY = storageMaster.GetAbsFromRelH(desiredH);

        // 01 check if the space is inside the board
        if (IsWithinBorders(gridX, gridY))
        {
            // within borders
            withinBorders = true;
        }

        // 01.1 - check if in the service area
        if (currentX == 0 ||
            currentX == storageMaster.storageAreaW - 1 ||
            currentY == 0 ||
            currentY == storageMaster.storageAreaH - 1)
        {
            withinServiceArea = true;
        }

        // 02 check if there is a crate on hold
        if (crateOnHold != 0)
        {
            crateToBeReleased = true;
        }

        // 03 check if the crate will be released - there is a crate + clamp released + not in the service lane
        if (crateToBeReleased && releaseClampFlag && !withinServiceArea)
        {
            crateIsReleased = true;
        }

        // 04 check if there is crate to be picked up
        //if (!(storageCreator.IsTileVacant(desiredX - (int)(storageCreator.storageAreaOriginW), desiredY - (int)(storageCreator.storageAreaOriginH))))
        //{
        //    crateToBePickedUp = true;
        //}
        if (withinBorders && !(storageMaster.IsTileVacant(gridX, gridY)))
        {
            crateToBePickedUp = true;
        }

        // 05 start evaluating and moving - case by case
        if (withinBorders)
        {

            // 05.1 dump old crate if any
            if (crateIsReleased)
            {
                crateMaster.GetComponent<CrateController>().SpawnCrateByType(crateOnHold, transform.position.x, transform.position.y);
                crateOnHold = 0;
            }

            // 05.02 move to new location if OK
            if ((crateToBePickedUp && crateOnHold == 0) || !crateToBePickedUp)
            {
                playerTransform.Translate(new Vector3(directionX, directionY));
                moved = true;

            }

            // 05.03 pick up new crate if any
            if (moved && crateToBePickedUp)
            {
                GameObject incomingCrateGO = GetCrateFromCoordinates((int)transform.position.x, (int)transform.position.y);
                Crate incomingCrate = crateMaster.crateGrid[(int)transform.position.x, (int)transform.position.y];
                crateOnHold = incomingCrate.GetCrateType();
                // TODO : Erase function to be objectified
                crateMaster.GetComponent<CrateController>().EraseCrate(incomingCrate);
                //crateMaster.GetComponent<CrateController>().EraseCrate(incomingCrateGO, (int)transform.position.x, (int)transform.position.y);
                Destroy(incomingCrateGO);
                // TODO :: Destoy CRATE
                incomingCrate = null;
            }
        }

        //Debug.Log("moved to: " + playerTransform.position.x + " / " + playerTransform.position.y);
    }



    private GameObject GetCrateFromCoordinates(int coordW, int coordH)
    {
        foreach (GameObject currentCrate in crateMaster.GetComponent<CrateController>().cratesList)
        {
            Transform crrCrateTransform = currentCrate.transform;
            if ((crrCrateTransform.position.x == coordW) && (crrCrateTransform.position.y == coordH))
            {
                return currentCrate;
            }
        }
        return null;
    }

    private bool IsWithinBorders(int posX, int posY)
    {
        return storageMaster.IsWithinBorders(posX, posY);
    }
}
