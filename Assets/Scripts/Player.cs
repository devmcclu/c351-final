using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;
    public int health;

    // Start is called before the first frame update
    protected override void Start()
    {
        health = GameManager.instance.playerHealth;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.playersTurn){
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if(horizontal != 0)
        {
            vertical = 0;
        }
        if (horizontal != 0 || vertical != 0)
        {
            //AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        //Wall hitWall = component as Wall
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void HealthLoss(int loss)
    {
        health -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver ()
    {
        //Check if health total is less than or equal to zero.
        if (health <= 0) 
        {
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver ();
        }
    }
}

