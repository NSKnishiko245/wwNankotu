using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditCamera : MonoBehaviour
{
    private float mouse_sensitivity;
    private float mouse_move_x;
    private float mouse_move_y;


    void Start()
    {
        mouse_sensitivity = 0.2f;
    }

    void Update()
    {
        // ƒY[ƒ€
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Input.mouseScrollDelta.y);

        // ‰¡ˆÚ“®
        if (Input.GetMouseButton(2))
        {
            mouse_move_x += Input.GetAxis("Mouse X") * mouse_sensitivity;
            mouse_move_y += Input.GetAxis("Mouse Y") * mouse_sensitivity;

            if (Mathf.Abs(mouse_move_x) >= 1.0f)
            {
                transform.position = new Vector3(transform.position.x - (int)mouse_move_x, transform.position.y, transform.position.z);
                mouse_move_x = 0.0f;
            }
            if (Mathf.Abs(mouse_move_y) >= 1.0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (int)mouse_move_y, transform.position.z);
                mouse_move_y = 0.0f;
            }
        }
        else
        {
            mouse_move_x = mouse_move_y = 0.0f;
        }
    }
}