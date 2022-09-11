using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        int fps = 0;
        float lastFPSTime = Time.realtimeSinceStartup;

        yield return null;

        while (true)
        {
            fps++;

            if (Time.realtimeSinceStartup - lastFPSTime > 0.999)
            {
                GetComponent<Text>().text = fps.ToString();
                fps = 0;
                lastFPSTime = Time.realtimeSinceStartup;
            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
