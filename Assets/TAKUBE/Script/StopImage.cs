using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopImage : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        offset = new Vector3(0.5f, 1, 0);
        this.transform.position = target.position + offset;
        Debug.Log(this.transform.position);
    }
}
