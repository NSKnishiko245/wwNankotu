using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OPMovie : MonoBehaviour
{
    [SerializeField]
    VideoPlayer videoPlayer;

    public AudioClip sound1;
    AudioSource audioSource;
    void Start()
    {
        videoPlayer.loopPointReached += LoopPointReached;
        videoPlayer.Play();
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("joystick button 0"))
        {
            //音(sound1)を鳴らす
            audioSource.PlayOneShot(sound1);
            SceneManager.LoadScene("TitleScene");
        }

    }

    public void LoopPointReached(VideoPlayer vp)
    {

        // 動画再生完了時の処理
        SceneManager.LoadScene("TitleScene");
    }
}
