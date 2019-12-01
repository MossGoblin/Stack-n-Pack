using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GroupMaster : MonoBehaviour
{
    // props
    private List<Group> groupList;
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
                int newColor = 0;
                if (!master.crateMaster.startingColorUsed)
                {
                    newColor = master.crateMaster.randomStartingColorIndex;
                    master.crateMaster.startingColorUsed = true;
                }
                else
                {
                    newColor = master.crateMaster.GetNewColor();
                }
                groupToColorMap.Add(newGroup, newColor);
                break;
            case 1: // 1 nbr
                // find nbr and assign its group to crate
                for (int count = 0; count < 4; count++)
                {
                    if (crateNbrs[count] != null)
                    {
                        // set new group to crate
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

                if (NbrCount(crateNbrs) == 1)
                {
                    for (int count = 0; count < 4; count++)
                    {
                        if (crateNbrs[count] != null)
                        {
                            // set up the group for the crate
                            crate.SetGroup(crateNbrs[count].Group);
                            // find group OBJ and add crate to it
                            newGroup = GetGroupByIndex(crateNbrs[count].Group);
                            newGroup.AddCrate(crate);
                            break;
                        }
                    }
                    break;                   
                }
                else // more than 1 group in the nbrs - find the largest one and assign all relevant crates to it
                {
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

                    // set up the largest group for the crate
                    crate.SetGroup(largestGroupIndex);
                    // find group OBJ and add the crate to it
                    newGroup = GetGroupByIndex(largestGroupIndex);
                    newGroup.AddCrate(crate);
                    // remove obsolete groups
                    obsoleteGroups.Remove(largestGroupIndex); // make sure the largest group is NOT obsolete
                    Group largestGroup = newGroup;
                    foreach (Crate checkCrate in master.crateMaster.crateList) // the new crate is not in the crateList YET!
                    {
                        if (obsoleteGroups.Contains(checkCrate.Group)) // if a group is in the obolete list..

                        {
                            // remove the crate from the current group
                            GetGroupByCrate(checkCrate).RemoveCrate(checkCrate);
                            master.orderMaster.RemoveGroupMatches(GetGroupByCrate(checkCrate));
                            // .. assign the crate the new group, 
                            checkCrate.SetGroup(largestGroupIndex);
                            // .. then add it to the group
                            largestGroup.AddCrate(checkCrate);
                        }
                    }
                    foreach (int obsoleteGroupIndex in obsoleteGroups) // remove each group with an obsolete index (crates already assigned to the largest group)
                    {
                        Group obsoleteGroup = GetGroupByIndex(obsoleteGroupIndex);
                        RemoveGroup(obsoleteGroup);
                        // groupToColorMap.Remove(obsoleteGroup);
                        // groupList.Remove(obsoleteGroup);
                    }
                }
            break;
        }
        // TODO ?? should ADD update underlying tile graphics
    }

    public void RemoveCrateFromGroup(Crate crate)
    {
        // prepare structures
        List<Crate> updatedCrates = new List<Crate>(); // list of crates with updated groups
        List<Crate> checkedCrates = new List<Crate>(); // list of crates that are being checked
        Stack<Crate> stack = new Stack<Crate>();

        // get first nbrs
        Crate[] crateNbrs = crate.Neighbours();

        // find group to be dissolved
        Group group = GetGroupByCrate(crate);
        // remove group from order/group matches
        master.orderMaster.RemoveGroupMatches(group);
        // remove crate from group
        group.RemoveCrate(crate);

        // iterate nbrs - add to updated, add to stack, process stack
        foreach (Crate nbr in crateNbrs)
        {
            if (nbr != null && !checkedCrates.Contains(nbr))
            {
                checkedCrates.Add(nbr); // mark as checked
                stack.Push(nbr); // add to stack
                // move crate to new group
                Group newGroup = new Group(nbr, NewGroupNumber()); // prepare new group with nbr
                groupList.Add(newGroup);
                groupToColorMap.Add(newGroup, master.crateMaster.GetNewColor());
                // process stack
                ProcessStack(stack, checkedCrates, updatedCrates, newGroup);
            }
        }

        // Remove old group
        RemoveGroup(group);
        // groupToColorMap.Remove(group);
        // groupList.Remove(group);
        // master.orderMaster.RemoveGroupMatches(group);

        // TODO : trigger order re-check
    }

    private void ProcessStack(Stack<Crate> stack, List<Crate> checkList, List<Crate> updateList, Group newGroup)
    {
        // INGRESS
        // get crate from stack - get nbrs - add nbrs to stack and list
        while (stack.Count > 0)
        {
            Crate nextCrate = stack.Peek(); // get crate from stack - PEEK ONLY
            Crate[] nbrs = nextCrate.Neighbours(); // get nbrs
            foreach (Crate nbr in nbrs) // get deeper if there are unchecked nbrs
            {
                if (nbr != null && !checkList.Contains(nbr))
                {
                    stack.Push(nbr);
                    checkList.Add(nbr);
                    ProcessStack(stack, checkList, updateList, newGroup);
                }
            }

            // EGRESS
            // pull a crate from the stack - reassign group
            if (stack.Count > 0)
            {
                Crate crateToUpdate = stack.Pop();
                if (!updateList.Contains(crateToUpdate)) // was the crate already updated?
                {
                    GetGroupByCrate(crateToUpdate).RemoveCrate(crateToUpdate); // remove crate from old group
                    master.orderMaster.RemoveGroupMatches(GetGroupByCrate(crateToUpdate));
                    newGroup.AddCrate(crateToUpdate); // add crate to new group
                    crateToUpdate.SetGroup(newGroup.Index); // update group index in crate
                    updateList.Add(crateToUpdate); // mark crate as updated
                }
            }
        }
    }

    public void RecheckAllGroupsContent()
    {
        foreach (var group in groupList)
        {
            group.RebuildContent();
        }
        // trigger recheck of all order/group matches
        master.orderMaster.CheckOrderGroupMatches();
    }

    public Group GetGroupByCrate(Crate crate)
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

    public List<Group> GroupList()
    {
        return groupList;
    }

    public void RemoveGroup(Group group)
    {
        // check for crates - if no crates - proceed
        // remove group from grouplist
        // remove group from color mapping
        // remove group from matchlist
        if (group.Content.SequenceEqual(new int[]{0, 0, 0, 0, 0, 0}))
        {
            groupList.Remove(group);
            master.crateMaster.RemoveColorMapping(group);
            master.orderMaster.RemoveGroupMatches(group);
        }
    }

private int NbrCount(Crate[] nbrs)
{
    int nbrsCount = 0;
        for (int count = 0; count < 4; count++)
        {
            if (nbrs[count] != null)
            {
                nbrsCount++;
            }
        }
		return nbrsCount;
}}
