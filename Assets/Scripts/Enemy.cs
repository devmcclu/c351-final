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
        GameManager.instance.objectPosistions[(int)transform.position.x, (int)transform.position.y] = null;
        base.AttemptMove<T>(xDir, yDir);
        //Update the position of the enemy in objectPosistions
        GameManager.instance.objectPosistions[(int)transform.position.x, (int)transform.position.y] = this.gameObject;
        skipMove = true;
    }

    //Function for how the enemy is moved
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        //If the enemy is on the same X coord, move the y
        //Else, move on the x coord
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else 
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.HealthLoss(playerDamage);
    }
}
