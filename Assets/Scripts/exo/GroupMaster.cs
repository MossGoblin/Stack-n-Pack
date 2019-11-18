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
        Group newGroup = null;
        switch (nbrsCount)
        {
            case 0: // no nbrs
                NewGroupNumber();
                crate.SetGroup(lastCreatedGroup);
                newGroup = new Group(crate, lastCreatedGroup); // TODO : NEW GROUP
                groupList.Add(newGroup);
                groupToColorMap.Add(newGroup, master.crateMaster.GetNewColor());
                break;
            case 1: // 1 nbr
                // find nbr and assign its group to crate
                for (int count = 0; count < 4; count++)
                {
                    if (crateNbrs[count] != null)
                    {
                        crate.SetGroup(crateNbrs[count].Group);
                        // find group OBJ and add crate to it
                        newGroup = GetGroupByIndex(crateNbrs[count].Group);
                        newGroup.AddCrate(crate);
                        break;
                    }
                }
                break;
            default: // more than 1 nbr

                // if all nbrs are of the same group
                List<int> nbrGroupList = new List<int>();
                for (int count = 0; count < 4; count++)
                {
                    if ((crateNbrs[count] != null)) // if this is a nbr
                    {
                        if (nbrGroupList.Contains(crateNbrs[count].Group))
                        {
                            break;
                        }
                        nbrGroupList.Add(crateNbrs[count].Group);
                    }
                }
                if (nbrGroupList.Count == 1) // 1 group in the nbrs
                {
                    for (int count = 0; count < 4; count++)
                    {
                        if (crateNbrs[count] != null)
                        {
                            crate.SetGroup(crateNbrs[count].Group);
                            // find group OBJ and add crate to it
                            newGroup = GetGroupByIndex(crateNbrs[count].Group);
                            newGroup.AddCrate(crate);
                            break;
                        }
                    }
                    break;                   
                }
                // TODO :: HERE -- proliferate group number !!
                // if there are 2 or more nbrs and they are of more than 1 group
                // find the largest adjacent group
                int largestGroupIndex = 0;
                int largestGroupSize = 0;
                List<int> obsoleteGroups = new List<int>(); ;
                for (int count = 0; count < 4; count++)
                {
                    if ((crateNbrs[count] != null)) // if this is a nbr
                    {
                        obsoleteGroups.Add(crateNbrs[count].Group); // add nbr.Group to obsolete groups
                        Group group = GetGroupByCrate(crateNbrs[count]); // find the group of that nbr
                        if (group.CrateList.Count > largestGroupSize) // if this group is largest so far, note that in the 'largest...' variables
                        {
                            largestGroupSize = group.CrateList.Count;
                            largestGroupIndex = group.Index;
                        }
                    }
                }


                crate.SetGroup(largestGroupIndex);
                // find group OBJ and add crate to it
                newGroup = GetGroupByIndex(largestGroupIndex);
                newGroup.AddCrate(crate);
                // remove obsolete groups
                obsoleteGroups.Remove(largestGroupIndex); // make sure the largest group is removed from obsolete
                Group largestGroup = GetGroupByIndex(largestGroupIndex);
                foreach (Crate checkCrate in master.crateMaster.crateList) // the new crate is not in the crateList YET!
                {
                    if (obsoleteGroups.Contains(checkCrate.Group)) // if a group is in the obolete list -> assign the crate the new group, 
                                                                   // ... then add it to the group
                    {
                        checkCrate.SetGroup(largestGroupIndex);
                        largestGroup.AddCrate(crate);
                    }
                }
                foreach (int obsoleteGroupIndex in obsoleteGroups) // remove each group with an obsolete index (crates already assigned to the largest group)
                {
                    Group obsoleteGroup = GetGroupByIndex(obsoleteGroupIndex);
                    groupList.Remove(obsoleteGroup);
                    groupToColorMap.Remove(obsoleteGroup);
                }
            break;
        }

        // TODO : ADD update underlying tile graphics

    }

    public void RemoveCrateFromGroup(Crate crate)
    {
        List<Crate> updatedCrates = new List<Crate>();

        // get nbrs
        Crate[] startingNbrs = crate.Neighbours();

        // find the group to be dissolved
        Group obsoleteGroup = GetGroupByCrate(crate);

        // nbr array to list
        List<Crate> startingNbrsList = new List<Crate>();
        foreach (Crate nbrCrate in startingNbrs)
        {
            if (nbrCrate != null)
            {
                startingNbrsList.Add(nbrCrate);
            }
        }

        // for each nbr - create new group
        foreach (Crate startingNbr in startingNbrsList)
        {

            int newGroupNumber = NewGroupNumber();
            Group newGroup = new Group(startingNbr, newGroupNumber);
            groupList.Add(newGroup);
            groupToColorMap.Add(newGroup, master.crateMaster.GetNewColor());

            Stack<Crate> progressStack = new Stack<Crate>();
            progressStack.Push(startingNbr);
            // add all connected to the stack, using new group number
            ProgressNbrsStack(progressStack, newGroupNumber, updatedCrates);
        }

        // remove obsolete group
        groupToColorMap.Remove(obsoleteGroup); // remove color mapping
        groupList.Remove(obsoleteGroup); // remove the group itself

        // TODO : REM update underlying tile graphics

    }

    private void ProgressNbrsStack(Stack<Crate> stack, int newGroupNumber, List<Crate> updatedCrates)
    {
        // INGRESS -- fill the stack
        // get nbrs, push each unpushed into the stack, then iterate
        // get nbrs
        Crate[] nbrs = stack.Peek().Neighbours(); // get current nbrs
        List<Crate> nbrList = new List<Crate>();
        foreach (Crate nbrCrate in nbrs)
        {
            if (nbrCrate != null)
            {
                nbrList.Add(nbrCrate);
            }
        }
        if (nbrList.Count > 0) // if there are nbrs
        {
            foreach (Crate nbrCrate in nbrList) // add nbrs to stack if not already added
            {
                if (!stack.Contains(nbrCrate))
                {
                    stack.Push(nbrCrate);
                    ProgressNbrsStack(stack, newGroupNumber, updatedCrates);  // if new nbr was added to stack - > delve in
                }
            }
        }

        // EGRESS -- empty the stack and apply new group number if a new group number has not yet been applied
        if (!updatedCrates.Contains(stack.Peek()))
        {
            // create the new group
            updatedCrates.Add(stack.Peek());
            GetGroupByIndex(stack.Peek().Group).RemoveCrate(stack.Peek());
            stack.Pop().SetGroup(newGroupNumber);
        }
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
        lastCreatedGroup++;
        return lastCreatedGroup;
    }
}
