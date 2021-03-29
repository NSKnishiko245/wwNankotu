using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorMove : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.GetComponent<Transform>().position.z;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        this.GetComponent<Transform>().position = mousePos;
    }
}
