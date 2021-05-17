using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    //SE
    public AudioClip m_audioClip;

    private bool m_HitFlg;

    private int m_nowCnt = 0;
    private int m_addCnt = 0;

    //ブロックに当たっているのか
    public GameObject m_objCollision;
    private bool m_isNormalBlockHit = false; 
    private int m_normalHitcnt = 0;
    private int m_aadnormalHitCnt = 0;

    private bool isGravity = true;
    private bool m_isFirst_hit = false;

    //
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_addCnt < m_nowCnt)
        {
           // this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition|RigidbodyConstraints.FreezeRotation;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //離れたらヒット状態をfalseに
            m_isFirst_hit = false;
        }

        if (m_aadnormalHitCnt < m_normalHitcnt)
        {
            Debug.Log("ノーマルブロックと当たっていない");
            m_isNormalBlockHit = false;
            //m_objCollision.SetActive(true);
            m_objCollision.GetComponent<BoxCollider>().isTrigger = false;
            //this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        
        Debug.Log(m_aadnormalHitCnt);

        m_nowCnt++;
        m_normalHitcnt++;
    }

    //端っこの当たり判定
    private void OnTriggerStay(Collider other)
    {
        if (!isGravity) return;
        if (other.transform.tag == "GimicMoveBar")
        {
            //if (m_isNormalBlockHit) return; 

            //初めてバーに触れた時
            if (!m_isFirst_hit)
            {
                m_isFirst_hit = true;//ヒット状態に
                //SE再生
                AudioSource.PlayClipAtPoint(m_audioClip, this.transform.position);//SE再生
                //エフェクト再生

                //Debug.Log("レールと初めて当たった");

            }
            if (!m_isNormalBlockHit)
            {
                this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
                // AudioSource.PlayClipAtPoint(m_audioClip, this.transform.position);//SE
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            m_addCnt = m_nowCnt;
            m_addCnt++;
        }

        if(other.transform.tag == "Block")
        {
            Debug.Log("当たっている");
            //    Debug.Log("normalblockhit");
            m_objCollision.GetComponent<BoxCollider>().isTrigger=true;
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
       
            m_isNormalBlockHit = true;
            m_aadnormalHitCnt = m_normalHitcnt;
            m_aadnormalHitCnt+=5;
        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "GimicMoveBar")
        {
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    public void TurnOnGravity()
    {
        isGravity = true;
    }
    public void TurnOffGravity()
    {
        isGravity = false;
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }
}