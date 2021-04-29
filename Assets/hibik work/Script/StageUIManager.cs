using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject stageManager;
    [SerializeField] private GameObject stageUIManager;
    [SerializeField] private GameObject editCanvas;

    [SerializeField] private GameObject selectGear;
    [SerializeField] private GameObject retryGear;

    [SerializeField] private int stageDisplayCntInit;   // ステージを表示するまでの時間
    private int stageDisplayCnt;
    [SerializeField] private bool editFlg = false;  // true:エディット表示

    private bool menuFlg = false;                　 // true:メニュー表示中
    private bool menuFirstFlg = false;              // true:メニューを開いた瞬間

    [SerializeField] private AudioSource StageBgmSource;

    // 選択中の項目
    private enum STATUS
    {
        STAGE_SELECT,
        RETRY,
    }
    private STATUS status;

    private void Awake()
    {
        if (!editFlg) editCanvas.SetActive(false);

        // ステージを表示するまでの時間をセット
        stageDisplayCnt = stageDisplayCntInit;
        // ステージを非表示
        player.SetActive(false);
        stageManager.SetActive(false);
    }

    private void Update()
    {
        if (stageDisplayCnt == 0)
        {
            // ステージを表示
            player.SetActive(true);
            stageManager.SetActive(true);
        }
        else stageDisplayCnt--;

        if (stageManager.GetComponent<StageManager>().IsGameClear || Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("clear");
            SilverMedalConditions();
            stageUIManager.GetComponent<ClearUI>().ClearFlgOn();
        }

        // メニュー表示中
        if (menuFlg)
        {
            MenuOperation(); // メニュー画面の操作

            // ステージを非表示
            player.SetActive(false);
            stageManager.SetActive(false);

            // メニューを非表示にする
            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 3"))
            {
                menuFlg = false;
                // ページを戻す
                eventSystem.GetComponent<IgnoreMouseInputModule>().BackPage();
                // ステージを表示するまでの時間をセット
                stageDisplayCnt = stageDisplayCntInit;
            }
        }
        // メニュー非表示中
        else
        {
            // メニューを表示する
            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 3"))
            {
                // クリア後は表示しない
                if (!stageUIManager.GetComponent<ClearUI>().GetCLearFlg())
                {
                    menuFlg = true;
                    menuFirstFlg = true;
                    // ページを進める
                    eventSystem.GetComponent<IgnoreMouseInputModule>().NextPage();
                }
            }
        }
    }

    // メニュー画面の操作
    private void MenuOperation()
    {
        // メニューを開いた直後の処理
        if (menuFirstFlg)
        {
            status = STATUS.STAGE_SELECT;
            menuFirstFlg = false;
        }

        // 選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            status = STATUS.STAGE_SELECT;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            status = STATUS.RETRY;
        }

        // ステージセレクト選択中
        if (status == STATUS.STAGE_SELECT)
        {
            // 歯車回転
            selectGear.GetComponent<GearRotation>().SetRotFlg(true);
            retryGear.GetComponent<GearRotation>().SetRotFlg(false);

            // 決定でシーン遷移
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                SceneManager.LoadScene("SelectScene");
            }
        }
        // リトライ選択中
        if (status == STATUS.RETRY)
        {
            // 歯車回転
            selectGear.GetComponent<GearRotation>().SetRotFlg(false);
            retryGear.GetComponent<GearRotation>().SetRotFlg(true);

            // 決定でシーン遷移
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                // 現在のScene名を取得する
                Scene loadScene = SceneManager.GetActiveScene();
                // Sceneの読み直し
                SceneManager.LoadScene(loadScene.name);
            }
        }
    }

    // 銀メダル取得
    private void SilverMedalConditions()
    {
        if (StageSelectManager.silverConditions[StageManager.stageNum] >=
            stageManager.GetComponent<StageManager>().rotateNum)
        {
            StageSelectManager.score[StageManager.stageNum].isSilver = true;
            Debug.Log("銀メダル取得のノルマ" + StageSelectManager.silverConditions[1]);
            Debug.Log("ステージを折った回数" + stageManager.GetComponent<StageManager>().rotateNum);
        }
    }
}