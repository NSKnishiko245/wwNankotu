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
    public bool isMoveEx = false;

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
        if (delta == 1.0f)
        {
            isMove = false;
            isMoveEx = false;
        }
    }

    public void Move(float dist)
    {
        delta = 0.0f;
        startMarker = this.transform.position;
        endMarker.x = transform.position.x + dist;
        isMove = true;
        isMoveEx = true;
    }
    public void SetPos(float dist)
    {
        if (isMove) return;
        delta = 0.0f;
        startMarker = this.transform.position;
        endMarker = transform.position;
        endMarker.x = dist;
        isMove = true;
    }
}