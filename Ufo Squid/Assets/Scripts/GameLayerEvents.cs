using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayerEvents : MonoBehaviour
{
    private GameController gameController;
    void StartGame()
    {
        gameController.StartGame();
    }

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
}
