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
    public Player m_player; //プレイヤー
    public GameObject[] m_tileObj;  //タイル
    public OrimeObj[] m_OriObj;    //折り目
    //パーティクル描画用
    public GameObject particleObject;


    private int nowParentNum = 0;

    float[] angle;

    int cnt = 0;
    int m_frame = 1;

    bool m_isInputSp = false;
    bool m_isInputRight = false;
    bool m_isInputLeft = false;
    bool m_isInputAny = false;

    //回転用
    bool m_LeftStart = false;
    bool m_RightStart = false;
    bool rotStart = false;
    float rotSpeed = 1.5f; // second
    float rotAngle = 180f;
    float variation;
    float rot;

    // Start is called before the first frame update
    void Start()
    {
        variation = rotAngle / rotSpeed;
        nowParentNum = 0;
        //折り目の本数分
        angle = new float[m_OriObj.Length];
        for (int i = 0; i < m_OriObj.Length; i++)
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

        //プレイヤーの触れているオブジェクトNoが折り目の数以下なら
        if (m_player.GetHitOriobjNum() >= 0 && m_player.GetHitOriobjNum() < m_OriObj.Length)
        {
           // float tri = Input.GetAxis("L_R_Trigger");
            //回転中じゃなったら
            if (!rotStart)
            {
                if (Input.GetKey(KeyCode.Space)|| Input.GetKey("joystick button 5"))
                {
                    m_isInputSp = true;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    ParentReset();
                }

                float tri = Input.GetAxis("L_R_Trigger");
                if (!m_isInputAny)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow)||tri >= 1)
                    {
                        m_isInputRight = true;
                        m_isInputAny = true;
                        m_frame = 1;

                        //OnBtn(m_OriObj[cnt]);
                        OnBtn(m_OriObj[m_player.GetHitOriobjNum()]);

                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow)|| tri <=-1)
                    {
                        m_isInputLeft = true;
                        m_isInputAny = true;
                        m_frame = 1;

                        //OnBtn(m_OriObj[cnt]);
                        OnBtn(m_OriObj[m_player.GetHitOriobjNum()]);
                    }
                    else
                    {
                        m_isInputAny = false;
                    }
                }
            }
        }
        //スペース押された状態
       // if (m_isInputSp)
        {
            if (m_isInputLeft)
            { 
                //親子関係をセットする
                ParentSetLefttoRight();
                //回転
                m_LeftStart = true;
            }
            else if (m_isInputRight)
            {
                //親子関係をセットする
                ParentSetRighttoLeft();
                //回転
                m_RightStart = true;
            }
        }

        if (m_frame < 0)
        {
            //回転
            if (m_LeftStart)
            {
                RotationOriLeft(m_OriObj[m_player.GetHitOriobjNum()]);
            }
            else if (m_RightStart)
            {
                RotationOriRight(m_OriObj[m_player.GetHitOriobjNum()]);
            }
        }
        m_frame--;
        Debug.Log(Input.GetAxisRaw("Horizontal2"));
    }


    //折り目の部分から左から右へ親子関係をセットする
    private void ParentSetLefttoRight()
    {
        if (nowParentNum > m_player.GetHitOriobjNum() && Mathf.Abs(angle[m_player.GetHitOriobjNum()]) < 1.0f)
        {
            ParentReset();
            Debug.Log("LRreset");
            for (int cnt = m_tileObj.Length - 1; cnt > 0; cnt--)
            {
                m_tileObj[cnt].transform.parent = m_OriObj[cnt - 1].transform;
                m_OriObj[cnt - 1].transform.parent = m_tileObj[cnt - 1].transform;
            }
            nowParentNum = 0;
        }
    }
    //折り目の部分から右からひだりへ親子関係をセットする
    private void ParentSetRighttoLeft()
    {
        if (nowParentNum <= m_player.GetHitOriobjNum() && Mathf.Abs(angle[m_player.GetHitOriobjNum()]) < 1.0f)
        {
            ParentReset();
            Debug.Log("RLreset");
            for (int cnt = 0; cnt < m_tileObj.Length - 1; cnt++)
            {
                m_tileObj[cnt].transform.parent = m_OriObj[cnt].transform;
                m_OriObj[cnt].transform.parent = m_tileObj[cnt + 1].transform;
            }
            nowParentNum = m_tileObj.Length - 1;
        }
    }
    //親子関係をリセット
    private void ParentReset()
    {
        for (int i = 0; i < m_tileObj.Length; i++)
        {
            m_tileObj[i].transform.parent = this.gameObject.transform;
        }
        for (int i = 0; i < m_OriObj.Length; i++)
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
            //angle[obj.m_Number] += variation * Time.deltaTime;
           // SetChildActive(m_tileObj[0], false);
          //  SetChildActive(m_tileObj[1], false);
            //SetChildActive(m_tileObj[2], false);
            //SetChildActive(m_tileObj[3], false);
            //SetChildActive(m_tileObj[4], false);
            //SetChildActive(m_tileObj[5], false);
            if (rot >= rotAngle)
            {
               

                //SetChildActive(m_tileObj[0], true);
                //SetChildActive(m_tileObj[1], true);
                //SetChildActive(m_tileObj[2], true);
                //SetChildActive(m_tileObj[3], true);
                //SetChildActive(m_tileObj[4], true);
                //SetChildActive(m_tileObj[5], true);
                TileReset();
                m_LeftStart = false;
                rotStart = false;
                m_isInputAny = false;
                angle[obj.m_Number] += 180;
                Debug.Log(angle[obj.m_Number]);
                obj.transform.localRotation = Quaternion.Euler(0, angle[obj.m_Number], 0);
            }
        }
    }

    private void RotationOriRight(OrimeObj obj)
    {
        if (rotStart)
        {
            obj.transform.Rotate(0, -variation * Time.deltaTime, 0);
            rot += variation * Time.deltaTime;
            //angle[obj.m_Number] += -variation * Time.deltaTime;
           // SetChildActive(m_tileObj[0], false);
           // SetChildActive(m_tileObj[1], false);
            //SetChildActive(m_tileObj[2], false);
            //SetChildActive(m_tileObj[3], false);
            //SetChildActive(m_tileObj[4], false);
            //SetChildActive(m_tileObj[5], false);
            if (rot >= rotAngle)
            {
                //SetChildActive(m_tileObj[0], true);
                //SetChildActive(m_tileObj[1], true);
                //SetChildActive(m_tileObj[2], true);
                //SetChildActive(m_tileObj[3], true);
                //SetChildActive(m_tileObj[4], true);
                //SetChildActive(m_tileObj[5], true);
                TileReset();
                m_RightStart = false;
                rotStart = false;
                m_isInputAny = false;
                angle[obj.m_Number] -= 180;
                obj.transform.localRotation = Quaternion.Euler(0, angle[obj.m_Number], 0);
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

    //タイルのコンポーネント
    private void TileReset()
    {
        for (int i = 0; i < m_tileObj.Length; i++)
        {
            m_tileObj[i].GetComponent<ScreenShot>().ResetTexture();
        }
    }

    //タイルの子供を
    private void SetChildActive(GameObject obj,bool flg)
    {
        obj.transform.Find("Cube").gameObject.SetActive(flg);
        if (flg)
        {
            Instantiate(particleObject, obj.transform.Find("Cube").transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
        }
    }
}
