using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialManagaer : MonoBehaviour
{
    [SerializeField] private Player Player;
    [SerializeField] private GameObject Bar;
    [SerializeField] private StageManager stagemanager;

    private int Animnum = 1;
    private int TutorialNum = 1;
    private int RotateNum = 1;
    private int Stagenum;   // ステージ番号

    private GameObject Point_prefab;
    private GameObject Point;
    private GameObject mainCam;

    private bool PushFlg;

    //カウントダウン
    [SerializeField] public float Pointcountdown = 5.0f;
    [SerializeField] public float Oricountdown = 1.0f;
    private bool CountFlg;


    [SerializeField] private GameObject Lstick;
    [SerializeField] private GameObject Rstick;
    [SerializeField] private GameObject Controller;

    Animator LAnim;
    Animator RAnim;
    Animator ControllerAnim;

    public bool IsPlayerMove { get; private set; }

    public bool IsPlayerLMove { get; private set; }

    public bool IsRotateMove { get; private set; }

    public bool IsLMove { get; private set; }

    public bool IsRMove { get; private set; }

    public bool IsPoint { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        LAnim = Lstick.GetComponent<Animator>();
        RAnim = Rstick.GetComponent<Animator>();
        ControllerAnim = Controller.GetComponent<Animator>();

        //stagemanager = GameObject.Find("stageManager");

        IsPlayerMove = true;
        IsPlayerLMove = true;
        IsRotateMove = true;
        IsLMove = true;
        IsRMove = true;
        IsPoint = false;
        CountFlg = true;
        PushFlg = false;
        mainCam = Camera.main.gameObject;
        Stagenum = StageManager.stageNum;

        if (Stagenum != 1)
        {
            TutorialNum = 5;
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
            return;
        }


        //時間をカウントダウンする
        Controller.GetComponent<RectTransform>().SetAsLastSibling();


        // 左スティックからの入力情報を取得
        float input_x = Input.GetAxis("Horizontal");

        // 右スティックからの入力情報を取得
        float R_Stick_value = Input.GetAxis("tutorialStick");

        //LAnim.SetInteger("tutorialNum", Animnum);
        //RAnim.SetInteger("RtutorialNum", Animnum);


        //if (input_x==0)
        //{

        //    movecountdown -= Time.deltaTime;
        //    if (movecountdown <= 0.0f)
        //    {

        //        Debug.Log("移動UI表示");
        //        ControllerAnim.SetBool("FukidasiFlg", true);
        //        LAnim.SetInteger("tutorialNum", Animnum);
        //        RAnim.SetInteger("RtutorialNum", Animnum);
        //        movecountdown = 5.0f;
        //    }
        //}
        //else if(Animnum!=5)
        //{
        //    movecountdown = 5.0f;
        //    ControllerAnim.SetBool("FukidasiFlg", false);
        //}
        //else
        //{
        //    movecountdown = 5.0f;
        //}

        //if (Player.GetComponent<Player>().IsHitBar)
        //{

        //    Oricountdown -= Time.deltaTime;
        //    if (Oricountdown <= 0)
        //    {
        //        Animnum = 2;
        //        ControllerAnim.SetBool("FukidasiFlg", true);
        //        Debug.Log("UI表示");
        //        LAnim.SetInteger("tutorialNum", Animnum);
        //        RAnim.SetInteger("RtutorialNum", Animnum);
        //        Oricountdown = 1.0f;
        //    }

        //    if(0 < R_Stick_Value || 0 > R_Stick_Value)
        //    {
        //        Animnum = 5;
        //        ControllerAnim.SetBool("FukidasiFlg", true);
        //        LAnim.SetInteger("tutorialNum", Animnum);
        //        RAnim.SetInteger("RtutorialNum", Animnum);
        //    }
        //}
        //else
        //{
        //    Oricountdown = 1.0f;
        //}


        //if(Player.GetComponent<Player>().IsHitBar&& 0 < R_Stick_Value || 0 > R_Stick_Value)
        //{
        //    Animnum = 5;
        //    ControllerAnim.SetBool("FukidasiFlg", true);
        //    LAnim.SetInteger("tutorialNum",Animnum);
        //    RAnim.SetInteger("RtutorialNum", Animnum);
        //}

        //if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
        //{
        //    ControllerAnim.SetBool("FukidasiFlg", false);
        //}

        switch (TutorialNum)
        {
            case 1:
                Debug.Log(Player.GetComponent<Player>().IsHitPoint);
                IsPlayerMove = true;
                IsRotateMove = false;
                IsLMove = false;
                IsRMove = false;
                Pointcountdown -= Time.deltaTime;
                ControllerAnim.SetBool("FukidasiFlg", true);
                LAnim.SetBool("LStick", true);
                RAnim.SetBool("RStickLMove", false);
                if (Pointcountdown <= 0.0f)
                {
                    if (CountFlg)
                    {
                        CountFlg = false;
                        Point = Instantiate(Point_prefab, new Vector3(1.5f, -2.5f, 0.0f), Quaternion.identity);
                        //ControllerAnim.SetBool("FukidasiFlg", true);
                        //LAnim.SetBool("LStick", true);
                        //RAnim.SetBool("RStickLMove", false);
                    }
                }

                if (Player.GetComponent<Player>().IsHitPoint)
                {

                    TutorialNum = 2;
                    ControllerAnim.SetBool("FukidasiFlg", false);
                    LAnim.SetBool("LStick", false);
                    CountFlg = true;
                    IsPoint = true;
                    Destroy(Point);
                }
                break;
            case 2:
                Debug.Log(Player.GetComponent<Player>().IsHitPoint);
                IsPoint = false;
                if (RotateNum == 1)
                {
                    IsPlayerMove = true;
                    if (CountFlg)
                    {
                        CountFlg = false;
                        Point = Instantiate(Point_prefab, new Vector3(0.17f, -2.5f, 0.0f), Quaternion.identity);

                    }

                    if (Player.GetComponent<Player>().IsHitBar)
                    {
                        IsPlayerMove = false;
                        IsPoint = true;
                        Destroy(Point);
                        RotateNum = 2;
                    }

                }
                if (RotateNum == 2)
                {
                    IsLMove = true;
                    IsRMove = false;
                    ControllerAnim.SetBool("FukidasiFlg", true);
                    LAnim.SetBool("LStick", false);
                    RAnim.SetBool("RStickLMove", true);
                    if (stagemanager.GetComponent<StageManager>().IsRotate)
                    {
                        Pointcountdown = 1.5f;
                        IsLMove = false;
                        RotateNum = 3;
                        RAnim.SetBool("RStickLMove", false);
                    }

                }
                if (RotateNum == 3)
                {
                    Debug.Log(RotateNum);
                    IsPlayerMove = true;


                    Pointcountdown -= Time.deltaTime;
                    if (Pointcountdown <= 0.0f)
                    {
                        IsRotateMove = true;
                        PushFlg = true;
                        Pointcountdown = 9999.0f;
                    }

                    LAnim.SetBool("LStick", false);
                    RAnim.SetBool("RStickPush", true);
                    if (PushFlg)
                    {
                        if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
                        {
                            PushFlg = false;
                            Debug.Log("押しました");
                            Point = Instantiate(Point_prefab, new Vector3(0.17f, -2.5f, 0.0f), Quaternion.identity);
                            ControllerAnim.SetBool("FukidasiFlg", false);
                            RAnim.SetBool("RStickPush", false);
                        }
                    }

                    if (Player.GetComponent<Player>().IsHitPoint)
                    {
                        IsPlayerMove = false;
                        IsPoint = true;
                        Destroy(Point);
                        RotateNum = 4;
                    }
                }

                if (RotateNum == 4)
                {
                    Debug.Log(RotateNum);

                    IsPlayerMove = false;
                    IsRMove = true;
                    ControllerAnim.SetBool("FukidasiFlg", true);
                    LAnim.SetBool("LStick", false);
                    RAnim.SetBool("RStickPush", false);
                    RAnim.SetBool("RStickRMove", true);
                    if (stagemanager.GetComponent<StageManager>().IsRotate)
                    {
                        IsRotateMove = false;
                        CountFlg = true;
                        TutorialNum = 3;
                        ControllerAnim.SetBool("FukidasiFlg", false);
                        LAnim.SetBool("LStick", false);
                        RAnim.SetBool("RStickRMove", false);
                    }

                }
                break;
            case 3:
                Debug.Log(Player.GetComponent<Player>().IsHitPoint);
                IsPoint = false;
                IsPlayerMove = true;
                LAnim.SetBool("LStick", false);
                RAnim.SetBool("RStickPush", true);

                if (CountFlg)
                {

                    CountFlg = false;

                    Point = Instantiate(Point_prefab, new Vector3(4.0f, -2.5f, 0.0f), Quaternion.identity);

                }


                {
                    if (Player.GetComponent<Player>().IsHitPoint)
                    {
                        IsRotateMove = true;
                        IsPoint = true;
                        IsPlayerMove = false;
                        PushFlg = true;
                        ControllerAnim.SetBool("FukidasiFlg", true);
                        RAnim.SetBool("RStickPush", true);
                        Destroy(Point);
                    }
                }

                if (PushFlg)
                {
                    if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.K))
                    {
                        PushFlg = false;
                        TutorialNum = 4;
                        CountFlg = true;
                        ControllerAnim.SetBool("FukidasiFlg", false);
                    }
                }
                break;

            case 4:
                IsPlayerMove = true;
                IsRotateMove = true;
                IsLMove = true;
                IsRMove = true;
                //IsPoint = false;
                //IsPlayerMove = true;
                //if (CountFlg)
                //{
                //    CountFlg = false;
                //    Point = Instantiate(Point_prefab, new Vector3(5.0f, -2.7f, 0.0f), Quaternion.identity);
                //}

                //if (Player.GetComponent<Player>().IsHitPoint)
                //{
                //    IsPoint = true;
                //    Destroy(Point);
                //}

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
