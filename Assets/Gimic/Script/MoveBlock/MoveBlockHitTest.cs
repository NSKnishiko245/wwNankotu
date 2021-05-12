using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockHitTest : MonoBehaviour
{
    public bool isHit { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        isHit = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.tag);
        if (other.tag == "ClimbBlock" || other.tag == "GimicMoveBlock" || other.tag == "Block") isHit = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isHit = false;
    }

    public bool HitMoveBlock()
    {
        Vector3 ray_pos = transform.position + Vector3.right * 0.5f;
        Ray ray = new Ray(ray_pos, Vector3.right);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f))
        {
            if (hit.transform.tag == "ClimbBlock" || hit.transform.tag == "GimicMoveBlock") return true;
        }
        else
        {
            Debug.Log("ik");
        }
        return false;
    }
}
