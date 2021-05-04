using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordersScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float scale = (1920f / 1080f) * ((float)Screen.width / Screen.height);

        transform.localScale = new Vector3(transform.localScale.x * scale, transform.localScale.y, transform.localScale.x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
