using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public GameObject StageCountText;
    public GameObject MapManager;
    
    // テキストの数字を上げる
    public void OnClick_Increment()
    {
        int num = int.Parse(StageCountText.GetComponent<Text>().text);
        num++;
        StageCountText.GetComponent<Text>().text = num.ToString();
        LoadMap();
    }
    // テキストの数字を下げる
    public void OnClick_Decrement()
    {
        int num = int.Parse(StageCountText.GetComponent<Text>().text);
        if (num > 1)
        {
            num--;
            StageCountText.GetComponent<Text>().text = num.ToString();
            LoadMap();
        }
    }

    // 現在作成しているステージを外部ファイル(CSV)に書き出す
    public void OnClick_Save()
    {
        MapManager.GetComponent<MapEdit>().SaveMap("Stage" + StageCountText.GetComponent<Text>().text);
    }
    public void LoadMap()
    {
        MapManager.GetComponent<MapEdit>().LoadMap("Stage" + StageCountText.GetComponent<Text>().text);
    }
}
