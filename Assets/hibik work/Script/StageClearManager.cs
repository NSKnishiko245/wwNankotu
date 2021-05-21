using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearManager : MonoBehaviour
{
    public static bool[] StageClear = new bool[6];

    // Start is called before the first frame update
    void Start()
    {
        //scene‚ğ•Ï‚¦‚Ä‚à’l‚ğÁ‚³‚È‚¢‚æ‚¤‚É
        // DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(StageClear[0]);
    }

    public void SetStageClear(int stagenum, bool isclear)
    {
        StageClear[stagenum] = isclear;
    }

    public bool GetStageClear(int stagenum)
    {
        return StageClear[stagenum];
    }
}
