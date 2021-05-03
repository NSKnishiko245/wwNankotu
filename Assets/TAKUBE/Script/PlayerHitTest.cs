using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitTest : MonoBehaviour
{
    public float HitBlockHeight { get; private set; }
    public bool isHit { get; private set; }

    void Start()
    {
        HitBlockHeight = 0.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.transform.tag;
        if (tag == "Block" || tag == "GimicBreakBlock" || tag == "GimicMoveBlock")
        {
            Vector3 ray_pos = other.transform.position;
            Ray ray = new Ray(ray_pos, Vector3.up);
            if (!Physics.Raycast(ray, out RaycastHit hit, other.transform.lossyScale.y))
            {
                HitBlockHeight = other.transform.lossyScale.y;
                isHit = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isHit = false;
    }
    public void ResetHitFlg()
    {
        isHit = false;
    }
}