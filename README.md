# Stack-n-Pack

A simple tile arrangement game

## STATUS

* This is an old project and I don't quite remember if all features are implemented. Most of them seem to be.

* There is a change in the game logic I would implement right away and (of course) the game needs rebalancing (grid size vs resources and such), but let's say that it's a working prototype.

* P.S. The idiotic UI with almost unreadable text? Blame on the 'prototype' word.


## OUTLINE

* The player starts with an empty grid

* Player is represented by a robot, the size of a grid cell

* Using the arrow keys, the player can move the robot around within the grid

* Each grid cell can contain a crate

* There are 5 types of crates, color coded by rarity (chance of being generated)

* While moving, the player can pick up crates or drop crates on empty cells

* The player can have only one crate picked up at any moment

* In the beginning the grid is empty; it is encircled by a service lane, 1 cell thick, on which crates can not be placed, but the player can use to move around

* Outside of the grid (and outside of the service lane) there are 4 factories; while standing on a cell in the service lane adjacent any of the factories the player can interact with it to pick up a crate from that factory

* The color of a factory shows the crate that can be picked up

* In the beginning of the game and each time the player picks a crate from a factory, the factory generates new random crate (in accordance with the rarity of the types of crates)

* Placing the crates in the grid groups them by orthogonal adjacency

* Above the grid there is a HUD which displays a number of pending orders

* The orders are randomly generated lists of crates - a set number of crate types, randomly generated number of crates per type

* When the composition of a group of crates in the grid matches exactly one of the orders, the player can trigger execution of the order - the crates of the group are removed from the grid and the order is also replaced by a new one

* Only the composition of a group matters, the way the crates are arranged on the grid is not relevant

* The orders have 3 levels of complexity - higher level orders have higher total number of crates and contain predominately crates of rarer types

* Fulfilling an order earns energy points

* Matching 1 order with multiple groups will allow all of the matching groups to be dispatched when the order is completed, but with diminishing return (probably halving the reward with each repetition)
  
  * NOTE: Not sure if that was implemented

* Picking up crates, placing crates and moving while having a loaded crate costs the player energy points

* The game tracks both the current energy points of the player as well as the total earned amount of energy points

* The player plays either until he reaches a pre-determined number of total earned points or if his current points drop to 0
