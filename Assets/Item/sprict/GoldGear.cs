//プレイヤーが金色のギアをふれたら消す

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGear : MonoBehaviour
{

    public ParticleSystem m_particle;//取得時のエフェクト

    private Animator m_anim;   //取得時のアニメーション
    private bool m_getflg = false;
    // Start is called before the first frame update
    void Start()
    {
        m_anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void DestroyGear()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Instantiate(m_particle, this.transform.position, Quaternion.identity); //消滅時にエフェクトを使用する
            //Destroy(this.gameObject);
            m_getflg = true;
            m_anim.SetBool("GetFlg",m_getflg);
        }
    }
}
