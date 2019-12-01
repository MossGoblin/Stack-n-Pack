using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrateMaster : MonoBehaviour
{
    // color palette
    public Color[] paletteArray;
    int[] baseRedSteps = new int[] { 0, -1, 0, 0, 1, 0 };
    float hueChangeFactor = 8f; // 2 by default; value of 1 would make the step duration 255 ticks
    public Dictionary<int, int> colorChunks;
    public List<Crate> crateList;

    public int randomStartingColorIndex;
    public bool startingColorUsed;

    public Color[] colorPool = new Color[]
    {
        new Color(0.5f, 0.4f, 0.0f, 1f),
        new Color(1, 1, 0, 1),
        new Color(1, 0, 0, 1),
        new Color(0, 1, 0, 1),
        new Color(0, 0, 1, 1),
        new Color(1, 0, 1, 1)
    };

    // crate prefab
    [SerializeField] GameObject cratePrefab;

    // refs
    public Conductor master;
    Storage gridRef;
    [SerializeField] Transform crateHolder;
    GroupMaster groupMaster;

    // Start is called before the first frame update
    void Start()
    {
        groupMaster = master.groupMaster;
        colorChunks = new Dictionary<int, int>();
        int paletteLength = (256 * 6 / (int)hueChangeFactor);
        paletteArray = new Color[paletteLength];
        crateList = new List<Crate>();

        BuildColorPalette();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int RandomType()
    {        
        // refs
        this.gridRef = master.gridRef;


        int rndNumber = UnityEngine.Random.Range(0, 100);
        int cumulativeChance = 0;
        for (int count = 0; count < 6; count++)
        {
            cumulativeChance += master.Rarity[count];
            if (rndNumber <= cumulativeChance)
            {
                return count + 1;
            }
        }
        return 0;
    }
    private bool BuildColorPalette()
    {

        // double cycle - the outer loop marks steps; the inner loop is for the duration of the steps

        // set base valued for colors
        float clrRed = 256;
        float clrGrn = 0;
        float clrBlu = 0;
        int paletteCounter = 0;
        float maxStepDuration = (256f / hueChangeFactor) - 1;

        for (float stepCounter = 0; stepCounter < 6; stepCounter += 1f) // outer loop 
        {
            for (float stepDuration = 0; stepDuration < maxStepDuration; stepDuration += 1) // inner loop
            {
                // fill in a palette grid
                float nrmRed = NormalizeColor(clrRed);
                float nrmGrn = NormalizeColor(clrGrn);
                float nrmBlu = NormalizeColor(clrBlu);
                Color newColor = new Color(nrmRed, nrmGrn, nrmBlu, 1f);

                //PlacePaletteTile(paletteCounter, newColor);

                // fill in the color in the palette
                paletteArray[paletteCounter] = newColor;


                // update colors according to steps and duration
                clrRed += ((GetStep(0, (int)stepCounter)) * hueChangeFactor);
                clrGrn += ((GetStep(4, (int)stepCounter)) * hueChangeFactor);
                clrBlu += ((GetStep(2, (int)stepCounter)) * hueChangeFactor);

                paletteCounter++;
            }
        }
        // initiate first large groupStrip - pick a random point in the color array
        randomStartingColorIndex = UnityEngine.Random.Range(0, paletteArray.Length - 1);
        colorChunks.Add(randomStartingColorIndex, paletteArray.Length - 1);
        startingColorUsed = false;

        return true;
    }

    private int GetStep(int baseColor, int stepNumber) // for baseColor supply 0 for Red, 4 for Green, 2 for Blue
    {
        int newStep = baseRedSteps[(stepNumber + baseColor) % 6];
        return newStep;
    }

    private float NormalizeColor(float color)
    {

        float newValue = color / 255f;
        return newValue;
    }

    public void PlaceCrate(int type, (int posX, int posY) pos)
    {
        GameObject newCrateGO = Instantiate(cratePrefab, new Vector3(pos.posX, pos.posY, 0), Quaternion.identity, crateHolder);
        Crate newCrate = new Crate(pos.posX, pos.posY, master, type - 1); // TODO :: NEW CRATE TYPE
        newCrate.SetUpGO(newCrateGO);
        crateList.Add(newCrate);
        groupMaster.AssignCrateToNewGroup(newCrate);
        // TODO : trigger order re-check
    }

    // Pick a new color from the palette
    public int GetNewColor()
    {
        if (!startingColorUsed)
        {
            startingColorUsed = true;
            return randomStartingColorIndex;
        }
        // iterate chunks
        int largestChunk = 0;
        int largestChunkIndex = 0;
        foreach(var chunkPair in colorChunks)
        {
            if (chunkPair.Value > largestChunk)
            {
                largestChunkIndex = chunkPair.Key;
                largestChunk = chunkPair.Value;
            }
        }

        // pick the middle of the chink as new index
        int newColorIndex = ((largestChunk / 2) + largestChunkIndex + 1) % paletteArray.Length; // make sure the color index loops

        // cut the old chunk in half
        colorChunks[largestChunkIndex] = largestChunk / 2;
        
        // register the new color in the chunk dict
        colorChunks.Add(newColorIndex, largestChunk / 2);

        return newColorIndex;
    }

    public bool RemoveColorMapping(Group group)
    {
        int oldChunkIndex = master.groupMaster.groupToColorMap[group]; // the index of the color from the grou/color map
        int oldChunkIndexPosition = colorChunks.Keys.ToList().IndexOf(oldChunkIndex);
        int oldChunkSize = colorChunks[oldChunkIndex]; // the size of the chunk for that color in the group/color map
        master.groupMaster.groupToColorMap.Remove(group); // remove the group/color pair
        colorChunks.Remove(oldChunkIndex); // remove the obsolete chunk
        int previousChunkIndexPosition = 0;
        
        //check if this was the last color
        if (colorChunks.Count == 0)
        {
            randomStartingColorIndex = UnityEngine.Random.Range(0, paletteArray.Length - 1);
            colorChunks.Add(randomStartingColorIndex, paletteArray.Length - 1);
            startingColorUsed = false;            
            return true;
        }
        if (oldChunkIndexPosition > 0)
        {
            previousChunkIndexPosition = oldChunkIndexPosition - 1;
        }
        else
        {
            previousChunkIndexPosition = colorChunks.Count - 1;
        }
        int previousChunkIndex = colorChunks.Keys.ToList()[previousChunkIndexPosition];
        colorChunks[previousChunkIndex] += oldChunkSize + 1;
        return true;
    }

    public void RemoveCrate(Crate crate)
    {
        crateList.Remove(crate);
        gridRef.storageGrid[crate.PositionX_Grid, crate.PositionY_Grid] = null;
        GameObject crateGO = crate.CrateGO;
        GameObject.Destroy(crateGO);
        // remove crate from group; recalculate group content

    }
}
