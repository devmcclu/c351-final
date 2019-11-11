using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Make the GameManager a singleton
    public static GameManager instance = null;
    //Get access to the BoardManager
    public BoardManager boardScript;

    public int playerHealth = 100;

    [HideInInspector] public bool playersTurn = true;

    private int level = 3;

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
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        //Setup the board
       boardScript.SetupScene(level);
    }

    public void GameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
