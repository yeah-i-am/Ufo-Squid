using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float scale = Mathf.Max(Screen.width / 1080f, Screen.height / 1920f);

        transform.localScale = new Vector3(scale, scale, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
