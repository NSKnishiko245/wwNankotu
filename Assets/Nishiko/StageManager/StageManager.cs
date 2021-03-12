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
    public OrimeObj[] m_OriObj;    //折り目

    float[] angle;

    //遅延用スクリーンショットをする為にわざと１フレームずらす
    int m_frameCnt = 1;

    int cnt=0;


    bool m_isInputSp = false;
    bool m_isInputRight = false;
    bool m_isInputLeft = false;
    bool m_isInputAny = false;

    //回転用
    bool m_LeftStart = false;
    bool m_RightStart = false;
    bool rotStart = false;
    float speed = 2.0f;
    float rotAngle = 180f;
    float variation;
    float rot;

    // Start is called before the first frame update
    void Start()
    {
        variation = rotAngle / speed;
        
        //折り目の本数分
        angle = new float[m_OriObj.Length];
        for(int i = 0; i < m_OriObj.Length; i++)
        {
            angle[i] = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_isInputSp = false;
        m_isInputRight = false;
        m_isInputLeft = false;


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cnt++;
            Debug.Log(cnt);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cnt--;
            Debug.Log(cnt);
        }



        //回転中じゃなったら
        if (!rotStart)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                m_isInputSp = true;
            }
            if (Input.GetKey(KeyCode.R))
            {
                ParentReset();
            }
            if (!m_isInputAny)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    m_isInputRight = true;
                    m_isInputAny = true;

                     OnBtn(m_OriObj[cnt]);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    m_isInputLeft = true;
                    m_isInputAny = true;

                     OnBtn(m_OriObj[cnt]);
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
                m_LeftStart = true;
                m_frameCnt = 1;
            }
            else if (m_isInputRight)
            {
                //親子関係をセットする
                ParentSetRighttoLeft();
                //回転
                //RotationOri(m_OriObj[1]);
                m_RightStart = true;
                m_frameCnt = 1;
            }
        }

        //回転
        if (m_LeftStart)
        {
            if (m_frameCnt <= 0)
            {
                RotationOriLeft(m_OriObj[cnt]);
            }
            m_frameCnt--;
        }
        else if (m_RightStart)
        {
            if (m_frameCnt <= 0)
            {
                RotationOriRight(m_OriObj[cnt]);
            }
            m_frameCnt--;
        }

        //Debug.Log(angle);
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
    private void RotationOriLeft(OrimeObj obj)
    {
        if (rotStart)
        {

            obj.transform.Rotate(0, variation * Time.deltaTime, 0);
            rot += variation * Time.deltaTime;
            SetObjChildActive(m_tileObj[0], false);
            SetObjChildActive(m_tileObj[1], false);
            SetObjChildActive(m_tileObj[2], false);
           // SetObjChildActive(m_tileObj[3], false);
            if (rot >= rotAngle)
            {
                SetObjChildActive(m_tileObj[0], true);
                SetObjChildActive(m_tileObj[1], true);
                SetObjChildActive(m_tileObj[2], true);
               // SetObjChildActive(m_tileObj[3], true);
                angle[obj.m_Number] += 180;
                m_LeftStart = false;
                rotStart = false;
                m_isInputAny = false;
                obj.transform.localRotation = Quaternion.Euler(0, angle[obj.m_Number], 0);
               // ParentReset();
            }
        }
    }

    private void RotationOriRight(OrimeObj obj)
    {
        if (rotStart)
        {
            obj.transform.Rotate(0, -variation * Time.deltaTime, 0);
            rot += variation * Time.deltaTime;
            // angle[obj.m_Number] += -variation * Time.deltaTime;
            SetObjChildActive(m_tileObj[0], false);
            SetObjChildActive(m_tileObj[1], false);
            SetObjChildActive(m_tileObj[2], false);
            //SetObjChildActive(m_tileObj[3], false);
            if (rot >= rotAngle)
            {
                SetObjChildActive(m_tileObj[0], true);
                SetObjChildActive(m_tileObj[1], true);
                SetObjChildActive(m_tileObj[2], true);
               // SetObjChildActive(m_tileObj[3], true);
                angle[obj.m_Number] -= 180;
                m_RightStart = false;
                rotStart = false;
                m_isInputAny = false;
                obj.transform.localRotation = Quaternion.Euler(0, angle[obj.m_Number], 0);
                //ParentReset();
            }
        }
    }

    private void OnBtn(OrimeObj obj)
    {
        //回転角度を初期化する。
        rot = 0f;
        obj.transform.localRotation = Quaternion.Euler(0, angle[obj.m_Number], 0);
        rotStart = true;
    }

    //子オブジェクトを非アクティブに
    private void SetObjChildActive(GameObject obj,bool flg)
    {
        obj.transform.Find("block").gameObject.SetActive(flg);
    }
}
