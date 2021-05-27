using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialManagaer : MonoBehaviour
{
    [SerializeField] private Player Player;
    [SerializeField] private GameObject Bar;
    [SerializeField] private StageManager stagemanager;

    public Text Text;

    private int Animnum = 1;
    private int TutorialNum = 1;
    private int RotateNum = 1;
    private int BordNum;
    private int Stagenum;   // ステージ番号

    

    private GameObject Point_prefab;
    private GameObject Point;
    private GameObject mainCam;

    private bool PushFlg;
    private bool textFlg;
    //カウントダウン
    [SerializeField] public float Pointcountdown = 5.0f;
    [SerializeField] public float Oricountdown = 1.0f;
    [SerializeField] public float TextCount = 5.0f;
    [SerializeField] public float Pushcount;
    private bool CountFlg;

    [SerializeField] private GameObject Lstick;
    [SerializeField] private GameObject Rstick;
    [SerializeField] private GameObject Controller;
    [SerializeField] private GameObject Mobiusface;
    [SerializeField] private GameObject MobiusBody;
    [SerializeField] private GameObject Bord;
    [SerializeField] private GameObject Black;

    Animator LAnim;
    Animator RAnim;
    Animator ControllerAnim;
    Animator MobiusfaceAnim;
    Animator MobiusBodyAnim;
    Animator BordAnim;
    Animator BlackAnim;

    public bool IsPlayerMove { get; private set; }

    public bool IsPlayerLMove { get; private set; }

    public bool IsRotateMove { get; private set; }

    public bool IsLMove { get; private set; }

    public bool IsRMove { get; private set; }

    public bool IsPoint { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        BordNum = 5;
        LAnim = Lstick.GetComponent<Animator>();
        RAnim = Rstick.GetComponent<Animator>();
        ControllerAnim = Controller.GetComponent<Animator>();
        MobiusfaceAnim = Mobiusface.GetComponent<Animator>();
        MobiusBodyAnim = MobiusBody.GetComponent<Animator>();
        BordAnim = Bord.GetComponent<Animator>();
        BlackAnim = Black.GetComponent<Animator>();

        //stagemanager = GameObject.Find("stageManager");
        IsPlayerMove = true;
        IsPlayerLMove = true;
        IsRotateMove = true;
        IsLMove = true;
        IsRMove = true;
        IsPoint = false;
        CountFlg = true;
        PushFlg = false;
        textFlg = false;
        mainCam = Camera.main.gameObject;
        Stagenum = StageManager.stageNum;

        if (Stagenum != 1)
        {
            TutorialNum = 7;
            BordNum = 2;
            BordAnim.SetInteger("text", BordNum);
        }
        else
        {
            TutorialNum = 1;
        }
        Point_prefab = Resources.Load<GameObject>("Point");


    }

    // Update is called once per frame
    void Update()
    {
        //ステージ開始時のカメラワーク中は、操作しない
        if (mainCam.GetComponent<StartCamera>().isMoving)
        {
            textFlg = true;
            return;
        }

        //時間をカウントダウンする
        Controller.GetComponent<RectTransform>().SetAsLastSibling();

        // 左スティックからの入力情報を取得
        float input_x = Input.GetAxis("Horizontal");

        // 右スティックからの入力情報を取得
        float R_Stick_value = Input.GetAxis("tutorialStick");

        switch (TutorialNum)
        {
            case 1:
                IsRotateMove = false;
                IsLMove = false;
                IsRMove = false;
                Pointcountdown -= Time.deltaTime;
                TextCount -= Time.deltaTime;
                if (textFlg)
                {
                    BlackAnim.SetBool("Black", true);
                    BordNum = 3;
                    BordAnim.SetInteger("text", BordNum);
                   
                }
                if (TextCount <= 0.0f)
                {
                    Text.text = "まずはこの世界の進め方の説明をするビウスよ！";
                    TextCount = 1000.0f;
                }
                else
                {
                    IsPlayerMove = false;
                }
                if(Pointcountdown <= 0.0f)
                {
                    IsPlayerMove = true;
                    if (CountFlg)
                    {
                        CountFlg = false;
                        Point = Instantiate(Point_prefab, new Vector3(1.5f, -2.5f, 0.0f), Quaternion.identity);
                        Text.text= "まずは矢印に向かって歩いてみるビウス！";
                        MobiusBodyAnim.SetBool("Right", true);
                        MobiusfaceAnim.SetBool("Smile", true);
                        ControllerAnim.SetBool("FukidasiFlg", true);
                        LAnim.SetBool("LStick", true);
                        RAnim.SetBool("RStickLMove", false);
                    }
                }

                if (Player.GetComponent<Player>().IsHitPoint)
                {
                    Text.text = "高い段差に阻まれて進むことが出来ないビウス";
                    ControllerAnim.SetBool("FukidasiFlg", false);
                    LAnim.SetBool("LStick", false);
                    IsPoint = true;
                    TutorialNum = 2;
                    TextCount = 2.0f;
                    Destroy(Point);
                }
                break;
            case 2:
                IsPoint = false;
                if (RotateNum == 1)
                {
                    TextCount -= Time.deltaTime;
                    if (TextCount <= 0.0f)
                    {
                        //BordNum = 1;
                        //BordAnim.SetInteger("text", BordNum);
                        TextCount = 1000.0f;
                        CountFlg = true;
                    }
                    IsPlayerMove = true;

                    if (CountFlg)
                    {
                        CountFlg = false;
                        Point = Instantiate(Point_prefab, new Vector3(0.17f, -2.5f, 0.0f), Quaternion.identity);

                    }

                    if (Player.GetComponent<Player>().IsHitBar)
                    {
                        BordNum = 0;
                        BordAnim.SetInteger("text", BordNum);
                        Text.text = "フィールドを折って世界を変えるビウス！！";
                        IsPlayerMove = false;
                        IsPoint = true;
                        Destroy(Point);
                        RotateNum = 2;
                        TextCount = 10.0f;
                    }
                    else
                    {
                        
                    }
                }
                if (RotateNum == 2)
                {
                    IsLMove = false;
                    IsRMove = true;
                    ControllerAnim.SetBool("FukidasiFlg", true);
                    LAnim.SetBool("LStick", false);
                    RAnim.SetBool("RStickRMove", true);
                    if (stagemanager.GetComponent<StageManager>().IsRotate)
                    {
                        Text.text = "フィールドを上手く折れたでビウス！\n裏表の無い世界に突入ビウス！";
                        Pointcountdown = 1.5f;
                        IsRMove = false;
                        RotateNum = 3;
                        MobiusBodyAnim.SetBool("Both", true);
                        RAnim.SetBool("RStickRMove", false);
                        ControllerAnim.SetBool("FukidasiFlg", false);
                    }

                }
                if (RotateNum == 3)
                {
                    Debug.Log(RotateNum);
                    IsPlayerMove = true;
                    Pushcount -= Time.deltaTime;
                    if (Pushcount <= 0.0f)
                    {
                        IsRotateMove = true;
                        PushFlg = true;
                        //ControllerAnim.SetBool("FukidasiFlg", true);
                        //LAnim.SetBool("LStick", false);
                        //RAnim.SetBool("RStickPush", true);
                        CountFlg = true;
                        TutorialNum = 3;
                        Pushcount = 9999.0f;
                       
                    }

                    //LAnim.SetBool("LStick", false);
                    //RAnim.SetBool("RStickPush", true);
                //    if (PushFlg)
                //    {
                //        if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
                //        {
                //            PushFlg = false;
                //            BordNum = 1;
                //            BordAnim.SetInteger("text", BordNum);
                //            Point = Instantiate(Point_prefab, new Vector3(0.17f, -2.5f, 0.0f), Quaternion.identity);
                //            ControllerAnim.SetBool("FukidasiFlg", false);
                //            RAnim.SetBool("RStickPush", false);
                //        }
                //    }

                //    if (Player.GetComponent<Player>().IsHitPoint)
                //    {
                //        IsPlayerMove = false;
                //        IsPoint = true;
                //        Destroy(Point);
                //        RotateNum = 4;
                //    }
                //}

                //if (RotateNum == 4)
                //{
                //    Debug.Log(RotateNum);

                //    IsPlayerMove = false;
                //    IsRMove = true;
                //    ControllerAnim.SetBool("FukidasiFlg", true);
                //    LAnim.SetBool("LStick", false);
                //    RAnim.SetBool("RStickPush", false);
                //    RAnim.SetBool("RStickRMove", true);
                //    if (stagemanager.GetComponent<StageManager>().IsRotate)
                //    {
                //        IsRotateMove = false;
                //        CountFlg = true;
                //        TutorialNum = 3;
                //        ControllerAnim.SetBool("FukidasiFlg", false);
                //        LAnim.SetBool("LStick", false);
                //        RAnim.SetBool("RStickRMove", false);
                //    }

                }
                break;
            case 3:
                Debug.Log(Player.GetComponent<Player>().IsHitPoint);
                IsPoint = false;
                IsPlayerMove = true;
                //LAnim.SetBool("LStick", false);
                //RAnim.SetBool("RStickPush", true);

                if (CountFlg)
                {

                    CountFlg = false;

                    Point = Instantiate(Point_prefab, new Vector3(4.0f, -2.5f, 0.0f), Quaternion.identity);

                }


                {
                    if (Player.GetComponent<Player>().IsHitPoint)
                    {
                        Text.text = "裏表の無い世界はいつでも解除が可能ビウス！\nスティック押し込んでみるビウス";
                        IsRotateMove = true;
                        IsPoint = true;
                        IsPlayerMove = false;
                        PushFlg = true;
                        BordNum = 0;
                        BordAnim.SetInteger("text", BordNum);
                        ControllerAnim.SetBool("FukidasiFlg", true);
                        RAnim.SetBool("RStickPush", true);
                        Destroy(Point);
                    }
                }

                if (PushFlg)
                {
                    if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
                    {
                        IsPlayerMove = true;
                        PushFlg = false;
                        TutorialNum = 4;
                        CountFlg = true;
                        ControllerAnim.SetBool("FukidasiFlg", false);
                    }
                }
                break;

            case 4:
                
                IsRotateMove = true;
                IsLMove = true;
                IsRMove = true;
                IsPoint = false;
                IsPlayerMove = true;
                if (CountFlg)
                {
                    CountFlg = false;
                    Point = Instantiate(Point_prefab, new Vector3(5.5f, -2.7f, 0.0f), Quaternion.identity);
                }

                if (Player.GetComponent<Player>().IsHitPoint)
                {
                    BordNum = 0;
                    BordAnim.SetInteger("text", BordNum);
                    Text.text = "この世界は横に上下反転世界が\n無限に続いているビウス！";
                    IsPlayerMove = false;
                    IsPoint = true;
                    TutorialNum = 5;
                    TextCount = 5.0f;
                    Destroy(Point);
                }
                break;

            case 5:
                TextCount -= Time.deltaTime;
                if (TextCount <= 0.0f)
                {
                    Text.text = "蜃気楼の中に実際に入って\n目の前にあるゴールに向かって進むビウス！";
                    IsPlayerMove = true;
                    TextCount = 1000.0f;
                    
                }
                break;
            default:

                break;


        }


    }



    private bool OriCheck()
    {
        // 右スティックからの入力情報を取得
        float R_Stick_Value = Input.GetAxis("Horizontal2");

        if (0 < R_Stick_Value || 0 > R_Stick_Value)
        {

            Animnum = 3;
            return true;
        }
        return false;
    }

    private bool ReturnOri()
    {
        if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
        {

            return true;
        }
        return false;
    }
}
