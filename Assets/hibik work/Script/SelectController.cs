using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SelectController : MonoBehaviour
{
    private GameObject stage1Button;

    void Awake()
    {
        stage1Button = GameObject.Find("Stage1Button");
        // 選択状態にする
        EventSystem.current.SetSelectedGameObject(stage1Button);
    }

    public void OnStageButton(int stageNo)
    {
        SceneManager.LoadScene("Stage" + stageNo + "Scene");
    }
}
