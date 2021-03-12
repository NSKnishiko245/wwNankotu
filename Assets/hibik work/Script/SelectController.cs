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
        // ‘I‘ğó‘Ô‚É‚·‚é
        EventSystem.current.SetSelectedGameObject(stage1Button);
    }

    public void OnStageButton(int stageNo)
    {
        SceneManager.LoadScene("Stage" + stageNo + "Scene");
    }
}
