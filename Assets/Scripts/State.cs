using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public GameObject[,] board;
    public int turn; //1 = Zombies' turn, -1 = Player's turn
    public bool skipTurn; //Whether the zombies' next turn will be skipped
    public Vector2Int playerLoc; //Player's location on the board
    public Vector2Int[] legalMoves = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
    public List<Vector2Int> lastMove; //The last move that was made
    public int healthLost; //How much health the player has lost

    //This is static so that the Enemy class can reference the list order after minimax returns a result
    public static List<GameObject> zombies = new List<GameObject>(); //A list of all zombies, in the same order as enemyLocs
    public List<Vector2Int> enemyLocs = new List<Vector2Int>(); //All enemies' locations, with this zombie's location as element 0
    public GameObject thisZombie;

    //The boolean argument 'reinitialize' means that the Player and Enemy locations should be recomputed from the objectPositions array
    public State(GameObject[,] objectPositions, int t, bool skip, int healthL, GameObject zombie, bool reinitialize, List<Vector2Int> eLocs, Vector2Int playerL)
    {
        thisZombie = zombie;
        healthLost = healthL;
        board = objectPositions;
        turn = t;
        skipTurn = skip;

        if (!reinitialize)
        {
            enemyLocs = eLocs;
            playerLoc = playerL;
        }
        else
        {
            enemyLocs = new List<Vector2Int>();
            zombies = new List<GameObject>();
            playerLoc = new Vector2Int();

            //Find the player and enemies
            for (int r = 0; r < board.GetLength(0); r++)
            {
                for (int c = 0; c < board.GetLength(1); c++)
                {
                    if (board[r, c] != null)
                    {
                        if (board[r, c].CompareTag("Player"))
                        {
                            playerLoc = new Vector2Int(r, c);
                        }
                        if (board[r, c].CompareTag("Enemy"))
                        {
                            if (board[r, c].Equals(thisZombie))
                            {
                                enemyLocs.Insert(0, new Vector2Int(r, c));
                                zombies.Insert(0, board[r,c]);
                            }
                            else
                            {
                                enemyLocs.Add(new Vector2Int(r, c));
                                zombies.Add(board[r,c]);
                            }
                        }
                    }
                }
            }
        }
    }

    //Returns a list of possible move combinations for the first n+1 zombies in the list
    public List<List<Vector2Int>> getMoves(int n)
    {
        List<List<Vector2Int>> moves = new List<List<Vector2Int>>();
        
        //Only looking at first zombie
        if (n == 0)
        {
            //Just add the four possible moves
            foreach (Vector2Int moveDirection in legalMoves)
            {
                List<Vector2Int> newMove = new List<Vector2Int>();
                newMove.Add(moveDirection);
                moves.Add(newMove);
            }
        } else
        {
            //Recursively find all permutations of moves for n+1 zombies
            foreach (Vector2Int moveDirection in legalMoves)
            {
                //This is a list of move permutations for the n zombies before it
                List<List<Vector2Int>> movePermutations = getMoves(n - 1);

                //Add this zombie's move to each move permutation and add to the list
                foreach (List<Vector2Int> movePermutation in movePermutations)
                {
                    movePermutation.Add(moveDirection);
                    moves.Add(movePermutation);
                }
            }
        }

        return moves;
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
    public List<State> GetChildren()
    {
        List<State> children = new List<State>();
        //bool uselessMoveAdded = false; //This is used to prevent adding more than one ineffective move (like moving into a wall)

        List<List<Vector2Int>> moves;

        //Zombies' turn
        if (turn == 1)
        {
            moves = getMoves(enemyLocs.Count - 1);
        } else
        {
            moves = getMoves(0);
        }

        //Debug.Log(moves[0][0] + ", " + moves[1][0] + ", " + moves[2][0] + ", " + moves[3][0] + ", ");

        for (int i = 0; i < moves.Count; i++)
        {
            List<Vector2Int> move = moves[i];
            State child = GetChild(move);

            /*
            //If the move changed nothing, only add it if another useless move has not been added
            if(child.playerLoc.Equals(this.playerLoc) && child.enemyLoc.Equals(this.enemyLoc) && child.healthLost == this.healthLost)
            {
                if(!uselessMoveAdded)
                {
                    uselessMoveAdded = true;
                    children.Add(child);
                }
            } else
            {
                children.Add(child);
            }
            */

            children.Add(child);
        }

        return children;
    }

    //Returns the child state after the specified move is attempted
    public State GetChild(List<Vector2Int> move)
    {
        State child = new State((GameObject[,])board.Clone(), turn, skipTurn, healthLost, thisZombie, false, new List<Vector2Int>(enemyLocs), playerLoc); //Clones the current state
        for(int i = 0; i < move.Count; i++)
        {
            child.makeMove(move[i], i);
        }
        
        //Set the child state's last move and flip its turn
        child.lastMove = move;
        child.turn = -child.turn;

        if (Enemy.skipEveryOtherTurn && turn == 1)
        {
            skipTurn = !skipTurn;
        }

        return child;
    }

    //The integer zombieNum specifies which zombie to move
    public void makeMove(Vector2Int move, int zombieNum)
    {
        //If zombies' turn
        if (turn == 1)
        {
            Vector2Int newPosition = enemyLocs[zombieNum] + move;
            bool okay = true;

            //Out of bounds
            if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= board.GetLength(0) || newPosition.y >= board.GetLength(1))
            {
                okay = false;
            }

            //Move if empty and turn not skipped and not out of bounds
            if (okay)
            {
                if (!skipTurn)
                {
                    if (board[newPosition[0], newPosition[1]] == null)
                    {
                        board[newPosition[0], newPosition[1]] = board[enemyLocs[zombieNum][0], enemyLocs[zombieNum][1]];
                        board[enemyLocs[zombieNum][0], enemyLocs[zombieNum][1]] = null;
                        enemyLocs[zombieNum] = newPosition;
                    }
                    else if (board[newPosition[0], newPosition[1]].CompareTag("Player"))
                    {
                        healthLost += 10;
                    }
                }
            }
        }

        //If player's turn
        if (turn == -1)
        {
            Vector2Int newPosition = playerLoc + move;
            bool okay = true;

            //Out of bounds
            if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= board.GetLength(0) || newPosition.y >= board.GetLength(1))
            {
                okay = false;
            }

            //Move if empty and not out of bounds
            if (okay && board[newPosition[0], newPosition[1]] == null)
            {
                board[newPosition[0], newPosition[1]] = board[playerLoc[0], playerLoc[1]];
                board[playerLoc[0], playerLoc[1]] = null;
                playerLoc = newPosition;
            }
        }
    }
}
