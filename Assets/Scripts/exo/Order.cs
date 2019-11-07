using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    // props
    public int ContentIndex { get; private set; }
    private int complexity;
    
    // refs
    private Storage gridRef;

    public Order (int complexity, Storage grid)
    {
        this.complexity = complexity;
        gridRef = grid;
    }

    private void GenerateContent()
    {
            /*
            1   2   3   4   5   6
            2   2   2   2   2
            3   3   3   3   3   3
                4   4   4   4   4
            */
            int minType = 0;
            int maxType = 0;
            int typeCount = complexity;

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

        // pick typeCount number of types from amongst minType and maxType
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
            ContentIndex += (int)(Math.Pow(10, selectedTypes[typeCount])) * UnityEngine.Random.Range(complexity - 1, complexity);
        }
    }

    public int RandomOrderType(int min, int max)
    {
            int rndNumber = UnityEngine.Random.Range(min, max); // CHECK FOR INCLUSIVITY
            int cumulativeChance = 0;
            for (int count = 0; count < 7; count++)
            {
                cumulativeChance += gridRef.Rarity[count];
                if (rndNumber <= gridRef.Rarity[count])
                {
                    return count + 1;
                }
            }
            return 0;


    }
}
