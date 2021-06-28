using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class SamnaleMovie : MonoBehaviour
{
    /// 制御するVideo Playerのリスト
    /// </summary>
    public VideoPlayer playMovie;
    public int num = 1;
    public int initNum = -1;
    public int playFrame;
    // Start is called before the first frame update
    void Start()
    {
        playMovie = this.GetComponent<VideoPlayer>();
        playMovie.Play();
        //playMovie.frame = 1;
        //playMovie.time = 1;
        playMovie.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            num++;
            
        }
       // if (num == initNum)
        {
            if (num >= 36)
            {
                num = 36;
            }
            if (num <= 1)
            {
                num = 1;
            }
            playMovie.clip = Resources.Load<VideoClip>("Movie/StageSamnale/stage_" + num);
          //  Debug.Log(playMovie.clip.name + "ロード");
        }
        //if (initNum != num)
        //{
        //    playMovie.clip = Resources.Load<VideoClip>("StageSamnale/" + num);
        //}
        playFrame--;
        if (playFrame < 0)
        {
            //playMovie.frame = 0;
            //playMovie.time = 0;
            //return;
            if (!playMovie.isPlaying)
            {
                playMovie.Play();
            }
        }

        //if (!this.enabled)
        //{

        //}

    }

    public void ChangeClip(int _num)
    {
        num = _num;
        playMovie = this.GetComponent<VideoPlayer>();
        playMovie.clip = Resources.Load<VideoClip>("StageSamnale/" + num);
    }
}
