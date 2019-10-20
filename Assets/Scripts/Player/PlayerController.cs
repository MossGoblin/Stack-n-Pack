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

    // factory coordinates
    List<int> factoryCoordinates;

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

        // Initiate factory coordinates list
        factoryCoordinates = new List<int>();
        factoryCoordinates.Add(100); // bottom left; y = 1, x = 0
        factoryCoordinates.Add((storageMaster.storageHight - 1) * 100); // top left; y = storage area hight - 1, x = 0
        factoryCoordinates.Add((storageMaster.storageHight - 1) * 100 + (storageMaster.storageWidth - 1)); // top right; y = storage area hight - 1, x = storage area width
        factoryCoordinates.Add(100 + storageMaster.storageWidth - 1); // bottom right; y = 1, x = storage area width
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
            int posX = storageMaster.GetGridFromWorld_X((int)playerTransform.position.x);
            int posY = storageMaster.GetGridFromWorld_Y((int)playerTransform.position.y);
            int playerAddress = posY * 100 + posX;
            crateOnHold = LoadCrateFromFactory(playerAddress);
        }

        return true;
    }

    /// <summary>
    /// Returns the type number from a factory that matches the player grid address
    /// </summary>
    /// <param name="playerAddress">Player address = y(grid)*100 + x(grid)</param>
    /// <returns></returns>
    private int LoadCrateFromFactory(int playerAddress)
    {
        int factoryNumber = 0;
        // First check if no crate on hold
        if (crateOnHold == 0)
        {
            // check if on the proper space
            // get the factory number by the player address
            if (!factoryCoordinates.Contains(playerAddress))
            {
                Debug.Log("Player not in a position to load a crate");
                return 0;
            }
            factoryNumber = factoryCoordinates.IndexOf(playerAddress);
        }

        int result = storageMaster.factoryMap[factoryNumber];

        // reset factory content
        int randomType = crateMaster.GetRandomType();
        storageMaster.factoryMap[factoryNumber] = randomType;

        // rcolor factory
        RecolorFactory(factoryNumber);

        return result;
    }

    private void RecolorFactory(int factoryNumber)
    {
        Color[] colorPool = new Color[]
            {
                new Color(0.5f, 0.4f, 0.0f, 1f),
                new Color(1, 1, 0, 1),
                new Color(1, 0, 0, 1),
                new Color(0, 1, 0, 1),
                new Color(0, 0, 1, 1),
                new Color(1, 0, 1, 1)
            };
        storageMaster.factoryList[factoryNumber].GetComponent<SpriteRenderer>().color = colorPool[storageMaster.factoryMap[factoryNumber]];
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

        int currentX= storageMaster.GetGridFromWorld_X(currentW); // grid view
        int currentY = storageMaster.GetGridFromWorld_Y(currentH); // grid view

        int destinationW = (int)(currentW + directionX); // world view
        int destinationH = (int)(currentH + directionY); // world view

        int destinationX = storageMaster.GetGridFromWorld_X(destinationW); // grid view
        int destinationY = storageMaster.GetGridFromWorld_Y(destinationH); // grid view

        // 01 check if the DESTINATION space is inside the board
        if (IsWithinBorders(destinationX, destinationY))
        {
            // within borders
            destinationWithinBorders = true;
        }

        // 01.1 - check if CURRENT space is in the service area
        if (currentX == 0 ||
            currentX == storageMaster.storageWidth - 1 ||
            currentY == 0 ||
            currentY == storageMaster.storageHight - 1)
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
