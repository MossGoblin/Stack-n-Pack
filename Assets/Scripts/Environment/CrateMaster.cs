using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public int[,] groupGrid;
    // current number of groups
    int groupCount;

    // 2 - a list of groups
    private List<bool> groupList;

    // Start is called before the first frame update
    void Start()
    {
        cratesList = new List<GameObject>();
        groupList = new List<bool>();
        InitGroupGrid();
        groupCount = GetSmallestUnusedGroupNumber();
    }

    private int GetSmallestUnusedGroupNumber()
    {
        if (groupList.IndexOf(false) >= 0)
        {
            return groupList.IndexOf(false) + 1;
        }
        else
        {
            groupList.Add(true);
            return groupList.Count;
        }
    }

    private void InitGroupGrid()
    {
        groupGrid = new int[storageCreator.storageAreaW, storageCreator.storageAreaH];
        for (int countH = 0; countH < storageCreator.storageAreaH; countH++)
        {
            for (int countW = 0; countW < storageCreator.storageAreaW; countW++)
            {
                groupGrid[countW, countH] = 0;
            }
        }
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
        int coordAbsW = storageCreator.GetComponent<StorageAreaCreator>().GetAbsFromRelW((int)coordW);
        int coordAbsH = storageCreator.GetComponent<StorageAreaCreator>().GetAbsFromRelH((int)coordH);
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
        int registerPositionW = (int)newCrate.transform.position.x;
        int registerPositionH = (int)newCrate.transform.position.y;
        AssignCrateToGroup(GetCrateTypeFromName(newCrate), registerPositionW, registerPositionH);

    }


    public void EraseCrate(GameObject newCrate, int absPosW, int absPosH)
    {
        cratesList.Remove(newCrate);

        // relative coordinates
        int posW = storageCreator.GetAbsFromRelW(absPosW);
        int posH = storageCreator.GetAbsFromRelH(absPosH);

        storageCreator.MarkVacancyGrid(posW, posH, true);
        groupGrid[posW, posH] = 0;

        // REMOVE CRATE FROM GROUP

        // find all nbrs
        // prep nbr grid -- 0 to 3: up, right, down, left
        int[] nbrs = new int[4];
        for (int count = 0; count < 4; count++)
        {
            nbrs[count] = -1;
        }
        // up
        if (posH + 1 <= storageCreator.storageAreaH)
        {
            if (!storageCreator.IsTileVacant(posW, posH + 1))
            {
                nbrs[0] = posW * 100 + posH + 1;
            }
        }
        // right
        if (posW + 1 <= storageCreator.storageAreaW)
        {
            if (!storageCreator.IsTileVacant(posW + 1, posH))
            {
                nbrs[1] = (posW + 1) * 100 + posH;
            }
        }
        // down
        if (posH - 1 >= 0)
        {
            if (!storageCreator.IsTileVacant(posW, posH - 1))
            {
                nbrs[2] = posW * 100 + posH - 1;
            }
        }
        // left
        if (posW - 1 >= 0)
        {
            if (!storageCreator.IsTileVacant(posW - 1, posH))
            {
                nbrs[3] = (posW - 1) * 100 + posH;
            }
        }
        // check if any nbrs
        if (nbrs.Sum() > -4) // at least 1 nbr
        {
            // set up Q
            Queue<int> startQ = new Queue<int>();
            for (int count = 0; count < 3; count++)
            {
                if (nbrs[count] >= 0)
                {
                    startQ.Enqueue(nbrs[count]);
                }
            }

            // work through the starting Q
            while(startQ.Count>0)
            {
                Queue<int> checkQ = new Queue<int>();
                int newGroupNumber = GetSmallestUnusedGroupNumber();
                int startCell = startQ.Dequeue();
                checkQ.Enqueue(startCell);
                WorkNeighboursOf(checkQ, startCell, newGroupNumber);
            }
        }


        // put cell in Q
        // for each nbr of removed crate:
        // A : make sure it is in Q
        // A if no nbrs - A over
        // assign new number
        // deQ
    }

    private bool WorkNeighboursOf(Queue<int> checkQ, int cell, int newGroupNumber)
    {
        // INGRESS
        // collect all nbrs and EnQ
        int posH = cell % 100;
        int posW = (cell - (cell % 100)) / 100;
        int nbrsNumber = 0;
        // up
        if (posH + 1 <= storageCreator.storageAreaH)
        {
            if (!storageCreator.IsTileVacant(posW, posH + 1))
            {
                checkQ.Enqueue(posW + 100 * (posH+1));
                nbrsNumber++;
                WorkNeighboursOf(checkQ, posW + 100 * (posH + 1), newGroupNumber);
            }
        }
        // right
        if (posW + 1 <= storageCreator.storageAreaW)
        {
            if (!storageCreator.IsTileVacant(posW + 1, posH))
            {
                checkQ.Enqueue((posW+1) + 100 * posH);
                nbrsNumber++;
                WorkNeighboursOf(checkQ, (posW + 1) + 100 * posH, newGroupNumber);
            }
        }
        // down
        if (posH - 1 >= 0)
        {
            if (!storageCreator.IsTileVacant(posW, posH - 1))
            {
                checkQ.Enqueue(posW + 100 * (posH-1));
                nbrsNumber++;
                WorkNeighboursOf(checkQ, posW + 100 * (posH - 1), newGroupNumber);
            }
        }
        // left
        if (posW - 1 >= 0)
        {
            if (!storageCreator.IsTileVacant(posW - 1, posH))
            {
                checkQ.Enqueue((posW-1) + 100 * posH);
                nbrsNumber++;
                WorkNeighboursOf(checkQ, (posW - 1) + 100 * posH, newGroupNumber);
            }
        }
        if (nbrsNumber == 0)
        {
            // - - -
            // EGRESS
            int crrCell = checkQ.Dequeue();
            int newPosH = crrCell % 100;
            int newPosW = (crrCell - (crrCell % 100)) / 100;
            groupGrid[newPosW, newPosH] = newGroupNumber;
            return true;
        }


        return true;
    }

    private bool AssignCrateToGroup(int crateType, int cratePositionW, int cratePositionH)
    {
        // advance group counter

        // perform neighbour check
        // collect neighbours
        int up = 0;
        int right = 0;
        int down = 0;
        int left = 0;
        int relW = storageCreator.GetAbsFromRelW(cratePositionW);
        int relH = storageCreator.GetAbsFromRelH(cratePositionH);
        List<int> adjGroups = new List<int>();
        List<int> adjGroupList = new List<int>();

        // up
        if (relH + 1 <= storageCreator.storageAreaH)
        {
            if (!storageCreator.IsTileVacant(relW, relH + 1))
            {
                up = groupGrid[relW, relH + 1];
            }
        }
        // right
        if (relW + 1 <= storageCreator.storageAreaW)
        {
            if (!storageCreator.IsTileVacant(relW + 1, relH))
            {
                right = groupGrid[relW + 1, relH];
            }
        }
        // down
        if (relH - 1 >= 0)
        {
            if (!storageCreator.IsTileVacant(relW, relH - 1))
            {
                down = groupGrid[relW, relH - 1];
            }
        }
        // left
        if (relW - 1 >= 0)
        {
            if (!storageCreator.IsTileVacant(relW - 1, relH))
            {
                left = groupGrid[relW - 1, relH];
            }
        }

        // neighbours collected; see how many different groups there are among them
        if (up + right + down + left == 0) // no neighbours
        {
            groupGrid[relW, relH] = groupCount++;
            return true;
        }

        // build a list of unique group numbers - all above 0
        if (up > 0 && adjGroupList.IndexOf(up) < 0)
        {
            adjGroupList.Add(up);
        }
        if (right > 0 && adjGroupList.IndexOf(right) < 0)
        {
            adjGroupList.Add(right);
        }
        if (down > 0 && adjGroupList.IndexOf(down) < 0)
        {
            adjGroupList.Add(down);
        }
        if (left > 0 && adjGroupList.IndexOf(left) < 0)
        {
            adjGroupList.Add(left);
        }

        // if there is only one neighbouring group
        if (adjGroupList.Count == 1)
        {
            groupGrid[relW, relH] = adjGroupList[0];
            return true;
        }
        else if (adjGroupList.Count > 1)
        {
            // the heavy case
            // find the smallest group number
            int minGroupNumber = adjGroupList[0];
            foreach (int item in adjGroupList)
            {
                minGroupNumber = Mathf.Min(minGroupNumber, item);
            }
            // assign group number
            groupGrid[relW, relH] = minGroupNumber;
            // reassign larger groups
            foreach (int item in adjGroupList)
            {
                if (item != minGroupNumber)
                {
                    RessignGroupTo(item, minGroupNumber);
                }
            }

            return true;
        }
        return false;
    }

    private void RessignGroupTo(int targetGroup, int minGroupNumber)
    {
        for (int countW = 0; countW < storageCreator.storageAreaW; countW++)
        {
            for (int countH = 0; countH < storageCreator.storageAreaH; countH++)
            {
                if (groupGrid[countW, countH] == targetGroup)
                {
                    groupGrid[countW, countH] = minGroupNumber;
                }
            }
        }
    }

    private GameObject GetCrateByCoordinates(float positionW, float positionH)
    {
        foreach (GameObject crate in cratesList)
        {
            if ((crate.transform.position.x == positionW) && (crate.transform.position.y == positionH))
            {
                return crate;
            }
        }
        return null;
    }


    public int GetCrateTypeFromName(GameObject crate)
    {
        string crateName = crate.transform.name;

        switch (crateName)
        {
            case "crate 01":
                return 1;
            case "crate 02":
                return 2;
            case "crate 03":
                return 3;
            default:
                return 4;
        }
    }

}
