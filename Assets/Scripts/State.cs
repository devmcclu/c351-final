using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public GameObject[,] board;
    public int turn; //1 = Zombies' turn, -1 = Player's turn
    public bool skipTurn; //Whether the zombies' next turn will be skipped
    public 
    public State(GameObject[,] objectPositions)
    {
        board = objectPositions;    
    }

    //Returns the possible child states
    public State[] getChildren()
    {

    }
}
