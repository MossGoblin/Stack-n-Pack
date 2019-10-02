using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageMaster : MonoBehaviour
{
    // Start is called before the first frame update
    StorageAreaCreator areaCreator;
    CrateMaster crateMaster;

    [SerializeField] PlayerController player;
    [SerializeField] int initialRandomSpawnNumber;

    void Start()
    {
        areaCreator = GetComponentInChildren<StorageAreaCreator>();
        crateMaster = GetComponentInChildren<CrateMaster>();

        if (areaCreator.CreateFloor())
        {
            Debug.Log("Floor created");
        }

        areaCreator.PlacePipes();

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // check if the hold is empty and create a random crate inside if it is
            if (player.crateOnHold == 0)
            {
                int randType = (int)Mathf.Round(Random.Range(1, 6));
                player.crateOnHold = randType;
            }

        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //if (areaCreator.HasSpace())
            //{
            //    float positionW = 0;
            //    float positionH = 1;
            //    int crateType = 3;
            //    crateMaster.CreateCrateByType(crateType, positionW, positionH);
            //}
            //areaCreator.PlacePipes();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //crateMaster.BuildGroups();

        }
    }
}
