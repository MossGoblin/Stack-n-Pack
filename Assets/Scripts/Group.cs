using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group
{
    public List<Crate> CrateList { get; private set; }
    public int Index { get; private set; }
    public int[] Content { get; private set; }
    
    public Group(Crate initialCrate, int index)
    {
        CrateList = new List<Crate>();
        CrateList.Add(initialCrate);
        Index = index;
        BuildContentArray();
    }

    public void AddCrate(Crate crate)
    {
        CrateList.Add(crate);
        BuildContentArray();
    }

    public void RemoveCrate(Crate crate)
    {
        CrateList.Remove(crate);
        BuildContentArray();
    }

    private void BuildContentArray()
    {
        // reset content array
        Content = new int[6];
        for (int count = 0; count < 6; count++)
        {
            Content[count] = 0;
        }

        // rebuild
        foreach (Crate crate in CrateList)
        {
            Content[crate.Content]++;
        }
    }
}