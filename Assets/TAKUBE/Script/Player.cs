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

    private Rigidbody rb;
    private Vector3 pos;
    private bool Jumpflg;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //キーボード操作
        float x = Input.GetAxis("Horizontal");
        //移動処理
        if (x>0)
        {
            transform.position += transform.right * Speed * Time.deltaTime;
        }
        else if (x<0)
        {
            transform.position -= transform.right * Speed * Time.deltaTime;
        }

        //ジャンプ処理
        if (Jumpflg)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = transform.up * Jump;
                Jumpflg = false;
            }
        }
        if (rb.velocity.magnitude == 0f)
        {
            Jumpflg = true;
        }


        //コントローラー操作
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        if(lsh>1)
        {
            transform.position += transform.right * Speed * Time.deltaTime;
        }
        else if(lsh<-1)
        {
            transform.position -= transform.right * Speed * Time.deltaTime;
        }

        //ジャンプ処理（Aボタン押下）
        if(Input.GetKeyDown("joystick button 0"))
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

        //LT・RTトリガー押下確認用
        float tri = Input.GetAxis("L_R_Trigger");
        if (tri > 0)
        {
           // Debug.Log("L trigger:" + tri);
        }
        else if (tri < 0)
        {
            //Debug.Log("R trigger:" + tri);
        }
        else
        {
            //Debug.Log(" trigger:none");
        }

        //フィールドを元に戻す（Xボタン押下）変更する可能性大
        if (Input.GetKeyDown("joystick button 2"))
        {
            //Debug.Log("X Button:on");
        }
        else
        {
            //Debug.Log("X Button:none");
        }
        //Debug.Log(m_hitOriNum);
    }

    //触れている折り目のオブジェクトの番号をセット
    public void SetHitNum(int num)
    {
        m_hitOriNum = num;
    }
   
    public int GetHitOriobjNum()
    {
        return m_hitOriNum;
    }
}
