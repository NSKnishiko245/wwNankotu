using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    // サウンド
    [SerializeField] private AudioSource selectBgmSource;
    [SerializeField] private AudioSource selectDecSource;

    [SerializeField] private GameObject eventSystem;

    [SerializeField] private int stageNum = 0;

    [SerializeField] private float sceneChangeTime; // シーン遷移までの時間
    private int sceneChangeCnt = 0;                 // シーン遷移のカウンタ
    private bool sceneChangeFlg = false;            // true:シーン遷移開始
    //private bool sceneChangeFirstFlg = true;        // シーン遷移開始後に１度だけ通る処理

    private int pageInterval;                       // ページをめくれるまでの待機時間
    [SerializeField] private int pageIntervalInit;  // ページをめくれるまでの待機時間の初期値

    private GameObject[] goldImage;
    private GameObject[] silverImage;
    private GameObject[] copperImage;
    public struct Score
    {
        public bool isGold;
        public bool isSilver;
        public bool isCopper;
    }
    public static Score[] score = new Score[11];

    private void Awake()
    {
        pageInterval = pageIntervalInit;

        goldImage = GameObject.FindGameObjectsWithTag("GoldImage");
        silverImage = GameObject.FindGameObjectsWithTag("SilverImage");
        copperImage = GameObject.FindGameObjectsWithTag("CopperImage");

        ScoreReset();
        score[1].isGold = true;
        score[1].isSilver = true;
        score[1].isCopper = true;
        score[2].isGold = true;
        score[2].isSilver = false;
        score[2].isCopper = true;
        score[3].isGold = true;
        score[3].isSilver = true;
        score[3].isCopper = false;
    }

    private void Update()
    {
        PageOperation();    // ページをめくる
        SceneChange();      // シーン遷移
        ScoreDisplay();
    }

    // ページをめくる
    private void PageOperation()
    {
        if (pageInterval == 0)
        {
            // 次のページへ進む
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("joystick button 5"))
            {
                eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();
                pageInterval = pageIntervalInit;
            }
            // 前のページに戻る
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("joystick button 4"))
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
        // シーン遷移開始
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            selectDecSource.Play();
            sceneChangeFlg = true;
        }

        if (sceneChangeFlg)
        {
            // 一定時間経過すると遷移する
            if (sceneChangeCnt > sceneChangeTime * 60)
            {
                // 現在のページ取得(ステージ番号)
                stageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum();
                // ステージに遷移
                if (stageNum > 0) SceneManager.LoadScene("Stage" + stageNum + "Scene");
                // 遷移しなければフラグを無効
                sceneChangeFlg = false;
            }
            sceneChangeCnt++;
        }
    }

    private void ScoreDisplay()
    {
        // 現在のページ取得(ステージ番号)
        stageNum = eventSystem.GetComponent<IgnoreMouseInputModule>().GetPageNum();

        for (int i = 0; i < goldImage.Length; i++)
        {
            if (score[stageNum].isGold) goldImage[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            else goldImage[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }

        for (int i = 0; i < silverImage.Length; i++)
        {
            if (score[stageNum].isSilver) silverImage[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            else silverImage[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }

        for (int i = 0; i < copperImage.Length; i++)
        {
            if (score[stageNum].isCopper) copperImage[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            else copperImage[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    private void ScoreReset()
    {
        for (int i = 0; i < 11; i++)
        {
            score[i].isGold = false;
            score[i].isSilver = false;
            score[i].isCopper = false;
        }
    }
}
