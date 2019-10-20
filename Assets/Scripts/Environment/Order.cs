using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Order
{
    int contentIndex; // full content index
    int complexity; // complexity to use for generating content

    private OrderController controller;

    public Order(int complexity, OrderController controller)
    {
        this.complexity = complexity;
        this.controller = controller;
        GenerateContent();
    }

    private void GenerateContent()
    {
        // get the ingredients for the receipt from the order controller
        int minTypes = controller.complexityMap[complexity];
        int maxTypes = controller.complexityMap[1 + complexity];
        int avgCrates = controller.complexityMap[2 + complexity]; // how do we use this average to pick a number of crates?? 0.5*avg to 2*avg ??

        Random rnd = new Random();
        int numberOfTypes = rnd.Next(minTypes, maxTypes);
        // cycle the determined number of types


    }
}