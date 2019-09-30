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
    private List<int> groupList;
    private int nextGroupNumber;

    // Start is called before the first frame update
    void Start()
    {
        cratesList = new List<GameObject>();
        groupList = new List<int>();
        InitGroupGrid();
        nextGroupNumber = 0;
    }

    private int GetSmallestUnusedGroupNumber()
        // TODO :: Smallest Available Group - Reworked
    {

        // increase nextGroupNumber
        nextGroupNumber++;

        // add nextGroupNumber to the list
        groupList.Add(nextGroupNumber);

        return nextGroupNumber;
    }

    private void InitGroupGrid()
    {
        // TODO : Fix it - getting the area dimensions before they are updated
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

            Debug.Log("crate @ " + randomW + " / " + randomH + " in gr " + nextGroupNumber);
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
        Debug.Log("crate @ " + coordW + " / " + coordH + " in gr: " + nextGroupNumber);

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


    public void EraseCrate(GameObject oldCrate, int posW, int posH)
    {
        cratesList.Remove(oldCrate);
        // absolute coordinates
        int absPosW = storageCreator.GetAbsFromRelW(posW);
        int absPosH = storageCreator.GetAbsFromRelH(posH);

        int oldGroupNumber = groupGrid[absPosW, absPosH];

        // if there are no nbrs - remove the group from the group list
        // TODO :: Remove group from group list - removing single crate
        groupList.Remove(groupGrid[absPosW, absPosH]);

        // mark position in the grid as vacant
        storageCreator.MarkVacancyGrid(absPosW, absPosH, true);

        // remove the group index from the group grid
        groupGrid[absPosW, absPosH] = 0;


        // create stack
        Stack<int> startingStack = new Stack<int>();

        // 01 iterate nbrs

        // check for any nbrs
        // NEW ITERATION - TRYG!!

        for (double nbrsAngle = 0; nbrsAngle <= Math.PI * 1.5; nbrsAngle += Math.PI * 0.5)
        {
            int nbrXCoord = (int)Math.Sin(nbrsAngle);
            int nbrYCoord = (int)Math.Cos(nbrsAngle);

            // nbr coords
            int nbrX = absPosW + nbrXCoord;
            int nbrY = absPosH + nbrYCoord;
            // if nbr within borders
            if (IsWithinBorders(nbrX, nbrY))
            {
                // if there is a nbr at this position
                if (!storageCreator.IsTileVacant(nbrX, nbrY))
                {
                    // found at least one nbr
                    // add nbr to stack
                    int nbrCoord = nbrY * 100 + nbrX;
                    startingStack.Push(nbrCoord);
                    // pick a group number
                    // send the stack to the processor
                    int newGroupNumber = GetSmallestUnusedGroupNumber();
                    WorkNeighboursOf(startingStack, newGroupNumber);
                }
            }
        }

        // Remove color from colorChunks
        RemoveColor(oldGroupNumber);
    }

    private void RemoveColor(int oldGroupNumber) // TODO :: RemoveColor(groupNumber)
    {
        // FIRST check if there is only one color!
        // remove the color associated with the removed group
        // find the color index in the colorChunks - tbd
        // find the previous chunk - prev
        // add tbd.value+1 to prv.value
        // remove tbd
        if (storageCreator.colorChunks.Count > 1)
        {
            int obsoleteColorIndex = storageCreator.groupToColorMap[oldGroupNumber];
            int prevColorIndex = FindPrevColorIndex(obsoleteColorIndex);
            int obsoleteColorChunkSize = storageCreator.colorChunks[obsoleteColorIndex];
            storageCreator.colorChunks[prevColorIndex] = storageCreator.colorChunks[prevColorIndex] + storageCreator.colorChunks[obsoleteColorIndex] + 1;
            storageCreator.colorChunks.Remove(obsoleteColorIndex);
            // un-map the old group from it's color
            storageCreator.groupToColorMap.Remove(oldGroupNumber);
        }
        else // only one group - create new color, remap to default group 1
        {
            storageCreator.groupToColorMap.Remove(oldGroupNumber);
            storageCreator.colorChunks.Clear();
            // pich a random color
            int randomStartingColorIndex = UnityEngine.Random.Range(0, storageCreator.paletteArray.Length - 1);
            storageCreator.colorChunks.Add(randomStartingColorIndex, storageCreator.paletteArray.Length - 1);
            // map to the first possible group
            storageCreator.groupToColorMap[1] = randomStartingColorIndex;
        }
    }

    private int FindPrevColorIndex(int obsoleteColorIndex)
    {
        // iterate through keys in colorChunks; find the largest one that is smaller than obsoleteColorIndex
        int prevIndex = 0;
        int largestindex = 0;
        foreach (var kvp in storageCreator.colorChunks)
        {
            if (kvp.Key >= prevIndex && kvp.Key < obsoleteColorIndex)
            {
                prevIndex = kvp.Key;
            }
            if (kvp.Key > largestindex)
            {
                largestindex = kvp.Key;
            }
        }
        if (prevIndex == 0) // if no previous index was found, obsoleteColorIndex was the smallest - return the largest
        {
            return largestindex;
        }
        return prevIndex;
    }

    private bool WorkNeighboursOf(Stack<int> progressStack, int newGroupNumber)
    {
        // INGRESS
        int crrCrateCode = progressStack.Peek();
        int posX = crrCrateCode % 100;
        int posY = (crrCrateCode - (crrCrateCode % 100)) / 100;
        // find if any unchecked nbrs -- send them for processing
        for (double nbrsAngle = 0; nbrsAngle <= Math.PI * 1.5; nbrsAngle += Math.PI * 0.5)
        {
            int nbrDeltaX = (int)Math.Sin(nbrsAngle);
            int nbrDeltaY = (int)Math.Cos(nbrsAngle);
            // nbr coords
            int nbrX = posX + nbrDeltaX;
            int nbrY = posY + nbrDeltaY;
            // if nbr within borders
            if (IsWithinBorders(nbrX, nbrY))
            {
                // if there is a nbr at this position AND it is unchecked
                if ((!storageCreator.IsTileVacant(nbrX, nbrY)) &&
                    !progressStack.Contains(nbrY*100+nbrX))
                {
                    // there is an unchecked nbr at this position - stack it and send it to the processor
                    progressStack.Push(nbrY * 100 + nbrX);
                    WorkNeighboursOf(progressStack, newGroupNumber);
                }
            }
        }

        // if no nbrs are present
        // EGRESS
        // mark the crate with the new group
        // remove the crate from the stack
        progressStack.Pop();
        groupGrid[posX, posY] = newGroupNumber;

        return true;
    }

    private bool AssignCrateToGroup(int crateType, int cratePositionW, int cratePositionH)
    {

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
        if (relH + 1 < storageCreator.storageAreaH)
        {
            if (!storageCreator.IsTileVacant(relW, relH + 1))
            {
                up = groupGrid[relW, relH + 1];
            }
        }
        // right
        if (relW + 1 < storageCreator.storageAreaW)
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
            groupGrid[relW, relH] = GetSmallestUnusedGroupNumber();
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
            // TODO : CHECK FOR ILLEGAL GROUP CREATION
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
        // TODO :: Remove group from group list - reassign group
        groupList.Remove(targetGroup);
        // TODO :: Remove color from dictionary - has this been done ?
        RemoveColor(targetGroup);
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

    private bool IsWithinBorders(int posX, int posY)
    {
        return storageCreator.IsWithinBorders(posX, posY);
    }

}
