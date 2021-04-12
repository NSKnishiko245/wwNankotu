using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private AudioSource titleBgmSource;
    [SerializeField] private AudioSource titleStartSource;

    [SerializeField] private float sceneChangeTime;
    private int sceneChangeCnt = 0;
    private bool sceneChangeFlg = false;
    bool sceneChangeFirstFlg = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || (Input.GetKeyDown("joystick button 0")))
        {
            sceneChangeFlg = true;
        }

        if (sceneChangeFlg)
        {
            sceneChangeCnt++;

            if (sceneChangeFirstFlg)
            {
                titleBgmSource.Stop();
                titleStartSource.Play();
                sceneChangeFirstFlg = false;
            }

            if (sceneChangeCnt > sceneChangeTime * 60)
            {
                SceneManager.LoadScene("SelectScene");
            }
        }
    }
}
