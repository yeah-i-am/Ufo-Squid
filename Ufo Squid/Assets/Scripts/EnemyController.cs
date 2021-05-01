using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameController gameController;

    private Queue<GameObject> freeEnemies;
    private Queue<GameObject> onScreenEnemies;

    private float[] line = new float[3];
    private float spawnEnemyY;

    private Vector2 screenMax;
    private float enemySize;

    private float lastSpawnTime;

    private float timeBetweenEnemies => spawnTime / gameController.Difficulty;
    private bool NeedToSpawn => Time.time - lastSpawnTime > timeBetweenEnemies;

    public float spawnTime = 3f;

    public GameObject Enemy;
    public Transform gameLayer;

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        screenMax = -Camera.main.ScreenToWorldPoint(Vector2.zero);

        enemySize = Enemy.GetComponent<SpriteRenderer>().bounds.max.y;

        spawnEnemyY = screenMax.y + enemySize;

        float screenSize = screenMax.x * 2;

        for (int i = 0; i < 3; i++)
            line[i] = -screenMax.x + (1 + i) / 4f * screenSize;
    }

    public void Initialize()
    {
        freeEnemies = new Queue<GameObject>();
        onScreenEnemies = new Queue<GameObject>();
        lastSpawnTime = -239;
    }

    private void SetEnemy( GameObject enemy )
    {
        onScreenEnemies.Enqueue(enemy);
        enemy.SetActive(true);

        enemy.transform.position = new Vector3(line[Random.Range(0, 3)], spawnEnemyY, 0);
    }

    private void SpawnEnemy()
    {
        lastSpawnTime = Time.time;

        if (freeEnemies.Count != 0)
            SetEnemy(freeEnemies.Dequeue());
        else
            SetEnemy(Instantiate(Enemy, gameLayer));
    }

    void Update()
    {
        if (!gameController.GameStarted)
            return;

        if (NeedToSpawn)
            SpawnEnemy();

        var enemyTransform = onScreenEnemies.Peek().transform;

        if (enemyTransform.position.y + enemySize < -screenMax.y)
        {
            var enemy = onScreenEnemies.Dequeue();
            enemy.SetActive(false);
            freeEnemies.Enqueue(enemy);
        }
    }
}
