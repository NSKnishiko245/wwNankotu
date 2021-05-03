using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialManagaer : MonoBehaviour
{
    [SerializeField] private Player Player;
    [SerializeField] private BarRotate BarRotate;

    private int Tutorialnum = 1;
    private int Animnum = 1;

    //カウントダウン
    [SerializeField] public float movecountdown = 5.0f;
    [SerializeField] public float Oricountdown = 1.0f;

    [SerializeField] private GameObject Lstick;
    [SerializeField] private GameObject Rstick;
    [SerializeField] private GameObject Controller;

    Animator LAnim;
    Animator RAnim;
    Animator ControllerAnim;

    // Start is called before the first frame update
    void Start()
    {
        LAnim = Lstick.GetComponent<Animator>();
        RAnim = Rstick.GetComponent<Animator>();
        ControllerAnim = Controller.GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {
        //時間をカウントダウンする

        Controller.GetComponent<RectTransform>().SetAsLastSibling();


        float input_x = Input.GetAxis("Horizontal");

        //switch(Tutorialnum)
        //{
        //    case 1:
        //        MoveCheck();
        //        Debug.Log("移動");
        //        break;

        //    case 2:
        //        OriCheck();
        //        Debug.Log("折りたたみ");
        //        break;

        //    case 3:
        //        ReturnOri();
        //        Debug.Log("折戻し");
        //        break;
        //    default:
        //        Animnum = 4;
        //        Debug.Log("説明終了");
        //        break;
        //}

        //LAnim.SetInteger("tutorialNum", Animnum);
        //RAnim.SetInteger("RtutorialNum", Animnum);


        if(input_x==0)
        {
            movecountdown -= Time.deltaTime;
            if (movecountdown <= 0.0f)
            {
                Debug.Log("移動UI表示");
                ControllerAnim.SetBool("FukidasiFlg", true);
                LAnim.SetInteger("tutorialNum", Animnum);
                RAnim.SetInteger("RtutorialNum", Animnum);
                movecountdown = 5.0f;
            }
        }
        else
        {
            movecountdown = 5.0f;
            ControllerAnim.SetBool("FukidasiFlg", false);
        }

        if (Player.GetComponent<Player>().IsHitBar)
        {
            Oricountdown -= Time.deltaTime;
            if (Oricountdown <= 0)
            {
                Animnum = 2;
                ControllerAnim.SetBool("FukidasiFlg", true);
                Debug.Log("UI表示");
                LAnim.SetInteger("tutorialNum", Animnum);
                RAnim.SetInteger("RtutorialNum", Animnum);
                Oricountdown = 1.0f;
            }
        }
        else
        {
            Oricountdown = 1.0f;
        }
    }


    //private bool MoveCheck()
    //{
    //    //キーボード操作
    //    float inputValue_x = Input.GetAxis("Horizontal");

        

    //    if (Player.GetComponent<Player>().IsHitPoint)
    //    {
    //        Tutorialnum = 2;
    //        Animnum = 2;
           
    //        return true;
    //    }

    //    return false;
    //}

    private bool OriCheck()
    {
        // 右スティックからの入力情報を取得
        float R_Stick_Value = Input.GetAxis("Horizontal2");

        if (0 < R_Stick_Value || 0 > R_Stick_Value)
        {
            Tutorialnum = 3;
            Animnum = 3;
            return true;
        }
        return false;
    }

    private bool ReturnOri()
    {
        if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
        {
            Tutorialnum = 4;
            return true;
        }
        return false;
    }
}
