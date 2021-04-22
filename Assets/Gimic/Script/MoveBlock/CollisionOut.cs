using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionOut : MonoBehaviour
{
    // 当たり判定用オブジェクト
    public GameObject m_collisionobj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "GimicMoveCol")
        {
            //当たっている
            // m_isHit = true;
            m_collisionobj.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "GimicMoveCol")
        {
            //離れている
            // m_isHit = false;
            m_collisionobj.SetActive(true);
        }
    }

    //端っこ同士が当たっているか返す
   
}
