using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim : MonoBehaviour
{
    //アニメーション
    private Animator m_anim;
    public GameObject m_playobj;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playobj.GetComponent<Player>().IsMove)
        {
            m_anim.SetBool("move_flg", true);
        }
        else
        {
            m_anim.SetBool("move_flg", false);
        }
    }
}
