# Zombie Survival
## About
Zombie Survival is a game built in Unity demonstrating the use of Minimax with alpha-beta pruning in a turn based game environment.

The player moves with the arrow keys, with the objectives being to collect as many gems as possible and get to the exit. The game is over when the player runs out of health or gets to the exit, upon which the game will quit

## How to Use
Zombie Survival was built using Unity 2019.2.11 (which can be found [here](https://unity3d.com/get-unity/download/archive)). Attached to the project in the relases section are builds for Windows, Mac OS, and Linux. The build containes a version of the game with 3 Zombies working at a minimax depth of 4.

In order to alter the Minimax settings, change the values of the Enemy1 prefab found in `Assets/Prefabs`. You can change the depth of the minimax algorithm and the choice of heuristic (using 1, 2, and 3). To change the characteristics of the board, such as size of the board and the amount of Zombies and Walls, change the values in the GameManager prefab found in `Assets/Prefabs`.
