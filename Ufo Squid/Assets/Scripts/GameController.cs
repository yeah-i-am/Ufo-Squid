using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private float gameStartTime;
    private bool gameStarted;

    public float difficultyScale = 1f;

    private bool Sound = true;

    private EnemyController enemyController;

    public float Difficulty
    {
        get => difficultyScale * (Time.time - gameStartTime) * 0.001f + 1f;
    }
    public bool GameStarted => gameStarted;

    private void Start()
    {
        enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
    }

    public void StartGame()
    {
        gameStartTime = Time.time;
        gameStarted = true;

        enemyController.Initialize();
    }

    public void ChangeSoundState()
    {
        Sound = !Sound;
    }

    void Update()
    {
        
    }
}
