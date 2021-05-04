using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBehavior : MonoBehaviour
{
    private GameController gameController;

    public float speed = 1f;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        transform.Translate(0, -gameController.Difficulty * speed * Time.deltaTime, 0);
    }
}
