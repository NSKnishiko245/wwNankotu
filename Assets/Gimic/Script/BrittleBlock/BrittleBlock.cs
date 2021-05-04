//==============================================
//名前：脆いブロック
//概要：脆いブロック同士がぶつかると壊れる
//==============================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleBlock : MonoBehaviour
{
    [Header("消滅時に使うEffect")]
    public ParticleSystem m_endeffect;  //消滅時に使うエフェクト

    [Header("消滅時SE")]
    public AudioClip m_sound;
    // private AudioSource m_audioSource;
    public GameObject m_blockobj;

    // Start is called before the first frame update
    void Start()
    {
        //Componentを取得
       // m_audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "ClimbBlock")
        {
            AudioSource.PlayClipAtPoint(m_sound, this.transform.position);//SE再生
            Instantiate(m_endeffect, this.transform.position, Quaternion.identity); //消滅時にエフェクトを使用する
            Destroy(m_blockobj.gameObject);
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.tag == "Block")
    //    {
    //        m_audioSource.PlayOneShot(m_sound); //SE再生
    //        Instantiate(m_endeffect, this.transform.position, Quaternion.identity); //消滅時にエフェクトを使用する
    //        Destroy(this.gameObject);
    //    }
    //}
}
