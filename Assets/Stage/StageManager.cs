//============================
//name:StageManager
//概要:タイルを折り曲げる処理
//============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject FrontEffectCamera;
    public GameObject Grid;

    public GameObject MapManager;

    private GameObject tutorialManager;

    public GameObject Effect;

    private int[,] BlockNum_Map;
    private GameObject[,] Block_Map;
    private bool isCopy = false;


    private GameObject BigParent;

    private Vector3 startPos;
    private Vector3 endPos;

    WARPSTATE MoveState;

    private List<GameObject> Tile_List;
    private List<GameObject> Tile_subList;
    private List<GameObject> Bar_List;
    private List<GameObject> Bar_subList;

    private GameObject yugami;

    private Vector3 MapPos;
    private Vector3 MapPos_Add;

    public GameObject UnderBorder;

    public GameObject L_Smoke;
    public GameObject R_Smoke;

    public float waitTime = 1.0f;
    private float waitTimer = 0.0f;

    //パーティクル描画用
    public GameObject particleObject;

    // ステージ番号
    public static int stageNum = 1;
    public int rotateNum;
    public bool initFlg = true;


    private bool isInputOff = false;

    private int HitBarIdx = -1;   // プレイヤーと衝突したバー
    private int LeftBarIdx = -1;   // 一番左のバー
    private int RightBarIdx = -1;   // 一番右のバー

    public bool IsGameOver { get; private set; }
    public bool IsGameClear { get; private set; }

    public bool IsRotate { get; private set; }

    public bool IsSmog { get; private set; }

    bool flg = false;
    bool rerotFlg = false;
    bool resetFlg = false;


    bool CanYouCopy = true;
    float timer = 0.0f;


    bool oneFrame = false;

    bool is3D = true;

    public bool isMove { get; private set; }

    public float waitInterval = 0.5f;

    //float firstWaitTime = 1.0f;
    public float firstWaitTimer;


    public float oriInterval = 1.0f;
    private float oriIntervalTimer = 0.0f;


    private int oriBarNum = 0;



    private enum CONTROLLERSTATE
    {
        L_TRIGGER = -1,
        R_TRIGGER = 1,
    }
    private enum STICKSTATE
    {
        L_DOWN = -1,
        R_DOWN = 1,
    }
    private enum WARPSTATE
    {
        NEUTRAL,
        TO_LEFT,    // 左へワープ
        TO_RIGHT,   // 右へワープ
    }

    private enum ROTATESTATE
    {
        NEUTRAL,
        L_ROTATE,
        R_ROTATE,
    }
    ROTATESTATE RotateState = ROTATESTATE.NEUTRAL;

    // 一番初めの更新処理時に行う初期化処理
    void Init()
    {
        // マップエディットクラスからマップ構築メソッドをコールし、そのマップデータを取得
        MapManager.GetComponent<MapEdit>().CreateStage_Game(stageNum);
        Block_Map = MapManager.GetComponent<MapEdit>().BlockList;
        BlockNum_Map = MapManager.GetComponent<MapEdit>().BlockMap;
        Tile_List = MapManager.GetComponent<MapEdit>().TileList;
        Bar_List = MapManager.GetComponent<MapEdit>().BarList;
        // マップからプレイヤーの初期位置を捜し、プレイヤーを配置
        RespawnPlayer();
        // ステージ内オブジェクトを１つのオブジェクトの子供にする
        // ステージの初期化
        GetComponent<StageRotate>().Init();
        ParentReset();

        //チュートリアルマネージャー取得
        tutorialManager = GameObject.Find("TutorialManager");
        // フラグ初期化
        IsGameClear = IsGameOver = IsRotate = IsSmog = false;

        Tile_subList = new List<GameObject>();
        Bar_subList = new List<GameObject>();

        RotateState = ROTATESTATE.NEUTRAL;
        rotateNum = 0;


        BigParent = new GameObject();
        BigParent.AddComponent<InvisibleBlock>();


        Grid.GetComponent<CreateGrid>().SetAlpha(0);

        foreach (var bar in Tile_List)
        {
            bar.transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
        }
    }

    void Update()
    {
        if (firstWaitTimer > 0)
        {
            FrontEffectCamera.SetActive(false);
            Player.SetActive(false);
            firstWaitTimer -= Time.deltaTime;
            return;
        }

        Player.SetActive(true);

        if (initFlg)
        {
            FrontEffectCamera.SetActive(true);
            Init();
            initFlg = false;
            CreateParticle();
        }

        Debug.Log(Player.transform.GetChild((int)global::Player.PLAYERHITBOX.BOTTOM).gameObject.GetComponent<HitAction>().isHit);

        oriIntervalTimer -= Time.deltaTime;

        // 左スティックの入力値を取得
        float L_Stick_Value = Input.GetAxis("Horizontal");

        HitBarIdx = GetHitBarIndex();       // プレイヤーと衝突中のバーを取得
        LeftBarIdx = GetLeftBarIndex();     // ステージの一番左のバーを取得
        RightBarIdx = GetRightBarIndex();   // ステージの一番右のバーを取得
        
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    ChangeTileTexture();
        //}

        if (HitBarIdx != -1)
        {
            Grid.GetComponent<CreateGrid>().AlphaIncrease = false;
            if ((!isLeftBar(HitBarIdx) && !isRightBar(HitBarIdx)) && Grid.GetComponent<MeshRenderer>().enabled)
            {
                for (int i = 0; i < Bar_List.Count; i++)
                {
                    if (Bar_List[i].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED) break;
                    if (i == Bar_List.Count - 1)
                    {
                        Grid.GetComponent<CreateGrid>().AlphaIncrease = true;
                    }
                }
            }
        }
        else
        {
            Grid.GetComponent<CreateGrid>().AlphaIncrease = false;
        }

        if (isStopStage() && !Camera.main.GetComponent<MoveCamera>().isMoveEx)
        {
            if (Player.transform.position.x < Bar_List[RightBarIdx].transform.position.x && Player.transform.position.x > Bar_List[LeftBarIdx].transform.position.x)
                isMove = false;
        }
        else isMove = true;


        // ステージが停止している時、プレイヤーを動かせる
        if (isStopStage() && !Camera.main.GetComponent<MoveCamera>().isMoveEx)
        {
            ChangeBarAlpha();

            // プレイヤーの更新、プレイヤーにおける移動可能領域の設定など
            Player.GetComponent<Player>().TurnOnGravity();
            if (CanYouCopy && !rerotFlg)
            {
                Player.GetComponent<Player>().TurnOnMove();
            }
            if (Bar_List.Count > 0)
            {
                Player.GetComponent<Player>().BorderLine_l = Bar_List[LeftBarIdx].transform.position.x;
                Player.GetComponent<Player>().BorderLine_r = Bar_List[RightBarIdx].transform.position.x;
            }
            Player.transform.parent = null;

            if (Player.transform.position.x - Bar_List[LeftBarIdx].transform.position.x <= 1)
            {
                if (!isCopy && !rerotFlg && Player.transform.position.x >= Bar_List[LeftBarIdx].transform.position.x && CanYouCopy)
                {
                    CopyStage(WARPSTATE.TO_RIGHT);
                    isCopy = true;
                }
            }
            // プレイヤーが右端のバーに接触した場合
            else if (Bar_List[RightBarIdx].transform.position.x - Player.transform.position.x <= 1)
            {
                if (!isCopy && !rerotFlg && Player.transform.position.x <= Bar_List[RightBarIdx].transform.position.x && CanYouCopy)
                {
                    CopyStage(WARPSTATE.TO_LEFT);
                    isCopy = true;
                }
            }
            else if (isCopy)
            {
                DeleteCopy();
                isCopy = false;
            }

            if (!IsGameClear)
            {
                L_Smoke.SetActive(true);
                R_Smoke.SetActive(true);
                if (!L_Smoke.GetComponent<ParticleSystem>().isPlaying)
                {
                    L_Smoke.GetComponent<ParticleSystem>().Play();
                    R_Smoke.GetComponent<ParticleSystem>().Play();
                }
            }

            timer += Time.deltaTime;
            if (timer >= waitInterval) CanYouCopy = true;


            if (!rerotFlg)
            {
                FrontEffectCamera.SetActive(true);
            }

            //SetModeGoalEffect(3);
        }
        else
        {
            timer = 0;
            CanYouCopy = false;

            // ステージが動いている時はプレイヤーを停止
            Player.GetComponent<Player>().TurnOffMove();

            L_Smoke.SetActive(false);
            R_Smoke.SetActive(false);

            //Grid.GetComponent<MeshRenderer>().enabled = false;
        }

        if (IsGameClear)
        {
            L_Smoke.SetActive(false);
            R_Smoke.SetActive(false);

            for (int i = 0; i < Tile_List.Count; i++)
            {
                Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
            }
        }

        // 右スティックからの入力情報を取得
        float R_Stick_Value = Input.GetAxis("Horizontal2");


        if (oneFrame)
        {
            oneFrame = false;

            for (int i = 0; i < Tile_List.Count; i++)
            {
                Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().TurnOnScreenShot();
            }
        }
        // ステージの状況を見て回転できるかどうかを判断
        if (CanYouRotate() && !IsGameClear && oriIntervalTimer <= 0.0f)
        {
            if (tutorialManager.GetComponent<tutorialManagaer>().IsRMove)
            {
                // プレイヤーに衝突しているバーがあった場合、トリガーの入力値を参照し回転させる
                if (!FinishManager.menuFlg)
                {
                    if (R_Stick_Value == (int)CONTROLLERSTATE.R_TRIGGER || Input.GetKeyDown(KeyCode.D))
                    {
                        SetModeGoalEffect(0);
                        SetModeGoalEffect(2);
                        RotateBar(HitBarIdx, BarRotate.ROTSTATEOUTERDATA.ROTATE_LEFT);
                        RotateState = ROTATESTATE.L_ROTATE;
                        ScreenShot();
                        rotateNum++;
                        Player.GetComponent<Player>().moveDir = global::Player.MOVEDIR.RIGHT;
                        IsRotate = true;
                        oriBarNum = HitBarIdx;
                    }
                }
            }
            if (tutorialManager.GetComponent<tutorialManagaer>().IsLMove)
            {
                if (!FinishManager.menuFlg)
                {
                    if (R_Stick_Value == (int)CONTROLLERSTATE.L_TRIGGER || Input.GetKeyDown(KeyCode.A))
                    {
                        SetModeGoalEffect(0);
                        SetModeGoalEffect(2);
                        RotateBar(HitBarIdx, BarRotate.ROTSTATEOUTERDATA.ROTATE_RIGHT);
                        RotateState = ROTATESTATE.R_ROTATE;
                        ScreenShot();
                        rotateNum++;
                        Player.GetComponent<Player>().moveDir = global::Player.MOVEDIR.LEFT;
                        IsRotate = true;
                        oriBarNum = HitBarIdx;
                    }
                }
            }
        }

        if (rerotFlg)
        {
            if (flg)
            {
                SecondFunc();
            }
            if (Tile_List[0].activeSelf)
            {
                if (Tile_List[0].transform.Find("TileChild").GetComponent<ScreenShot>().isFinishedScreenShot())
                {
                    ThirdFunc();
                }
            }
            if (Tile_List[Tile_List.Count - 1].activeSelf)
            {
                if (Tile_List[Tile_List.Count - 1].transform.Find("TileChild").GetComponent<ScreenShot>().isFinishedScreenShot())
                {
                    FourFunc();
                }
            }
        }
        else
        {
            for (int i = 0; i < Tile_List.Count; i++)
            {
                if (Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().isFinishedScreenShot())
                {
                    SetAllBlockActive(false);
                    Player.transform.Find("walk_UV").gameObject.SetActive(true);
                    FrontEffectCamera.SetActive(true);
                    Grid.GetComponent<MeshRenderer>().enabled = false;

                    break;
                }
            }
        }

        if (tutorialManager.GetComponent<tutorialManagaer>().IsRotateMove && oriIntervalTimer <= 0.0f)
        {
            // 回転済みのステージを戻す処理
            if (Input.GetKeyDown("joystick button 9") || Input.GetKeyDown(KeyCode.S))
            {
                IsRotate = false;
                if (!Camera.main.GetComponent<MoveCamera>().isMoveEx && !IsGameClear)
                {
                    // 回転済みのバーを検出したら元に戻す回転処理を開始
                    for (int i = 0; i < Bar_List.Count; i++)
                    {
                        if (Bar_List[i].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED)
                        {
                            //rotateNum++;
                            SetModeGoalEffect(0);
                            SetModeGoalEffect(2);
                            FirstFunc();
                            Player.GetComponent<Player>().TurnOffMove();
                            Player.GetComponent<Player>().FixPos();

                            Player.GetComponent<Player>().PlayerFixEx();
                            break;
                        }
                    }
                }
            }
        }

        if (Player.transform.position.x < Bar_List[LeftBarIdx].transform.position.x)
        {
            if (isCopy)
            {
                oriIntervalTimer = oriInterval * 2.0f;
                DecidedStage(WARPSTATE.TO_RIGHT);
            }
        }
        else if (Player.transform.position.x > Bar_List[RightBarIdx].transform.position.x)
        {
            if (isCopy)
            {
                oriIntervalTimer = oriInterval * 3.0f;
                DecidedStage(WARPSTATE.TO_LEFT);
            }
        }

        if (!isCopy && !IsGameClear)
        {
            Camera.main.GetComponent<MoveCamera>().SetPos(Bar_List[LeftBarIdx].transform.position.x + (Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x) / 2.0f);
        }

        // ゲームクリア検知
        if (Player.GetComponent<Player>().IsHitGoalBlock)
        {
            IsGameClear = true;
            //GameObject goal1 = GameObject.Find("2(Clone)/GoalObj1").gameObject;
            //GameObject goal2 = GameObject.Find("2(Clone)/GoalObj2").gameObject;
            //goal1.GetComponent<GoalScript>().SetStartFlg(true);
            //goal2.GetComponent<GoalScript>().SetStartFlg(true);
            //SetModeGoalEffect(3);
            SetModeGoalEffect(1);
        }
        // ゲームオーバー検知
        else if (UnderBorder.GetComponent<HitAction>().isHit || waitTime <= waitTimer || Player.GetComponent<Player>().deadFlg)
        {
            DeleteCopyForMenu();
            IsGameOver = true;
            SetModeGoalEffect(0);
            SetModeGoalEffect(2);
        }
        else if (isStopStage() && !Camera.main.GetComponent<MoveCamera>().isMoveEx && !rerotFlg)
        {
            SetModeGoalEffect(3);
        }

        if (UnderBorder.GetComponent<HitCreateEffect>().isFinished)
        {
            Player.GetComponent<Player>().TurnOffMove();
            waitTimer += Time.deltaTime;
            Player.transform.Find("walk_UV").gameObject.SetActive(false);
        }

        if (!isStopStage())
        {
            resetFlg = true;
            //Player.transform.GetChild((int)global::Player.PLAYERHITBOX.BOTTOM).gameObject.GetComponent<HitAction>().isHit = false;
        }
        else
        {
            if (resetFlg)
            {
                //for (int k = 0; k < Tile_List.Count; k++)
                //{
                //    Tile_List[k].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                //}
                ChangeTileTexture();
                resetFlg = false;
            }
            ParentReset();
        }

        L_Smoke.transform.position = Bar_List[LeftBarIdx].transform.position;
        R_Smoke.transform.position = Bar_List[RightBarIdx].transform.position;

        if (Player.transform.position.x < Bar_List[LeftBarIdx].transform.position.x || Player.transform.position.x > Bar_List[RightBarIdx].transform.position.x)
        {
            Camera.main.GetComponent<MoveCamera>().isMove = false;
        }
    }

    // バーの回転処理
    private void RotateBar(int bar_idx, BarRotate.ROTSTATEOUTERDATA rotstate)
    {
        Bar_List[bar_idx].GetComponent<BarRotate>().ReverseRotateFlg = GetComponent<StageRotate>().isReverse;
        ParentReset();
        // ステージを折る直前、バーにステージが反転しているかどうかを知らせる
        if (rotstate != BarRotate.ROTSTATEOUTERDATA.REROTATE)
        {
            SettingParent(rotstate);
            // 折った時の位置補正
            Vector3 playerPos = Player.transform.position;
            playerPos.x = Bar_List[bar_idx].transform.position.x;
            if (rotstate == BarRotate.ROTSTATEOUTERDATA.ROTATE_LEFT)
            {
                playerPos.x += Player.transform.localScale.x * 0.6f;
            }
            if (rotstate == BarRotate.ROTSTATEOUTERDATA.ROTATE_RIGHT)
            {
                playerPos.x -= Player.transform.localScale.x * 0.6f;
            }
            //Player.transform.position = playerPos;
        }
        else
        {
            if (RotateState == ROTATESTATE.L_ROTATE)
            {
                SettingParent(BarRotate.ROTSTATEOUTERDATA.ROTATE_LEFT);
            }
            if (RotateState == ROTATESTATE.R_ROTATE)
            {
                SettingParent(BarRotate.ROTSTATEOUTERDATA.ROTATE_RIGHT);
            }
        }
        // 回転処理
        Bar_List[bar_idx].GetComponent<BarRotate>().Rotation(rotstate);
        //oriIntervalTimer = oriInterval;
    }

    // 回転可能かどうかをチェック
    private bool CanYouRotate()
    {
        // バーがプレイヤーと衝突してない時、回転させない
        if (HitBarIdx < 0) return false;

        // バーがステージの両端に位置する時、回転させない
        if (HitBarIdx == LeftBarIdx || HitBarIdx == RightBarIdx) return false;

        // 回転中や回転後のバーがあった場合、回転させない
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].GetComponent<BarRotate>().RotateState != BarRotate.ROTSTATEINNERDATA.NEUTRAL) return false;
        }

        // プレイヤーと衝突しているバーの両隣にブロックがあった場合は、回転させない
        Vector3 ray_pos = new Vector3(Bar_List[HitBarIdx].transform.position.x, Player.transform.position.y, -0.25f);
        Ray ray = new Ray(ray_pos + Vector3.left * Bar_List[HitBarIdx].transform.localScale.x / 2.0f, Vector3.right);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f))
        {
            string tag = hit.collider.tag;
            if (tag == "Block" || tag == "GoalBlock" || tag == "GimicMoveBlock" || tag == "GimicMoveBar" || tag == "GimicBreakBlock" || tag == "ClimbBlock") return false;
        }
        ray = new Ray(ray_pos + Vector3.right * Bar_List[HitBarIdx].transform.localScale.x / 2.0f, Vector3.left);
        if (Physics.Raycast(ray, out hit, 0.5f))
        {
            string tag = hit.collider.tag;
            if (tag == "Block" || tag == "GoalBlock" || tag == "GimicMoveBlock" || tag == "GimicMoveBar" || tag == "GimicBreakBlock" || tag == "ClimbBlock") return false;
        }

        return true;
    }

    // プレイヤーと衝突しているバーを返す（戻り値が -1 以外なら衝突している）
    private int GetHitBarIndex()
    {
        int rotBarNum;
        for (rotBarNum = Bar_List.Count - 1; rotBarNum >= 0; rotBarNum--)
        {
            // カウント変数がプレイヤーと衝突しているバーと同じになればループを抜ける
            if (Bar_List[rotBarNum].GetComponent<BarRotate>().IsHit) break;
        }
        return rotBarNum;
    }

    // 回転直前に親子関係を形成する
    private void SettingParent(BarRotate.ROTSTATEOUTERDATA rotstate)
    {
        if (rotstate == BarRotate.ROTSTATEOUTERDATA.ROTATE_LEFT)
        {
            // 右から左へ順に親にしていく（２つのバーを比較した時、要素番号の小さい方が親）
            for (int cnt = Bar_List.Count - 1; cnt > 0; cnt--)
            {
                Tile_List[cnt - 1].transform.parent = Bar_List[cnt].transform;
                Bar_List[cnt - 1].transform.parent = Tile_List[cnt - 1].transform;
            }
        }
        if (rotstate == BarRotate.ROTSTATEOUTERDATA.ROTATE_RIGHT)
        {
            // 左から右へ順に親にしていく（２つのバーを比較した時、要素番号の大きい方が親）
            for (int cnt = 0; cnt < Bar_List.Count - 1; cnt++)
            {
                Tile_List[cnt].transform.parent = Bar_List[cnt].transform;
                Bar_List[cnt + 1].transform.parent = Tile_List[cnt].transform;
            }
        }
    }

    //親子関係をリセット
    private void ParentReset()
    {
        for (int i = 0; i < Tile_List.Count; i++)
        {
            Tile_List[i].transform.parent = this.gameObject.transform;
        }
        for (int i = 0; i < Bar_List.Count; i++)
        {
            Bar_List[i].transform.parent = this.gameObject.transform;
        }
    }

    // 引数で渡された要素番号のバーが全てのバーの中で一番左に位置しているかどうか
    private bool isLeftBar(int bar_idx)
    {
        if (bar_idx < 0) return false;
        float min_x = Bar_List[bar_idx].transform.position.x;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x < min_x)
            {
                if (min_x - Bar_List[i].transform.position.x > 0.02) return false;
            }
        }
        return true;
    }
    // 引数で渡された要素番号のバーが全てのバーの中で一番右に位置しているかどうか
    private bool isRightBar(int bar_idx)
    {
        if (bar_idx < 0) return false;
        float max_x = Bar_List[bar_idx].transform.position.x;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x > max_x)
            {
                if (Bar_List[i].transform.position.x - max_x > 0.02f) return false;
            }
        }
        return true;
    }

    // 一番左のバーの要素番号を取得
    private int GetLeftBarIndex()
    {
        int bar_idx = 0;
        float min_x = 999;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x < min_x)
            {
                min_x = Bar_List[i].transform.position.x;
                bar_idx = i;
            }
        }
        return bar_idx;
    }
    // 一番右のバーの要素番号を取得
    private int GetRightBarIndex()
    {
        int bar_idx = 0;
        float max_x = -999;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x > max_x)
            {
                max_x = Bar_List[i].transform.position.x;
                bar_idx = i;
            }
        }
        return bar_idx;
    }

    // 現在ステージが停止しているかどうか（ステージやバーが回転していないかどうか）
    private bool isStopStage()
    {
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (!(Bar_List[i].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.NEUTRAL ||
                Bar_List[i].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED) ||
                GetComponent<StageRotate>().isRotNow)
            {
                return false;
            }
        }

        SetAllBlockActive(true);
        return true;
    }
    // マップからプレイヤーナンバーを捜しその位置にプレイヤーを配置
    private void RespawnPlayer()
    {
        for (int y = 0; y < Block_Map.GetLength(0); y++)
        {
            for (int x = 0; x < Block_Map.GetLength(1); x++)
            {
                if (BlockNum_Map[y, x] != 0)
                {
                    if (Block_Map[y, x].transform.tag == "Player")
                    {
                        Player.transform.position = Block_Map[y, x].transform.position;
                    }
                }
            }
        }
    }
    public void ResetStage(int stage_idx)
    {
        stageNum = stage_idx;
        initFlg = true;
    }
    private void CopyStage(WARPSTATE state, bool flg = false)
    {
        if (UnderBorder.GetComponent<HitCreateEffect>().isFinished) return;

        Destroy(BigParent);
        BigParent = new GameObject();
        ParentReset();

        // タイル複製
        for (int i = 0; i < Tile_List.Count; i++)
        {
            GameObject tile = GameObject.Instantiate(Tile_List[i]);
            tile.transform.Find("TileChild").GetComponent<ScreenShot>().TurnTexture();
            tile.transform.parent = BigParent.transform;

            foreach (Transform child in tile.GetComponentsInChildren<Transform>())
            {
                if (child.tag == "Block" || child.tag == "GoalBlock" || child.tag == "GimicMoveBlock" || child.tag == "GimicMoveBar" || child.tag == "GimicBreakBlock" || child.tag == "GimicClearBlock")
                {
                    child.parent = tile.transform;
                    child.transform.position = new Vector3(child.transform.position.x, -child.transform.position.y, child.transform.position.z);

                    if (child.tag == "GimicMoveBlock")
                    {
                        child.GetComponent<MoveBlock>().TurnOffGravity();
                    }
                    // タイル複製
                    if (child.tag == "GoalBlock")
                    {
                        child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetPlayEffectFlg(false);
                        child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetPlayEffectFlg(false);
                    }

                }
                for (int j = 0; j < Tile_List.Count; j++)
                {
                    foreach (Transform child2 in Tile_List[j].GetComponentsInChildren<Transform>())
                    {
                        if (child.tag == "GoalBlock")
                        {
                            if (child2.tag=="GoalBlock")
                            {
                                //child.transform.Find("GoalObj1").GetComponent<GoalScript>().CreateSandPerticle();
                                //child.transform.Find("GoalObj2").GetComponent<GoalScript>().CreateSandPerticle();
                                //child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetGearVibrateTime(30);
                                //child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetGearVibrateTime(30);
                                //child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetSilverState(StageSelectManager.silverConditions[stageNum], rotateNum);
                                //child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetSilverState(StageSelectManager.silverConditions[stageNum], rotateNum);
                                //Debug.Log(StageSelectManager.silverConditions[StageManager.stageNum]);
                                //
                                if (child2.transform.Find("GoalObj1").GetComponent<GoalScript>().GetIsNoSilverGear())
                                {
                                    child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetIsNoSilverGear(true);
                                    child.transform.Find("GoalObj1").GetComponent<GoalScript>().GetGear().GetComponent<MeshRenderer>().enabled = false;
                                }
                                if (child2.transform.Find("GoalObj2").GetComponent<GoalScript>().GetIsNoSilverGear())
                                {
                                    child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetIsNoSilverGear(true);
                                    //child.transform.Find("GoalObj2").GetComponent<GoalScript>().GetGear().SetActive(false);
                                    child.transform.Find("GoalObj2").GetComponent<GoalScript>().GetGear().GetComponent<MeshRenderer>().enabled = false;
                                }
                            }
                        }
                    }
                }
            }

            Tile_subList.Add(tile);
        }

        // バー複製
        for (int i = 0; i < Bar_List.Count; i++)
        {
            GameObject bar = GameObject.Instantiate(Bar_List[i]);
            Bar_List[i].GetComponent<BarRotate>().CopyBarState(bar);
            bar.transform.parent = BigParent.transform;

            //if (i == LeftBarIdx || i == RightBarIdx)
            //{
            //    Bar_List[i].GetComponent<MeshRenderer>().enabled = false;
            //}
            Bar_subList.Add(bar);
        }

        BigParent.AddComponent<InvisibleBlock>();
        BigParent.GetComponent<InvisibleBlock>().SetAlpha(0);
        BigParent.GetComponent<InvisibleBlock>().AlphaState = InvisibleBlock.ALPHASTATE.INCREASE;


        // ずらす
        if (state == WARPSTATE.TO_LEFT)
        {
            if (!flg)
            {
                MapPos_Add = Vector3.right * (((Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x)));
                BigParent.transform.position = MapPos_Add;
            }
            else
            {
                IsSmog = true;
                BigParent.transform.position = Vector3.left * (((Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x)));
                if (MapPos_Add.x < 0) MapPos_Add.x = -MapPos_Add.x;
            }
        }
        else
        {
            if (!flg)
            {
                MapPos_Add = Vector3.left * (((Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x)));
                BigParent.transform.position = MapPos_Add;
            }
            else
            {
                BigParent.transform.position = Vector3.right * (((Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x)));
                if (MapPos_Add.x > 0) MapPos_Add.x = -MapPos_Add.x;
            }
        }


        // 蜃気楼発生
        if (File.Exists(Application.dataPath + "/Resources/Prefabs/" + "yugami.prefab"))
        {
            yugami = Resources.Load("Prefabs/yugami") as GameObject;
            yugami = GameObject.Instantiate(yugami);
            yugami.transform.localScale = new Vector3(Mathf.Abs(MapPos_Add.x) * 0.1f, Block_Map.GetLength(0), 1.0f);

            float addPos;
            if (state == WARPSTATE.TO_LEFT)
            {
                if (!flg)
                {
                    yugami.transform.position = new Vector3(Bar_List[RightBarIdx].transform.position.x, 0.0f, -0.5f);
                    addPos = (yugami.transform.localScale.x * 10.0f) / 2.0f;
                    yugami.transform.Translate(Vector3.right * addPos);
                }
                else
                {
                    yugami.transform.position = new Vector3(Bar_List[LeftBarIdx].transform.position.x, 0.0f, -0.5f);
                    addPos = (yugami.transform.localScale.x * 10.0f) / 2.0f;
                    yugami.transform.Translate(Vector3.left * addPos);
                }
            }
            else
            {
                if (!flg)
                {
                    yugami.transform.position = new Vector3(Bar_List[LeftBarIdx].transform.position.x, 0.0f, -0.5f);
                    addPos = (yugami.transform.localScale.x * 10.0f) / 2.0f;
                    yugami.transform.Translate(Vector3.left * addPos);
                }
                else
                {
                    yugami.transform.position = new Vector3(Bar_List[RightBarIdx].transform.position.x, 0.0f, -0.5f);
                    addPos = (yugami.transform.localScale.x * 10.0f) / 2.0f;
                    yugami.transform.Translate(Vector3.right * addPos);
                }
            }
        }
    }
    private void DecidedStage(WARPSTATE state)
    {
        SetModeGoalEffect(0);
        SetModeGoalEffect(2);

        BigParent.GetComponent<InvisibleBlock>().SetAlpha(1.0f);

        for (int i = 0; i < Tile_List.Count; i++) Tile_List[i].transform.parent = BigParent.transform;
        Tile_List.Clear();

        for (int i = 0; i < Bar_List.Count; i++) Bar_List[i].transform.parent = BigParent.transform;
        Bar_List.Clear();


        Tile_List = new List<GameObject>(Tile_subList);
        Tile_subList.Clear();
        Bar_List = new List<GameObject>(Bar_subList);
        Bar_subList.Clear();

        ParentReset();

        foreach (var tile in Tile_List)
        {
            foreach (Transform child in tile.transform)
            {
                if (child.tag == "GimicMoveBlock")
                {
                    child.GetComponent<MoveBlock>().TurnOnGravity();
                }
                if (child.tag == "GoalBlock")
                {
                    child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetPlayEffectFlg(true);
                    child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetPlayEffectFlg(true);
                }
            }
        }

        //Destroy(BigParent);
        //BigParent = new GameObject();

        Destroy(yugami);

        CopyStage(state, true);
        //isCopy = true;


        //BigParent.GetComponent<InvisibleBlock>().SetAlpha(0.2f);
        //BigParent.GetComponent<InvisibleBlock>().AlphaState = InvisibleBlock.ALPHASTATE.DECREASE;

        //float addPos = (yugami.transform.localScale.x * 10.0f) / 2.0f;

        //yugami.transform.Translate(Vector3.left * MapPos_Add.x);


        Camera.main.GetComponent<MoveCamera>().Move(MapPos_Add.x);



        Grid.transform.position += MapPos_Add;

    }
    private void DeleteCopy()
    {
        //for (int i = 0; i < Tile_List.Count; i++) Destroy(Tile_List[i]);
        Tile_subList.Clear();

        //for (int i = 0; i < Bar_List.Count; i++) Destroy(Bar_List[i]);
        Bar_subList.Clear();

        if (isCopy)
        {
            BigParent.GetComponent<InvisibleBlock>().AlphaState = InvisibleBlock.ALPHASTATE.DECREASE;
        }
        //Destroy(BigParent);

        Destroy(yugami);
    }

    public void DeleteCopyForMenu()
    {
        if (!initFlg)
        {
            Tile_subList.Clear();
            Bar_subList.Clear();
        }
        Destroy(BigParent);
        Destroy(yugami);

        isCopy = false;
    }

    private void SetAllBlockActive(bool activeFlg)
    {
        foreach (var obj in Tile_List)
        {
            foreach (Transform childTransform in obj.transform)
            {
                if (childTransform.tag == "Block" || childTransform.tag == "GoalBlock" || childTransform.tag == "GimicMoveBlock" || childTransform.tag == "GimicMoveBar" || childTransform.tag == "GimicBreakBlock" || childTransform.tag == "GimicClearBlock")
                {
                    childTransform.gameObject.SetActive(activeFlg);
                    //if (is3D != activeFlg)
                    //{
                    //    GameObject ef = Instantiate(Effect);
                    //    ef.transform.position = childTransform.position;
                    //    ef.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                    //    ef.GetComponent<ParticleSystem>().Play();
                    //}
                }
            }
        }

        if (!activeFlg)
        {
            foreach (var obj in Bar_List)
            {
                obj.GetComponent<MeshRenderer>().enabled = activeFlg;
            }
        }
        else
        {
            ChangeBarAlpha();
        }

        if (is3D == false && activeFlg == true)
        {
            Grid.GetComponent<MeshRenderer>().enabled = true;
            if (RotateState != ROTATESTATE.NEUTRAL)
            {
                // ここぉぉおっ！！！
                for (int i = 0; i < Tile_List.Count; i++)
                {
                    foreach (Transform child in Tile_List[i].GetComponentsInChildren<Transform>())
                    {
                        if (child.tag == "GoalBlock")
                        {
                            if (!child.transform.Find("GoalObj1").GetComponent<GoalScript>().GetIsNoSilverGear())
                            {
                                child.transform.Find("GoalObj1").GetComponent<GoalScript>().CreateSandPerticle();
                                child.transform.Find("GoalObj2").GetComponent<GoalScript>().CreateSandPerticle();
                                child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetGearVibrateTime(30);
                                child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetGearVibrateTime(30);
                                child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetSilverState(StageSelectManager.silverConditions[stageNum], rotateNum);
                                child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetSilverState(StageSelectManager.silverConditions[stageNum], rotateNum);
                                //Debug.Log(StageSelectManager.silverConditions[StageManager.stageNum]);
                            }
                        }
                    }
                }
            }
        }
        if (is3D != activeFlg)
        {
            oriIntervalTimer = oriInterval;
        }
        is3D = activeFlg;

        float x = Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x;
        int result;
        float excess = x % 1.0f;
        if (Mathf.Abs(excess) >= 0.5)
        {
            result = Mathf.CeilToInt(x);
        }
        else
        {
            result = Mathf.FloorToInt(x);
        }

        Grid.GetComponent<CreateGrid>().ReGrid(result, 0);
        Grid.transform.position = new Vector3(
            Bar_List[LeftBarIdx].transform.position.x + (Bar_List[RightBarIdx].transform.position.x - Bar_List[LeftBarIdx].transform.position.x) / 2.0f, 0.0f, 0.0f
        );
    }

    private void ScreenShot()
    {
        oneFrame = true;

        Player.transform.Find("walk_UV").gameObject.SetActive(false);
        FrontEffectCamera.SetActive(false);
        Grid.GetComponent<MeshRenderer>().enabled = false;
        Grid.GetComponent<CreateGrid>().SetAlpha(0);
    }

    private void FirstFunc()
    {
        ChangeBlockClear(true);
        rerotFlg = true;
        bool tileActive = true;
        for (int k = 0; k < Tile_List.Count; k++)
        {
            Tile_List[k].SetActive(tileActive);
            if (Bar_List[k + 1].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED)
            {
                tileActive = false;
            }
        }
        Player.transform.Find("walk_UV").gameObject.SetActive(false);
        FrontEffectCamera.SetActive(false);
        DeleteCopy();
        isCopy = false;
        Grid.GetComponent<MeshRenderer>().enabled = false;
        Grid.GetComponent<CreateGrid>().SetAlpha(0);

        flg = true;
    }
    private void SecondFunc()
    {
        bool tileActive2 = true;
        for (int k = 0; k < Tile_List.Count; k++)
        {
            if (tileActive2)
            {
                Tile_List[k].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                Tile_List[k].transform.Find("TileChild").GetComponent<ScreenShot>().TurnOnScreenShot();
            }
            if (Bar_List[k + 1].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED)
            {
                tileActive2 = false;
            }
        }
        flg = false;
    }
    private void ThirdFunc()
    {
        bool tileActive = true;
        for (int k = Tile_List.Count - 1; k >= 0; k--)
        {
            Tile_List[k].SetActive(tileActive);
            if (tileActive)
            {
                Tile_List[k].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                Tile_List[k].transform.Find("TileChild").GetComponent<ScreenShot>().TurnOnScreenShot();
            }
            if (Bar_List[k].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED)
            {
                tileActive = false;
            }
        }
    }
    private void FourFunc()
    {
        for (int i = 0; i < Tile_List.Count; i++)
        {
            Tile_List[i].SetActive(true);
            if (Tile_List[i].transform.localRotation.y != 0.0f)
            {
                Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ReverseTexture();
            }
        }

        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].GetComponent<BarRotate>().RotateState == BarRotate.ROTSTATEINNERDATA.ROTATED)
            {
                RotateBar(i, BarRotate.ROTSTATEOUTERDATA.REROTATE);
                break;
            }
        }
        Player.transform.Find("walk_UV").gameObject.SetActive(true);
        FrontEffectCamera.SetActive(true);
        rerotFlg = false;
        RotateState = ROTATESTATE.NEUTRAL;

        //Player.GetComponent<Player>().TurnOnGravity();

        SetAllBlockActive(true);
        ChangeBlockClear(false);
        SetAllBlockActive(false);
    }

    private void ChangeBlockClear(bool flg)
    {
        // タイル複製
        for (int i = 0; i < Tile_List.Count; i++)
        {
            foreach (Transform child in Tile_List[i].GetComponentsInChildren<Transform>())
            {
                if (child.tag == "GimicClearBlock")
                {
                    if (flg)
                    {
                        child.GetComponent<ClearBlock>().Clear();
                    }
                    else
                    {
                        child.GetComponent<ClearBlock>().start();
                    }
                }
            }
        }
    }

    // 0 --> 消す , 1 --> クリア時 , 2 --> 停止 , 3 --> 再生
    public void SetModeGoalEffect(int modeCnt)
    {
        // タイル複製
        for (int i = 0; i < Tile_List.Count; i++)
        {
            foreach (Transform child in Tile_List[i].GetComponentsInChildren<Transform>())
            {
                if (child.tag == "GoalBlock")
                {
                    switch (modeCnt) {
                        case 0:
                            child.transform.Find("GoalObj1").GetComponent<GoalScript>().ClearParticle();
                            child.transform.Find("GoalObj2").GetComponent<GoalScript>().ClearParticle();
                            break;
                        case 1:
                            child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetStartFlg(true);
                            child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetStartFlg(true);
                            break;
                        case 2:
                            child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetPlayEffectFlg(false);
                            child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetPlayEffectFlg(false);
                            break;
                        case 3:
                            child.transform.Find("GoalObj1").GetComponent<GoalScript>().SetPlayEffectFlg(true);
                            child.transform.Find("GoalObj2").GetComponent<GoalScript>().SetPlayEffectFlg(true);
                            break;
                        case 4:
                            child.transform.Find("GoalObj1").GetComponent<GoalScript>().ChangeColor(GoalScript.E_ParticleColor.GOLD); ;
                            child.transform.Find("GoalObj2").GetComponent<GoalScript>().ChangeColor(GoalScript.E_ParticleColor.GOLD);;
                            break;
                    }
                    return;
                }
            }
        }
    }

    public void FixPlayerPos()
    {
        if (Player.transform.position.x > Bar_List[RightBarIdx].transform.position.x)
        {
            Player.transform.position = new Vector3(Bar_List[RightBarIdx].transform.position.x - 0.1f, Player.transform.position.y, Player.transform.position.z);
        }
        else if (Player.transform.position.x < Bar_List[LeftBarIdx].transform.position.x)
        {
            Player.transform.position = new Vector3(Bar_List[LeftBarIdx].transform.position.x + 0.1f, Player.transform.position.y, Player.transform.position.z);
        }
    }

    private void ChangeBarAlpha()
    {
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (isLeftBar(i) || isRightBar(i))
            {
                if (i == oriBarNum && RotateState != ROTATESTATE.NEUTRAL)
                {
                    Bar_List[i].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    Bar_List[i].GetComponent<MeshRenderer>().enabled = false;
                }

            }
            else
            {
                if (RotateState != ROTATESTATE.NEUTRAL)
                {
                    Bar_List[i].GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    Bar_List[i].GetComponent<MeshRenderer>().enabled = true;
                }
            }

            if (i == 0 || i == Bar_List.Count - 1)
            {
                if (!(isLeftBar(i) || isRightBar(i)))
                {
                    if (RotateState != ROTATESTATE.NEUTRAL)
                    {
                        Bar_List[i].GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        Bar_List[i].GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
        }
        if ((isLeftBar(0) || isRightBar(0)) && (isLeftBar(Bar_List.Count - 1) || isRightBar(Bar_List.Count - 1)))
        {
            if (RotateState != ROTATESTATE.NEUTRAL)
            {
                Bar_List[0].GetComponent<MeshRenderer>().enabled = true;
            }
        }

    }

    public void CreateParticle()
    {
        foreach (var obj in Tile_List)
        {
            foreach (Transform childTransform in obj.transform)
            {
                if (childTransform.tag == "Block" || childTransform.tag == "GoalBlock" || childTransform.tag == "GimicMoveBlock" || childTransform.tag == "GimicMoveBar" || childTransform.tag == "GimicBreakBlock" || childTransform.tag == "GimicClearBlock")
                {
                    GameObject ef = Instantiate(Effect);
                    ef.transform.position = childTransform.position;
                    ef.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    ef.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }

    private void ChangeTileTexture()
    {
        for (int i = 0; i < Tile_List.Count; i++)
        {
            switch (RotateState)
            {
                case ROTATESTATE.L_ROTATE:
                    if (oriBarNum > i) Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture(true);
                    else Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                    break;
                case ROTATESTATE.R_ROTATE:
                    if (oriBarNum <= i) Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture(true);
                    else Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                    break;
                case ROTATESTATE.NEUTRAL:
                    Tile_List[i].transform.Find("TileChild").GetComponent<ScreenShot>().ResetTexture();
                    break;
            }
        }
    }
}