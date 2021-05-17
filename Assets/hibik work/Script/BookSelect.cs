using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BookSelect : MonoBehaviour
{
    private GameObject eventSystem;
    private Animator[] bookAnim;
    private Animator[] bookLAnim;
    private GameObject[] book;
    private GameObject[] bookBack;
    private Animator cameraAnim;

    private int bookMax = 6;    // 本の最大数
    private int stageMax = 43;  // ステージの最大数

    public static int bookNum = 0;    // 本の番号
    private int bookSelectCntInit = 30;
    private int bookSelectCnt = 0;
    private bool bookSelectFlg = false;
    private int bookRemoveCnt = 60;

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

    // シーンの状態
    private enum STATUS
    {
        BOOK_SELECT,
        STAGE_SELECT,
    }
    private STATUS status;



    //==============================================================
    // 初期処理
    //==============================================================
    private void Awake()
    {
        sceneChangeCnt = sceneChangeCntInit;
        pageInterval = pageIntervalInit;
        operationCnt = operationCntInit;

        eventSystem = GameObject.Find("EventSystem");
        cameraAnim = GameObject.Find("Main Camera").GetComponent<Animator>();

        bookAnim = new Animator[bookMax];
        bookLAnim = new Animator[bookMax];
        book = new GameObject[bookMax];
        bookBack = new GameObject[bookMax];

        for (int i = 0; i < bookMax; i++)
        {
            bookAnim[i] = GameObject.Find("BookModel" + (i + 1)).GetComponent<Animator>();
            bookLAnim[i] = GameObject.Find("book_L" + (i + 1)).GetComponent<Animator>();
            book[i] = GameObject.Find("BookModel" + (i + 1));
            bookBack[i] = GameObject.Find("book_back" + (i + 1));
        }
        bookAnim[bookNum].SetBool("isUp", true);

        this.GetComponent<PostEffectController>().SetVigFlg(false);
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {
        switch (status)
        {
            //-----------------------------------
            // 本選択中
            //-----------------------------------
            case STATUS.BOOK_SELECT:

                if (!bookSelectFlg)
                {

                    if (bookSelectCnt == 0)
                    {
                        // 左の本を選択
                        if (Input.GetAxis("Horizontal") < 0)
                        {
                            if (bookNum > 0)
                            {
                                bookNum--;
                                bookSelectCnt = bookSelectCntInit;
                                // 選択中の本が上に上がる
                                bookAnim[bookNum].SetBool("isUp", true);
                                bookAnim[bookNum + 1].SetBool("isUp", false);
                            }
                        }
                        // 右の本を選択
                        if (Input.GetAxis("Horizontal") > 0)
                        {
                            if (bookNum < bookMax - 1)
                            {
                                bookNum++;
                                bookSelectCnt = bookSelectCntInit;
                                // 選択中の本が上に上がる
                                bookAnim[bookNum].SetBool("isUp", true);
                                bookAnim[bookNum - 1].SetBool("isUp", false);
                            }
                        }
                    }
                    else bookSelectCnt--;

                    // 本を決定
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
                    {
                        // 選択中の本を机に移動
                        bookAnim[bookNum].SetBool("isRemove", true);
                        cameraAnim.SetBool("isAnim", true);
                        bookSelectFlg = true;
                    }
                }

                else
                {
                    if (bookRemoveCnt == 0)
                    {
                        bookBack[bookNum].SetActive(false);
                        book[bookNum].transform.localPosition = new Vector3(-0.77f, 50.08f, -50.61f);
                        book[bookNum].transform.localScale = new Vector3(2.215f, 2.115f, 2.8f);
                        book[bookNum].transform.localEulerAngles = new Vector3(88.8f, 180.0f, 0.0f);
                        bookRemoveCnt = -1;
                        book[bookNum].transform.DOLocalMove(new Vector3(-0.77f, -0.08f, 0.61f), 1.0f).OnComplete(() =>
                        {
                            sceneChangeFlg = true;
                        });
                    }
                    else bookRemoveCnt--;
                }

                if (sceneChangeFlg)
                {
                    if (sceneChangeCnt == 0)
                    {
                        SceneManager.LoadScene("StageSelect" + (bookNum + 1));
                    }
                    else sceneChangeCnt--;
                }

                break;

            //-----------------------------------
            // ステージ選択中
            //-----------------------------------
            case STATUS.STAGE_SELECT:
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

                break;

            default:
                break;
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
}
