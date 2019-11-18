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
    private bool useMinimax = false;

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
        if(skipMove)
        {
            skipMove = false;
            return;
        }

        //Remove the current position of the enemy in objectPosistions
        GameManager.instance.objectPositions[(int)transform.position.x, (int)transform.position.y] = null;
        base.AttemptMove<T>(xDir, yDir);
        //Update the position of the enemy in objectPosistions
        GameManager.instance.objectPositions[(int)transform.position.x, (int)transform.position.y] = this.gameObject;
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
        }
        else
        {
            //If the enemy is on the same X coord, move the y
            //Else, move on the x coord
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            {
                yDir = target.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
            }
        }
        AttemptMove<Player>(xDir, yDir);
    }

    //Returns a new State object representing the current state of play
    private State CurrentState()
    {
        return null;
    }

    //Returns an array of 3 doubles. returnArray[0] = value, returnArray[1] = xDir of best move, returnArray[2] = yDir of best move.
    private double[] Minimax(State state, int depth)
    {
        double outcome = gameResult(state);
        if (outcome != 0.1)
        {
            return new double[] { outcome*1000000, 0, 0 };
        }
        if (depth == 0) {
            return new double[]{ heuristic(state), 0, 0 };
        }
        //Console.WriteLine(basicHeuristic());
        //return new double[] { 1, 0, 1 };
        return null;
    }

    //Needs to be written. Specification: -1 = player win, 0 = tie, 1 = zombie win, 0.1 = no result yet
    private double gameResult(State s)
    {
        return 0.1;
    }

    private double heuristic(State state)
    {
        return basicHeuristic(state);
    }
    private double basicHeuristic(State state)
    {
        //Fill in
        return 0;
    }

    /** Wrong, please ignore
    //Just negative the taxicab distance between player and enemy
    private double basicHeuristic()
    {
        return -Mathf.Abs(target.position.x - transform.position.x) - Mathf.Abs(target.position.y - transform.position.y);
    }
    */
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.HealthLoss(playerDamage);
    }
}
