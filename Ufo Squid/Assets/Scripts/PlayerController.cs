using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameController gameController;
    private SpriteRenderer spriteRenderer;

    public float speed = 1f;

    [Range(0.01f, 0.99f)]
    public float patternOutBoost = 0.7f;

    [Range(1.01f, 2f)]
    public float patternBoost = 0.7f;

    private int direction = -1;

    private bool collidePattern = false;
    private float mmmm;

    void Start()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        gameController.OnGameStart += delegate { mmmm = 0; direction = -1; };
    }

    private void HandleTouch()
    {
        direction = -direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
    }

    private void FixedUpdate()
    {
        if (!gameController.GameStarted)
            return;

        if (collidePattern)
        {
            mmmm++;
            collidePattern = false;
        }
        else
            mmmm *= 0.9f;

        gameController.patternBoost = 1f - 1f / Mathf.Log10(10f + mmmm);
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

    private void OnEnable()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        gameController.SetSkin();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameController.GameStarted)
            return;

        gameController.Death();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider == GetComponents<PolygonCollider2D>()[1])
            collidePattern = true;
    }
}
