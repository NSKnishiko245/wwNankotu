using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRotate : MonoBehaviour
{
    public bool isRotNow { get; private set; }
    public void TurnOnRotate() { isRotNow = true; }

    private float rotate_speed = 1.0f;
    public bool isReverse { get; private set; }

    private float rotStartInterval = 0.5f;
    private float timer = 0.0f;

    void Start()
    {
        isReverse = false;
        isRotNow = false;
    }

    void Update()
    {
        if (isRotNow)
        {
            timer += Time.deltaTime;
            if (timer < rotStartInterval) return;

            transform.Rotate(rotate_speed, 0.0f, 0.0f);
            if (!isReverse)
            {
                if (transform.eulerAngles.x >= 180)
                {
                    transform.eulerAngles = new Vector3(180, 0.0f, 0.0f);
                    isRotNow = false;
                    isReverse = true;
                }
            }
            else
            {
                if (transform.eulerAngles.x <= 180)
                {
                    transform.eulerAngles = new Vector3(0, 0.0f, 0.0f);
                    isRotNow = false;
                    isReverse = false;
                }
            }
        }
        else
        {
            timer = 0.0f;
        }
    }
}