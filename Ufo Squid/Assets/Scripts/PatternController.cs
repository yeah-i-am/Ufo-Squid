using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternController : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private List<GameObject> freePatterns;
    private List<GameObject> onScreenPatterns;

    private Vector2 screenMax;
    private float patternSize;
    private float spawnPatternY;
    private float baseAlpha = 30f / 256f;

    private bool NeedToSpawn =>
        onScreenPatterns.Count == 0 || onScreenPatterns[onScreenPatterns.Count - 1].transform.position.y + patternSize <= screenMax.y;

    void Start()
    {
        onScreenPatterns = new List<GameObject>();

        gameController = FindObjectOfType<GameController>();

        screenMax = -Camera.main.ScreenToWorldPoint(Vector2.zero);

        patternSize = freePatterns[0].GetComponent<SpriteRenderer>().bounds.max.y;

        spawnPatternY = screenMax.y + patternSize;

        gameController.OnDeath += delegate { StartCoroutine("DeInit"); };
    }

    private void SpawnPattern()
    {
        int patternID = Random.Range(0, freePatterns.Count);

        if (onScreenPatterns.Count == 0)
            patternID = 0;

        GameObject pattern = freePatterns[patternID];
        freePatterns.RemoveAt(patternID);

        pattern.transform.position = new Vector3(0, spawnPatternY, 1);
        pattern.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, baseAlpha);
        pattern.SetActive(true);
        onScreenPatterns.Add(pattern);
    }

    public IEnumerator DeInit()
    {
        for (int i = 128; i > 0; i -= 3)
        {
            foreach (var pattern in onScreenPatterns)
            {
                SpriteRenderer sr = pattern.GetComponent<SpriteRenderer>();
                Color c = sr.color;
                c.a = c.a * 0.9f;
                sr.color = c;
            }

            yield return null;
        }

        while (onScreenPatterns.Count != 0)
            FreePattern();
    }

    private void FreePattern()
    {
        GameObject pattern = onScreenPatterns[0];
        onScreenPatterns.RemoveAt(0);

        pattern.SetActive(false);
        freePatterns.Add(pattern);
    }

    void Update()
    {
        if (!gameController.GameStarted)
            return;

        if (NeedToSpawn)
            SpawnPattern();

        var patternTransform = onScreenPatterns[0].transform;

        if (patternTransform.position.y + patternSize < -screenMax.y)
            FreePattern();

        foreach (GameObject pattern in onScreenPatterns)
            pattern.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, baseAlpha * (1f + 2f * gameController.patternBoost));
    }
}
