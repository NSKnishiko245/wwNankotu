//==============================================
//名前：脆いブロック
//概要：脆いブロック同士がぶつかると壊れる
//==============================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleBlock : MonoBehaviour
{
    public GameObject m_objBlock;

    [Header("消滅時に使うEffect")]
    public ParticleSystem m_endeffect;  //消滅時に使うエフェクト

    [Header("消滅時SE")]
    public AudioClip m_sound;
    private AudioSource m_audioSource;


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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Block")
    //    {
    //        m_audioSource.PlayOneShot(m_sound); //SE再生
    //        Instantiate(m_endeffect, this.transform.position, Quaternion.identity); //消滅時にエフェクトを使用する
    //        m_objBlock.SetActive(false);   //ブロック同士がぶつかると壊れる
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Block")
        {
            //m_audioSource.PlayOneShot(m_sound); //SE再生
            Instantiate(m_endeffect, this.transform.position, Quaternion.identity); //消滅時にエフェクトを使用する
            m_objBlock.SetActive(false);   //ブロック同士がぶつかると壊れる
        }
    }
}
