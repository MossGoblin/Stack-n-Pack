﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateMaster : MonoBehaviour
{
    // color palette
    public Color[] paletteArray;
    int[] baseRedSteps = new int[] { 0, -1, 0, 0, 1, 0 };
    float hueChangeFactor = 8f; // 2 by default; value of 1 would make the step duration 255 ticks
    public Dictionary<int, int> colorChunks;
    public Dictionary<int, int> groupToColorMap; // mapping for groups and color indeces - TO BE RESOLVED VIA GroupMaster
    public List<Crate> crateList;

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
        groupToColorMap = new Dictionary<int, int>();

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
            cumulativeChance += gridRef.Rarity[count];
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
        int randomStartingColorIndex = UnityEngine.Random.Range(0, paletteArray.Length - 1);
        colorChunks.Add(randomStartingColorIndex, paletteArray.Length - 1);
        // map to the first possible group
        groupToColorMap[1] = randomStartingColorIndex;
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
        Crate newCrate = new Crate(pos.posX, pos.posY, master, type);
        newCrate.SetUpGO(newCrateGO);
        //testCrate.SetGroup(1); // TEMP GROUP 1
        groupMaster.AssignCrateToNewGroup(newCrate);
        gridRef.storageGrid[pos.posX, pos.posY] = newCrate;
    }
}
