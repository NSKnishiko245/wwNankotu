using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public bool IsHitBar { get; private set; }

    private Vector3 moveValue;

    public float downSpeed = 5.0f;

    //プレイヤーの向きを変える
    private Vector3 prev;
    public bool IsMove { get; private set; }

    public enum PLAYERHITBOX
    {
        RIGHT,
        LEFT,
        BOTTOM,
    }

    public enum MOVEDIR
    {
        NEUTRAL,
        RIGHT,
        LEFT
    }
    public MOVEDIR moveDir { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        moveDir = MOVEDIR.NEUTRAL;

        IsHitGoalBlock = false;
        pos = transform.position;
        rb = GetComponent<Rigidbody>();
        IsHitGoalBlock = false;

        prev = this.transform.position;

        transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().dir = PlayerHitTest.COLLISIONDIRECTION.RIGHT;
        transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().dir = PlayerHitTest.COLLISIONDIRECTION.LEFT;
    }

    // Update is called once per frame
    void Update()
    {
        //キーボード操作
        inputValue_x = Input.GetAxis("Horizontal");

        // 入力をなしにする場合
        if (!inputFlg || Mathf.Abs(rb.velocity.y) > 0.2f) inputValue_x = 0.0f;
        //if (!transform.GetChild((int)PLAYERHITBOX.BOTTOM).gameObject.GetComponent<HitAction>().isHit) inputValue_x = 0.0f;

        if (!IsHitGoalBlock)
        {
            //移動処理
            moveValue = transform.right * Speed * Time.deltaTime;
            if (inputValue_x > 0)
            {
                transform.position += moveValue;
                // transform.rotation = Quaternion.Euler(0, 0, 0);
                IsMove = true;
            }
            else if (inputValue_x < 0)
            {
                transform.position -= moveValue;
                //transform.rotation = Quaternion.Euler(0, 180, 0);
                //m_anim.SetBool("move_flg", true);
                IsMove = true;
            }
            else
            {
                IsMove = false;
            }
        }
        ////ジャンプ処理（Aボタン押下）
        //if(Input.GetKeyDown("joystick button 1"))
        //{
        //    if (Jumpflg)
        //    {
        //        rb.velocity = transform.up * Jump;
        //        Jumpflg = false;
        //    }
        //}
        //if (rb.velocity.magnitude == 0f)
        //{
        //    Jumpflg = true;
        //}
        HitTest();

        //Vector3 diff = this.transform.position - prev;
        //if (diff.magnitude > 0.01f)
        //{
        //    transform.rotation = Quaternion.LookRotation(diff);
        //}
        // Debug.Log(transform.rotation);
        //prev=transform.position;

        if (moveDir == MOVEDIR.RIGHT)
        {
            this.transform.DOMoveX(transform.position.x + 0.5f, 1.0f);
            moveDir = MOVEDIR.NEUTRAL;
        }
        else if (moveDir == MOVEDIR.LEFT)
        {
            this.transform.DOMoveX(transform.position.x - 0.5f, 1.0f);
            moveDir = MOVEDIR.NEUTRAL;
        }
    }

    public void TurnOnMove()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.35f);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        inputFlg = true;
    }
    public void TurnOffMove()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        inputFlg = false;
    }
    public void TurnOnGravity()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void FixPos()
    {

        if (inputValue_x > 0)
        {
            transform.position -= moveValue;
        }
        else if (inputValue_x < 0)
        {
            transform.position += moveValue;
        }
    }

    private void HitTest()
    {
        if (transform.GetChild((int)PLAYERHITBOX.BOTTOM).gameObject.GetComponent<HitAction>().isHit)
        {
            //Debug.Log(inputValue_x);
            //足場に触れている間useGravityを無効
            //this.gameObject.GetComponent<Rigidbody>().useGravity = false;

            if (inputValue_x < 0 && IsClimb())
            {
                if (transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().isHit)
                {
                    transform.position += Vector3.up * transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().HitBlockHeight;
                    transform.position += Vector3.left * 0.3f;
                    transform.GetChild((int)PLAYERHITBOX.RIGHT).gameObject.GetComponent<PlayerHitTest>().ResetHitFlg();
                }
            }
            if (inputValue_x > 0 && IsClimb())
            {
                if (transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().isHit)
                {
                    transform.position += Vector3.up * transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().HitBlockHeight;
                    transform.position += Vector3.right * 0.3f;
                    transform.GetChild((int)PLAYERHITBOX.LEFT).gameObject.GetComponent<PlayerHitTest>().ResetHitFlg();
                }
            }
        }
        else
        {
            //足場に触れていないのでuseGravityを有効
            // this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            //transform.position -= Vector3.up * 0.1f;
        }
        rb.AddForce(Vector3.down * downSpeed);
    }

    public bool IsPut()
    {
        return transform.GetChild((int)PLAYERHITBOX.BOTTOM).gameObject.GetComponent<HitAction>().isHit;
    }

    private bool IsClimb()
    {
        Vector3 ray_pos = transform.position;
        Ray ray = new Ray(ray_pos, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.0f))
        {
            string tag = hit.transform.tag;
            if (tag == "ClimbBlock" || tag == "GimicMoveBlock" || tag == "GimicBreakBlock" || tag == "GimicClearBlock") return false;
            //if (hit.transform.tag == "ClimbBlock") return false;
        }
        return true;
    }

    //移動分
    public float GetInputValue()
    {
        return inputValue_x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "GoalBlock")
        {
            IsHitGoalBlock = true;
            IsMove = false;
        }

        if (other.transform.tag == "Bar")
        {
            IsHitBar = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Bar")
        {
            IsHitBar = false;
        }
    }
}