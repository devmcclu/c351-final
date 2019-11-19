using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public GameObject[,] board;
    public int turn; //1 = Zombies' turn, -1 = Player's turn
    public bool skipTurn; //Whether the zombies' next turn will be skipped
    private Vector2Int playerLoc; //Player's location on the board
    private Vector2Int enemyLoc; //Enemy's location on the board

    public State(GameObject[,] objectPositions, int t, bool skip)
    {
        board = objectPositions;
        turn = t;
        skipTurn = skip;
        for(int r = 0; r < board.GetLength(0); r++)
        {
            for(int c = 0; c < board.GetLength(1); c++)
            {
                if (board[r, c].CompareTag("Player"))
                {
                    playerLoc = new Vector2Int(r,c);
                }
                if (board[r, c].CompareTag("Enemy"))
                {
                    enemyLoc = new Vector2Int(r, c);
                }
            }
        }
    }

    //Returns the possible child states
    public State[] GetChildren()
    {
        return null;
    }

    //Returns the child state after the specified move is attempted. Unfinished.
    public State GetChild(Vector2Int move)
    {
        State child = new State(board, turn, skipTurn); //Clones the current state
        //If zombies' turn
        if(turn == 1)
        {
            Vector2Int newPosition = enemyLoc + move;
            //Move if empty
            if(board[newPosition[0], newPosition[1]] == null)
            {
                board[newPosition[0], newPosition[1]] = board[enemyLoc[0], enemyLoc[1]];
                board[enemyLoc[0], enemyLoc[1]] = null;
                enemyLoc = newPosition;
            }
            
        }

    }

    public void makeMove(Vector2Int move)
    {
        
    }
}
