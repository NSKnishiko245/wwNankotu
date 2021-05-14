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

    private bool isGravity = true;

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
        }
        m_nowCnt++;
    }

    //í[Ç¡Ç±ÇÃìñÇΩÇËîªíË
    private void OnTriggerStay(Collider other)
    {
        if (!isGravity) return;
        if (other.transform.tag == "GimicMoveBar")
        {
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            // AudioSource.PlayClipAtPoint(m_audioClip, this.transform.position);//SEçƒê∂
            
            {
                this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
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