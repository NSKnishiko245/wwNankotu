using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndRoll : MonoBehaviour
{
    //　テキストのスクロールスピード
    [SerializeField]
    private float textScrollSpeed = 2;
    //　テキストの制限位置
    [SerializeField]
    private float limitPosition = 60f;
    //　エンドロールが終了したかどうか
    private bool isStopEndRoll;
    //　シーン移動用コルーチン
    private Coroutine endRollCoroutine;

    private bool speedUpFlg = false;

    // Update is called once per frame
    void Update()
    {
        //　エンドロールが終了した時
        if (isStopEndRoll)
        {
            endRollCoroutine = StartCoroutine(GoToNextScene());
        }
        else
        {
            //　エンドロール用テキストがリミットを越えるまで動かす
            if (transform.position.y <= limitPosition)
            {
                transform.position = new Vector3(0, transform.position.y + textScrollSpeed * Time.deltaTime, -4);
            }
            else
            {
                isStopEndRoll = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            speedUpFlg = true;
        }

        if (speedUpFlg) textScrollSpeed += 0.3f;
    }

    IEnumerator GoToNextScene()
    {
        //　5秒間待つ
        //yield return new WaitForSeconds(5f);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            StopCoroutine(endRollCoroutine);
            SceneManager.LoadScene("BonusScene");
        }

        yield return null;
    }
}
