using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //スタートと終わりの目印
    private Vector3 startMarker;
    private Vector3 endMarker;

    // スピード
    public float speed = 1.0F;

    private float delta = 1.0f;

    public bool isMove = false;

    void Start()
    {
        startMarker = endMarker = transform.position;
    }

    void Update()
    {
        if (!isMove) return;

        if (delta < 1.0f)
        {
            delta += Time.deltaTime;
            if (delta > 1.0f)
            {
                delta = 1.0f;
            }
        }

        // オブジェクトの移動
        transform.position = Vector3.Lerp(startMarker, endMarker, delta);
        if (delta == 1.0f) isMove = false;
    }

    public void Move(float dist)
    {
        delta = 0.0f;
        startMarker = this.transform.position;
        endMarker.x = transform.position.x + dist;
        isMove = true;
    }
}