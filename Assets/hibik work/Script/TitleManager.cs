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

    [SerializeField] private float sceneChangeTime; // シーン遷移までの時間
    private int sceneChangeCnt = 0;                // シーン遷移のカウンタ
    private bool sceneChangeFlg = false;            // true:シーン遷移開始
    private bool sceneChangeFirstFlg = true;        // シーン遷移開始後に１度だけ通る処理

    [SerializeField] private int canvasAnimStartCnt;
    [SerializeField] private int cameraAnimStartCnt;


    private void Awake()
    {
        canvasAnim = canvas.GetComponent<Animator>();
        cameraAnim = mainCamera.GetComponent<Animator>();
        pressAAnim = GameObject.Find("PressAImage").GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            sceneChangeFlg = true;
            pressAAnim.SetFloat("speed", 5.0f);
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
}
