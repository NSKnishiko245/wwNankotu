using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDelete : MonoBehaviour
{
    private int m_nowCnt = 0;
    private int m_addCnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_addCnt < m_nowCnt)
        {
            this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }

        m_nowCnt++;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "ClimbBlock")
        {
            Debug.Log("HIT");
            //this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            this.gameObject.SetActive(false);
            m_addCnt = m_nowCnt;
            m_addCnt++;
        }
    }
}
