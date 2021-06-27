using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectClear : MonoBehaviour
{
    public int m_lifeFrame = 60;
    public int m_intervalframe=3;
    private bool m_isStart=false;
    private int m_cnt = 0;

    public ParticleSystem m_particle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isStart)
        {
            if (m_cnt % m_intervalframe==0)
            {
                Instantiate(m_particle, this.transform.position, Quaternion.identity);//きらきらエフェクト
                
            }


            if (m_cnt > m_lifeFrame) Destroy(this.gameObject);
            Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            m_cnt++;
        }
    }
    public void SetIsStart(bool flg)
    {
        m_isStart = flg;
    }
}
