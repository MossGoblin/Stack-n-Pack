using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // player holding space
    public int crateOnHold;
    public GameObject incomingCrate;

    // player gameobject transform
    private Transform playerTransform;

    // Crate controller ref
    [SerializeField] GameObject crateController;

    // storage area master link
    [SerializeField] StorageAreaCreator storageCreator;

    // release trigger
    public bool releaseClampFlag;
    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log("Up");
            MovePlayer(1,0);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left");
            MovePlayer(0,-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right");
            MovePlayer(0,1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down");
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

        // check for storage boundaries
        int desiredW = (int)(playerTransform.position.x + directionX);
        int desiredH = (int)(playerTransform.position.y + directionY);

        // 01 check if the space is inside the board
        // PLAYERS BORDER CHECK NOT WORKING

        if (((desiredH <= storageCreator.storageAreaEndPointH) &&
             (desiredW <= storageCreator.storageAreaEndPointW) &&
             (desiredH >= storageCreator.storageAreaOriginH) &&
             (desiredW >= storageCreator.storageAreaOriginW)))
        {
            // within borders
            withinBorders = true;
        }

        // 02 check if there is a crate on hold
        if (crateOnHold != 0)
        {
            crateToBeReleased = true;
        }

        // 03 check if the crate will be released
        if (crateToBeReleased && releaseClampFlag)
        {
            crateIsReleased = true;
        }

        // 04 check if there is crate to be picked up
        //if (!(storageCreator.IsTileVacant(desiredX - (int)(storageCreator.storageAreaOriginW), desiredY - (int)(storageCreator.storageAreaOriginH))))
        //{
        //    crateToBePickedUp = true;
        //}
        int gridX = storageCreator.GetAbsFromRelW(desiredW);
        int gridY = storageCreator.GetAbsFromRelH(desiredH);
        if (withinBorders && !(storageCreator.IsTileVacant(gridX, gridY)))
        {
            crateToBePickedUp = true;
        }

        // 05 start evaluating and moving - case by case
        if (withinBorders)
        {

            // 05.1 dump old crate if any
            if (crateIsReleased)
            {
                crateController.GetComponent<CrateMaster>().CreateCrateByType(crateOnHold, transform.position.x, transform.position.y);
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
                GameObject incomingCrate = GetCrateFromCoordinates((int)transform.position.x, (int)transform.position.y);
                crateOnHold = crateController.GetComponent<CrateMaster>().GetCrateTypeFromName(incomingCrate);
                crateController.GetComponent<CrateMaster>().EraseCrate(incomingCrate, (int)transform.position.x, (int)transform.position.y);
                Destroy(incomingCrate);
                int absoluteW = storageCreator.GetComponent<StorageAreaCreator>().GetAbsFromRelW((int)transform.position.x);
                int absoluteH = storageCreator.GetComponent<StorageAreaCreator>().GetAbsFromRelH((int)transform.position.y);
                //storageCreator.GetComponent<StorageAreaCreator>().MarkVacancyGrid(absoluteW, absoluteH, true);
            }
        }

        Debug.Log("moved to: " + playerTransform.position.x + " / " + playerTransform.position.y);
    }



    private GameObject GetCrateFromCoordinates(int coordW, int coordH)
    {
        foreach (GameObject currentCrate in crateController.GetComponent<CrateMaster>().cratesList)
        {
            Transform crrCrateTransform = currentCrate.transform;
            if ((crrCrateTransform.position.x == coordW) && (crrCrateTransform.position.y == coordH))
            {
                return currentCrate;
            }
        }
        return null;
    }
}
