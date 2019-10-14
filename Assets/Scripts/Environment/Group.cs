using System;
using System.Collections.Generic;
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
            List<Crate> nbrsList = crate.GetNbrs();
            crate = null;
        }

    }
}
