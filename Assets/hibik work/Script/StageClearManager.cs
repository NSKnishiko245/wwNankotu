using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearManager : MonoBehaviour
{
    public static bool[] StageClear = new bool[6];
    public static bool[] m_isGetCopper = new bool[37];
    
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

    public void SetCopper(int stagenum, bool copper)
    {

        m_isGetCopper[stagenum] = copper;
    }

    public bool GetCopper(int stagenum)
    {
        return m_isGetCopper[stagenum];
    }
}
