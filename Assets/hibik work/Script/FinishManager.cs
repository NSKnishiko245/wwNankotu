using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject yes;
    [SerializeField] private GameObject no;
    [SerializeField] private GameObject galiver;

    public static bool menuFlg = false;  // true:ウインドウを表示中

    // ウインドウの状態
    enum STATUS
    {
        ESCAPE,
        DATA_DELETE,
    }
    private STATUS status;

    // 選択状態
    enum SELECT
    {
        YES,
        NO,
    }
    private SELECT select;

    private float selectSize = 4.5f;    // 選択時のUIのサイズ
    private float unselectSize = 4.0f;  // 非選択時のUIのサイズ


    //==============================================================
    // 初期処理
    //==============================================================
    private void Awake()
    {
        panel.SetActive(false);
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            status = STATUS.ESCAPE;
            text.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Finish/FinishText");

            // ウインドウ表示切替
            ChangeMenuDisplay();
        }

        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            if (!menuFlg && Input.GetKeyDown(KeyCode.K))
            {
                status = STATUS.DATA_DELETE;
                text.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Finish/DeleteText");

                // ウインドウ表示切替
                ChangeMenuDisplay();
            }
        }

        if (menuFlg)
        {
            // ウインドウ内の操作
            MenuOperetion();
        }
    }

    //==============================================================
    // ウインドウ表示切替
    //==============================================================
    private void ChangeMenuDisplay()
    {
        if (!menuFlg)
        {
            // ウインドウ表示
            panel.SetActive(true);
            menuFlg = true;
            select = SELECT.NO;
        }
        else
        {
            // ウインドウ非表示
            panel.SetActive(false);
            menuFlg = false;
        }
    }

    //==============================================================
    // ウインドウ内の操作
    //==============================================================
    private void MenuOperetion()
    {
        // 左キー押下でYESを選択
        if (select != SELECT.YES && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            select = SELECT.YES;
        }

        // 右キー押下でNOを選択
        if (select != SELECT.NO && Input.GetKeyDown(KeyCode.RightArrow))
        {
            select = SELECT.NO;
        }

        // YES選択時
        if (select == SELECT.YES)
        {
            // UIのサイズ変更
            yes.transform.localScale = new Vector3(selectSize, selectSize, 1);
            no.transform.localScale = new Vector3(unselectSize, unselectSize, 1);

            // UIの画像変更
            yes.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/finish/yes_on");
            no.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/finish/no_off");

            // Zキー押上で決定
            if (Input.GetKeyUp(KeyCode.Z))
            {
                // 終了画面の処理
                if (status == STATUS.ESCAPE)
                {
                    // ゲーム終了
                    Application.Quit();
                }

                // データ削除画面の処理
                else if (status == STATUS.DATA_DELETE)
                {
                    menuFlg = false;
                    StageSelectManager.DeleteSaveDate();
                    Destroy(galiver.gameObject);
                    SceneManager.LoadScene("Op");
                }
            }
        }

        // NO選択時
        else if (select == SELECT.NO)
        {
            // UIのサイズ変更
            yes.transform.localScale = new Vector3(unselectSize, unselectSize, 1);
            no.transform.localScale = new Vector3(selectSize, selectSize, 1);

            // UIの画像変更
            yes.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/finish/yes_off");
            no.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/finish/no_on");

            // Zキー押上で決定
            if (Input.GetKeyUp(KeyCode.Z))
            {
                // ウインドウ表示切替
                ChangeMenuDisplay();
            }
        }
    }
}