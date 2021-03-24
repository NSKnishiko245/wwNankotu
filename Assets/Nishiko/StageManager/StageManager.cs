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
    private int[,] m_blockMap;
    private GameObject[,] m_blockList;
    private List<GameObject> m_tileObj;  //タイル
    private List<GameObject> m_OriObj;    //折り目
    //パーティクル描画用
    public GameObject particleObject;


    public GameObject MapManager;


    private int nowParentNum = 0;


    public int stageNum = 1;

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

    int orinum = 0;

    bool firstFlg = false;

    // 現在の回転状態
    private enum ROTSTATE
    {
        NEUTRAL,
        ROTATE,
        REROTATE,
        ROTATED,
    }
    ROTSTATE rotState = ROTSTATE.NEUTRAL;

    void StartEx()
    {
        // エディットオブジェクトを取得し、マップ構築後、このクラスが持つ配列にデータを格納
        MapManager.GetComponent<MapEdit>().CreateStage_Game(stageNum);
        m_blockList = MapManager.GetComponent<MapEdit>().BlockList;
        m_blockMap = MapManager.GetComponent<MapEdit>().BlockMap;
        m_tileObj = MapManager.GetComponent<MapEdit>().TileList;
        m_OriObj = MapManager.GetComponent<MapEdit>().BarList;

        // タイルとバーをステージマネージャーの子供にする
        for (int i = 0; i < m_tileObj.Count; i++)
        {
            m_tileObj[i].transform.parent = this.transform;
        }
        for (int i = 0; i < m_OriObj.Count; i++)
        {
            m_OriObj[i].transform.parent = this.transform;
        }
        //this.transform.Rotate(90.0f, 0.0f, 0.0f);

        variation = rotAngle / rotSpeed;
        nowParentNum = 0;
        //折り目の本数分
        angle = new float[m_OriObj.Count];
        for (int i = 0; i < m_OriObj.Count; i++)
        {
            angle[i] = 0.0f;
        }
    }

    void Update()
    {
        if (!firstFlg)
        {
            StartEx();
            firstFlg = true;
        }

        m_isInputSp = false;
        m_isInputRight = false;
        m_isInputLeft = false;

        if (rotState == ROTSTATE.NEUTRAL)
        {
            orinum = -1;
            for (int i = 0; i < m_OriObj.Count; i++)
            {
                if (m_OriObj[i].GetComponent<Cbar>().stay)
                {
                    orinum = i;
                }
            }
        }

        //プレイヤーの触れているオブジェクトNoが折り目の数以下なら
        if (orinum >= 0)
        {
            float tri = Input.GetAxis("L_R_Trigger");
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
                if (!m_isInputAny && m_isInputSp)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow)||tri>=1)
                    {
                        m_isInputRight = true;
                        m_isInputAny = true;
                        m_frame = 1;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow)|| tri <= -1)
                    {
                        m_isInputLeft = true;
                        m_isInputAny = true;
                        m_frame = 1;
                    }
                    else
                    {
                        m_isInputAny = false;
                    }
                }
            }
        }
        //スペース押された状態
        if (m_isInputSp && rotState == ROTSTATE.NEUTRAL && orinum >= 0)
        {
            if (m_isInputLeft)
            {
                ParentReset();
                ParentSetLefttoRight();
                //回転
                m_LeftStart = true;
                rotState = ROTSTATE.ROTATE;
                OnBtn();
            }
            else if (m_isInputRight)
            {
                ParentReset();
                ParentSetRighttoLeft();
                //回転
                m_RightStart = true;
                rotState = ROTSTATE.ROTATE;
                OnBtn();
            }
        }

        if (Input.GetKeyDown("joystick button 0") && rotState == ROTSTATE.ROTATED && orinum >= 0)
        {
            if (angle[orinum] >= 170)
            {
                ParentSetRighttoLeft();
                //回転
                m_RightStart = true;
                rotState = ROTSTATE.REROTATE;
                OnBtn();
            }
            else
            {
                ParentSetLefttoRight();
                //回転
                m_LeftStart = true;
                rotState = ROTSTATE.REROTATE;
                OnBtn();
            }
        }

        if (m_frame <= 0 && orinum >= 0)
        {
            //回転
            if (m_LeftStart)
            {
                RotationOriLeft(m_OriObj[orinum], orinum);
            }
            else if (m_RightStart)
            {
                RotationOriRight(m_OriObj[orinum], orinum);
            }
        }
        m_frame--;


        // ワープ処理
        Vector2Int hitBlockNum = new Vector2Int(-1, -1);
        for (int y = 0; y < m_blockList.GetLength(0); y++)
        {
            for (int x = 0; x < m_blockList.GetLength(1); x++)
            {
                if (m_blockMap[y, x] != 0)
                {
                    if (m_blockList[y, x].tag == "BorderLine")
                    {
                        if (m_blockList[y, x].GetComponent<HitAction>().isHit)
                        {
                            Debug.Log("find");
                            hitBlockNum = new Vector2Int(x, y);
                            // ２重のループを抜ける為の処理
                            x = m_blockList.GetLength(1);
                            y = m_blockList.GetLength(0);
                        }
                    }
                }
            }
        }

        if (hitBlockNum.x != -1)
        {
            if (hitBlockNum.x < m_blockList.GetLength(1) / 2)
            {
                for (int y = 0; y < m_blockList.GetLength(0); y++)
                {
                    for (int x = m_blockList.GetLength(1) - 1; x >= 0; x--)
                    {
                        if (m_blockMap[y, x] != 0)
                        {
                            if (m_blockList[y, x].tag == "BorderLine")
                            {
                                if (y == m_blockList.GetLength(0) - hitBlockNum.y)
                                {
                                    m_player.transform.position = new Vector3(
                                        m_blockList[y, x].transform.position.x - m_blockList[y, x].transform.lossyScale.x,
                                        m_blockList[y, x].transform.position.y,
                                        m_blockList[y, x].transform.position.z
                                    );
                                    // ２重のループを抜ける為の処理
                                    x = -1;
                                    y = m_blockList.GetLength(0);

                                    if (y < m_blockList.GetLength(0) / 2)
                                    {
                                        Physics.gravity = new Vector3(0, -9.8f, 0);
                                    }
                                    else
                                    {
                                        Physics.gravity = new Vector3(0, 9.8f, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < m_blockList.GetLength(0); y++)
                {
                    for (int x = 0; x < m_blockList.GetLength(1); x++)
                    {
                        if (m_blockMap[y, x] != 0)
                        {
                            if (m_blockList[y, x].tag == "BorderLine")
                            {
                                if (y == m_blockList.GetLength(0) - hitBlockNum.y)
                                {
                                    m_player.transform.position = new Vector3(
                                        m_blockList[y, x].transform.position.x + m_blockList[y, x].transform.lossyScale.x,
                                        m_blockList[y, x].transform.position.y,
                                        m_blockList[y, x].transform.position.z
                                    );
                                    // ２重のループを抜ける為の処理
                                    x = m_blockList.GetLength(1);
                                    y = m_blockList.GetLength(0);

                                    if (y < m_blockList.GetLength(0) / 2)
                                    {
                                        Physics.gravity = new Vector3(0, -9.8f, 0);
                                    }
                                    else
                                    {
                                        Physics.gravity = new Vector3(0, 9.8f, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    // タイルを右から左へ回転させる処理
    private void ParentSetLefttoRight()
    {
        for (int cnt = m_tileObj.Count - 1; cnt > 0; cnt--)
        {
            m_tileObj[cnt].transform.parent = m_OriObj[cnt - 1].transform;
            m_OriObj[cnt - 1].transform.parent = m_tileObj[cnt - 1].transform;
        }
        nowParentNum = 0;
    }
    // タイルを左から右へ回転させる処理
    private void ParentSetRighttoLeft()
    {
        for (int cnt = 0; cnt < m_tileObj.Count - 1; cnt++)
        {
            m_tileObj[cnt].transform.parent = m_OriObj[cnt].transform;
            m_OriObj[cnt].transform.parent = m_tileObj[cnt + 1].transform;
        }
        nowParentNum = m_tileObj.Count - 1;
    }
    //親子関係をリセット
    private void ParentReset()
    {
        for (int i = 0; i < m_tileObj.Count; i++)
        {
            m_tileObj[i].transform.parent = this.gameObject.transform;
        }
        for (int i = 0; i < m_OriObj.Count; i++)
        {
            m_OriObj[i].transform.parent = this.gameObject.transform;
        }
    }

    // 裏面かどうか
    private bool IsBackTile()
    {
        int orinum = 999;
        for (int i = 0; i < m_player.GetHitOriNumList().Count; i++)
        {
            if (orinum > m_player.GetHitOriNumList()[i])
            {
                orinum = m_player.GetHitOriNumList()[i];
            }
        }

        for (int i = orinum; i < m_OriObj.Count; i++)
        {
            if (Mathf.Abs(angle[i]) > 170.0f)
            {
                return true;
            }
        }
        return false;
    }
    private bool IsBackTile2()
    {
        int orinum = 0;
        for (int i = 0; i < m_player.GetHitOriNumList().Count; i++)
        {
            if (orinum < m_player.GetHitOriNumList()[i])
            {
                orinum = m_player.GetHitOriNumList()[i];
            }
        }

        for (int i = orinum; i >= 0; i--)
        {
            if (Mathf.Abs(angle[i]) > 170.0f)
            {
                return true;
            }
        }
        return false;
    }

    //折り目を180度回転(折り目のオブジェクト)
    private void RotationOriLeft(GameObject obj, int num)
    {
        if (rotStart)
        {
            obj.transform.Rotate(0, variation * Time.deltaTime, 0);
            rot += variation * Time.deltaTime;
            if (rot >= rotAngle)
            {
               
                for (int i = 0; i < m_tileObj.Count; i++)
                {
                    SetChildActive(m_tileObj[i], true);
                }
                TileReset();
                m_LeftStart = false;
                rotStart = false;
                m_isInputAny = false;
                if (rotState == ROTSTATE.ROTATE)
                {
                    angle[num] += 180;
                    rotState = ROTSTATE.ROTATED;
                }
                else
                {
                    angle[num] = 0;
                    rotState = ROTSTATE.NEUTRAL;
                }
                obj.transform.localRotation = Quaternion.Euler(0, angle[num], 0);
            }
        }
    }

    private void RotationOriRight(GameObject obj, int num)
    {
        if (rotStart)
        {
            obj.transform.Rotate(0, -variation * Time.deltaTime, 0);
            rot += variation * Time.deltaTime;
            if (rot >= rotAngle)
            {
                for (int i = 0; i < m_tileObj.Count; i++)
                {
                    SetChildActive(m_tileObj[i], true);
                }
                TileReset();
                m_RightStart = false;
                rotStart = false;
                m_isInputAny = false;
                if (rotState == ROTSTATE.ROTATE)
                {
                    angle[num] -= 180;
                    rotState = ROTSTATE.ROTATED;
                }
                else
                {
                    angle[num] = 0;
                    rotState = ROTSTATE.NEUTRAL;

                }
                obj.transform.localRotation = Quaternion.Euler(0, angle[num], 0);
            }
        }
    }

    private void OnBtn()
    {
        if (orinum < 0)
        {
            return;
        }
        //回転角度を初期化する。
        rot = 0f;
        m_OriObj[orinum].transform.localRotation = Quaternion.Euler(0, angle[orinum], 0);
        rotStart = true;
    }

    //タイルのコンポーネント
    private void TileReset()
    {
        //for (int i = 0; i < m_tileObj.Count; i++)
        //{
        //    m_tileObj[i].GetComponent<ScreenShot>().ResetTexture();
        //}
    }

    //タイルの子供を
    private void SetChildActive(GameObject obj,bool flg)
    {
        //obj.transform.Find("Cube").gameObject.SetActive(flg);
        //if (flg)
        //{
        //    Instantiate(particleObject, obj.transform.Find("Cube").transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
        //}
    }
}
