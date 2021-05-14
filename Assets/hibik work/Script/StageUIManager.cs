using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    private GameObject eventSystem;
    private GameObject stageManager;
    private GameObject tutorialUI;
    private GameObject editCanvas;
    private GameObject player;
    public  GameObject frontEffectCamera;

    private GameObject bookL;
    private Animator bookLAnim;

    // メニューのUI
    private GameObject menuSelectGear;
    private GameObject menuRetryGear;

    // クリアのUI
    private GameObject clearSelectGear;
    private GameObject clearSelectGear2;
    private GameObject clearNextGear;
    private GameObject clearNextGear2;

    // サウンド
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource resultSource;
    [SerializeField] private AudioSource selectDecSource;

    [SerializeField] private bool editFlg = false;  // true:エディット表示
    [SerializeField] private int stageNum;   // ステージ番号

    // デバック用テキスト
    Text stageNumText;
    Text rotateNumText;
    Text silverMedalNumText;
    InputField inputField;

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

    private int startPageCnt = 45;  // 開始時のページがめくれるまでの時間
    private int endBookCnt;    // 終了時の本が閉じるまでの時間

    private bool statusFirstFlg = true;
    private bool menuCommandFirstFlg = true;
    private bool clearCommandFirstFlg = true;
    private bool goldMedalFlg = false;
    private bool inputFlg = false;

    private bool stageDisplayFlg = true;

    // シーンの状態
    private enum STATUS
    {
        START,
        PLAY,
        MENU,
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
        eventSystem = GameObject.Find("EventSystem");
        stageManager = GameObject.Find("stageManager");
        tutorialUI = GameObject.Find("TutorialUI");
        editCanvas = GameObject.Find("EditCanvas");
        player = GameObject.Find("Player");
        bookL = GameObject.Find("book_L");
        menuSelectGear = GameObject.Find("SelectGearImage");
        menuRetryGear = GameObject.Find("RetryGearImage");
        clearSelectGear = GameObject.Find("C_SelectGearImage");
        clearSelectGear2 = GameObject.Find("SelectUnderGearImage");
        clearNextGear = GameObject.Find("NextGearImage");
        clearNextGear2 = GameObject.Find("NextUnderGearImage");

        // ステージ番号取得
        stageNum = StageManager.stageNum;

        if (stageNum != 1) tutorialUI.SetActive(false);

        // エディットを非表示
        if (!editFlg) editCanvas.SetActive(false);

        StageDisplay(false);

        // 変数初期化
        status = STATUS.START;
        stageDisplayCnt = stageDisplayCntInit;
        clearCommandOperationCnt = clearCommandOperationCntInit;
        gameOverCnt = gameOverCntInit;

        bookLAnim = bookL.GetComponent<Animator>();
        bookLAnim.SetBool("isAnim", true);

        stageNumText = GameObject.Find("StageNumText").GetComponent<Text>();
        stageNumText.text = "ステージ番号:" + StageManager.stageNum;

        rotateNumText = GameObject.Find("RotateNumText").GetComponent<Text>();

        silverMedalNumText = GameObject.Find("SilverMedalNumText").GetComponent<Text>();
        silverMedalNumText.text = "銀メダルの回数:" + StageSelectManager.silverConditions[StageManager.stageNum];

        inputField = GameObject.Find("InputField").GetComponent<InputField>();
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
                // カウントが０になるとプレイヤーとステージを表示する
                if (stageDisplayCnt == 0)
                {
                    StageDisplay(true);
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
                        sceneChangeCnt = 120;
                        endBookCnt = 0;
                    }
                    else gameOverCnt--;
                }

                // メニューを開く
                if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 7"))
                {
                    status = STATUS.MENU;

                    // ページを進める
                    eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();

                    // チュートリアル非表示
                    if (stageNum == 1) tutorialUI.SetActive(false);
                }

                // ステージクリア
                if (stageManager.GetComponent<StageManager>().IsGameClear || Input.GetKeyDown(KeyCode.C))
                {
                    status = STATUS.CLEAR;
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
                }

                // メニューコマンド更新処理
                MenuCommandOperation();

                // コマンド決定
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
                {
                    status = STATUS.COMMAND_DECISION;
                    selectDecSource.Play();
                    sceneChangeCnt = 120;
                    endBookCnt = 0;
                }

                // メニューを閉じる
                if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 7"))
                {
                    status = STATUS.PLAY;

                    // ページを戻す
                    eventSystem.GetComponent<IgnoreMouseInputModule>().BackPage();

                    // ステージを表示するまでの時間をセット
                    stageDisplayCnt = stageDisplayCntInit;
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

                    bgmSource.Stop();
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

                    statusFirstFlg = false;
                }

                if (clearCommandOperationCnt == 0)
                {
                    // クリアコマンド更新処理
                    ClearCommandOperation();

                    // コマンド決定
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
                    {
                        status = STATUS.COMMAND_DECISION;
                        selectDecSource.Play();
                        this.GetComponent<ScoreAnimation>().EndFlgOn();
                        sceneChangeCnt = 180;
                        endBookCnt = 90;
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

                    statusFirstFlg = false;
                }

                // 本のモデルを閉じる
                if (endBookCnt == 0) eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
                else endBookCnt--;

                // 一定時間経過すると遷移する
                if (sceneChangeCnt == 0) SceneManager.LoadScene(changeSceneName);
                else sceneChangeCnt--;
                break;

            default:
                break;
        }

        if (tempStatus != status) statusFirstFlg = true;

        // デバッグ用テキスト更新処理
        DebugUpdate();
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
                    changeSceneName = "SelectScene";
                    // 歯車回転
                    menuSelectGear.GetComponent<GearRotation>().SetRotFlg(true);
                    menuRetryGear.GetComponent<GearRotation>().SetRotFlg(false);
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
                    menuRetryGear.GetComponent<GearRotation>().SetRotFlg(true);
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
                    changeSceneName = "SelectScene";
                    // 歯車回転
                    clearSelectGear.GetComponent<GearRotation>().SetRotFlg(true);
                    clearSelectGear2.GetComponent<GearRotation>().SetRotFlg(true);
                    clearNextGear.GetComponent<GearRotation>().SetRotFlg(false);
                    clearNextGear2.GetComponent<GearRotation>().SetRotFlg(false);
                    clearCommandFirstFlg = false;
                }

                if (Input.GetAxis("Horizontal") > 0)
                {
                    command = COMMAND.NEXT;
                    clearCommandFirstFlg = true;
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
        rotateNumText.text = "折った回数:" + stageManager.GetComponent<StageManager>().rotateNum;

        if (!inputFlg)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                inputField.ActivateInputField();
                inputFlg = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StageManager.stageNum = int.Parse(inputField.text);
                SceneManager.LoadScene("Stage1Scene");
            }
        }
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
            stageManager.SetActive(true);
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

    public void SetGoldMedalFlg(bool sts)
    {
        goldMedalFlg = sts;
    }
}