[initial] Initial commit
[safety commit] picking up and dropping crates is working fine; noticed a problem with the random placing of crates -- will look at it later (and also it is only for testing purposes)
[incremental] placed the pipe placeholders; time to implement some managed crate placement (probably directly in the hold) to create stacks, so that I can evaluate them
[incremental] Control deployment of crates through the robot is OK; now to work on some recursive group recognition
[preliminary, incremental] right before I attempt the recursive group count
[incremental] I might actually be able to pull off group assignment without recursion...
[incremental] finally recursive breaking down of groups is working (apparently); meanwhile simple border checks for movement apparently went to hell; to be checked...
[incremental] fixed the out-of-boundry error while moving
[incremental] working on the tile group highlighting
[incremental] grouping color kinda ready (lazy version); found a problem with the recursion - used a queue instead of a stack, so I overflow... *facepalm*
[incremental] reworked the group assignment to use a stack and basically fixed the recursion, as it only appeared to be fine; next: prope group number management and may be some Unity work on the highlighted tiles