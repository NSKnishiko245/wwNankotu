using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    int frameCount;     // フレーム数
    float prevTime;     // 前回の経過時間
    Text fpsText;       // FPS表示用テキスト

    void Start()
    {
        // FPS固定
        Application.targetFrameRate = 60;

        fpsText = GameObject.Find("FPSText").GetComponent<Text>();

        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update()
    {
        ++frameCount;

        // 経過時間 - 前回の経過時間
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            // FPSをテキストにセット
            float fps = frameCount / time;
            fpsText.text = "fps:" + fps.ToString("f2");

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
