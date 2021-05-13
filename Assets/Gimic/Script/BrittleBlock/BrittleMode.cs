using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleMode : MonoBehaviour
{
    public GameObject m_objBrittleBlock;
    bool m_IsMove=false;//壊れている状態か？
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_objBrittleBlock.GetComponent<BrittleBlock>().GetIsBroke())
        {
            if (!m_IsMove)//動いていなければ
            {
                m_IsMove = true;
                Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbodyを取得
                Vector3 force = new Vector3(Random.Range(-1, 1),    // X軸
                                            Random.Range(1.0f, 2.0f), // Y軸
                                            Random.Range(-1, -2));   // Z軸
                rb.AddForce(force, ForceMode.Impulse);          // 力を加える
                this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }

    }
}
