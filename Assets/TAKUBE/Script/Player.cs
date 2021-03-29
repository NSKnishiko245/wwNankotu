using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("プレイヤーの移動速度")]
    public float Speed;
    [Header("プレイヤーのジャンプ力")]
    public float Jump;

    private int m_hitOriNum = 0;
    private List<int> m_HitOriNumList = new List<int>();

    private Rigidbody rb;
    private Vector3 pos;
    private bool Jumpflg;

    private bool inputFlg = true;

    public float BorderLine_l;
    public float BorderLine_r;

    // バーとの接触時の補正動作
    //public enum AUTOMOVE
    //{
    //    NEUTRAL,
    //    MOVE_LEFT,
    //    MOVE_RIGHT
    //}
    //public AUTOMOVE AutoMove { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        //AutoMove = AUTOMOVE.NEUTRAL;
        pos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (AutoMove != AUTOMOVE.NEUTRAL)
        //{
        //    AutoMovePlayer();
        //}

        //キーボード操作
        float x = Input.GetAxis("Horizontal");
        if (!inputFlg) x = 0.0f;
        //移動処理
        Vector3 moveValue = transform.right * Speed * Time.deltaTime;
        if (x>0)
        {
            transform.position += moveValue;
            if (transform.position.x + transform.localScale.x / 2.0f >= BorderLine_r)
            {
                transform.position -= moveValue;
            }
        }
        else if (x<0)
        {
            transform.position -= moveValue;
            if (transform.position.x - transform.localScale.x / 2.0f <= BorderLine_l)
            {
                transform.position += moveValue;
            }
        }
        

        //コントローラー操作
        //float lsh = Input.GetAxis("L_Stick_H");
        //float lsv = Input.GetAxis("L_Stick_V");

        //if (!inputFlg)
        //{
        //    lsh = lsv = 0.0f;
        //}

        //if(lsh>1)
        //{
        //    transform.position += transform.right * Speed * Time.deltaTime;
        //}
        //else if(lsh<-1)
        //{
        //    transform.position -= transform.right * Speed * Time.deltaTime;
        //}

        //ジャンプ処理（Aボタン押下）
        if(Input.GetKeyDown("joystick button 1"))
        {
            if (Jumpflg)
            {
                rb.velocity = transform.up * Jump;
                Jumpflg = false;
            }
        }
        if (rb.velocity.magnitude == 0f)
        {
            Jumpflg = true;
        }
        
    }

    //触れている折り目のオブジェクトの番号をセット
    public void SetHitNum(int num)
    {
        m_hitOriNum = num;
        //Debug.Log(num);

    }
    public void test(int num)
    {
        m_HitOriNumList.Add(num);
    }

    public void ResetHitOriNumList()
    {
        m_HitOriNumList.Clear();
    }


    public int GetHitOriobjNum()
    {
        return m_hitOriNum;
    }
    public List<int> GetHitOriNumList()
    {
        return m_HitOriNumList;
    }


    // バーを回転させる時のプレイヤーをバーから離していく処理
    //public void AutoMovePlayer()
    //{
    //    Vector3 distance = Vector3.zero;
    //    if (AutoMove == AUTOMOVE.MOVE_LEFT)     distance = Vector3.right;
    //    if (AutoMove == AUTOMOVE.MOVE_RIGHT)    distance = Vector3.left;

    //    Ray ray = new Ray(transform.position, distance);
    //    if (Physics.Raycast(ray, out RaycastHit hit, transform.localScale.x / 2.0f))
    //    {
    //        if (hit.collider.CompareTag("Bar"))
    //        {
    //            Debug.Log("move");
    //            transform.Translate(distance);
    //        }
    //        else
    //        {
    //            AutoMove = AUTOMOVE.NEUTRAL;
    //        }
    //    }
    //}


    public void TurnOnMove()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        inputFlg = true;
    }
    public void TurnOffMove()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        inputFlg = false;
    }
}
