using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    //Class to create an upper and lower limit
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    //Size of the board
    public int columns = 8;
    public int rows = 8;
    //Upper and lower limit of wallCount
    public Count wallCount = new Count(5, 9);
    //Arrays for different tile types (extendable in engine)
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] enemyTiles;

    //Board object
    private Transform boardHolder;
    //Vector3 list to keep hold of all the tiles and objects
    private List<Vector3> gridPositions = new List<Vector3>();

    //Create a new list of all board objects
    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                //Debug.Log(gridPositions.Count);
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //Create a new board
    void BoardSetup()
    {
        //Board object
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //Make the floor
                GameObject toInstatiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //Make walls on the outer edges
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstatiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                //Instantiate the object as a child of the board
                GameObject instance = Instantiate(toInstatiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //Fine a random position on the board to put an object
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    //Put a tile on the board based on the minimum and maximum allowed on the board
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();


        //Test for object spawning
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
    }

}
