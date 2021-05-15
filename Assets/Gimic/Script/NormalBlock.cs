using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBlock : MonoBehaviour
{
    private float i=0.1f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(i, i, i);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x >= 1)
        {
            i += 0.01f;
            if (i >= 1.0f)
            {
                i = 1;
            }
            Vector3 i2;
            i2 = new Vector3(i, i, i);
            transform.localScale =i2;
        }
    }
}
