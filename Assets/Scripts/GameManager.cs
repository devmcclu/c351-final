using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float turnDelay = .1f;
    //Make the GameManager a singleton
    public static GameManager instance = null;
    //Get access to the BoardManager
    public BoardManager boardScript;

    public int playerHealth = 100;

    [HideInInspector] public bool playersTurn = true;

    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    //Array for storing the positions of the player, walls, and enemies
    public GameObject[,] objectPositions;

    // Awake is called before the first frame update
    void Awake()
    {
        //Create the GameManager if one does not exist, else destroy the existing one
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        //Keep the GameManager through all levels
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();

        //Create the array storing the current position of the player, walls, and enemies
        objectPositions = new GameObject[boardScript.columns, boardScript.rows];
        //Set the player GameObject as the first position in the array
        objectPositions[0,0] = FindObjectOfType<Player>().gameObject;
        //Find all the walls
        Wall[] walls = FindObjectsOfType<Wall>();
        for (int i = 0; i < walls.Length; i++)
        {
            Wall currentWall = walls[i];
            objectPositions[(int)currentWall.transform.position.x, (int)currentWall.transform.position.y] = currentWall.gameObject;
        }
    }

    void InitGame()
    {
        //Setup the board
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    public void GameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(playersTurn || enemiesMoving)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
        //Find the position of all the enemies and put them in the array
        for(int i = 0; i < enemies.Count; i++){
            Enemy currentEnemy = enemies[i];
            objectPositions[(int)currentEnemy.transform.position.x, (int)currentEnemy.transform.position.y] = currentEnemy.gameObject;
        }
        //Debug.Log(objectPositions);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        RebuildObjectPositions();

        for(int i = 0; i < enemies.Count; i++)
        {
            //enemies[i].MoveEnemy();

            //Only the first zombie needs to run minimax. It will tell all the others what move to make.
            if (i == 0)
            {
                enemies[0].MoveEnemy(); 
            }

            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public void RebuildObjectPositions()
    {
        objectPositions = new GameObject[boardScript.columns, boardScript.rows];
        //Find the player and put it in the objectPositions array
        GameObject newPlayer = FindObjectOfType<Player>().gameObject;
        objectPositions[(int)newPlayer.transform.position.x, (int)newPlayer.transform.position.y] = newPlayer.gameObject;
        //Find all the walls
        Wall[] walls = FindObjectsOfType<Wall>();
        for (int i = 0; i < walls.Length; i++)
        {
            Wall currentWall = walls[i];
            objectPositions[(int)currentWall.transform.position.x, (int)currentWall.transform.position.y] = currentWall.gameObject;
        }

        //Find the position of all the enemies and put them in the array
        for(int i = 0; i < enemies.Count; i++){
            Enemy currentEnemy = enemies[i];
            objectPositions[(int)currentEnemy.transform.position.x, (int)currentEnemy.transform.position.y] = currentEnemy.gameObject;
        }
    }
}
