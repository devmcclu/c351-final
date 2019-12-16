using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;
    public int health;
    public int score;
    public Text healthText;
    public Text scoreText;

    // Start is called before the first frame update
    protected override void Start()
    {
        health = GameManager.instance.playerHealth;
        scoreText.text = "Score: 0";
        base.Start();
        UpdateHealth();
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
            AttemptMove<Enemy>(horizontal, vertical);
        }
    }

    public void OnTriggerEnter2D (Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if(other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay
            Invoke ("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }
        else if(other.tag == "Item")
        {
            Destroy(other.gameObject);
            score += 5;
            scoreText.text = "Score: " + score.ToString();
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Remove the current position of the player in objectPosistions
        //GameManager.instance.objectPositions.SetValue(null, (int)transform.position.x, (int)transform.position.y);
        base.AttemptMove<T>(xDir, yDir);
        //Update the position of the player in objectPosistions
        //GameManager.instance.objectPositions[(int)transform.position.x, (int)transform.position.y] = this.gameObject;
        GameManager.instance.RebuildObjectPositions();
        //RaycastHit2D hit;

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
        UpdateHealth();
        CheckIfGameOver();
    }

    public void UpdateHealth(){
        healthText.text = "Health: " + health;
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

