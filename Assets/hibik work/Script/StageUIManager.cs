using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject stageManager;
    [SerializeField] private GameObject stageUIManager;
    [SerializeField] private GameObject editCanvas;

    [SerializeField] private GameObject selectGear;
    [SerializeField] private GameObject retryGear;

    [SerializeField] private int stageNum;          // ステージ番号
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
    }

    private void Update()
    {
        if (stageManager.GetComponent<StageManager>().IsGameClear)
        {
            stageUIManager.GetComponent<ClearUI>().ClearFlgOn();
        }

        // 決定でシーン遷移
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown("joystick button 6"))
        {
            SceneManager.LoadScene("SelectScene");
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown("joystick button 7"))
        {
            // 現在のScene名を取得する
            Scene loadScene = SceneManager.GetActiveScene();
            // Sceneの読み直し
            SceneManager.LoadScene(loadScene.name);
        }

        // メニュー表示中
        if (menuFlg)
        {
            //stageManager.SetActive(false);
            MenuOperation(); // メニュー画面の操作

            // メニューを非表示にする
            if (Input.GetKeyDown(KeyCode.M))
            {
                menuFlg = false;
                // ページを戻す
                eventSystem.GetComponent<IgnoreMouseInputModule>().BackPage();
                stageManager.SetActive(true);

            }
        }
        // メニュー非表示中
        else
        {
            // メニューを表示する
            if (Input.GetKeyDown(KeyCode.M))
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

    // ステージ番号取得
    public int GetStageNum()
    {
        return stageNum;
    }
}
