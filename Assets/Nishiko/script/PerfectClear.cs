using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectClear : MonoBehaviour
{
    public int m_lifeFrame = 60;
    private bool m_life = true;
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
            if (m_cnt % m_intervalframe==0&&m_life)
            {
                Instantiate(m_particle, this.transform.position, Quaternion.identity);//きらきらエフェクト
                
            }


            if (m_cnt > m_lifeFrame) m_life = false;
            
            m_cnt++;
        }
    }
    public void SetIsStart(bool flg)
    {
        m_isStart = flg;
    }
}
