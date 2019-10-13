**TODO**

1. find a sleeker way to traverse 4 orthogonal nbrs - WORKED (I think)
	*perhaps a circle after all
    
= = =
        for (double nbrsAngle = 0; nbrsAngle <= Math.PI * 1.5; nbrsAngle += Math.PI * 0.5)
        {
            int nbrsXCount = (int)Math.Sin(nbrsAngle);
            int nbrsYCount = (int)Math.Cos(nbrsAngle);
= = =

2. rework group number selection and group overall number management - DONE

        // add nextGroupNumber to the list
        groupList.Add(nextGroupNumber);

        // increase nextGroupNumber
        nextGroupNumber++;

= = =

3. implement removing colors from colorChunks (merging chunks) to finish with the in situ highlights - DONE

= = =

4. Proper placement of pipes and a space for the orders - DONE
- pipes are OK, the place needs the (semi)final size of teh storage area - DONE

= = =

5. build the area, reserved for incoming crates - a 1 tile thin zone circumnavigating the storage area - DONE

 - color within RecolorGrid() - DONE
 - in player movement do not placement of tiles - DONE

rules:
- movement - Allowed
- picking - Allowed
- dropping - NOT Allowed

= = =

6. proper crate spawning - DONE
in the begining assign crate types to probability indeces to assure that the most common type is different each time
assign probabilities to type; deliver through pipes
types:
1 - 30
2 - 25
3 - 20
4 - 15
5 - 10

TODO : exclude the service lane crates from groups and highlighting - DONE (unless an edge case can break it)

= = =

X. Change the way crates come into play - outside of the pipes, then picked up eith an Enter

= = =

7. group assessment
- in Update, after RecolorGroups()
- get the group number and indeces from the group list
- for each index, count the group tiles for each type of crate
- build hash using the crate type quantity in the group (decimal system)


= = =

8. orders
- three sizes - small, medium, large
	-- small orders:
		5 sim orders
		2-3 colors
		2-3 blocks per color
		4-9 blocks total
	-- medoim orders:
		4 sim orders
		3-4 colors
		3-4 blocks per color
		9-16 blocks total
	-- large orders:
		3 sim orders
		4-5 colors
		4-5 blocks per color
		16-20 blocks total

- calculate order hash, same system as for the groups from 7.	

= = =

Objectification TASKS:
 -- remove Vacancy Grid	
	