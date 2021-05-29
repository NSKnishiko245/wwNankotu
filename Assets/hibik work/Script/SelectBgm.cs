using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectBgm : MonoBehaviour
{
    // インスタンスの実体
    private static SelectBgm instance = null;

    // インスタンスのプロパティーは、実体が存在しないとき（＝初回参照時）実体を探して登録する
    public static SelectBgm Instance => instance
        ?? (instance = GameObject.FindWithTag("SelectBgm").GetComponent<SelectBgm>());

    private void Awake()
    {
        // もしインスタンスが複数存在するなら、自らを破棄する
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        // 唯一のインスタンスなら、シーン遷移しても残す
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StageSelectScene" ||
            SceneManager.GetActiveScene().name == "BookSelectScene")
        {
            this.gameObject.GetComponent<AudioSource>().UnPause();
        }
        else
        {
            this.GetComponent<AudioSource>().Pause();
        }
    }

    private void OnDestroy()
    {
        // 破棄時に、登録した実体の解除を行う
        if (this == Instance) instance = null;
    }
}
