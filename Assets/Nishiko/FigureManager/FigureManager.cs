using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//バラバラのパーツをひとつに戻す

public class FigureManager : MonoBehaviour
{
    public GameObject[] m_objPoint;

    public GameObject[] m_objModelparts;    //モデルのパーツの総数

    //初期位置
    private Vector3 m_firstPos;

    //スタート位置
    private Vector3[] m_startPos=new Vector3[5];


   

    private bool[] m_isStageClear = new bool[6]; 

    private bool[] m_isSet = new bool[5];   //パーツが元の位置にいるか？ 
    private int[] m_WaitTime = new int[5]; //待ち時間

    private bool[] m_isMove = new bool[5];  //移動中？
    private bool[] m_isFinish = new bool[5];

    private float[]distance_two=new float[5];
    private float[] present_Location = new float[5];

    int[] cnt=new int[5];


    //
    public int Speed = 1;

    [Header("移動時のパーティクル")]
    public ParticleSystem m_objKirakiraMove;

    [Header("完成した時に出すパーティクル")]
    public ParticleSystem m_PerfectParticle;
    private int m_cnt = 0;


    //銅を取ったらパーツまで運ぶ(最大5個まで運ぶ)
    //全ステージ分銅
    public GameObject m_objCopper;
    private bool[] m_isCopper = new bool[37];
    private bool[] m_isCopperFinish = new bool[37];
    private bool[] m_isCopperMove = new bool[37];

    private float[] distance_Copper = new float[37];　//初期位置からパーツまで
    private Vector3[] m_CoppernowPos = new Vector3[37];//現在位置
    private Vector3[] m_CopperTarGetPos=new Vector3[37];//目標位置
    private float[] Copper_Location = new float[37];
    private int[] CopperCnt = new int[37];
    private int[] m_CopperWait = new int[37];
    //private bool[] m_isCopperMove = new bool[5];
    //public GameObject m_objCopper; 
    //private bool m_isCopperMove;
    //private Vector3[] m_isTargetPos = new Vector3[5];

    //private bool[] m_isCopper = new bool[36];
    //private bool[] m_isCopperFinish = new bool[36];

    //private float[] distance_Copper = new float[5];


    // Start is called before the first frame update
    void Start()
    {
        //foreach (var obj in m_objModelparts)
        //{
        //    obj.transform.position = m_objPoint[0].transform.position;
        //}
       

        //シーンを変えても残り続ける様に
        DontDestroyOnLoad(gameObject);

        //初期位置を記憶させる
        m_firstPos = gameObject.transform.position;

        for (int i = 0; i < distance_two.Length; i++)
        {
            m_WaitTime[i] = 0;
            cnt[i] = 0;
            m_startPos[i] = m_objModelparts[i].transform.position;  //スタート位置保存
            distance_two[i] = Vector3.Distance(m_startPos[i], m_objPoint[i].transform.position);
        }

        //２点の距離を求める。
        for (int i = 1; i < 36; i++)
        {
            CopperCnt[i] = 0;
            m_CopperWait[i] = 0;
            m_CoppernowPos[i] = m_objCopper.transform.position;
            distance_Copper[i] = Vector3.Distance(m_objCopper.transform.position, m_objModelparts[0].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //銅を更新
        CopperMove();

        //難易度６をクリアしたらキラキラパーティクル
        if (m_isStageClear[5])
        {
            if (m_cnt%10==0)
            {
                Instantiate(m_PerfectParticle, this.transform.position, Quaternion.identity);//きらきらエフェクト
            }
            m_cnt++;
        }

        if (SceneManager.GetActiveScene().name == "Stage1Scene")
        {
            for(int i = 0; i < m_objModelparts.Length; i++)
            {
                m_objModelparts[i].SetActive(false);
            }
           // this.gameObject.transform.parent = GameObject.Find("Main Camera").transform;
            //if(GameObject.Find("Player").GetComponent<Player>().IsHitGoalBlock)
            //{
            //    this.gameObject.transform.parent = null;
            //    //移動した分を元に戻す
            //    this.gameObject.transform.position = m_firstPos;
            //}
        }
        else
        {
            for (int i = 0; i < m_objModelparts.Length; i++)
            {
                m_objModelparts[i].SetActive(true);
            }
        }
       

        for (int i = 0; i < 6; i++)
        {
            if (StageClearManager.StageClear[i])
            {
                m_isStageClear[i] = true;
            }
        }


        //if ((Input.GetAxis("LTrigger") > 0 && Input.GetAxis("RTrigger") > 0))
        //{
        //    m_isStageClear[0] = true;
        //    m_isStageClear[1] = true;
        //    m_isStageClear[2] = true;
        //    m_isStageClear[3] = true;
        //    m_isStageClear[4] = true;
        //    m_isStageClear[5] = true;
        //}

        //if (Input.GetKeyDown(KeyCode.F1)) m_isStageClear[0] = true;
        //if (Input.GetKeyDown(KeyCode.F2)) m_isStageClear[1] = true;
        //if (Input.GetKeyDown(KeyCode.F3)) m_isStageClear[2] = true;
        //if (Input.GetKeyDown(KeyCode.F4)) m_isStageClear[3] = true;
        //if (Input.GetKeyDown(KeyCode.F5)) m_isStageClear[4] = true;
        //if (Input.GetKeyDown(KeyCode.F6)) m_isStageClear[5] = true;

        //本ごとに全ての銅メダルを集めたか？
        for (int i = 0; i < m_isStageClear.Length-1; i++)
        {
            if (m_isStageClear[i])//銅を全て集めたら
            {
                if (!m_isFinish[i])//元の位置に戻っていなければ
                {
                    m_isSet[i] = true;
                    //m_isMove[i] = true;
                }
                if (m_isSet[i])
                {
                    //Instantiate(m_objKirakiraMove, m_objModelparts[i].transform.position, Quaternion.identity);//きらきらエフェクト
                    if (m_WaitTime[i] >= 240)
                    {
                        m_isMove[i] = true;
                    }
                    m_WaitTime[i]++;
                }
            }

        }




        //移動
        for(int i = 0; i < m_isMove.Length; i++)
        {
            if (m_isMove[i])
            {
                if (!m_isFinish[i])
                {
                    //現在の位置
                    present_Location[i] = (cnt[i]/60.0f) / distance_two[i];
                    cnt[i]+=Speed;
                    m_objModelparts[i].transform.position = Vector3.Lerp(m_startPos[i], m_objPoint[i].transform.position, present_Location[i]);
                    Instantiate(m_objKirakiraMove, m_objModelparts[i].transform.position, Quaternion.identity);//きらきらエフェクト
                    m_objModelparts[i].SetActive(false);
                    if (Vector3.Distance(m_objModelparts[i].transform.position, m_objPoint[i].transform.position) <= 0.1f)
                    {
                        //角度を元に戻す
                        //Quaternion rot = new Quaternion(66, 0, 0, 1);
                        m_objModelparts[i].transform.rotation = transform.rotation;
                        m_objModelparts[i].SetActive(true);
                        m_isSet[i] = false;
                        m_isFinish[i] = true;

                    }
                }
            }
        }

    }

    //ステージごとに銅を取ったかチェック
    void CopperMove()
    {
       // Debug.Log(m_isCopper[1]);
        //本で銅を取ったか更新する
        for (int cnt = 1; cnt < 36; cnt++)
        {
            if (cnt == 6) continue;
            if (cnt == 12) continue;
            if (cnt == 18) continue;
            if (cnt == 24) continue;
            if (cnt == 30) continue;
            if (cnt == 36) continue;

            if (StageClearManager.m_isGetCopper[cnt])
            {
                m_isCopper[cnt] = StageClearManager.m_isGetCopper[cnt];
            }
        }

        //銅をパーツまで移動させたか？
        for(int i = 1; i <= 36; i++)
        {
            if (m_isCopper[i])
            {
                if (!m_isCopperFinish[i])//移動が終わっていなければ
                {
                    //目標位置を決める
                    if (i >= 1 && i < 6)//ステージ１～５
                    {
                        m_CopperTarGetPos[i] = m_objModelparts[0].transform.position;
                    }
                    else if (i >= 7 && i < 12)//ステージ１～５
                    {
                        m_CopperTarGetPos[i] = m_objModelparts[1].transform.position;
                    }
                    else if (i >= 13 && i < 18)//ステージ１～５
                    {
                        m_CopperTarGetPos[i] = m_objModelparts[2].transform.position;
                    }
                    else if (i >= 19 && i < 24)//ステージ１～５
                    {
                        m_CopperTarGetPos[i] = m_objModelparts[3].transform.position;
                    }
                    else if (i >= 25 && i < 30)//ステージ１～５
                    {
                        m_CopperTarGetPos[i] = m_objModelparts[4].transform.position;
                    }
                    else if (i >= 31 && i < 36)//ステージ１～５
                    {
                        m_CopperTarGetPos[i] = this.transform.position;
                    }
                    //後で追加

                    if (m_CopperWait[i] >= (60 + ((i % 6) * 20)))
                    {
                        m_isCopperMove[i] = true;//移動中にする
                    }
                    m_CopperWait[i]++;
                }
            }
        }

        //銅を移動させる
        for (int i = 1; i <= 36; i++)
        {
            if (m_isCopperMove[i])//移動中なら
            {
                Copper_Location[i]= (CopperCnt[i] / 60.0f) / distance_Copper[i];
                CopperCnt[i] += Speed;
                m_CoppernowPos[i] = Vector3.Lerp(m_objCopper.transform.position, m_CopperTarGetPos[i], Copper_Location[i]);
                Instantiate(m_objKirakiraMove, m_CoppernowPos[i], Quaternion.identity);
                if (Vector3.Distance(m_CoppernowPos[i], m_CopperTarGetPos[i]) <= 0.1f)
                {
                    m_isCopperMove[i] = false;
                    m_isCopperFinish[i] = true;
                }
            }
        }















        //for(int cnt = 0; cnt < 36; cnt++)
        //{
        //    if (StageSelectManager.score[cnt].isCopper)
        //    {
        //        m_isCopper[cnt] = StageSelectManager.score[cnt].isCopper;
        //    }
        //}

            //for(int i = 0; i < 36; i++)
            //{
            //    if (m_isCopper[i])//そのステージで銅を取ったか？
            //    {
            //        if (!m_isCopperFinish[i])//移動し終わっているか？
            //        {
            //            if (!m_isCopperMove)//ムーブ状態ではなければ
            //            {
            //                //ムーブ状態に
            //                m_isCopperMove = true;
            //                //目的地を設定
            //                if (i >= 1 && i < 6)//ステージ１～５
            //                {
            //                    m_isTargetPos[0] = m_objModelparts[0].transform.position;
            //                }
            //            }

            //                //まだなら移動処理スタート
            //                //for(int cnt = 0; cnt < 5; cnt++)
            //                //{
            //                //    if (!m_isCopperMove[cnt])
            //                //    {
            //                //        m_isCopperMove[cnt] = true;
            //                //        break;
            //                //    }
            //                //}

            //            if (m_isCopperMove)
            //            {
            //                ////目的地を設定
            //                //if (i >= 1 && i < 6)//ステージ１～５
            //                //{
            //                //    m_isTargetPos[0] = m_objModelparts[0].transform.position;
            //                //}



            //            }
            //        }
            //    }
            //}
    }

    public void FigurePositionInit(int stageClearNum)
    {
        if (stageClearNum < 5)
        {
            m_objModelparts[stageClearNum].transform.position = m_objPoint[stageClearNum].transform.position;
            m_objModelparts[stageClearNum].transform.rotation = m_objPoint[stageClearNum].transform.rotation;
            m_isFinish[stageClearNum] = m_isCopperFinish[stageClearNum] = true;
        }
        m_isStageClear[stageClearNum] = true;
    }
}
