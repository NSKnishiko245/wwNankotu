//============================
//name:StageManager
//概要:タイルを折り曲げる処理
//警告が多すぎる問題勃発
//============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] m_tileObj;  //タイル
    public GameObject[] m_OriObj;    //折り目

    bool m_isInputSp = false;
    bool m_isInputRight = false;
    bool m_isInputLeft = false;
    bool m_isInputAny = false;

    //回転用
    bool rotStart = false;
    float speed = 3.0f;
    float rotAngle = 180f;
    float variation;
    float rot;

    // Start is called before the first frame update
    void Start()
    {
        variation = rotAngle / speed;
    }

    // Update is called once per frame
    void Update()
    {
        m_isInputSp = false;
        m_isInputRight = false;
        m_isInputLeft = false;
        //回転中じゃなったら
        if (!rotStart)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                m_isInputSp = true;
            }
            else if (Input.GetKey(KeyCode.R))
            {
                //親子関係をリセット
                ParentReset();
            }
            if (!m_isInputAny)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    m_isInputRight = true;
                    m_isInputAny = true;
                    OnBtn(m_OriObj[1]);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    m_isInputLeft = true;
                    m_isInputAny = true;
                    OnBtn(m_OriObj[1]);
                }
                else
                {
                    m_isInputAny = false;
                }
            }
        }
        //スペース押された状態
        if (m_isInputSp)
        {
            if (m_isInputLeft)
            {
                //親子関係をセットする
                ParentSetLefttoRight();
                //回転
                //RotationOri(m_OriObj[1]);
            }
            else if (m_isInputRight)
            {
                //親子関係をセットする
                ParentSetRighttoLeft();
                //回転
                //RotationOri(m_OriObj[1]);
            }
        }
        //回転
        RotationOriLeft(m_OriObj[1]);
    }


    //折り目の部分から左から右へ親子関係をセットする
    private void ParentSetLefttoRight()
    {
        for(int cnt = m_tileObj.Length-1; cnt > 0; cnt--)
        {
            m_tileObj[cnt].transform.parent = m_OriObj[cnt - 1].transform;
            m_OriObj[cnt-1].transform.parent = m_tileObj[cnt - 1].transform;
        }
    }
    //折り目の部分から右からひだりへ親子関係をセットする
    private void ParentSetRighttoLeft()
    {
        for (int cnt = 0; cnt < m_tileObj.Length-1; cnt++)
        {
            m_tileObj[cnt].transform.parent = m_OriObj[cnt].transform;
            m_OriObj[cnt].transform.parent = m_tileObj[cnt + 1].transform;
        }
    }
    //親子関係をリセット
    private void ParentReset()
    {
        for(int i = 0; i < m_tileObj.Length; i++)
        {
            m_tileObj[i].transform.parent = this.gameObject.transform;
        }
        for(int i = 0; i < m_OriObj.Length; i++)
        {
            m_OriObj[i].transform.parent = this.gameObject.transform;
        }
    }
    
    //折り目を180度回転(折り目のオブジェクト)
    private void RotationOriLeft(GameObject obj)
    {
        if (rotStart)
        {
            obj.transform.Rotate(0, variation * Time.deltaTime,0);
            rot += variation * Time.deltaTime;
            if (rot >= rotAngle)
            {
                rotStart = false;
                obj.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private void OnBtn(GameObject obj)
    {
        //回転角度を初期化する。
        rot = 0f;
        obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        rotStart = true;
    }
}
