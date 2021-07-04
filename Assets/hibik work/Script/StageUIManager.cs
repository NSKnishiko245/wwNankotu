using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class StageUIManager : MonoBehaviour
{
    private GameObject eventSystem;
    private GameObject stageManager;
    private GameObject tutorialUI;
    private GameObject editCanvas;
    private GameObject player;
    public GameObject frontEffectCamera;
    private GameObject Point;

    private GameObject bookL;
    private Animator bookLAnim;

    // メニューのUI
    private GameObject menuSelectGear;
    private GameObject menuRetryGear;
    private GameObject menuSelect;
    private GameObject menuRetry;
    private GameObject menuBoard;
    private GameObject menuStageNum;
    private GameObject menuOperation1;
    private GameObject menuOperation2;
    private GameObject copper;
    private GameObject silver;
    private GameObject gold;


    // ヒントのUI
    private GameObject hintBoard;
    private GameObject hintMovie;
    private GameObject hintUI;
    private GameObject tv;
    private GameObject hukidasi;
    private GameObject eIcon;

    public static int missCnt = 0;  // 失敗した回数
    public static float hintCnt = 0.0f;
    private float hintDispTime = 5.0f;
    private float hintDeleteTime = 10.0f;
    private bool hintFlg = false;
    private bool hintFirstFlg = false;
    private bool hintUIFlg = false;
    public static int hintOpenCnt = 0;
    private int eIconTime = 0;
    private int eIconTimeInit = 20;
    private int eIconNum = 0;

    // クリアのUI
    private GameObject clearSelectGear;
    private GameObject clearSelectGear2;
    private GameObject clearNextGear;
    private GameObject clearNextGear2;
    private GameObject clearSelect;
    private GameObject clearNext;


    [SerializeField] private Material[] material = new Material[6];

    // サウンド
    private AudioSource bgmSource;
    [SerializeField] private AudioSource resultSource;
    [SerializeField] private AudioSource selectDecSource;
    private int bgmNum;

    [SerializeField] private bool editFlg = false;  // true:エディット表示
    [SerializeField] private bool debugFlg = false;  // true:デバッグテキスト表示

    [SerializeField] private int stageNum;   // ステージ番号

    // デバック用テキスト
    GameObject debug;
    Text stageNumText;
    Text hintTimeText;
    Text rotateNumText;
    Text silverMedalNumText;
    //InputField inputField;

    // ステージ画像用
    GameObject stageImage;

    // シーン遷移までの時間
    private int sceneChangeCnt;
    private string changeSceneName;

    // プレイヤーとステージを表示するまでの時間
    [SerializeField] private int stageDisplayCntInit;
    private int stageDisplayCnt = 90;

    // クリアコマンドが操作可能になるまでの時間
    [SerializeField] private int clearCommandOperationCntInit;
    private int clearCommandOperationCnt = 0;

    // 死亡してからリトライするまでの時間
    [SerializeField] private int gameOverCntInit;
    private int gameOverCnt;

    public static int menuOperationCnt = 240;
    private int menuBufferCntInit = 75;
    private int menuBufferCnt;
    private bool menuBufferFlg = true;

    private int startPageCnt = 45;  // 開始時のページがめくれるまでの時間
    private int endBookCnt;    // 終了時の本が閉じるまでの時間

    private bool statusFirstFlg = true;
    private bool menuCommandFirstFlg = true;
    private bool clearCommandFirstFlg = true;
    private bool goldMedalFlg = false;
    private bool inputFlg = false;

    private bool stageDisplayFlg = true;
    public static bool nextPossibleFlg = true;

    // シーンの状態
    private enum STATUS
    {
        START,
        PLAY,
        MENU,
        HINT,
        CLEAR,
        COMMAND_DECISION,
    }
    private STATUS status;

    // 選択中のコマンド
    private enum COMMAND
    {
        STAGE_SELECT,
        RETRY,
        NEXT,
    }
    private COMMAND command;



    //==============================================================
    // 初期処理
    //==============================================================
    private void Awake()
    {
        // ステージ番号取得
        stageNum = StageManager.stageNum;

        // ステージの画像を取得
        stageImage = GameObject.Find("StageImage");
        Sprite sprite = Resources.Load<Sprite>("Sprite/Stage/" + StageManager.stageNum);
        stageImage.GetComponent<Image>().sprite = sprite;

        // ステージ番号のUIの設定
        Image tensPlaceImage = GameObject.Find("TensPlaceImage").GetComponent<Image>();
        sprite = Resources.Load<Sprite>("Sprite/Number/" + (stageNum / 10));
        tensPlaceImage.sprite = sprite;

        Image onesPlaceImage = GameObject.Find("OnesPlaceImage").GetComponent<Image>();
        sprite = Resources.Load<Sprite>("Sprite/Number/" + (stageNum % 10));
        onesPlaceImage.sprite = sprite;

        eventSystem = GameObject.Find("EventSystem");
        stageManager = GameObject.Find("stageManager");
        tutorialUI = GameObject.Find("TutorialUI");
        editCanvas = GameObject.Find("EditCanvas");
        player = GameObject.Find("Player");
        bookL = GameObject.Find("book_L");
        menuSelectGear = GameObject.Find("SelectGearImage");
        menuRetryGear = GameObject.Find("RetryGearImage");
        menuBoard = GameObject.Find("MenuBoard");
        menuStageNum = GameObject.Find("StageNum");
        menuOperation1 = GameObject.Find("OperationImage1");
        menuOperation2 = GameObject.Find("OperationImage2");
        clearSelectGear = GameObject.Find("C_SelectGearImage");
        clearSelectGear2 = GameObject.Find("SelectUnderGearImage");
        clearNextGear = GameObject.Find("NextGearImage");
        clearNextGear2 = GameObject.Find("NextUnderGearImage");
        menuSelect = GameObject.Find("StageSelect");
        menuRetry = GameObject.Find("Retry");
        clearSelect = GameObject.Find("C_StageSelect");
        clearNext = GameObject.Find("NextStage");
        hintBoard = GameObject.Find("HintBoard");
        hintMovie = GameObject.Find("SamnaleMovie");
        tv = GameObject.Find("UiTv");
        hukidasi = GameObject.Find("HukidasiImage");
        hintUI = GameObject.Find("UICanvas");
        copper = GameObject.Find("CopperImage");
        silver = GameObject.Find("SilverImage");
        gold = GameObject.Find("GoldImage");
        eIcon = GameObject.Find("Eicon");

        tv.transform.localScale = new Vector3(0, 0, 0);
        hukidasi.transform.localScale = new Vector3(0, 0, 0);

        // メダルを取得していたら色が付く
        if (StageSelectManager.score[stageNum].isGold)
        {
            gold.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else gold.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

        if (StageSelectManager.score[stageNum].isSilver)
        {
            silver.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else silver.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

        if (StageSelectManager.score[stageNum].isCopper)
        {
            copper.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else copper.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);



        GameObject.Find("book_L2").GetComponent<Renderer>().material = material[BookSelect.bookNum];
        GameObject.Find("book_R2").GetComponent<Renderer>().material = material[BookSelect.bookNum];

        hintMovie.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("Movie/Hint/stage_" + stageNum + "_hint");

        // ステージ１はチュートリアルのBGM
        if (StageManager.stageNum == 1) bgmNum = 0;
        // それ以外はステージごとのBGM
        else bgmNum = BookSelect.bookNum + 1;

        // ネクスト以外を選択していたらBGMをセットしなおす
        if (StageBgm.bgmFlg)
        {
            bgmSource = GameObject.Find("StageBGM").GetComponent<AudioSource>();
            AudioClip audio = Resources.Load("Sound/bgm/mainBGM_" + bgmNum, typeof(AudioClip)) as AudioClip;
            bgmSource.clip = audio;
            bgmSource.time = 0.0f;
            bgmSource.Play();
            //StageBgm.bgmFlg = false;
        }

        if (stageNum != 1)
        {
            tutorialUI.SetActive(false);
            GameObject tutorialTV = GameObject.Find("TVCanvas");
            tutorialTV.SetActive(false);
        }
        // エディットを非表示
        if (!editFlg) editCanvas.SetActive(false);

        StageDisplay(false);

        // 変数初期化
        status = STATUS.START;
        stageDisplayCnt = stageDisplayCntInit;
        clearCommandOperationCnt = clearCommandOperationCntInit;
        gameOverCnt = gameOverCntInit;
        menuBufferCnt = menuBufferCntInit;

        bookLAnim = bookL.GetComponent<Animator>();
        bookLAnim.SetBool("isAnim", true);

        if (debugFlg)
        {
            stageNumText = GameObject.Find("StageNumText").GetComponent<Text>();
            stageNumText.text = "ステージ番号:" + StageManager.stageNum;

            hintTimeText = GameObject.Find("HintTimeText").GetComponent<Text>();

            rotateNumText = GameObject.Find("RotateNumText").GetComponent<Text>();

            silverMedalNumText = GameObject.Find("SilverMedalNumText").GetComponent<Text>();
            silverMedalNumText.text = "銀メダルの回数:" + StageSelectManager.silverConditions[StageManager.stageNum];

            //inputField = GameObject.Find("InputField").GetComponent<InputField>();
        }
        else
        {
            debug = GameObject.Find("DebugCanvas");
            debug.SetActive(false);
        }
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {

        STATUS tempStatus = status;

        switch (status)
        {
            //-----------------------------------
            // 開始後
            //-----------------------------------
            case STATUS.START:
                if (startPageCnt == 0)
                {
                    eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();
                    status = STATUS.PLAY;
                }
                else startPageCnt--;
                break;

            //-----------------------------------
            // プレイ中
            //-----------------------------------
            case STATUS.PLAY:

                if (stageNum == 1)
                {
                    Point = GameObject.FindGameObjectWithTag("Point");

                }
                // カウントが０になるとプレイヤーとステージを表示する
                if (stageDisplayCnt == 0)
                {
                    if (StageBgm.bgmFlg) StageBgm.bgmFlg = false;
                    StageDisplay(true);
                    stageImage.SetActive(false);
                    if (stageNum == 1) tutorialUI.SetActive(true);
                }
                else stageDisplayCnt--;

                // プレイヤーが落ちたらリトライ
                if (stageManager.GetComponent<StageManager>().IsGameOver)
                {
                    if (gameOverCnt == 0)
                    {
                        if (stageNum == 1) tutorialUI.SetActive(false);
                        stageManager.SetActive(false);
                        changeSceneName = "Stage1Scene";
                        status = STATUS.COMMAND_DECISION;
                        command = COMMAND.RETRY;
                        sceneChangeCnt = 120;
                        endBookCnt = 0;
                    }
                    else gameOverCnt--;
                }

                if (menuBufferCnt == 0)
                {
                    menuBufferFlg = false;
                    menuBufferCnt = menuBufferCntInit;
                }
                else menuBufferCnt--;

                // メニューを開く
                if (menuOperationCnt == 0)
                {
                    if (stageNum == 1) { }
                    else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick button 7"))
                    {
                        if (!menuBufferFlg)
                        {
                            if (!stageManager.GetComponent<StageManager>().isMove)
                            {
                                menuBufferFlg = true;
                                status = STATUS.MENU;
                                stageImage.SetActive(true);

                                // ページを進める
                                eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();

                                // チュートリアル非表示
                                if (stageNum == 1)
                                {
                                    Point.SetActive(false);
                                    tutorialUI.SetActive(false);
                                }

                                stageManager.GetComponent<StageManager>().SetModeGoalEffect(0);
                                stageManager.GetComponent<StageManager>().SetModeGoalEffect(2);
                            }
                        }
                    }
                }
                else menuOperationCnt--;

                // ヒントを開く
                if (hintFlg)
                {
                    if (menuOperationCnt == 0)
                    {
                        if (stageNum == 1) { }
                        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 7"))
                        {
                            if (!menuBufferFlg)
                            {
                                if (!stageManager.GetComponent<StageManager>().isMove)
                                {
                                    menuBufferFlg = true;
                                    status = STATUS.HINT;
                                    stageImage.SetActive(true);
                                    hintOpenCnt++;

                                    // ページを進める
                                    eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();

                                    // チュートリアル非表示
                                    if (stageNum == 1)
                                    {
                                        Point.SetActive(false);
                                        tutorialUI.SetActive(false);
                                    }

                                    stageManager.GetComponent<StageManager>().SetModeGoalEffect(0);
                                    stageManager.GetComponent<StageManager>().SetModeGoalEffect(2);
                                }
                            }
                        }
                    }
                    else menuOperationCnt--;
                }

                // ステージクリア
                if (stageManager.GetComponent<StageManager>().IsGameClear || Input.GetKeyDown(KeyCode.C))
                {
                    status = STATUS.CLEAR;
                }
                if (goldMedalFlg)
                {
                    stageManager.GetComponent<StageManager>().SetModeGoalEffect(4);
                }
                break;

            //-----------------------------------
            // メニュー表示中
            //-----------------------------------
            case STATUS.MENU:
                if (statusFirstFlg)
                {
                    StageDisplay(false);
                    statusFirstFlg = false;

                    menuRetry.SetActive(true);
                    menuSelect.SetActive(true);
                    menuBoard.SetActive(true);
                    menuStageNum.SetActive(true);
                    menuOperation1.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    menuOperation2.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    hintBoard.SetActive(false);
                    hintMovie.SetActive(false);
                    copper.SetActive(true);
                    silver.SetActive(true);
                    gold.SetActive(true);
                }

                // メニューコマンド更新処理
                MenuCommandOperation();

                if (menuBufferCnt == 0)
                {
                    menuBufferFlg = false;
                    menuBufferCnt = menuBufferCntInit;
                }
                else menuBufferCnt--;

                // コマンド決定
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("joystick button 0"))
                {
                    status = STATUS.COMMAND_DECISION;
                    selectDecSource.Play();
                    sceneChangeCnt = 120;
                    endBookCnt = 0;
                }

                // メニューを閉じる
                if (!menuBufferFlg)
                {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick button 7"))
                    {
                        status = STATUS.PLAY;
                        menuBufferFlg = true;

                        // ページを戻す
                        eventSystem.GetComponent<IgnoreMouseInputModule>().BackPage();

                        // ステージを表示するまでの時間をセット
                        stageDisplayCnt = stageDisplayCntInit;

                        stageManager.GetComponent<StageManager>().FixPlayerPos();
                        stageManager.GetComponent<StageManager>().SetModeGoalEffect(3);

                        // チュートリアル表示
                        if (stageNum == 1)
                        {
                            Point.SetActive(true);
                            tutorialUI.SetActive(true);
                        }
                    }
                }
                break;

            //-----------------------------------
            // ヒント表示中
            //-----------------------------------
            case STATUS.HINT:
                if (statusFirstFlg)
                {
                    StageDisplay(false);
                    statusFirstFlg = false;

                    menuRetry.SetActive(false);
                    menuSelect.SetActive(false);
                    menuBoard.SetActive(false);
                    menuStageNum.SetActive(false);
                    menuOperation1.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    menuOperation2.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    copper.SetActive(false);
                    silver.SetActive(false);
                    gold.SetActive(false);
                    hintBoard.SetActive(true);
                    hintMovie.SetActive(true);
                }

                if (menuBufferCnt == 0)
                {
                    menuBufferFlg = false;
                    menuBufferCnt = menuBufferCntInit;
                }
                else menuBufferCnt--;

                // ヒントを閉じる
                if (!menuBufferFlg)
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 7"))
                    {
                        status = STATUS.PLAY;
                        menuBufferFlg = true;

                        // ページを戻す
                        eventSystem.GetComponent<IgnoreMouseInputModule>().BackPage();

                        // ステージを表示するまでの時間をセット
                        stageDisplayCnt = stageDisplayCntInit;

                        stageManager.GetComponent<StageManager>().FixPlayerPos();
                        stageManager.GetComponent<StageManager>().SetModeGoalEffect(3);

                        // チュートリアル表示
                        if (stageNum == 1)
                        {
                            Point.SetActive(true);
                            tutorialUI.SetActive(true);
                        }
                    }
                }
                break;

            //-----------------------------------
            // クリア後
            //-----------------------------------
            case STATUS.CLEAR:
                if (statusFirstFlg)
                {
                    // スコアアニメーション開始
                    this.GetComponent<ScoreAnimation>().StartFlgOn();

                    // チュートリアル非表示
                    if (stageNum == 1) tutorialUI.SetActive(false);

                    AudioSource bgm = GameObject.Find("StageBGM").GetComponent<AudioSource>();
                    bgm.Stop();
                    resultSource.Play();

                    // 銅メダル取得
                    StageSelectManager.score[StageManager.stageNum].isCopper = true;

                    // 銀メダル取得
                    SilverMedalConditions();

                    // 金メダル取得
                    if (goldMedalFlg)
                    {
                        StageSelectManager.score[StageManager.stageNum].isGold = true;
                        this.GetComponent<ScoreAnimation>().GoldFlgOn();
                    }

                    // メダルの取得状況を保存
                    MedalDataSave();

                    // ネクストステージが選択可能か判定
                    if (StageManager.stageNum % 6 == 5)
                    {
                        if (StageSelectManager.enterExtraFlg[BookSelect.bookNum] == true) nextPossibleFlg = true;
                        else nextPossibleFlg = false;
                    }
                    else nextPossibleFlg = true;
                    if (StageManager.stageNum % 6 == 0) nextPossibleFlg = false;

                    if (nextPossibleFlg) command = COMMAND.NEXT;
                    else command = COMMAND.STAGE_SELECT;

                    statusFirstFlg = false;
                }

                if (clearCommandOperationCnt == 0)
                {
                    if (this.GetComponent<ScoreAnimation>().GetOperationFlg())
                    {
                        // クリアコマンド更新処理
                        ClearCommandOperation();

                        // コマンド決定
                        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("joystick button 0"))
                        {
                            stageManager.GetComponent<StageManager>().SetModeGoalEffect(0);
                            stageManager.GetComponent<StageManager>().SetModeGoalEffect(2);
                            status = STATUS.COMMAND_DECISION;
                            selectDecSource.Play();
                            this.GetComponent<ScoreAnimation>().EndFlgOn();
                            sceneChangeCnt = 180;
                            endBookCnt = 90;
                        }
                    }
                }
                else clearCommandOperationCnt--;

                break;

            //-----------------------------------
            // コマンド決定後
            //-----------------------------------
            case STATUS.COMMAND_DECISION:
                if (statusFirstFlg)
                {
                    // ランタンの火を消す
                    this.GetComponent<PostEffectController>().SetFireFlg(false);
                    if (command == COMMAND.NEXT) StageSelectManager.selectPageNum++;

                    statusFirstFlg = false;
                }

                // 本のモデルを閉じる
                if (endBookCnt == 0) eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
                else endBookCnt--;

                // 一定時間経過すると遷移する
                if (sceneChangeCnt == 0)
                {
                    // ネクスト以外を選択していたらBGMをセットしなおして、失敗回数を初期化する
                    if (command != COMMAND.RETRY)
                    {
                        StageBgm.bgmFlg = true;
                        missCnt = 0;
                        hintCnt = 0.0f;
                        hintOpenCnt = 0;
                        menuOperationCnt = 240;
                    }
                    // ネクストを選択していたら失敗回数を加算
                    else
                    {
                        missCnt++;
                        menuOperationCnt = 60;
                    }

                    SceneManager.LoadScene(changeSceneName);
                }
                else sceneChangeCnt--;
                break;

            default:
                break;
        }

        if (tempStatus != status) statusFirstFlg = true;

        // デバッグ用テキスト更新処理
        if (debugFlg) DebugUpdate();

        // ヒント通知UI更新処理
        HintUIUpdate();
    }

    //==============================================================
    // メニューコマンド更新処理
    //==============================================================
    private void MenuCommandOperation()
    {
        switch (command)
        {
            //-----------------------------------
            // ステージセレクト選択中
            //-----------------------------------
            case COMMAND.STAGE_SELECT:
                if (menuCommandFirstFlg)
                {
                    changeSceneName = "StageSelectScene";
                    // 歯車回転
                    menuSelectGear.GetComponent<GearRotation>().SetRotFlg(true);
                    Debug.Log("select");
                    menuSelect.transform.localScale = new Vector3(2.75f, 2.75f, 1.0f);
                    menuRetryGear.GetComponent<GearRotation>().SetRotFlg(false);
                    menuRetry.transform.localScale = new Vector3(2.5f, 2.5f, 1.0f);
                    menuCommandFirstFlg = false;
                }

                if (Input.GetAxis("Vertical") < 0)
                {
                    command = COMMAND.RETRY;
                    menuCommandFirstFlg = true;
                }
                break;

            //-----------------------------------
            // リトライ選択中
            //-----------------------------------
            case COMMAND.RETRY:
                if (menuCommandFirstFlg)
                {
                    changeSceneName = SceneManager.GetActiveScene().name;
                    // 歯車回転
                    menuSelectGear.GetComponent<GearRotation>().SetRotFlg(false);
                    menuSelect.transform.localScale = new Vector3(2.5f, 2.5f, 1.0f);
                    menuRetryGear.GetComponent<GearRotation>().SetRotFlg(true);
                    menuRetry.transform.localScale = new Vector3(2.75f, 2.75f, 1.0f);
                    menuCommandFirstFlg = false;
                }

                if (Input.GetAxis("Vertical") > 0)
                {
                    command = COMMAND.STAGE_SELECT;
                    menuCommandFirstFlg = true;
                }
                break;

            default:
                break;
        }
    }

    //==============================================================
    // クリアコマンド更新処理
    //==============================================================
    private void ClearCommandOperation()
    {
        switch (command)
        {
            //-----------------------------------
            // ステージセレクト選択中
            //-----------------------------------
            case COMMAND.STAGE_SELECT:
                if (clearCommandFirstFlg)
                {
                    changeSceneName = "StageSelectScene";
                    // 歯車回転
                    clearSelectGear.GetComponent<GearRotation>().SetRotFlg(true);
                    clearSelectGear2.GetComponent<GearRotation>().SetRotFlg(true);
                    clearNextGear.GetComponent<GearRotation>().SetRotFlg(false);
                    clearNextGear2.GetComponent<GearRotation>().SetRotFlg(false);
                    clearSelect.transform.localScale = new Vector3(1.3f, 1.3f, 1.0f);
                    clearNext.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);

                    clearCommandFirstFlg = false;
                }
                if (nextPossibleFlg)
                {
                    if (Input.GetAxis("Horizontal") > 0)
                    {
                        command = COMMAND.NEXT;
                        clearCommandFirstFlg = true;
                    }
                }
                break;

            //-----------------------------------
            // ネクスト選択中
            //-----------------------------------
            case COMMAND.NEXT:
                if (clearCommandFirstFlg)
                {
                    StageManager.stageNum = stageNum + 1;
                    changeSceneName = "Stage1Scene";
                    // 歯車回転
                    clearSelectGear.GetComponent<GearRotation>().SetRotFlg(false);
                    clearSelectGear2.GetComponent<GearRotation>().SetRotFlg(false);
                    clearNextGear.GetComponent<GearRotation>().SetRotFlg(true);
                    clearNextGear2.GetComponent<GearRotation>().SetRotFlg(true);
                    clearNext.transform.localScale = new Vector3(1.3f, 1.3f, 1.0f);
                    clearSelect.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);

                    clearCommandFirstFlg = false;
                }

                if (Input.GetAxis("Horizontal") < 0)
                {
                    command = COMMAND.STAGE_SELECT;
                    clearCommandFirstFlg = true;
                }
                break;

            default:
                break;
        }
    }

    //==============================================================
    // 銀メダル取得処理
    //==============================================================
    private void SilverMedalConditions()
    {
        if (StageSelectManager.silverConditions[StageManager.stageNum] >=
            stageManager.GetComponent<StageManager>().rotateNum)
        {
            StageSelectManager.score[StageManager.stageNum].isSilver = true;
            this.GetComponent<ScoreAnimation>().SilverFlgOn();
            Debug.Log("ノルマ" + StageSelectManager.silverConditions[1]);
            Debug.Log("折った回数" + stageManager.GetComponent<StageManager>().rotateNum);
        }
    }

    //==============================================================
    // デバッグ用テキスト更新処理
    //==============================================================
    private void DebugUpdate()
    {
        hintTimeText.text = "タイマー:" + hintCnt.ToString("f2");

        rotateNumText.text = "折った回数:" + stageManager.GetComponent<StageManager>().rotateNum;

        //if (!inputFlg)
        //{
        //    if (Input.GetKeyDown(KeyCode.F1))
        //    {
        //        inputField.ActivateInputField();
        //        inputFlg = true;
        //    }
        //}
        //else
        //{
        //    if (Input.GetKeyDown(KeyCode.Return))
        //    {
        //        StageManager.stageNum = int.Parse(inputField.text);
        //        SceneManager.LoadScene("Stage1Scene");
        //    }
        //}
    }

    //==============================================================
    // ステージ表示切替
    //==============================================================
    public void StageDisplay(bool sts)
    {
        if (stageDisplayFlg == sts) return;

        if (sts)
        {
            // プレイヤーとステージを表示
            player.SetActive(true);
            //player.GetComponent<Player>().waitMoveTimer = 0.5f;
            stageManager.SetActive(true);
            if (!stageManager.GetComponent<StageManager>().initFlg)
            {
                stageManager.GetComponent<StageManager>().CreateParticle();
            }
            frontEffectCamera.SetActive(true);
        }
        else
        {
            // プレイヤーとステージを非表示
            player.SetActive(false);
            stageManager.SetActive(false);
            stageManager.GetComponent<StageManager>().DeleteCopyForMenu();
            frontEffectCamera.SetActive(false);
        }
        stageDisplayFlg = sts;
    }

    //==============================================================
    // メダルの取得状況を保存
    //==============================================================
    private void MedalDataSave()
    {
        int temp = System.Convert.ToInt32(StageSelectManager.score[stageNum].isCopper);
        PlayerPrefs.SetInt("Copper" + stageNum, temp);
        temp = System.Convert.ToInt32(StageSelectManager.score[stageNum].isSilver);
        PlayerPrefs.SetInt("Silver" + stageNum, temp);
        temp = System.Convert.ToInt32(StageSelectManager.score[stageNum].isGold);
        PlayerPrefs.SetInt("Gold" + stageNum, temp);

        StageSelectManager.SaveClearDate();

        Debug.Log("メダルデータ セーブ完了");
    }

    //==============================================================
    // ヒント通知UI更新処理
    //==============================================================
    private void HintUIUpdate()
    {
        hintCnt += Time.deltaTime;

        // ヒント通知UI表示
        if (hintCnt > hintDispTime && hintOpenCnt == 0 && stageNum != 1)
        {
            hintFirstFlg = true;
            hintOpenCnt++;
        }

        // ヒント通知UI非表示
        if (hintCnt > hintDeleteTime && stageNum != 1)
        {
            hintUIFlg = false;
        }

        if (hintFlg)
        {
            if (eIconTime == 0)
            {
                if (eIconNum == 0) eIconNum = 1;
                else eIconNum = 0;
                eIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/e_" + eIconNum);
                eIconTime = eIconTimeInit;
            }
            else eIconTime--;
        }

        if (hintFirstFlg)
        {
            hintFlg = true;
            hintUIFlg = true;
            tv.transform.DOScale(new Vector3(4000, 4000, 1100), 0.5f);
            hukidasi.transform.DOScale(new Vector3(2.5f, 2.5f, 1), 0.5f);
            hintFirstFlg = false;
        }

        if (hintFlg && !hintUIFlg)
        {
            tv.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
            hukidasi.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
        }
    }



    public bool GetStageDisplayFlg() { return stageDisplayFlg; }

    public void StageImageDisplay(bool sts)
    {
        stageImage.SetActive(sts);
    }

    public void SetGoldMedalFlg(bool sts)
    {
        goldMedalFlg = sts;
    }
}