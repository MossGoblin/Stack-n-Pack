using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GroupMaster : MonoBehaviour
{
    // props
    private List<Group> groupList;
    private List<int> groupIndices;
    private int lastCreatedGroup;
    public Dictionary<Group, int> groupToColorMap;

    // refs
    public Conductor master;
    private Storage gridRef;

    void Start()
    {
        // refs
        this.gridRef = master.gridRef;

        groupList = new List<Group>();
        groupIndices = new List<int>();
        groupToColorMap = new Dictionary<Group, int>();
        lastCreatedGroup = 0;
    }

    public void AssignCrateToNewGroup(Crate crate) // handle group management when a new crate is added
    {
        // get crate nbrs
        Crate[] crateNbrs = crate.Neighbours();
        // check number of nbrs
        int nbrsCount = 0;
        for (int count = 0; count < 4; count++)
        {
            if (crateNbrs[count] != null)
            {
                nbrsCount++;
            }
        }

        // CASES
        switch (nbrsCount)
        {
            case 0: // no nbrs
                lastCreatedGroup++;
                crate.SetGroup(lastCreatedGroup);
                Group newGroup = new Group(crate, lastCreatedGroup); // TODO : NEW GROUP
                groupToColorMap.Add(newGroup, master.crateMaster.GetNewColor());
            break;
            case 1: // 1 nbr
                // find nbr and assign its group to crate
                for (int count = 0; count < 4; count++)
                {
                    if (crateNbrs[count] != null)
                    {
                        crate.SetGroup(crateNbrs[count].Group);
                    }
                }
                break;
            default: // more than 1 nbr
                // find the largest adjacent group
                int largestGroupIndex = 0;
                int largestGroupSize = 0;
                List<int> obsoleteGroups = new List<int>(); ;
                for (int count = 0; count < 4; count++)
                {
                    if ((crateNbrs[count] != null))
                    {
                        obsoleteGroups.Add(crateNbrs[count].Group);
                        Group group = GetGroupByCrate(crateNbrs[count]); // find the group with the index of the crate
                        if (group.CrateList.Count > largestGroupSize) // if this group is largest so far, not that in the 'largest...' variables
                        {
                            largestGroupSize = group.CrateList.Count;
                            largestGroupIndex = group.Index;
                        }
                    }
                }
                crate.SetGroup(largestGroupIndex);
                // remove obsolete groups
                obsoleteGroups.Remove(largestGroupIndex); // make sure the largest group is removed from obsolete
                Group largestGroup = GetGroupByIndex(largestGroupIndex);
                foreach (Crate checkCrate in gridRef.storageGrid)
                {
                    if (obsoleteGroups.Contains(checkCrate.Group)) // if a group is in the obolete list -> assign the crate the new group, then add it to the group
                    {
                        checkCrate.SetGroup(largestGroupIndex);
                        largestGroup.AddCrate(crate);
                    }
                }
                foreach (int obsoleteGroupIndex in obsoleteGroups) // remove each group with an obsolete index (crates already assigned to the largest group)
                {
                    Group obsoleteGroup = GetGroupByIndex(obsoleteGroupIndex);
                    groupList.Remove(obsoleteGroup);
                }
            break;
        }

        // TODO : ADD update underlying tile graphics

    }

    public void RemoveCrateFromGroup(Crate crate)
    {

        // get nbrs
        Crate[] startingNbrs = crate.Neighbours();

        // remove crate from grid
        gridRef.storageGrid[crate.PositionX_Grid, crate.PositionY_Grid] = null;

        // find the group to be dissolved
        Group obsoleteGroup = GetGroupByCrate(crate);

        // for each nbr - create new group
        foreach (Crate startingNbr in startingNbrs)
        {
            Stack<Crate> progressStack = new Stack<Crate>();
            progressStack.Push(startingNbr);
            // add all connected to the stack, using new group number
            ProgressNbrsStack(progressStack, NewGroupNumber());
        }

        // remove obsolete group
        groupList.Remove(obsoleteGroup);

        // TODO : REM update underlying tile graphics

    }

    private void ProgressNbrsStack(Stack<Crate> stack, int newGroupNumber)
    {
        // INGRESS -- fill the stack
        // get nbrs, push each unpushed into the stack, then iterate
        // get nbrs
        Crate[] nbrs = stack.Peek().Neighbours(); // get current nbrs

        if (nbrs.Length > 0) // if there are nbrs
        {
            foreach (Crate nbrCrate in nbrs) // add nbrs to stack if not already added
            {
                if (!stack.Contains(nbrCrate))
                {
                    stack.Push(nbrCrate);
                    ProgressNbrsStack(stack, newGroupNumber);  // if new nbr was added to stack - > delve in
                }
            }
        }

        // EGRESS -- empty the stack and apply new group number
        stack.Pop().SetGroup(newGroupNumber);
    }

    private Group GetGroupByCrate(Crate crate)
    {
        foreach (Group group in groupList)
        {
            if (group.Index == crate.Group)
            {
                return group;
            }
        }
        return null;
    }

    public Group GetGroupByIndex(int index)
    {
        foreach (Group group in groupList)
        {
            if (group.Index == index)
            {
                return group;
            }
        }
        return null;
    }

    private int NewGroupNumber()
    {
        return lastCreatedGroup++;
    }
}
