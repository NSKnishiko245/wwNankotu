using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    // サウンド
    [SerializeField] private AudioSource titleBgmSource;
    [SerializeField] private AudioSource titleStartSource;
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private float sceneChangeTime; // シーン遷移までの時間
    private int sceneChangeCnt = 0;                // シーン遷移のカウンタ
    private bool sceneChangeFlg = false;            // true:シーン遷移開始
    private bool sceneChangeFirstFlg = true;        // シーン遷移開始後に１度だけ通る処理

    private void Awake()
    {
        ScoreReset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            sceneChangeFlg = true;

            //mainCamera.transform.DOLocalMove(new Vector3(0, -6.35f, -10.33f), 5.0f);
            //mainCamera.transform.DORotate(new Vector3(-27.255f, 0f, 0f), 5.0f);
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

            // 一定時間経過すると遷移する
            if (sceneChangeCnt > sceneChangeTime * 60)
            {
                SceneManager.LoadScene("SelectScene");
            }
            sceneChangeCnt++;
        }
    }

    private void ScoreReset()
    {
        for (int i = 0; i < 11; i++)
        {
            StageSelectManager.score[i].isGold = false;
            StageSelectManager.score[i].isSilver = false;
            StageSelectManager.score[i].isCopper = false;
        }
    }
}