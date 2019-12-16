using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    //How much damage is done to the player
    public int playerDamage;

    //Where the player is on the board
    private Transform target;
    //If it is the enemy turn or not
    private bool skipMove;
    //Use minimax or not
    private bool useMinimax = true;
    //Whether or not the zombies should skip every alternate turn
    public static bool skipEveryOtherTurn = false;

    public int speed = 1;

    public int id; //The enemy's id number

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        //Get the player object's position
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Check if it is the enemy's turn
        if(skipMove && skipEveryOtherTurn)
        {
            skipMove = false;
            return;
        }

        //Remove the current position of the enemy in objectPosistions
        //GameManager.instance.objectPositions.SetValue(null, (int)transform.position.x, (int)transform.position.y);
        base.AttemptMove<MovingObject>(xDir, yDir);
        //Update the position of the enemy in objectPosistions
        //GameManager.instance.objectPositions[(int)transform.position.x, (int)transform.position.y] = this.gameObject;
        GameManager.instance.RebuildObjectPositions();
        skipMove = true;
    }

    //Function for how the enemy is moved
    public void MoveEnemy()
    {

        int xDir = 0;
        int yDir = 0;

        if (useMinimax)
        {
            object[] minimaxResult = Minimax(CurrentState(), 3, -100000000, 100000000);

            List<Vector2Int> bestMoves = (List<Vector2Int>)minimaxResult[1];

            Vector2Int move0 = bestMoves[0];

            xDir = move0.x;
            yDir = move0.y;

            for (int i = 1; i < State.zombies.Count; i++)
            {
                Vector2Int thisMove = bestMoves[i];
                State.zombies[i].GetComponent<Enemy>().AttemptMove<MovingObject>(thisMove.x, thisMove.y);
            }

            //Debug.Log("MINIMAX RESULT: " + (double)minimaxResult[0] + ", " + ((List<Vector2Int>)minimaxResult[1])[0]);
        }
        else
        {
            //If the enemy is on the same X coord, move the y
            //Else, move on the x coord
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            {
                yDir = target.position.y > transform.position.y ? speed : -speed;
            }
            else
            {
                xDir = target.position.x > transform.position.x ? speed : -speed;
            }
        }
        AttemptMove<Player>(xDir, yDir);
    }

    //Returns a new State object representing the current state of play. Assumes it is enemy's turn and not skipped.
    private State CurrentState()
    {
        State s = new State(GameManager.instance.objectPositions, 1, false, 0, this.gameObject, true, null, new Vector2Int());
        //Debug.Log("State: " + s.playerLoc + ", " + s.enemyLocs[0]+ ", " + s.turn);
        s.PrintBoard();
        return s;
    }

    //Uses alpha-beta pruning for efficiency
    //Works with multiple zombies!
    //Returns an array of 2 objects. returnArray[0] = value (which is a double), and returnArray[1] = bestMove (which is a List<Vector2Int>)
    private object[] Minimax(State state, int depth, double alpha, double beta)
    {
        //Debug.Log("The current state is: " + state.playerLoc + ", " + state.enemyLocs[0]+ ", " + depth);
        double outcome = gameResult(state);
        if (outcome != 0.1)
        {
            return new object[] { outcome * 10000, new List<Vector2Int> { new Vector2Int(0, 0) } };
        }

        if (depth == 0) 
        {
            return new object[] {heuristic(state), new List<Vector2Int> { new Vector2Int(0, 0) } };
        }

        List<State> children = state.GetChildren();

        double bestValue = -1000000 * state.turn;
        State bestChild = null;

        foreach (State child in children)
        {
            object[] minimaxResult = Minimax(child, depth - 1, alpha, beta);
            //Debug.Log("MINIMAX RESULT: " + (double)minimaxResult[0] + ", " + ((List<Vector2Int>)minimaxResult[1])[0]);
            double value = (double)minimaxResult[0];

            //Max if zombies' turn
            if (state.turn == 1)
            {
                if (value > bestValue)
                {
                    bestValue = value;
                    bestChild = child;
                }
                if (value > alpha)
                {
                    alpha = value;
                }
                if(alpha >= beta)
                {
                    break;
                }
            }

            //Min if player's turn
            if (state.turn == -1)
            {
                if (value < bestValue)
                {
                    bestValue = value;
                    bestChild = child;
                }
                if (value < beta)
                {
                    beta = value;
                }
                if (alpha >= beta)
                {
                    break;
                }
            }
            //Debug.Log("The turn is " + state.turn + " and the best value is " + bestValue);
        }

        return new object[] {bestValue, bestChild.lastMove};
    }

    //Needs to be written. Specification: -1 = player win, 0 = tie, 1 = zombie win, 0.1 = no result yet
    private double gameResult(State s)
    {
        return 0.1;
    }

    private double heuristic(State state)
    {
        return taxicabHeuristic(state) + 10*state.healthLost;
        //return basicHeuristic(state) + 10*state.healthLost;
    }
    private double basicHeuristic(State state)
    {
        double score = 0;
        double weight = 0;
        score -= Mathf.Sqrt(Mathf.Abs( state.enemyLocs[0].x - state.playerLoc.x ) + Mathf.Abs( state.enemyLocs[0].y - state.playerLoc.y ));
        //score -= Mathf.Abs(state.enemyLoc.x - state.playerLoc.x) - Mathf.Abs(state.enemyLoc.y - state.playerLoc.y);
        weight = Mathf.Sqrt(Mathf.Pow(( state.enemyLocs[0].x - state.playerLoc.x ),2) + (Mathf.Pow(( state.enemyLocs[0].y - state.playerLoc.y ),2))) *0.1;
        
        if ((state.enemyLocs[0].x < GameManager.instance.boardScript.columns) && (state.playerLoc.x < GameManager.instance.boardScript.columns) &&
            (state.enemyLocs[0].y < GameManager.instance.boardScript.rows) && (state.playerLoc.y < GameManager.instance.boardScript.rows)){
            //Same x
            if(state.playerLoc.x == state.enemyLocs[0].x)
            {
                //Above enemy
                if(state.playerLoc.y > state.enemyLocs[0].y)
                {
                    for(int i = state.enemyLocs[0].y; i < state.playerLoc.y; i++)
                    {
                        if (ChcekForWall(state.playerLoc.x, i)) 
                        { 
                            //score -= 0.2;
                            score -= weight;
                        }
                    }      
                }
                //Below enemy
                else
                {
                    for(int i = state.playerLoc.y; i < state.enemyLocs[0].y; i++)
                    {
                        if (ChcekForWall(state.playerLoc.x, i))
                        {
                            //score -= 0.2;
                            score -= weight;
                        }
                    }
                }
            }
            //Same y
            else if(state.playerLoc.y == state.enemyLocs[0].y){

                //Right of enemy
                if(state.playerLoc.x > state.enemyLocs[0].x)
                {
                    for(int i = state.enemyLocs[0].x; i < state.playerLoc.x; i++)
                    {
                        if (ChcekForWall(i, state.playerLoc.y))
                        { 
                            //score -= 0.2;
                            score -= weight;
                        }
                    }
                }
                //Left of enemy
                else
                {
                    for(int i = state.playerLoc.y; i < state.enemyLocs[0].y; i++)
                    {
                        if (ChcekForWall(i, state.playerLoc.y)) score -= weight;
                    }
                }
            }
            //Above and right of enemy
            else if(state.playerLoc.x > state.enemyLocs[0].x && state.playerLoc.y > state.enemyLocs[0].y)
            {
                for(int i = state.enemyLocs[0].x; i < state.playerLoc.x + 1; i++)
                {
                    for(int j = state.enemyLocs[0].y; j < state.playerLoc.y + 1; j++)
                    {
                        if (GameManager.instance.objectPositions[i, j] != null)
                        {
                            if (ChcekForWall(i, j) == true) 
                            {
                                //score -= 0.2;
                                score -= weight;
                            }
                        }
                    }
                }
            }
            //Above and left of enenmy
            else if(target.position.x > state.enemyLocs[0].x && state.playerLoc.y < state.enemyLocs[0].y)
            {
                for(int i = state.enemyLocs[0].x; i < state.playerLoc.x + 1; i++)
                {
                    for(int j = state.playerLoc.y; j < state.enemyLocs[0].y + 1; j ++)
                    {
                        if (ChcekForWall(i, j) == true) 
                        {
                            //score -= 0.2;
                            score -= weight;
                        }
                    }
                }
            }
            //Below and right of enemy
            else if(state.playerLoc.x < state.enemyLocs[0].x && state.playerLoc.y > state.enemyLocs[0].y)
            {
                for(int i = state.playerLoc.x; i < state.enemyLocs[0].x + 1; i++)
                {
                    for(int j = state.enemyLocs[0].y; j < state.playerLoc.y + 1; j ++)
                    {
                        if (ChcekForWall(i, j) == true) 
                        {
                            //score -= 0.2;
                            score -= weight;
                        }
                    }
                }
            }
            //Below and left of enemy
            else if(state.playerLoc.x < state.enemyLocs[0].x && state.playerLoc.y < state.enemyLocs[0].y)
            {
                for(int i = state.playerLoc.x; i < state.enemyLocs[0].x + 1; i++)
                {
                    for(int j = state.playerLoc.y; j < state.enemyLocs[0].y + 1; j ++)
                    {
                        if (ChcekForWall(i, j) == true) 
                        {
                            //score -= 0.2;
                            score -= weight;
                        }
                    }
                }
            }
        }
        return score;
    }

    //Just negative the sum of taxicab distance between player and enemies
    private double taxicabHeuristic(State s)
    {
        double dist = 0;
        foreach (Vector2Int enemyLoc in s.enemyLocs)
        {
            dist += Mathf.Abs(enemyLoc.x - s.playerLoc.x) + Mathf.Abs(enemyLoc.y - s.playerLoc.y);
        }
        return -dist;
    }

    protected override void OnCantMove<T>(T component)
    {
        if (component.CompareTag("Player")) {
            Player hitPlayer = component as Player;
            hitPlayer.HealthLoss(playerDamage);
        }
    }

    private bool ChcekForWall(int x, int y)
    {
        if (GameManager.instance.objectPositions[x, y] != null)
        {
            if(GameManager.instance.objectPositions[x, y].CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
}
