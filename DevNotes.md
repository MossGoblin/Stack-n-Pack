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

3. implement removing colors from colorChunks (merging chunks) to finish with the in situ highlights
