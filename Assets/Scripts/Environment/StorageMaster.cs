using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageMaster : MonoBehaviour
{
    // Start is called before the first frame update
    StorageAreaCreator areaCreator;
    CrateMaster crateMaster;

    [SerializeField] int initialRandomSpawnNumber;

    void Start()
    {
        areaCreator = GetComponentInChildren<StorageAreaCreator>();
        crateMaster = GetComponentInChildren<CrateMaster>();

        if (areaCreator.CreateFloor())
        {
            Debug.Log("Floor created");
            if (crateMaster.SpawnCrate(initialRandomSpawnNumber));
            {
                Debug.Log("master: Done");
            }
        }



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (areaCreator.HasSpace())
            {
                crateMaster.SpawnCrate(1);
            }
        }
        
    }
}
