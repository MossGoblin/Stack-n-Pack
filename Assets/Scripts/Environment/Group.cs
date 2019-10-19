using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Group
{
    List<Crate> crateList;
    int index;

    public Group(Crate firstCrate, int index)
    {
        crateList = new List<Crate>();
        crateList.Add(firstCrate);
        this.index = index;
    }

    public int GetIndex()
    {
        return this.index;
    }

    public List<Crate> GetList()
    {
        return crateList;
    }

    public void AddCrate(Crate crate)
    {
        crateList.Add(crate);
    }

    // removing a crate
    public void RemoveCrate(Crate crate)
    {
        // remove crate
        // check nbrs
        // return nbrs list
        if (crate != null)
        {
            crateList.Remove(crate);
            Crate[] nbrsList = crate.GetNbrs();
            crate = null;
        }
    }

    // generate the group content index
    private int GenerateContentIndex()
    {
        // iterate crate types and apply multiplicator
        int result = 0;
        for (int countType = 0; countType < 6; countType++)
        {
            // get the number of crates of that type
            int typeCount = 0;
            foreach (Crate crate in crateList)
            {
                if (crate.GetCrateType() == countType)
                {
                    typeCount++;
                }
            }
            int addition = (int)Mathf.Pow(10f, (float)typeCount);
            result += addition;
        }
        return result;
    }

    public int GetContentIndex() // return an index of teh content of the group
    {
        return GenerateContentIndex(); // re-index only when requested to avoid undecessary operations
    }

}