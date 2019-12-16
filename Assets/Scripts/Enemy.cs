﻿using System;
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
    public static bool skipEveryOtherTurn = true;

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
        base.AttemptMove<T>(xDir, yDir);
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
            double[] minimaxResult = Minimax(CurrentState(), 5);

            xDir = (int)minimaxResult[1];
            yDir = (int)minimaxResult[2];
            //Debug.Log("MINIMAX RESULT: " + minimaxResult[0] + ", " + minimaxResult[1] + ", " + minimaxResult[2]);
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
        State s = new State(GameManager.instance.objectPositions, 1, false, 0);
        Debug.Log("State: " + s.playerLoc + ", " + s.enemyLoc + ", " + s.turn);
        s.PrintBoard();
        return s;
    }

    //Uses alpha-beta pruning
    //Currently only works with one zombie
    //Returns an array of 3 doubles. returnArray[0] = value, returnArray[1] = xDir of best move, returnArray[2] = yDir of best move.
    private double[] Minimax(State state, int depth)
    {
        //Debug.Log("The current state is: " + state.playerLoc + ", " + state.enemyLoc + ", " + depth);
        double outcome = gameResult(state);
        if (outcome != 0.1)
        {
            return new double[] { outcome*1000000, 0, 0 };
        }

        if (depth == 0) 
        {
            return new double[]{ heuristic(state), 0, 0 };
        }

        State[] children = state.GetChildren();

        double bestValue = -100000000 * state.turn;
        State bestChild = null;

        foreach (State child in children)
        {
            double[] minimaxResult = Minimax(child, depth - 1);
            //Debug.Log("Result: " + minimaxResult[0] + ", " + minimaxResult[1] + ", " + minimaxResult[2]);
            double value = minimaxResult[0];

            //Max if zombies' turn
            if (state.turn == 1)
            {
                if (value > bestValue)
                {
                    bestValue = value;
                    bestChild = child;
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
            }
            //Debug.Log("The turn is " + state.turn + " and the best value is " + bestValue);
        }

        return new double[] {bestValue, bestChild.lastMove.x, bestChild.lastMove.y };
    }

    //Needs to be written. Specification: -1 = player win, 0 = tie, 1 = zombie win, 0.1 = no result yet
    private double gameResult(State s)
    {
        return 0.1;
    }

    private double heuristic(State state)
    {
        //return taxicabHeuristic(state) + 10*state.healthLost;
        return basicHeuristic(state);
    }
    private double basicHeuristic(State state)
    {
        double score = 0;
        score -= Mathf.Sqrt(Mathf.Abs(state.enemyLoc.x - state.playerLoc.x) + Mathf.Abs(state.enemyLoc.y - state.playerLoc.y));
        if ((state.enemyLoc.x < GameManager.instance.boardScript.columns) && (state.playerLoc.x < GameManager.instance.boardScript.columns) &&
            (state.enemyLoc.y < GameManager.instance.boardScript.rows) && (state.playerLoc.y < GameManager.instance.boardScript.rows)){
            //Same x
            if(state.playerLoc.x == state.enemyLoc.x)
            {
                //Above enemy
                if(state.playerLoc.y > state.enemyLoc.y)
                {
                    for(int i = state.enemyLoc.y; i < state.playerLoc.y; i++)
                    {
                        if (ChcekForWall(state.playerLoc.x, i)) 
                        { 
                            score -= .2;
                        }
                    }      
                }
                //Below enemy
                else
                {
                    for(int i = state.playerLoc.y; i < state.enemyLoc.y; i++)
                    {
                        if (ChcekForWall(state.playerLoc.x, i))
                        {
                            score -= .2;
                        }
                    }
                }
            }
            //Same y
            else if(state.playerLoc.y == state.enemyLoc.y){

                //Right of enemy
                if(state.playerLoc.x > state.enemyLoc.x)
                {
                    for(int i = state.enemyLoc.x; i < state.playerLoc.x; i++)
                    {
                        if (ChcekForWall(i, state.playerLoc.y))
                        { 
                            score -= .2;
                        }
                    }
                }
                //Left of enemt
                else
                {
                    for(int i = state.playerLoc.y; i < state.enemyLoc.y; i++)
                    {
                        if (ChcekForWall(i, state.playerLoc.y)) score -= .2;
                    }
                }
            }
            //Above and right of enemy
            else if(state.playerLoc.x > state.enemyLoc.x && state.playerLoc.y > state.enemyLoc.y)
            {
                for(int i = state.enemyLoc.x; i < state.playerLoc.x + 1; i++)
                {
                    for(int j = state.enemyLoc.y; j < state.playerLoc.y + 1; j++)
                    {
                        if (GameManager.instance.objectPositions[i, j] != null)
                        {
                            if (ChcekForWall(i, j) == true) 
                            {
                                score -= .2;
                            }
                        }
                    }
                }
            }
            //Above and left of enenmy
            else if(target.position.x > state.enemyLoc.x && state.playerLoc.y < state.enemyLoc.y)
            {
                for(int i = state.enemyLoc.x; i < state.playerLoc.x + 1; i++)
                {
                    for(int j = state.playerLoc.y; j < state.enemyLoc.y + 1; j ++)
                    {
                        if (ChcekForWall(i, j) == true) 
                        {
                            score -= .2;
                        }
                    }
                }
            }
            //Below and right of enemy
            else if(state.playerLoc.x < state.enemyLoc.x && state.playerLoc.y > state.enemyLoc.y)
            {
                for(int i = state.playerLoc.x; i < state.enemyLoc.x + 1; i++)
                {
                    for(int j = state.enemyLoc.y; j < state.playerLoc.y + 1; j ++)
                    {
                        if (ChcekForWall(i, j) == true) 
                        {
                            score -= .2;
                        }
                    }
                }
            }
            //Below and left of enemy
            else if(state.playerLoc.x < state.enemyLoc.x && state.playerLoc.y < state.enemyLoc.y)
            {
                for(int i = state.playerLoc.x; i < state.enemyLoc.x + 1; i++)
                {
                    for(int j = state.playerLoc.y; j < state.enemyLoc.y + 1; j ++)
                    {
                        if (ChcekForWall(i, j) == true) 
                        {
                            score -= .2;
                        }
                    }
                }
            }
            //add the no wall
        }
        return score;
    }

    //Just negative the taxicab distance between player and enemy
    private double taxicabHeuristic(State s)
    {
        return -Mathf.Abs(s.enemyLoc.x - s.playerLoc.x) - Mathf.Abs(s.enemyLoc.y - s.playerLoc.y);
    }
    
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.HealthLoss(playerDamage);
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
