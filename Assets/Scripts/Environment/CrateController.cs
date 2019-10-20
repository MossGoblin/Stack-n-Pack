using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrateController : MonoBehaviour
{
    [SerializeField] GameObject[] crates;

    StorageController storageController;
    Transform crateHolderTransform;
    float positionW;
    float positionH;

    // crate type rarity structures
    public int[] rarity = new int[] { 30, 25, 20, 15, 10 };
    public int numberOfRarities;

    // stack structures
    // 1 - a grid tracking which positions are checked
    private bool[,] checkGrid;
    // temporary grid to track group numbers by cell
    public Crate[,] crateGrid;
    // current number of groups
    int groupCount;

    // 2 - a list of groups
    private List<int> groupList;
    private int nextGroupNumber;


    // group content
    Dictionary<int, string> groupContent;

    private void Awake()
    {
        numberOfRarities = rarity.Length;

    }
    // Start is called before the first frame update
    void Start()
    {

        // build refs
        storageController = GameObject.FindObjectOfType<StorageController>();
        crateHolderTransform = GameObject.FindObjectOfType<CrateController>().transform;
        
        groupList = new List<int>();
        InitGroupGrid();
        nextGroupNumber = 0;

        // init rarity map - map srate type to rarity indeces

        groupContent = new Dictionary<int, string>();
    }

    // Update is called once per frame
    void Update()
    {
        AssessCrateGroups();

        //if (Input.GetKeyDown(KeyCode.Return)) // temp group content report
        //{
        //    foreach (var group in groupContent)
        //    {
        //        Debug.Log(group.Key + ": " + group.Value);
        //    }
        //}


        // TODO :: DEV temp groupList report
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) // temp group content report
        {
            Debug.Log("== groupcount ==");
            foreach (var group in groupList)
            {
                Debug.Log(group.ToString());
            }
        }
    }


    public void AssessCrateGroups()
    {
        // if there are any groups at all (FOR DEBIGING PURPOSES)
        if (groupList.Count > 0)
        {

        // group by group
        for (int groupCount = 0; groupCount < groupList.Count; groupCount++)
        {
            // get group number
            int currentGroup = groupList[groupCount];
            // prep the group index
            string groupIndexString = "";

            // clear group members
            if (!groupList.Contains(currentGroup))
            {
                groupContent.Remove(currentGroup);
            }

            // clear group content history
            if (!groupContent.ContainsKey(currentGroup))
            {
                groupContent.Add(currentGroup, "");
            }
            groupContent[currentGroup] = "";

            // iterate group members
            for (int countY = 1; countY < storageController.storageHight - 1; countY++)
            {
                for (int countX = 1; countX < storageController.storageWidth - 1; countX++)
                {
                    if (crateGrid[countX, countY] != null &&  crateGrid[countX, countY].Group == currentGroup)
                    {
                        // get the crate type

                        Crate crate = crateGrid[countX, countY];

                        float crateType = (float)crate.GetCrateType();

                        // add the current type to the group index
                        int groupIndex = (int)Mathf.Pow(10, crateType - 1);

                        int currentIndex = 0;
                        if (groupContent.ContainsKey(currentGroup))
                        {
                            if (groupContent[currentGroup] != "")
                            {
                                currentIndex = int.Parse(groupContent[currentGroup]);
                            }
                        }

                        currentIndex += groupIndex;
                        groupIndexString = currentIndex.ToString();

                        groupContent[currentGroup] = groupIndexString;
                        //Debug.Log("group: " + currentGroup + " => " + groupIndexString);
                    }
                }
            }
        }

        }

    }

    /// <summary>
    /// Returns a random crate type (int), selected with accordance of the rarity distribution
    /// </summary>
    /// <returns>int</returns>
    internal int GetRandomType()
    {
        int rndNumber = (int)UnityEngine.Random.Range(0, 100);
        int cumulative = rarity[0];
        for (int count = 0; count < numberOfRarities; count++)
        {
            if (rndNumber <= cumulative)
            {
                return (count + 1);
            }
            cumulative += rarity[count + 1];
        }
        return 0;
    }

    private int GetSmallestUnusedGroupNumber()
    {

        // increase nextGroupNumber
        nextGroupNumber++;

        // add nextGroupNumber to the list
        groupList.Add(nextGroupNumber);

        return nextGroupNumber;
    }

    private void InitGroupGrid()
    {
        crateGrid = new Crate[storageController.storageWidth, storageController.storageHight];
        for (int countH = 0; countH < storageController.storageHight; countH++)
        {
            for (int countW = 0; countW < storageController.storageWidth; countW++)
            {
                crateGrid[countW, countH] = null;
            }
        }
    }

    public bool SpawnCrateAtRandomPsition(int number)
    {
        for (int count = 0; count < number; count++)
        {
            // find random coordinates
            float maxW = storageController.storageWidth-1;
            float maxH = storageController.storageHight-1;
            int randomW = (int)Mathf.Round(UnityEngine.Random.Range(0, maxW));
            int randomH = (int)Mathf.Round(UnityEngine.Random.Range(0, maxH));

            while (!storageController.IsTileAvailableForCrateRel(randomW, randomH))
            {
                randomW = randomW = (int)Mathf.Round(UnityEngine.Random.Range(0, maxW));
                randomH = randomH = (int)Mathf.Round(UnityEngine.Random.Range(0, maxH));
            }

            positionW = randomW + storageController.storageAreaOriginW;
            positionH = randomH + storageController.storageAreaOriginH;
            Debug.Log("found spot @ " + randomW + " / " + randomH);

            // Choose random crate type
            int crateType = UnityEngine.Random.Range(1, 6);

            bool result = SpawnCrateByType(crateType, randomW, randomH);

        }

        return true;
    }

    /// <summary>
    /// Creates a crate object and game object of a given type and spawns the game object at the given coordinates
    /// </summary>
    /// <param name="crateType">type to be used to create the objects</param>
    /// <param name="coordW">horizontal position (world)</param>
    /// <param name="coordH">vertical position (world)</param>
    /// <returns></returns>
    public bool SpawnCrateByType(int crateType, float coordW, float coordH)
    {
        // create crate GO
        GameObject newCrateGO = Instantiate(CrateGOTypeByTypeNumber(crateType), new Vector3(coordW, coordH), Quaternion.identity, crateHolderTransform);
        // create crate OBJ
        Crate newCrate = new Crate(newCrateGO, (int)coordW, (int)coordH, crateType, crateGrid);
        RegisterCrate(newCrate);
        //Debug.Log("crate @ " + coordW + " / " + coordH + " in gr: " + nextGroupNumber);

        return true;
    }

    public GameObject CrateGOTypeByTypeNumber(int crateType)
    {
        GameObject crateToSpawn;
        crateToSpawn = crates[crateType];
        crateToSpawn.name = "crate 0" + crateType;

        return crateToSpawn;
    }

    public void RegisterCrate(Crate newCrate)  // TODO :: Objectify - first sweep done
    {
        int registerPositionX = (int)newCrate.GetGridPosition()[0];
        int registerPositionY = (int)newCrate.GetGridPosition()[1];
        if (storageController.NotInServiceLaneGrid(registerPositionX, registerPositionY))
        {
            AssignCrateToGroup(newCrate);
        }
        crateGrid[registerPositionX, registerPositionY] = newCrate;
    }


    public void EraseCrate(Crate oldCrate) // TODO :: Objectify - first sweep done
    {
        GameObject oldCrateGO = oldCrate.SelfGO;
        int positionW = (int)oldCrate.GetWorldPosition()[0];
        int positionH = (int)oldCrate.GetWorldPosition()[1];
        // absolute coordinates
        int positionX = oldCrate.GetGridPosition()[0];
        int positionY = oldCrate.GetGridPosition()[1];

        int oldGroupNumber = oldCrate.Group;

        // if there are no nbrs - remove the group from the group list
        // Remove group from group list - removing single crate
        groupList.Remove(crateGrid[positionX, positionY].Group);

        // remove the group index from the group grid
        crateGrid[positionX, positionY] = null;


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
            int nbrX = positionX + nbrXCoord;
            int nbrY = positionY + nbrYCoord;
            // if nbr within borders
            if (IsWithinBorders(nbrX, nbrY) && NotInServiceLane(nbrX, nbrY))
            {
                // if there is a nbr at this position
                if (!storageController.IsTileEmpty(nbrX, nbrY))
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

        // Remove color from colorChunks if the crate was not in the service lane
        if (storageController.NotInServiceLaneWorld(positionW, positionH))
        {
            RemoveColor(oldGroupNumber);
        }
    }

    private bool NotInServiceLane(int nbrX, int nbrY)
    {
        return storageController.NotInServiceLaneWorld(nbrX, nbrY);
    }

    private void RemoveColor(int oldGroupNumber)
    {
        // FIRST check if there is only one color!
        // remove the color associated with the removed group
        // find the color index in the colorChunks - tbd
        // find the previous chunk - prev
        // add tbd.value+1 to prv.value
        // remove tbd

        // rempve color only if the crate is not in the service lane
        if (storageController.colorChunks.Count > 1)
        {
            int obsoleteColorIndex = storageController.groupToColorMap[oldGroupNumber];
            int prevColorIndex = FindPrevColorIndex(obsoleteColorIndex);
            int obsoleteColorChunkSize = storageController.colorChunks[obsoleteColorIndex];
            storageController.colorChunks[prevColorIndex] = storageController.colorChunks[prevColorIndex] + storageController.colorChunks[obsoleteColorIndex] + 1;
            storageController.colorChunks.Remove(obsoleteColorIndex);
            // un-map the old group from it's color
            storageController.groupToColorMap.Remove(oldGroupNumber);
        }
        else // only one group - create new color, remap to default group 1
        {
            storageController.groupToColorMap.Remove(oldGroupNumber);
            storageController.colorChunks.Clear();
            // pich a random color
            int randomStartingColorIndex = UnityEngine.Random.Range(0, storageController.paletteArray.Length - 1);
            storageController.colorChunks.Add(randomStartingColorIndex, storageController.paletteArray.Length - 1);
            // map to the first possible group
            storageController.groupToColorMap[1] = randomStartingColorIndex;
        }
    }

    private int FindPrevColorIndex(int obsoleteColorIndex)
    {
        // iterate through keys in colorChunks; find the largest one that is smaller than obsoleteColorIndex
        int prevIndex = 0;
        int largestindex = 0;
        foreach (var kvp in storageController.colorChunks)
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

    private bool WorkNeighboursOf(Stack<int> progressStack, int newGroupNumber) // NEEDS REWORK
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
                if ((!storageController.IsTileEmpty(nbrX, nbrY)) &&
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
        crateGrid[posX, posY].Group = newGroupNumber;

        return true;
    }

    private bool AssignCrateToGroup(Crate crate)  // TODO :: Objectify - first sweep done
    {
        // perform neighbour check
        // collect neighbours
        int absX = (int)crate.GetGridPosition()[0]; // absolute X
        int absY = (int)crate.GetGridPosition()[1]; // absolute Y
        List<Crate> adjGroups = new List<Crate>();
        List<Crate> adjGroupList = new List<Crate>();

        // top -- right -- bottom -- left

        Crate[] nbrs = crate.GetNbrs();
        Crate topNbr = nbrs[0];
        Crate rightNbr = nbrs[1];
        Crate bottomNbr = nbrs[2];
        Crate leftNbr = nbrs[3];

        // neighbours collected; see how many different groups there are among them
        if (topNbr == null && rightNbr == null && bottomNbr == null && leftNbr == null) // no neighbours
        {
            crate.Group = GetSmallestUnusedGroupNumber();
            return true;
        }

        // build a list of unique group numbers - all above 0
        for (int count = 0; count < 4; count++)
        {
            if (nbrs[count] != null && nbrs[count].Group > 0 && adjGroupList.IndexOf(nbrs[count]) < 0)
            {
                adjGroupList.Add(nbrs[count]);
            }
        }

        // if there is only one neighbouring group
        if (adjGroupList.Count == 1)
        {
            crateGrid[absX, absY] = adjGroupList[0];
            return true;
        }
        else if (adjGroupList.Count > 1)
        {
            // the heavy case
            // find the smallest group number
            int minGroupNumber = adjGroupList[0].Group; // TODO : rework to use the group number of the largest group
            foreach (Crate item in adjGroupList)
            {
                minGroupNumber = Mathf.Min(minGroupNumber, item.Group);
            }

            // assign group number
            crateGrid[absX, absY].Group = minGroupNumber;
            // reassign larger groups
            foreach (Crate item in adjGroupList)
            {
                if (item.Group != minGroupNumber)
                {
                    RessignGroupTo(item.Group, minGroupNumber);
                }
            }

            return true;
        }

        return false;
    }

    private void RessignGroupTo(int targetGroup, int minGroupNumber)
    {
        for (int countW = 0; countW < storageController.storageWidth; countW++)
        {
            for (int countH = 0; countH < storageController.storageHight; countH++)
            {
                if (crateGrid[countW, countH].Group == targetGroup)
                {
                    crateGrid[countW, countH].Group = minGroupNumber;
                }
            }
        }

        // Remove group from group list - reassign group
        groupList.Remove(targetGroup);
        RemoveColor(targetGroup);
    }

    private bool IsWithinBorders(int posX, int posY)
    {
        return storageController.IsWithinBorders(posX, posY);
    }

}
