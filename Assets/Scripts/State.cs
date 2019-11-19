using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public GameObject[,] board;
    public int turn; //1 = Zombies' turn, -1 = Player's turn
    public bool skipTurn; //Whether the zombies' next turn will be skipped
    public Vector2Int playerLoc; //Player's location on the board
    public Vector2Int enemyLoc; //Enemy's location on the board
    public Vector2Int[] legalMoves = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
    public Vector2Int lastMove;

    public State(GameObject[,] objectPositions, int t, bool skip)
    {
        board = objectPositions;
        turn = t;
        skipTurn = skip;
        for(int r = 0; r < board.GetLength(0); r++)
        {
            for(int c = 0; c < board.GetLength(1); c++)
            {
                if (board[r, c] != null)
                {
                    if (board[r, c].CompareTag("Player"))
                    {
                        playerLoc = new Vector2Int(r, c);
                    }
                    if (board[r, c].CompareTag("Enemy"))
                    {
                        enemyLoc = new Vector2Int(r, c);
                    }
                }
            }
        }
    }

    //Prints out the board
    public void PrintBoard()
    {
        for (int r = 0; r < board.GetLength(0); r++)
        {
            string line = "";
            for (int c = 0; c < board.GetLength(1); c++)
            {
                if (board[r, c] != null)
                {
                    line += (board[r, c].tag + ", ");
                } else
                {
                    line += "null, ";
                }
            }
            Debug.Log(line);
        }
    }

    //Returns the possible child states
    public State[] GetChildren()
    {
        State[] children = new State[legalMoves.Length];
        for(int i = 0; i < legalMoves.Length; i++)
        {
            Vector2Int move = legalMoves[i];
            children[i] = GetChild(move);
        }
        return children;
    }

    //Returns the child state after the specified move is attempted
    public State GetChild(Vector2Int move)
    {
        State child = new State(board, turn, skipTurn); //Clones the current state
        child.makeMove(move);
        return child;
    }

    public void makeMove(Vector2Int move)
    {
        //If zombies' turn
        if (turn == 1)
        {
            Vector2Int newPosition = enemyLoc + move;

            //Out of bounds
            if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= board.GetLength(0) || newPosition.y >= board.GetLength(1))
            {
                return;
            }

            //Move if empty and turn not skipped
            if (board[newPosition[0], newPosition[1]] == null && !skipTurn)
            {
                board[newPosition[0], newPosition[1]] = board[enemyLoc[0], enemyLoc[1]];
                board[enemyLoc[0], enemyLoc[1]] = null;
                enemyLoc = newPosition;
            }

            skipTurn = !skipTurn;
        }

        //If player's turn
        if (turn == -1)
        {
            Vector2Int newPosition = playerLoc + move;
            //Move if empty
            if (board[newPosition[0], newPosition[1]] == null)
            {
                board[newPosition[0], newPosition[1]] = board[playerLoc[0], playerLoc[1]];
                board[playerLoc[0], playerLoc[1]] = null;
                playerLoc = newPosition;
            }
        }

        turn = -turn;
    }
}
