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


   

    private bool[] m_isStageClear = new bool[5]; 

    private bool[] m_isSet = new bool[5];   //パーツが元の位置にいるか？ 
    private bool[] m_isMove = new bool[5];  //移動中？
    private bool[] m_isFinish = new bool[5];

    private float[]distance_two=new float[5];
    private float[] present_Location = new float[5];

    int[] cnt=new int[5];


    //
    public int Speed = 1;

    public ParticleSystem m_objKirakiraMove;

    // Start is called before the first frame update
    void Start()
    {
        //シーンを変えても残り続ける様に
        DontDestroyOnLoad(gameObject);

        //初期位置を記憶させる
        m_firstPos = gameObject.transform.position;

        for (int i = 0; i < distance_two.Length; i++)
        {
            cnt[i] = 0;
            m_startPos[i] = m_objModelparts[i].transform.position;  //スタート位置保存
            distance_two[i] = Vector3.Distance(m_startPos[i], m_objPoint[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
       

        if (Input.GetKeyDown(KeyCode.F1)) m_isStageClear[0] = true;
        for (int i = 0; i < 5; i++)
        {
            if (StageClearManager.StageClear[i])
            {
                m_isStageClear[i] = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F2)) m_isStageClear[1] = true;
        if (Input.GetKeyDown(KeyCode.F3)) m_isStageClear[2] = true;
        if (Input.GetKeyDown(KeyCode.F4)) m_isStageClear[3] = true;

        //本ごとに全ての銅メダルを集めたか？
        for (int i = 0; i < m_isStageClear.Length; i++)
        {
            if (m_isStageClear[i])//銅を全て集めたら
            {
                if (!m_isFinish[i])//元の位置に戻っていなければ
                {
                    m_isSet[i] = true;
                    m_isMove[i] = true;
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
                        m_isFinish[i] = true;
                    }
                }
            }
        }

    }

    
}
