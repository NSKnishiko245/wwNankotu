using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitTest : MonoBehaviour
{
    public float HitBlockHeight { get; private set; }
    public bool isHit { get; private set; }

    public enum COLLISIONDIRECTION
    {
        LEFT,
        RIGHT
    }
    public COLLISIONDIRECTION dir;

    void Start()
    {
        HitBlockHeight = 0.0f;
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.transform.tag != "ClimbBlock") return;

        //Vector3 ray_pos = other.transform.position;
        //Ray ray = new Ray(ray_pos, Vector3.up);
        //if (!Physics.Raycast(ray, out RaycastHit hit, other.transform.lossyScale.y))
        //{
        //    HitBlockHeight = other.transform.lossyScale.y;
        //    isHit = true;
        //}

        string tag = other.transform.tag;
        if (tag == "ClimbBlock"||tag=="GimicClearBlock"||tag == "Block"||tag=="GimicMoveBlock")
        {
            Vector3 ray_pos = other.transform.position + Vector3.down * 0.1f + Vector3.back * 0.38f;
            Ray ray = new Ray(ray_pos, Vector3.up);
            if (!Physics.Raycast(ray, out RaycastHit hit, other.transform.lossyScale.y))
            {
                HitBlockHeight = other.transform.lossyScale.y;
                isHit = true;

            }
            else
            {
                tag = hit.transform.tag;
                if (tag != "ClimbBlock")
                {
                    if (tag == "GimicClearBlock")
                    {
                        if (hit.transform.GetComponent<ClearBlock>().IsClear())
                        {
                            isHit = false;
                            return;
                        }
                    }
                    if (tag == "GimicOneWayBlock")
                    {
                        if ((hit.transform.GetComponent<OneWayCenter>().m_Direction == OneWayCenter.DIRECTION.LEFT && dir != COLLISIONDIRECTION.RIGHT) ||
                            hit.transform.GetComponent<OneWayCenter>().m_Direction == OneWayCenter.DIRECTION.RIGHT && dir != COLLISIONDIRECTION.LEFT)
                        {
                            isHit = false;
                            return;
                        }
                    }

                    if (tag == "GimicMoveBlock")
                    {
                         isHit = false;
                         return;
                    }

                    if (tag == "GimicMoveBar")
                    {
                        if (hit.transform.GetComponent<MoveBarHitCheck>().isHit)
                        {
                            isHit = false;
                            return;
                        }
                    }

                    HitBlockHeight = other.transform.lossyScale.y;
                    isHit = true;
                }
                else
                {
                    isHit = false;
                }
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