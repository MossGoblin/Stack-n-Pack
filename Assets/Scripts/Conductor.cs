using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    // props and fields
    public int inputWidth;
    public int inputHeight;

    // score/energy variables
    public int totalZap = 0;
    public int currentZap = 0;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Sprite serviceLaneSprite;
    [SerializeField] GameObject factoryLeft;
    [SerializeField] GameObject factoryRight;

    // refs
    public PlayerMaster playerMaster;
    public Storage gridRef;
    public CrateMaster crateMaster;
    public GroupMaster groupMaster;
    public OrderMaster orderMaster;
    public int[] Rarity { get; private set; } = new int[] { 29, 24, 19, 14, 9, 5 };

    private void Start()
    {
        // init grid
        int width = Math.Max(inputWidth, 7);
        int height = Math.Max(inputHeight, 5);
        gridRef = new Storage(width, height);
        // position camera
        PositionCamera();
        // build storage space
        BuildSpace();
        // place factories
        PlaceFactories();
        // generate orders
        // GenerateOrders();
        // place player
        PlacePlayer();
    }

    private void HandleInput()
    {
        bool dispatchTriggered = false;
        // TODO Place Input Here
                // FIXME : To be deleted - debugging option only
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Test Place a Crate
            LoadTestContent();
        }

        // order dispatching here
        int orderToDispatch = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            orderToDispatch = 0;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            orderToDispatch = 1;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            orderToDispatch = 2;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            orderToDispatch = 3;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            orderToDispatch = 4;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            orderToDispatch = 5;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            orderToDispatch = 6;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            orderToDispatch = 7;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            orderToDispatch = 8;
            dispatchTriggered = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            orderToDispatch = 9;
            dispatchTriggered = true;
        }
        if (dispatchTriggered)
        {
            orderMaster.TryToDispatchOrder(orderToDispatch);
        }
    }

    private void GenerateOrders()
    {
        Debug.Log("Orders to be generated");
        // TODO :: Issue orders
    }

    private void Update()
    {
        ApplyTileHighlight();

        HandleInput();
    }

    private void ApplyTileHighlight()
    {
        foreach (GameObject tile in gridRef.tileGrid)
        {
            int posX = (int)tile.transform.position.x;
            int posY = (int)tile.transform.position.y;
            if (gridRef.storageGrid[posX, posY] != null)
            {
                int groupIndex = gridRef.storageGrid[posX, posY].Group;
                Group group = groupMaster.GetGroupByIndex(groupIndex);
                int colorIndex = groupMaster.groupToColorMap[group];
                tile.GetComponent<SpriteRenderer>().color = crateMaster.paletteArray[colorIndex];
            }
            else
            {
                // FIXME :: Fix default tile color
                tile.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    private void PlacePlayer()
    {
        //throw new NotImplmentedException();
        Vector3 playerStartingPosition = new Vector3(gridRef.StorageWidth / 2, gridRef.StorageHeight / 2);
        Transform playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerTransform.transform.position = playerStartingPosition;
    }
    
    private void PositionCamera()
    {
        Camera mainCamera = Camera.main;
        int verticalUISpace = 8; // space above the storage grid plus padding (5 + 3)
        mainCamera.orthographicSize = (verticalUISpace + gridRef.StorageHeight + 1)/2;
        float newCameraPosY = mainCamera.orthographicSize - 1.5f;
        float cameraRatio = mainCamera.aspect;
        float newCameraPosX = mainCamera.transform.position.x + (gridRef.StorageWidth / 2);
        mainCamera.transform.position = new Vector3(newCameraPosX, newCameraPosY, -40);
    }

    private void LoadTestContent()
    {
        bool done = false;
        while(!done)
        {
            int newContent = (int)UnityEngine.Random.Range(1, 7);
            if (newContent != playerMaster.Content)
            {
                playerMaster.Content = newContent;
                done = true;
            }
        }
    }

    public void BuildSpace()
    {
        // get parent
        Transform tileHolder = GameObject.Find("TileHolder").GetComponent<Transform>();

        // iterate dimensions
        for (int countY = 0; countY < gridRef.StorageHeight; countY++)
        {
            for (int countX = 0; countX < gridRef.StorageWidth; countX++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(countX, countY, 0), Quaternion.identity, tileHolder);
                // re-color if in service lane
                if (countX == 0 ||
                    countX == gridRef.StorageWidth - 1||
                    countY == 0 ||
                    countY == gridRef.StorageHeight - 1)
                {
                    newTile.GetComponent<SpriteRenderer>().sprite = serviceLaneSprite;
                }
                gridRef.tileGrid[countX, countY] = newTile;
            }
        }
    }

    private void PlaceFactories()
    {
        Transform factoryHolder = GameObject.Find("FactoryHolder").GetComponent<Transform>();

        int[,] factoryCoordinates = new int[,] { 
            { 0, 1 }, 
            { 0, gridRef.StorageHeight - 2 }, 
            { gridRef.StorageWidth - 1, gridRef.StorageHeight - 2 }, 
            { gridRef.StorageWidth - 1, 1 } }; // X, Y for each factory

        for (int count = 0; count < 4; count++)
        {
            Vector3 position = new Vector3((float)factoryCoordinates[count, 0], (float)factoryCoordinates[count, 1]);
            GameObject factoryGO;
            if(count<2)
            {
                factoryGO = Instantiate(factoryLeft, position, Quaternion.identity, factoryHolder);
            }
            else
            {
                factoryGO = Instantiate(factoryRight, position, Quaternion.identity, factoryHolder);
            }
            Factory factoryOBJ = new Factory(factoryCoordinates[count, 0], factoryCoordinates[count, 1], this, factoryGO);
            // register factory
            gridRef.factoryGrid[count] = factoryOBJ;
        }
    }
}
