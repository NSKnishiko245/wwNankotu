using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class NewStageSelect : MonoBehaviour
{
    private GameObject eventSystem;
    private Animator[] bookAnim;
    private int bookMax = 5;    // 本の最大数
    private int stageMax = 43;  // ステージの最大数

    private int bookNum = 0;    // 本の番号
    private int bookSelectCntInit = 30;
    private int bookSelectCnt = 0;

    // サウンド
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource DecSource;


    [SerializeField] private float sceneChangeTime; // シーン遷移までの時間
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
    public static Score[] score;
    public static int[] silverCondition;

    // シーンの状態
    private enum STATUS
    {
        BOOK_SELECT,
        STAGE_SELECT,
    }
    private STATUS status;



    private void Awake()
    {
        pageInterval = pageIntervalInit;
        operationCnt = operationCntInit;

        eventSystem = GameObject.Find("EventSystem");

        bookAnim = new Animator[bookMax];
        for (int i = 0; i < bookMax; i++)
        {
            bookAnim[i] = GameObject.Find("BookModel" + (i + 1)).GetComponent<Animator>();
        }
        bookAnim[bookNum].SetBool("isUp", true);


        score = new Score[stageMax];
        silverCondition = new int[stageMax];
        goldMedal = new GameObject[stageMax];
        silverMedal = new GameObject[stageMax];
        copperMedal = new GameObject[stageMax];
        for (int i = 1; i < stageMax; i++)
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

    private void Update()
    {
        switch (status)
        {
            case STATUS.BOOK_SELECT:
                if (bookSelectCnt == 0)
                {
                    if (Input.GetAxis("Horizontal") < 0)
                    {
                        if (bookNum > 0)
                        {
                            bookNum--;
                            bookAnim[bookNum].SetBool("isUp", true);
                            bookAnim[bookNum + 1].SetBool("isUp", false);
                        }
                    }

                    if (Input.GetAxis("Horizontal") > 0)
                    {
                        if (bookNum < bookMax - 1)
                        {
                            bookNum++;
                            bookAnim[bookNum].SetBool("isUp", true);
                            bookAnim[bookNum - 1].SetBool("isUp", false);
                        }
                    }
                }
                else bookSelectCnt--;

                break;

            case STATUS.STAGE_SELECT:
                break;

            default:
                break;
        }



        if (operationCnt == 1) eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();

        if (operationCnt == 0)
        {
            if (!eventSystem.GetComponent<IgnoreMouseInputModule>().GetAllBackFlg())
            {
                PageOperation();
            }
        }
        else operationCnt--;

        SceneChange();      // シーン遷移

        if (!eventSystem.GetComponent<IgnoreMouseInputModule>().GetAllBackFlg())
        {
            //ScoreDisplay();
        }
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
            StageManager.stageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum();

            // シーン遷移開始
            if (StageManager.stageNum > 0)
            {
                // 本を閉じる
                eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
                // 部屋を暗くする
                this.GetComponent<PostEffectController>().SetVigFlg(true);

                sceneChangeFlg = true;
            }
        }

        if (eventSystem.GetComponent<IgnoreMouseInputModule>().GetBookCloseFlg())
        {
            // 遷移
            SceneManager.LoadScene("Stage1Scene");
        }
    }

    // 銀メダルの獲得条件をセット(ステージを折った回数)
    private void SilverConditionsSet()
    {
        silverCondition[0] = 0;
        silverCondition[1] = 3;
        silverCondition[2] = 3;
        silverCondition[3] = 3;
        silverCondition[4] = 3;
        silverCondition[5] = 3;
        silverCondition[6] = 3;
        silverCondition[7] = 3;
        silverCondition[8] = 3;
        silverCondition[9] = 3;
        silverCondition[10] = 3;

        silverCondition[11] = 3;
        silverCondition[12] = 3;
        silverCondition[13] = 3;
        silverCondition[14] = 3;
        silverCondition[15] = 3;
        silverCondition[16] = 3;
        silverCondition[17] = 3;
        silverCondition[18] = 3;
        silverCondition[19] = 3;
        silverCondition[20] = 3;

        silverCondition[21] = 3;
        silverCondition[22] = 3;
        silverCondition[23] = 3;
        silverCondition[24] = 3;
        silverCondition[25] = 3;
        silverCondition[26] = 3;
        silverCondition[27] = 3;
        silverCondition[28] = 3;
        silverCondition[29] = 3;
        silverCondition[30] = 3;

        silverCondition[31] = 3;
        silverCondition[32] = 3;
        silverCondition[33] = 3;
        silverCondition[34] = 3;
        silverCondition[35] = 3;
        silverCondition[36] = 3;
        silverCondition[37] = 3;
        silverCondition[38] = 3;
        silverCondition[39] = 3;
        silverCondition[40] = 3;

        silverCondition[41] = 3;
        silverCondition[42] = 3;
    }
}
