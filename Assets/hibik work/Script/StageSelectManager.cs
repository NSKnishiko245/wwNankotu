using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectManager : MonoBehaviour
{
    private GameObject eventSystem;
    [SerializeField] private GameObject bookL;
    private Animator bookLAnim;
    [SerializeField] private GameObject bookBack;
    private GameObject book;
    private GameObject bookUI;
    private Animator cameraAnim;
    [SerializeField] private int bookNum;
    [SerializeField] GameObject mist;
    private int bookMax = 6;    // 本の最大数
    private int bookStageMax = 6;  // 一冊あたりのステージの最大数
    private int stageMax = 42;
    private int firstStageNum;
    private int endStageNum;


    private int bookSelectCntInit = 30;
    private int bookSelectCnt = 0;
    private bool bookSelectFlg = false;
    private int bookRemoveCnt = 90;
    private bool stageEnterFlg = true;

    public static int selectPageNum = 1;
    public static bool selectPageMoveFlg = false;

    // サウンド
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource DecSource;
    [SerializeField] private AudioSource BuSource;



    private int sceneChangeCnt = 60;                 // シーン遷移のカウンタ
    private bool sceneChangeFlg = false;

    enum COMMAND
    {
        EMPTY,
        BOOK_SELECT,
        STAGE,
    }
    COMMAND command;

    [SerializeField] private int pageIntervalInit;  // ページをめくれるまでの待機時間の初期値
    private int pageInterval = 0;

    [SerializeField] private int operationCntInit;  // シーン遷移して操作可能になるまでの時間
    private int operationCnt = 0;

    // メダル
    private GameObject[] goldMedal;
    private GameObject[] silverMedal;
    private GameObject[] copperMedal;
    public struct Score
    {
        public bool isGold;
        public bool isSilver;
        public bool isCopper;
    }
    public static Score[] score = new Score[43];
    public static int[] silverConditions = new int[43];
    public static bool[] enterExtraFlg = new bool[6];

    //==============================================================
    // 初期処理
    //==============================================================
    private void Awake()
    {
        StageSelectManager.selectPageMoveFlg = true;

        pageInterval = pageIntervalInit;
        operationCnt = operationCntInit;
        eventSystem = GameObject.Find("EventSystem");
        bookLAnim = bookL.GetComponent<Animator>();
        bookLAnim.SetBool("isAnim", true);
        bookBack.SetActive(false);
        book = GameObject.Find("BookModel");
        bookUI = GameObject.Find("BookCanvas");
        cameraAnim = GameObject.Find("Main Camera").GetComponent<Animator>();
        mist = GameObject.Find("Mist");
        mist.SetActive(false);
        //score = new Score[stageMax];
        //silverConditions = new int[stageMax];
        goldMedal = new GameObject[stageMax + 1];
        silverMedal = new GameObject[stageMax + 1];
        copperMedal = new GameObject[stageMax + 1];
        command = COMMAND.EMPTY;

        BookSelect.bookNum = bookNum - 1;
        for (int i = 0; i < bookMax; i++)
        {
            if (i == BookSelect.bookNum)
            {
                firstStageNum = BookSelect.bookNum * bookStageMax + 1;
                endStageNum = firstStageNum + 5;
                break;
            }
        }

        for (int i = firstStageNum; i <= endStageNum; i++)
        {
            goldMedal[i] = GameObject.Find("GoldImage" + i);
            silverMedal[i] = GameObject.Find("SilverImage" + i);
            copperMedal[i] = GameObject.Find("CopperImage" + i);
            if (score[i].isGold) goldMedal[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            else goldMedal[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

            if (score[i].isSilver) silverMedal[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            else silverMedal[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

            if (score[i].isCopper) copperMedal[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            else copperMedal[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);
        }

        for (int i = 0; i <= 6; i++)
        {
            score[i].isCopper = true;
            score[i].isSilver = true;
            score[i].isGold = true;
        }

        SilverConditionsSet();

        this.GetComponent<PostEffectController>().SetVigFlg(false);
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {
        // １ページ目をめくる
        if (operationCnt == 1) eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();

        // 操作可能
        if (operationCnt == 0)
        {
            if (selectPageMoveFlg)
            {
                if (pageInterval == 0)
                {
                    // 次のページへ進む
                    eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();
                    pageInterval = pageIntervalInit;
                }
                else pageInterval--;

                if (selectPageNum == eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum())
                {
                    selectPageMoveFlg = false;
                }
            }
            else
            {
                // 本が閉じるときは操作不能にする
                if (!eventSystem.GetComponent<IgnoreMouseInputModule>().GetAllBackFlg())
                {
                    PageOperation(); // ページをめくる操作
                }
            }
        }
        else operationCnt--;

        if (!selectPageMoveFlg)
        {
            ExtraConditions();  // エクストラステージに入れるかチェック
            StageSceneChange(); // ステージ画面への遷移
            BookSelectChange(); // 本の選択画面への遷移
        }

        BookClearCheck(1, 5, 0);
        BookClearCheck(7, 11, 1);
        BookClearCheck(13, 17, 2);
        BookClearCheck(19, 23, 3);
        BookClearCheck(25, 29, 4);
    }

    //==============================================================
    // 難易度ごとにクリアしたかチェック
    //==============================================================
    private void BookClearCheck(int startNum, int endNum, int bookNum)
    {
        int stageNum = endNum - startNum + 1;
        int clearCnt = 0;
        for (int cnt = startNum; cnt <= endNum; cnt++)
        {
            if (score[cnt].isCopper)
            {
                clearCnt++;
            }
        }
        if (clearCnt == stageNum)
        {
            StageClearManager.StageClear[bookNum] = true;
        }
    }

    //==============================================================
    // ページをめくる操作
    //==============================================================
    private void PageOperation()
    {
        if (pageInterval == 0)
        {
            // 次のページへ進む
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0)
            {
                eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();
                pageInterval = pageIntervalInit;
            }
            // 前のページに戻る
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0)
            {
                eventSystem.GetComponent<IgnoreMouseInputModule>().BackPage();
                pageInterval = pageIntervalInit;
            }
        }
        else pageInterval--;
    }

    //==============================================================
    // ステージ画面への遷移
    //==============================================================
    private void StageSceneChange()
    {
        if (command == COMMAND.EMPTY && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")))
        {

            // 現在のページ取得(ステージ番号)
            StageManager.stageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum() + firstStageNum - 1;

            // シーン遷移開始
            if (StageManager.stageNum > 0)
            {
                if (stageEnterFlg)
                {
                    DecSource.Play();

                    command = COMMAND.STAGE;
                    selectPageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum();
                }
                else
                {
                    BuSource.Play();
                }
            }
        }

        if (command == COMMAND.STAGE)
        {
            // 本を閉じる
            eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
            // 部屋を暗くする
            this.GetComponent<PostEffectController>().SetVigFlg(true);

            // 本が閉じ終わった
            if (eventSystem.GetComponent<IgnoreMouseInputModule>().GetBookCloseFlg())
            {

                // 遷移
                SceneManager.LoadScene("Stage1Scene");
            }
        }
    }

    //==============================================================
    // 本の選択画面への遷移
    //==============================================================
    private void BookSelectChange()
    {
        if (command == COMMAND.EMPTY && (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 1")))
        {
            DecSource.Play();

            // 本を閉じる
            eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
            mist.SetActive(false);
            command = COMMAND.BOOK_SELECT;
        }

        // 本が閉じ終わった
        if (eventSystem.GetComponent<IgnoreMouseInputModule>().GetBookCloseFlg() && command == COMMAND.BOOK_SELECT)
        {
            bookUI.SetActive(false);
            bookBack.SetActive(true);
            cameraAnim.SetBool("isAnim", true);

            // 本が上に上がる
            if (bookRemoveCnt > 0)
            {
                Vector3 pos = new Vector3(0.0f, 0.5f, -0.5f);
                book.transform.position += pos;
                bookRemoveCnt--;
            }
            // 本が棚に戻る
            if (bookRemoveCnt == 0)
            {
                book.transform.localPosition = new Vector3(-2.0f, 49.73f, -50.55f);
                book.transform.localEulerAngles = new Vector3(25.0f, 0.0f, -90.0f);
                book.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                book.transform.DOLocalMove(new Vector3((-2.0f + (BookSelect.bookNum * 0.75f)), 9.73f, -0.55f), 0.75f).OnComplete(() =>
                {
                    sceneChangeFlg = true;
                });
                bookRemoveCnt = -1;
            }

            // シーン遷移
            if (sceneChangeFlg)
            {
                if (sceneChangeCnt == 0)
                {
                    selectPageNum = 1;
                    SceneManager.LoadScene("NewSelectScene");
                }
                else sceneChangeCnt--;
            }
        }
    }

    //==============================================================
    // エクストラステージに入れるかチェック
    //==============================================================
    private void ExtraConditions()
    {
        // 開いているページがエクストラステージの時
        if (eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum() + firstStageNum - 1 == endStageNum)
        {
            int cnt = 0;

            // 各ステージのメダルを取得できているかチェック
            for (int i = firstStageNum; i <= endStageNum - 1; i++)
            {
                if (score[i].isCopper && score[i].isSilver && score[i].isGold) cnt++;
            }

            // 条件を満たしていない時は入れない
            if (cnt != bookStageMax - 1)
            {
                stageEnterFlg = false;
                mist.SetActive(true);
            }
            else
            {
                enterExtraFlg[BookSelect.bookNum] = true;
            }
        }
        // 開いているページがエクストラステージ以外の時
        else
        {
            stageEnterFlg = true;
            mist.SetActive(false);
        }

        if (enterExtraFlg[BookSelect.bookNum] == true)
        {
            stageEnterFlg = true;
            mist.SetActive(false);
        }
    }

    //==============================================================
    // 銀メダルの獲得条件をセット(ステージを折った回数)
    //==============================================================
    private void SilverConditionsSet()
    {
        silverConditions[0] = 0;
        silverConditions[1] = 3;
        silverConditions[2] = 3;
        silverConditions[3] = 3;
        silverConditions[4] = 3;
        silverConditions[5] = 3;
        silverConditions[6] = 3;
        silverConditions[7] = 3;
        silverConditions[8] = 3;
        silverConditions[9] = 3;
        silverConditions[10] = 3;

        silverConditions[11] = 3;
        silverConditions[12] = 3;
        silverConditions[13] = 3;
        silverConditions[14] = 3;
        silverConditions[15] = 3;
        silverConditions[16] = 3;
        silverConditions[17] = 3;
        silverConditions[18] = 3;
        silverConditions[19] = 3;
        silverConditions[20] = 3;

        silverConditions[21] = 3;
        silverConditions[22] = 3;
        silverConditions[23] = 3;
        silverConditions[24] = 3;
        silverConditions[25] = 3;
        silverConditions[26] = 3;
        silverConditions[27] = 3;
        silverConditions[28] = 3;
        silverConditions[29] = 3;
        silverConditions[30] = 3;

        silverConditions[31] = 3;
        silverConditions[32] = 3;
        silverConditions[33] = 3;
        silverConditions[34] = 3;
        silverConditions[35] = 3;
        silverConditions[36] = 3;
        silverConditions[37] = 3;
        silverConditions[38] = 3;
        silverConditions[39] = 3;
        silverConditions[40] = 3;

        silverConditions[41] = 3;
        silverConditions[42] = 3;
    }
}
