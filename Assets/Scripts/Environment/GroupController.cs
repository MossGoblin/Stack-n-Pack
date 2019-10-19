using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class GroupController
    {
    // prepare exploration stack
    Stack<Crate> explorationStack = new Stack<Crate>();

    // list of all the groups
      // TO BE ADDED
    public void RemoveCrateFromGroup(Crate crate)
        {

            // having the crate
            // get crate nbrs
            Crate[] startNbrs = crate.GetNbrs();
            foreach (Crate startNbr in startNbrs)
            {
                // prepare new group number
                int newGroupNumber = NewGroupNumber();
                // INGRESS
                ExploreNbrs(startNbr);
            }
        }

        private void ExploreNbrs(Crate crate) // NEW
        {
            Crate[] newNbrs = crate.GetNbrs();
            if (newNbrs.Length > 0)
            {
                foreach (Crate nbr in newNbrs)
                {
                    explorationStack.Push(nbr);
                    ExploreNbrs(nbr); // INGRESS
                }
            }
            else // EGRESS
            {
                Crate nextNbr = explorationStack.Pop();
                nextNbr.Group = NewGroupNumber();
            }
        }
        private int NewGroupNumber() // NEW :: IMPLEMENT
        {
        throw new NotImplementedException();
        }

        // TODO : Methods -- pick a new group number
    }

