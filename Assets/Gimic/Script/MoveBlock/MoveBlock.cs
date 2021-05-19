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

    [Header("回転する歯車")]
    public GameObject m_objLeftGear;
    public GameObject m_objRightGear;

    [Header("火花エフェクト")]
    public ParticleSystem m_objParticle;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(this.GetComponent<Rigidbody>().velocity.y) > 1.0f)
        {
            // Debug.Log("火花発生");
            this.GetComponent<BoxCollider>().isTrigger = true;
            Instantiate(m_objParticle, this.transform.position, Quaternion.identity);
        }
        else
        {
            this.GetComponent<BoxCollider>().isTrigger = false;
        }


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
            //Debug.Log("ノーマルブロックと当たっていない");
            m_isNormalBlockHit = false;
            //m_objCollision.SetActive(true);
            //m_objCollision.GetComponent<BoxCollider>().isTrigger = false;
            //this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        
        //Debug.Log(m_aadnormalHitCnt);

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

            float VecY = Mathf.Abs(this.GetComponent<Rigidbody>().velocity.y);
            if (VecY <= 0.5f) VecY = 0.0f;
            //歯車回転
            m_objLeftGear.transform.Rotate(new Vector3(0, 0, -90* VecY) * Time.deltaTime, Space.World);
            m_objRightGear.transform.Rotate(new Vector3(0, 0, 90* VecY) * Time.deltaTime, Space.World);

            // AudioSource.PlayClipAtPoint(m_audioClip, this.transform.position);//SE
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            m_addCnt = m_nowCnt;
            m_addCnt++;
        }

        if(other.transform.tag == "Block")
        {
            //Debug.Log("当たっている");
            //    Debug.Log("normalblockhit");
           // m_objCollision.GetComponent<BoxCollider>().isTrigger=true;
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