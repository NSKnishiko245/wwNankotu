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
    private int bookStageMax = 10;  // 一冊あたりのステージの最大数
    private int stageMax = 42;
    private int firstStageNum;
    private int endStageNum;


    private int bookSelectCntInit = 30;
    private int bookSelectCnt = 0;
    private bool bookSelectFlg = false;
    private int bookRemoveCnt = 90;
    private bool stageEnterFlg = true;

    // サウンド
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource DecSource;


    [SerializeField] private int sceneChangeCntInit; // シーン遷移までの時間
    private int sceneChangeCnt = 0;                 // シーン遷移のカウンタ
    private bool sceneChangeFlg = false;

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

    //==============================================================
    // 初期処理
    //==============================================================
    private void Awake()
    {
        sceneChangeCnt = sceneChangeCntInit;
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

        BookSelect.bookNum = bookNum - 1;
        if (BookSelect.bookNum == 0)
        {
            firstStageNum = 1;
            endStageNum = 8;
        }
        if (BookSelect.bookNum == 1)
        {
            firstStageNum = 9;
            endStageNum = 15;
        }
        if (BookSelect.bookNum == 2)
        {
            firstStageNum = 16;
            endStageNum = 21;
        }
        if (BookSelect.bookNum == 3)
        {
            firstStageNum = 22;
            endStageNum = 28;
        }
        if (BookSelect.bookNum == 4)
        {
            firstStageNum = 29;
            endStageNum = 35;
        }
        if (BookSelect.bookNum == 5)
        {
            firstStageNum = 36;
            endStageNum = 42;
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

        SilverConditionsSet();

        this.GetComponent<PostEffectController>().SetVigFlg(false);
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {
        if (operationCnt == 1) eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();

        if (operationCnt == 0)
        {
            if (!eventSystem.GetComponent<IgnoreMouseInputModule>().GetAllBackFlg())
            {
                PageOperation();
            }
        }
        else operationCnt--;

        ExtraConditions();
        SceneChange();      // シーン遷移

    }

    // ページをめくる
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

    // シーン遷移
    private void SceneChange()
    {
        if (!sceneChangeFlg && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")))
        {
            DecSource.Play();

            // 現在のページ取得(ステージ番号)
            StageManager.stageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum() + firstStageNum - 1;

            // シーン遷移開始
            if (StageManager.stageNum > 0)
            {
                if (stageEnterFlg)
                {
                    // 本を閉じる
                    eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
                    // 部屋を暗くする
                    this.GetComponent<PostEffectController>().SetVigFlg(true);

                    sceneChangeFlg = true;
                }
            }
        }

        if (!sceneChangeFlg && (Input.GetKeyDown(KeyCode.B)))
        {
            DecSource.Play();

            // 本を閉じる
            eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
            sceneChangeFlg = true;
            bookSelectFlg = true;
            mist.SetActive(false);
        }

        if (eventSystem.GetComponent<IgnoreMouseInputModule>().GetBookCloseFlg())
        {
            bookUI.SetActive(false);
            cameraAnim.SetBool("isAnim", true);
            if (bookSelectFlg)
            {
                bookBack.SetActive(true);

                if (bookRemoveCnt == 0)
                {
                    book.transform.localPosition = new Vector3(-2.0f, 49.73f, -50.55f);
                    book.transform.localEulerAngles = new Vector3(25.0f, 0.0f, -90.0f);
                    book.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                    book.transform.DOLocalMove(new Vector3((-2.0f + (BookSelect.bookNum * 0.75f)), 9.73f, -0.55f), 1.0f).OnComplete(() =>
                         {
                             SceneManager.LoadScene("NewSelectScene");
                         });
                    bookRemoveCnt = -1;
                }
                else
                {
                    Vector3 pos = new Vector3(0.0f, 0.5f, -0.5f);
                    book.transform.position += pos;
                    bookRemoveCnt--;
                }
            }
            else
            {
                // 遷移
                SceneManager.LoadScene("Stage1Scene");
            }
        }
    }

    private void ExtraConditions()
    {
        if (eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum() + firstStageNum - 1 == endStageNum)
        {
            int cnt = 0;

            for (int i = firstStageNum; i <= endStageNum - 1; i++)
            {
                if (score[i].isCopper && score[i].isSilver && score[i].isGold) cnt++;
            }

            if (cnt != endStageNum - 1)
            {
                stageEnterFlg = false;
                mist.SetActive(true);
            }
        }
        else
        {
            stageEnterFlg = true;
            mist.SetActive(false);
        }
    }

    // 銀メダルの獲得条件をセット(ステージを折った回数)
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
