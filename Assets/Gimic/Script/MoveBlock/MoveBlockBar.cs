using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockBar : MonoBehaviour
{
    //動き始めるまでのフレーム
    public int m_frame = 30;
    private int m_cntframe = 0;//カウント用変数

    public CollisionOut m_UpCol;
    public CollisionOut m_DownCol;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
       
        //if (m_UpCol.GetIsHit())
        //{
        //    m_UpCol.gameObject.SetActive(false);
        //}
        //if (m_DownCol.GetIsHit())
        //{
        //    m_DownCol.gameObject.SetActive(false);
        //}
    }

    //MoveBlockとの当たり判定
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.transform.tag == "GimicMoveBlock")
    //    {
    //        Debug.Log("バーとブロック接触");
    //        other.gameObject.GetComponent<MoveBlock>().SetIsHit(true);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.transform.tag == "GimicMoveBlock")
    //    {
    //        other.gameObject.GetComponent<MoveBlock>().SetIsHit(false);
    //    }
    //}
}
