using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject mainCamera;
    private Animator canvasAnim;
    private Animator cameraAnim;
    private Animator pressAAnim;

    // サウンド
    [SerializeField] private AudioSource titleBgmSource;
    [SerializeField] private AudioSource titleStartSource;
    [SerializeField] private AudioSource commandSource;

    [SerializeField] private float sceneChangeTime; // シーン遷移までの時間
    private int sceneChangeCnt = 0;                // シーン遷移のカウンタ
    private bool sceneChangeFlg = false;            // true:シーン遷移開始
    private bool sceneChangeFirstFlg = true;        // シーン遷移開始後に１度だけ通る処理

    [SerializeField] private int canvasAnimStartCnt;
    [SerializeField] private int cameraAnimStartCnt;
    GameObject postprocess;
    private bool commandFlg = false;

    private void Awake()
    {
        canvasAnim = canvas.GetComponent<Animator>();
        cameraAnim = mainCamera.GetComponent<Animator>();
        pressAAnim = GameObject.Find("PressAImage").GetComponent<Animator>();
        postprocess = GameObject.Find("PostProcess");

        // メダル取得状況を読込
        MedalDataLoad();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("joystick button 0"))
        {
            sceneChangeFlg = true;
            pressAAnim.SetFloat("speed", 5.0f);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            for (int i = 0; i < StageSelectManager.stageMax; i++)
            {
                PlayerPrefs.DeleteKey("Copper" + (i + 1));
                PlayerPrefs.DeleteKey("Silver" + (i + 1));
                PlayerPrefs.DeleteKey("Gold" + (i + 1));
            }
        }


        // エクストラステージ解放コマンド
        if (!commandFlg)
        {
            if (Input.GetKeyDown(KeyCode.C) || (Input.GetAxis("LTrigger") > 0 && Input.GetAxis("RTrigger") > 0))
            {
                Debug.Log("エクストラステージ解放コマンド");
                commandSource.Play();

                for (int i = 0; i < 6; i++)
                {
                    StageSelectManager.enterExtraFlg[i] = true;
                }

                commandFlg = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.M) || (Input.GetAxis("LTrigger") > 0 && Input.GetAxis("RTrigger") > 0) && Input.GetKeyDown("joystick button 1"))
        {
            Debug.Log("メダル全解放コマンド");
            commandSource.Play();
            BookSelect.bonusFlg = true;

            for (int i = 0; i < 6; i++)
            {
                StageClearManager.StageClear[i] = true;
            }

            for (int i = 1; i < StageSelectManager.stageMax + 1; i++)
            {
                StageSelectManager.score[i].isCopper = true;
                StageSelectManager.score[i].isSilver = true;
                StageSelectManager.score[i].isGold = true;

                StageClearManager.m_isGetCopper[i] = true;
            }
        }

        // シーン遷移開始
        if (sceneChangeFlg)
        {
            // シーン遷移開始後に１度だけ通る処理
            if (sceneChangeFirstFlg)
            {
                titleBgmSource.Stop();
                titleStartSource.Play();
                sceneChangeFirstFlg = false;
            }

            if (canvasAnimStartCnt == 0)
            {
                canvasAnim.SetBool("isAnim", true);
            }
            else canvasAnimStartCnt--;

            if (cameraAnimStartCnt == 0)
            {
                cameraAnim.SetBool("isAnim", true);
                postprocess.GetComponent<PostEffectController>().SetVigFlg(false);
            }
            else cameraAnimStartCnt--;


            // 一定時間経過すると遷移する
            if (sceneChangeCnt > sceneChangeTime)
            {
                SceneManager.LoadScene("BookSelectScene");
            }
            sceneChangeCnt++;
        }
    }

    // テキストファイルからメダル取得状況を取得
    private void MedalDataLoad()
    {
        for (int i = 0; i < StageSelectManager.stageMax; i++)
        {
            int temp = PlayerPrefs.GetInt("Copper" + (i + 1));
            StageSelectManager.score[i + 1].isCopper = System.Convert.ToBoolean(temp);
            temp = PlayerPrefs.GetInt("Silver" + (i + 1));
            StageSelectManager.score[i + 1].isSilver = System.Convert.ToBoolean(temp);
            temp = PlayerPrefs.GetInt("Gold" + (i + 1));
            StageSelectManager.score[i + 1].isGold = System.Convert.ToBoolean(temp);
        }
        Debug.Log("メダルデータ ロード完了");
    }
}
