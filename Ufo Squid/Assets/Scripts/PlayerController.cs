using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameController gameController;
    private SpriteRenderer spriteRenderer;

    public float speed = 1f;
    private int direction = -1;

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void HandleTouch()
    {
        direction = -direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
    }

    void Update()
    {
        if (!gameController.GameStarted)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                HandleTouch();
        }
        else if (Input.GetMouseButtonDown(0))
            HandleTouch();

        transform.Translate(direction * gameController.Difficulty * speed * Time.deltaTime, 0, 0);
    }
}
