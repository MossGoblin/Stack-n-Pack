using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridSupport
{
    public static List<(int outer, int inner, int count)> Iterate(int outer_bound, int inner_bound)
    {
        int counter = 0;
        List<(int, int, int)> loop = new List<(int, int, int)>();
        for (int i = 0; i < outer_bound; i++)
        {
            for (int y = 0; y < inner_bound; y++)
            {
                var item = ( i, y, counter );
                loop.Add(item);
                counter++;
            }
        }
        return loop;
    }

    public static List<(int outer, int inner, int count)> IterateRange(int outer_lower, int outer_higher, int inner_lower, int inner_higher)
    {
        int counter = 0;
        List<(int, int, int)> loop = new List<(int, int, int)>();
        for (int i = outer_lower; i < outer_higher; i++)
        {
            for (int y = inner_lower; y < inner_higher; y++)
            {
                var item = (i, y, counter);
                loop.Add(item);
                counter++;
            }
        }
        return loop;
    }
}
