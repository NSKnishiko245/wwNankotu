using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class Hint : MonoBehaviour
{
    // ヒントページのUI
    public GameObject hintCanvas;
    public GameObject hintBoard;
    public GameObject hintMovie;

    // ヒント通知UI
    public GameObject tv;
    public GameObject hukidasi;
    public GameObject eKeyIcon;

    // デバッグ用テキスト
    public Text hintTimeText;

    [SerializeField]
    private float hintDispTime = 180.0f;    // ヒント通知UIが現れる時間
    [SerializeField]
    private float hintDeleteTime = 190.0f;  // ヒント通知UIが消える時間

    public static bool hintFlg = false;         // ヒント表示可能か

    private static int retryCnt = 0;            // リトライした回数
    private static float playTime = 0.0f;       // ステージのプレイ時間 
    private static bool hintNoticeFlg = false;  // ヒント通知UIを表示したことがあるか

    //==============================================================
    // 初期処理
    //==============================================================
    private void Start()
    {
        tv.transform.localScale = new Vector3(0, 0, 0);
        hukidasi.transform.localScale = new Vector3(0, 0, 0);

        // ヒント動画をセット
        hintMovie.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("Movie/Hint/stage_" + StageManager.stageNum + "_hint");
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {
        if (StageManager.stageNum != 1)
        {
            // プレイ時間加算
            if (StageUIManager.status == StageUIManager.STATUS.PLAY)
            {
                playTime += Time.deltaTime;
                hintTimeText.text = "プレイ時間：" + playTime.ToString("f2");
            }

            // ヒント通知UI表示
            if (playTime > hintDispTime && !hintNoticeFlg)
            {
                HintNoticeDispChange(true);
            }

            // ヒント通知UI非表示
            if (playTime > hintDeleteTime 
                || StageUIManager.status == StageUIManager.STATUS.HINT
                || StageUIManager.status == StageUIManager.STATUS.CLEAR)
            {
                HintNoticeDispChange(false);
            }
        }
    }

    //==============================================================
    // ヒントページのUI表示切替
    //==============================================================
    public void HintPageDispChange(bool sts)
    {
        hintBoard.SetActive(sts);
        hintMovie.SetActive(sts);
    }

    //==============================================================
    // ヒント通知UI表示切替
    //==============================================================
    public void HintNoticeDispChange(bool sts)
    {
        if (sts) // 表示
        {
            tv.transform.DOScale(new Vector3(4000, 4000, 1100), 0.5f);
            hukidasi.transform.DOScale(new Vector3(2.5f, 2.5f, 1), 0.5f);

            hintFlg = true;
            hintNoticeFlg = true;
        }
        else // 非表示
        {
            tv.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
            hukidasi.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
        }
    }

    //==============================================================
    // ヒント初期化
    //==============================================================
    public void HintReset()
    {
        playTime = 0;
        hintFlg = false;
        hintNoticeFlg = false;
    }
}
