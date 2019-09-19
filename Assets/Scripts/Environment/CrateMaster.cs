using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateMaster : MonoBehaviour
{
    [SerializeField] GameObject crateOne;
    [SerializeField] GameObject crateTwo;
    [SerializeField] GameObject crateThree;
    [SerializeField] GameObject crateFour;
    [SerializeField] GameObject genericCrate;
    [SerializeField] StorageAreaCreator storageCreator;
    [SerializeField] Transform crateHolder;
    float positionW;
    float positionH;

    [SerializeField] public List<GameObject> cratesList; // list of all the crates
    
    // stack structures
    // 1 - a grid tracking which positions are checked
    private bool[,] checkGrid;
    // temporary grid to track group numbers by cell
    private int[,] groupGrid;

    // 2 - a list of groups - may contain up to toal area / 2 number of groups
    private List<List<GameObject>> groupList;

    // Start is called before the first frame update
    void Start()
    {
        cratesList = new List<GameObject>();
        groupList = new List<List<GameObject>>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool SpawnCrateAtRandomPsition(int number)
    {
        for (int count = 0; count < number; count++)
        {
            // find random coordinates
            float maxW = storageCreator.storageAreaW-1;
            float maxH = storageCreator.storageAreaH-1;
            int randomW = (int)Mathf.Round(UnityEngine.Random.Range(0, maxW));
            int randomH = (int)Mathf.Round(UnityEngine.Random.Range(0, maxH));

            while (!storageCreator.IsTileAvailableForCrate(randomW, randomH))
            {
                randomW = randomW = (int)Mathf.Round(UnityEngine.Random.Range(0, maxW));
                randomH = randomH = (int)Mathf.Round(UnityEngine.Random.Range(0, maxH));
            }

            positionW = randomW + storageCreator.storageAreaOriginW;
            positionH = randomH + storageCreator.storageAreaOriginH;
            Debug.Log("found spot @ " + randomW + " / " + randomH);

            // Choose random crate type
            int crateType = UnityEngine.Random.Range(1, 5);

            bool result = CreateCrateByType(crateType, randomW, randomH);

            Debug.Log("crate @ " + randomW + " / " + randomH);
        }

        return true;
    }

    public bool CreateCrateByType(int crateType, float coordW, float coordH)
    {
        GameObject newCrate = Instantiate(NewCrateByType(crateType), new Vector3(coordW, coordH), Quaternion.identity);
        RegisterCrate(newCrate);
        newCrate.transform.SetParent(crateHolder);
        newCrate.name = "crate 0" + crateType;
        int coordAbsW = storageCreator.GetComponent<StorageAreaCreator>().GetRelToAbs_W((int)coordW);
        int coordAbsH = storageCreator.GetComponent<StorageAreaCreator>().GetRelToAbs_H((int)coordH);
        storageCreator.MarkVacancyGrid((int)coordAbsW, (int)coordAbsH, false);
        Debug.Log("crate @ " + coordW + " / " + coordH);

        return true;
    }

    public GameObject NewCrateByType(int crateType)
    {
        GameObject crateToSpawn;
        if (crateType == 1)
        {
            crateToSpawn = crateOne;
        }
        else if (crateType == 2)
        {
            crateToSpawn = crateTwo;
        }
        else if (crateType == 3)
        {
            crateToSpawn = crateThree;
        }
        else
        {
            crateToSpawn = crateFour;
        }

        return crateToSpawn;
    }

    public void RegisterCrate(GameObject newCrate)
    {
        cratesList.Add(newCrate);
    }

    public void EraseCrate(GameObject newCrate)
    {
        cratesList.Remove(newCrate);
    }

    public void BuildGroups()
    {
        // NON-RECURSIVE APPROACH
        // prepare to loop the grip multiple times
        int loopCount = 0;
        bool cleanCheck = false;
        // build groupGrid
        groupGrid = new int[storageCreator.storageAreaW, storageCreator.storageAreaH];
        for (int countW = 0; countW < storageCreator.storageAreaW; countW++)
        {
            for (int countH = 0; countH < storageCreator.storageAreaH; countH++)
            {
                groupGrid[countW, countH] = 0;
            }
        }

        // loop the grid
        int crrGroup = 1;
        while (!cleanCheck)
        {
            cleanCheck = true;
            for (int countH = 0; countH < storageCreator.storageAreaH; countH++)
            {
                for (int countW = 0; countW < storageCreator.storageAreaW; countW++)
                {
                    // check if cell is occupied
                    if (!storageCreator.IsTileVacant(countW, countH))
                    {
                        int minGroup = crrGroup;
                        int newMinGroup = crrGroup;

                        // check neighbours
                        // check UP, take it's group value if lower
                        //if (countH+1 <= storageCreator.storageAreaH
                        //    && !storageCreator.IsTileVacant(countW, countH + 1))
                        //{
                        //    if (groupGrid[countW, countH + 1] > 0)
                        //    {
                        //        newMinGroup = Mathf.Min(minGroup, groupGrid[countW, countH+1]);
                        //    }
                        //}
                        //// check RIGHT, take it's group value if lower
                        //if (countW+1 <= storageCreator.storageAreaW
                        //    && !storageCreator.IsTileVacant(countW+1, countH))
                        //{
                        //    if (groupGrid[countW+1, countH] > 0)
                        //    {
                        //        newMinGroup = Mathf.Min(minGroup, groupGrid[countW+1, countH]);
                        //    }
                        //}
                        //// check DOWN, take it's group value if lower
                        if (countH-1 >= 0
                            && !storageCreator.IsTileVacant(countW, countH-1))
                        {
                            if (groupGrid[countW, countH-1] > 0)
                            {
                                newMinGroup = Mathf.Min(minGroup, groupGrid[countW, countH-1]);
                            }
                        }
                        // check LEFT, take it's group value if lower
                        if (countW-1 >= 0
                            && !storageCreator.IsTileVacant(countW-1, countH))
                        {
                            if (groupGrid[countW-1, countH] > 0)
                            {
                                newMinGroup = Mathf.Min(minGroup, groupGrid[countW-1, countH]);
                            }
                        }
                        if (newMinGroup != minGroup)
                        {
                            cleanCheck = false;
                        }

                        // assign the celll to a group
                        groupGrid[countW, countH] = newMinGroup;
                    }
                    else
                    {
                        // the cell is not occupied
                        // advance the group count
                        // move along
                        crrGroup++;
                    }
                }
            }

            // AFTER THE GRID HAS BEEN CHECKED, SEE IF IT WAS CLEAN (cleanCheck) AND IF NOT - CHECK AGAIN
            // TODO :: HERE

        }


    }
}
