using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject stageManager;
    [SerializeField] private GameObject editCanvas;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject bookL;
    private Animator bookLAnim;

    // メニューのUI
    [SerializeField] private GameObject menuSelectGear;
    [SerializeField] private GameObject menuRetryGear;

    // クリアのUI
    [SerializeField] private GameObject clearSelectGear;
    [SerializeField] private GameObject clearSelectGear2;
    [SerializeField] private GameObject clearNextGear;
    [SerializeField] private GameObject clearNextGear2;

    // サウンド
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource resultSource;
    [SerializeField] private AudioSource selectDecSource;

    [SerializeField] private bool editFlg = false;  // true:エディット表示
    [SerializeField] private int stageNum;   // ステージ番号

    // シーン遷移までの時間
    private int sceneChangeCnt;
    private string changeSceneName;

    // プレイヤーとステージを表示するまでの時間
    [SerializeField] private int stageDisplayCntInit;
    private int stageDisplayCnt = 90;

    // クリアコマンドが操作可能になるまでの時間
    [SerializeField] private int clearCommandOperationCntInit;
    private int clearCommandOperationCnt = 0;

    private int startPageCnt = 45;  // 開始時のページがめくれるまでの時間
    private int endBookCnt;    // 終了時の本が閉じるまでの時間

    private bool statusFirstFlg = true;
    private bool menuCommandFirstFlg = true;
    private bool clearCommandFirstFlg = true;
    private bool goldMedalFlg = false;

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
        // ステージ番号取得
        stageNum = StageManager.stageNum;

        // エディットを非表示
        if (!editFlg) editCanvas.SetActive(false);

        StageDisplay(false);

        // 変数初期化
        status = STATUS.START;
        stageDisplayCnt = stageDisplayCntInit;
        clearCommandOperationCnt = clearCommandOperationCntInit;

        bookLAnim = bookL.GetComponent<Animator>();
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
                }
                else stageDisplayCnt--;

                // メニューを開く
                if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 3"))
                {
                    status = STATUS.MENU;

                    // ページを進める
                    eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();
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
                if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 3"))
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

                    bgmSource.Stop();
                    resultSource.Play();

                    // 銅メダル取得
                    StageSelectManager.score[StageManager.stageNum].isCopper = true;

                    // 銀メダル取得
                    SilverMedalConditions();

                    // 金メダル取得
                    if(goldMedalFlg) StageSelectManager.score[StageManager.stageNum].isGold = true;

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
                    changeSceneName = "Stage1Scene";
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
            Debug.Log("ノルマ" + StageSelectManager.silverConditions[1]);
            Debug.Log("折った回数" + stageManager.GetComponent<StageManager>().rotateNum);
        }
    }

    //==============================================================
    // ステージ表示切替
    //==============================================================
    public void StageDisplay(bool sts)
    {
        if(sts)
        {
            // プレイヤーとステージを表示
            player.SetActive(true);
            stageManager.SetActive(true);
        }
        else
        {
            // プレイヤーとステージを非表示
            player.SetActive(false);
            stageManager.SetActive(false);
        }
    }

    public void SetGoldMedalFlg(bool sts)
    {
        goldMedalFlg = sts;
    }
}