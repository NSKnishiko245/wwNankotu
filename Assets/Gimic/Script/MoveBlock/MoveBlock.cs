using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    //SE
    public AudioClip m_audioClip;


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
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }

        m_nowCnt++;
    }

    //端っこの当たり判定
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "GimicMoveBar")
        {
           // AudioSource.PlayClipAtPoint(m_audioClip, this.transform.position);//SE再生
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            m_addCnt = m_nowCnt;
            m_addCnt++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "GimicMoveBar")
        {
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

}
