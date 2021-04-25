//プレイヤーが金色のギアをふれたら消す

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGear : MonoBehaviour
{
    [SerializeField] private GameObject stageUIManager;
    private int stageNum;

    public ParticleSystem m_particle;//取得時のエフェクト

    private Animator m_anim;   //取得時のアニメーション
    
    private bool m_getflg = false;  //プレイヤーとふれたら
    private bool m_animend = false;
    private Vector3 _startPos;
    public float speed=1.0f;

    //二点間の距離を入れる
    private float distance_two;

    private GameObject m_objPlayer;//プレイヤーオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        m_anim = this.gameObject.GetComponent<Animator>();
        m_objPlayer = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //if (m_getflg)   //プレイヤーとふれたら
        //{
        //    this.transform.position = m_objPlayer.transform.position;
        //}
        if (m_animend)
        {

            distance_two = Vector3.Distance(_startPos, m_objPlayer.transform.position);

           float present_Locatoin = (Time.time * speed) / distance_two;
            //transform.position = Vector3.Lerp(_startPos, _targetPos, CalcMoveRatio());
           this.transform.position = Vector3.Lerp(transform.position, m_objPlayer.transform.position, present_Locatoin);
            Debug.Log(m_objPlayer.transform.position);
        }
    }

    //float CalcMoveRatio()
    //{
    //    var distCovered = (Time.time - _startTime) * Speed;
    //    return distCovered / JourneyLength;
    //}

    public void AnimEnd()
    {
        m_animend = true;
        _startPos = this.transform.position;
    }

    private void DestroyGear()
    {
        // ここでスコア加算
        stageNum = stageUIManager.GetComponent<StageUIManager>().GetStageNum();
        StageSelectManager.score[stageNum].isGold = true;


        Instantiate(m_particle, this.transform.position, Quaternion.identity); //消滅時にエフェクトを使用する
        Destroy(this.gameObject);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            m_getflg = true;
            m_anim.SetBool("GetFlg",m_getflg);
            if (m_animend)
            {
                DestroyGear();
            }
        }
    }
}
