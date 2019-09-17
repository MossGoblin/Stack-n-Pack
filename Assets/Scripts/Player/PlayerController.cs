using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // player holding space
    public GameObject crateOnHold;

    // player gameobject transform
    private Transform playerTransform;

    // Crate controller ref
    [SerializeField] GameObject crateController;

    // storage area master link
    [SerializeField] StorageAreaCreator storageCreator;

    // release trigger
    public bool releaseFlag;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponentInParent<Transform>();
        releaseFlag = false;
        crateOnHold = null;
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
            releaseFlag = true;
        }
        else
        {
            releaseFlag = false;
        }

        return true;
    }

    // Receives a direction; checks if the move is valid
    void MovePlayer(float directionY, float directionX)
    {
        // check if the player can move there
        // check for storage boundaries
        int desiredX = (int)(playerTransform.position.x + directionX);
        int desiredY = (int)(playerTransform.position.y + directionY);

        // check if the space is inside the board

        if (((desiredY <= storageCreator.storageAreaEndPointH) &&
             (desiredX <= storageCreator.storageAreaEndPointW) &&
             (desiredY >= storageCreator.storageAreaOriginH) &&
             (desiredX >= storageCreator.storageAreaOriginW)))
        {

            // check if there is a crate at the spot
            if ((storageCreator.IsTileAvailableForMove(desiredX - (int)(storageCreator.storageAreaOriginW), desiredY - (int)(storageCreator.storageAreaOriginH))))
            {
                // if there is a crate on the spot, check if the storage is empty
                if (crateOnHold == null)
                {
                    // TODO : add the new crate in the hold
                    // iterate through crate list, find crate by coordinates?
                    GameObject crateToCollect = GetCrateFromCoordinates(desiredX - (int)(storageCreator.storageAreaOriginW), desiredY - (int)(storageCreator.storageAreaOriginH));
                    crateOnHold = crateToCollect;
                    // TODO : HERE
                    playerTransform.Translate(new Vector3(directionX, directionY));
                }
                else
                {
                    // can not move!
                }
            }
            else
            {
                // no crate at the spot - free to move
                playerTransform.Translate(new Vector3(directionX, directionY));

            }

            Debug.Log("moved to: " + playerTransform.position.x + " / " + playerTransform.position.y);
        }
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
