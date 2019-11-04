# Stack-n-Pack
A simple tile arrangement game

* The player starts with an empty grid
* Pl is represented by a robot the size of a grid cell
* He can move the robot around the grid
* Each grid cell can contain crates (1 each)
* There are 5 types of crates, color coded by rarity (chance of being generated)
* While moving, the player can pick up crates or drop crates on empty cells
* The player can have only one crate picked up at any moment
* In the begining the grid is empty; it is encircled by a service lane, 1 cell thick, on which crates can not be placed, but the player can use to move around
* Right outside of the grid (and outside of the service lane) there are 4 factories; while standing on a cell in the service lane matching any of the factories the player can interact with it to pick up a crate from that factory
* In the brgining of the game and each time the player picks a crate from a factory, the factory generates new random crate (in accordance with the rarity of the types of crates)
* Placing the crates in the grid groups them by adjacency (orthogonal only)
* Outside of the grid there is a hud which displays a number of pending orders
* The orders are randomly generated lists of crates - a set number of crate types, randomly generated number of crates per type
* When the composition of a group of crates in the grid matches exactly one of the orders, the player can trigger execution of the order - the crates of teh gropu are removed from the grid and the order is also removed and replaced by a new one
* Only the composition of a group matters, the way the crates are arranged on the grid is not relevant
* The orders have 3 levels of complexity - increasing the total number of crates in each order and containing predominately crates of rarer types
* Fulfilling an order earns energy points
* Matching 1 order with multiple groups will allow all teh groups to be dispatched when the order is completed, but with diminishing return (probably halving the reward with each repetition)
* Picking up crates, placing crates and moving while having a loaded crate costs the player energy points
* The game tracks both the current energy points of the player as well as the total earned amount of energy points
* The player plays either until he reaches a pre-determined number of total earned points or if his current points drop to 0
