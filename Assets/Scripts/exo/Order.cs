using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    // props
    public int ContentIndex { get; private set; }
    private int complexity;
    private int[] rarityGrid;
    
    // refs
    // private Storage gridRef;

    public Order (int complexity, int[] rarityGrid)
    {
        this.complexity = complexity;
        this.rarityGrid = rarityGrid;
        GenerateContent();
    }

    private void GenerateContent() // is this breakdown still in use ??
    {
            /*
            types   1   2   3   4   5   6
            cmpl 1  3   3   3   3   3
            cmpl 2  4   4   4   4   4   4
            cmpl 3      5   5   5   5   5
            */
            int minType = 0;
            int maxType = 0;
            int typeCount = complexity + 2;

        switch (complexity)
        {
            case 1:
                minType = 1;
                maxType = 5;
            break;
            case 2:
                minType = 1;
                maxType = 6;
            break;
            case 3:
                minType = 2;
                maxType = 5;
            break;
        }

        // pick typeCount number of types from between minType and maxType
        List<int> selectedTypes = new List<int>();
        while(selectedTypes.Count < typeCount)
        {
            int newType = UnityEngine.Random.Range(minType, maxType); // CHECK INCLUSIVITY
            if (!selectedTypes.Contains(newType))
            {
                selectedTypes.Add(newType);
            }
        }

        // pick random number of crates (complexity-1 to complexity) for each of the selected type and calculate ContentIndex
        foreach (int type in selectedTypes)
        {
            int numberOfCrates = UnityEngine.Random.Range((int)Math.Max(1, complexity - 1), complexity);
            ContentIndex += (int)Math.Pow(10, type) * numberOfCrates;
            // ContentIndex += (int)(Math.Pow(10, selectedTypes[typeCount])) * UnityEngine.Random.Range(complexity - 1, complexity);
        }
    }

    public int RandomOrderType(int min, int max)
    {
            int rndNumber = UnityEngine.Random.Range(min, max); // CHECK FOR INCLUSIVITY
            int cumulativeChance = 0;
            for (int count = 0; count < 7; count++)
            {
                cumulativeChance += rarityGrid[count];
                if (rndNumber <= rarityGrid[count])
                {
                    return count + 1;
                }
            }
            return 0;


    }
}
