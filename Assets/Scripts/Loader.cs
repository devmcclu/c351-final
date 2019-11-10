using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{

    public GameManager gameManager;

    // Awake is called before the first frame update
    void Awake()
    {
        if (GameManager.instance == null){
            Instantiate(gameManager);
        }
    }
}
