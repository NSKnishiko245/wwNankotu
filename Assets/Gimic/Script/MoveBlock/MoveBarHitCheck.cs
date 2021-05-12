using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarHitCheck : MonoBehaviour
{
    public bool isHit { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isHit = transform.Find("hitbox").GetComponent<MoveBlockHitTest>().isHit;
    }
}
