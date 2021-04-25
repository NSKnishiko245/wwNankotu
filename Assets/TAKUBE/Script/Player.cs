using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("プレイヤーの移動速度")]
    public float Speed;
    [Header("プレイヤーのジャンプ力")]
    public float Jump;

    private Rigidbody rb;
    private Vector3 pos;
    private bool Jumpflg;

    private bool inputFlg = true;
    private float inputValue_x = 0.0f;

    public float BorderLine_l;
    public float BorderLine_r;

    public bool IsHitGoalBlock { get; private set; }

    private enum PLAYERHITBOX
    {
        RIGHT,
        LEFT,
        BOTTOM,
    }

    
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        rb = GetComponent<Rigidbody>();
        IsHitGoalBlock = false;
    }

    // Update is called once per frame
    void Update()
    {
        //キーボード操作
        inputValue_x = Input.GetAxis("Horizontal");

        // 入力をなしにする場合
        if (!inputFlg || Mathf.Abs(rb.velocity.y) > 0.02f) inputValue_x = 0.0f;

        //移動処理
        Vector3 moveValue = transform.right * Speed * Time.deltaTime;
        if (inputValue_x > 0)
        {
            //if (transform.position.x + transform.localScale.x / 2.0f < BorderLine_r)
            {
                transform.position += moveValue;
            }
        }
        else if (inputValue_x < 0)
        {
            //if (transform.position.x - transform.localScale.x / 2.0f > BorderLine_l)
            {
                transform.position -= moveValue;
            }
        }
        
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
        HitTest();
    }
    
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

    private void HitTest()
    {
        if (transform.GetChild((int)PLAYERHITBOX.BOTTOM).gameObject.GetComponent<HitAction>().isHit)
        {
            if (inputValue_x < 0 && IsClimb())
            {
                if (transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().isHit)
                {
                    transform.position += Vector3.up * transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().HitBlockHeight;
                    transform.position += Vector3.left * 0.2f;
                    transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().ResetHitFlg();
                }
            }
            if (inputValue_x > 0 && IsClimb())
            {
                if (transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().isHit)
                {
                    transform.position += Vector3.up * transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().HitBlockHeight;
                    transform.position += Vector3.right * 0.2f;
                    transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().ResetHitFlg();
                }
            }
        }
    }

    private bool IsClimb()
    {
        Vector3 ray_pos = transform.position;
        Ray ray = new Ray(ray_pos, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.0f))
        {
            if (hit.transform.tag == "Block") return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "GoalBlock")
        {
            IsHitGoalBlock = true;
        }
    }
}