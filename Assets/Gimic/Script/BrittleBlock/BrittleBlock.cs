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
    [Header("壊れた時に散らすオブジェクト")]
    public GameObject[] m_obj;

    [Header("破棄するオブジェクト")]
    public GameObject m_blockobj;

    void Start()
    {
  
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
            for (int i = 0; i < m_obj.Length; i++)
            {
                Instantiate(m_obj[i], this.transform.position, Quaternion.identity);
            }
            Destroy(m_blockobj.gameObject);
        }
    }

}
