using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageController : MonoBehaviour
{
    private GameObject menuPanel;
    private GameObject retryButton;
    bool menuFlg = false;   // true:メニュー表示　false:メニュー非表示

    void Awake()
    {
        menuPanel = GameObject.Find("MenuPanel");
        // 無効にする
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    void Update()
    {
        // メニュー表示切り替え
        if (Input.GetKeyDown(KeyCode.M))
        {
            MenuDisplay();
        }
    }

    public void OnRetryButton()
    {
        // 現在のScene名を取得する
        Scene loadScene = SceneManager.GetActiveScene();
        // Sceneの読み直し
        SceneManager.LoadScene(loadScene.name);
    }

    public void OnSelectButton()
    {
        SceneManager.LoadScene("SelectScene");
    }

    public void MenuDisplay()
    {
        if (!menuFlg)
        {
            menuFlg = true;
            // 有効状態切り替え
            menuPanel.SetActive(menuFlg);

            retryButton = GameObject.Find("RetryButton");
            // 選択状態にする
            EventSystem.current.SetSelectedGameObject(retryButton);
        }
        else
        {
            menuFlg = false;
            // 有効状態切り替え
            menuPanel.SetActive(menuFlg);
        }
    }
}
