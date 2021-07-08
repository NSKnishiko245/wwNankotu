using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BonusManager : MonoBehaviour
{
    private GameObject eventSystem;
    [SerializeField] private GameObject bookL;
    private Animator bookLAnim;
    [SerializeField] private GameObject bookBack;
    private GameObject book;
    private GameObject bookUI;
    private Animator cameraAnim;

    private int bookRemoveCnt = 90;

    // サウンド
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource DecSource;


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
        command = COMMAND.EMPTY;

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
            // 本が閉じるときは操作不能にする
            if (!eventSystem.GetComponent<IgnoreMouseInputModule>().GetAllBackFlg())
            {
                PageOperation(); // ページをめくる操作


                // 現在のページ取得(ステージ番号)
                int pageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum();
                if (pageNum == 3)
                {
                    if (!FinishManager.menuFlg)
                    {
                        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("joystick button 0"))
                        {
                            // 遷移
                            SceneManager.LoadScene("EndRoll");
                        }
                    }
                }
            }
        }
        else operationCnt--;

        BookSelectChange(); // 本の選択画面への遷移
    }

    //==============================================================
    // ページをめくる操作
    //==============================================================
    private void PageOperation()
    {
        if (pageInterval == 0)
        {
            if (!FinishManager.menuFlg)
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
        }
        else pageInterval--;
    }

    //==============================================================
    // 本の選択画面への遷移
    //==============================================================
    private void BookSelectChange()
    {
        if (!FinishManager.menuFlg)
        {
            if (command == COMMAND.EMPTY && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown("joystick button 1")))
            {
                DecSource.Play();

                // 本を閉じる
                eventSystem.GetComponent<IgnoreMouseInputModule>().AllBackPage();
                command = COMMAND.BOOK_SELECT;
            }
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
                    SceneManager.LoadScene("BookSelectScene");
                }
                else sceneChangeCnt--;
            }
        }
    }
}