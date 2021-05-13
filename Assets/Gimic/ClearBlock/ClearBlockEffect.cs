using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBlockEffect : MonoBehaviour
{
    public ParticleSystem m_objMist;
    [Header("¶¬ŠÔŠu")]
    public float frameOut = 1;
    private float timeElapsed = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += 1;

        if (timeElapsed >= frameOut)
        {
            Instantiate(m_objMist, this.transform.position, Quaternion.identity);
            //‰Œ¶¬
            timeElapsed = 0.0f;
        }
    }
}
