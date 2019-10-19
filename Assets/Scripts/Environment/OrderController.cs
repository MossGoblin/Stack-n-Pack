using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class OrderController
    {
        // list of existing orders
        List<Order> orderList;

        //complexity map - do we need this?
        public int[] complexityMap = new int[3] {2, 4, 3}; // minimum types, maximum types, average crates FOR a simple order; +1 for each for a higher level

        // receipt list
        // what does a receipt look like ?
        // simple order:
        // - 2 to 4 types
        // - each type - 2 to 4 crates
        // complex order:
        // - 4 - 6 types
        // - each type - 4 to 6 crates
        // OR
        // minimum types = 2/3/4
        // maximum types = 4/5/6
        // average crates per type = 3/4/5
    }