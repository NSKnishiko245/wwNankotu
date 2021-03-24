using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public GameObject StageCountText;
    public GameObject MapManager;
    
    // テキストの数字を上げる
    public void StageCount_Increment()
    {
        int num = int.Parse(StageCountText.GetComponent<Text>().text);
        num++;
        StageCountText.GetComponent<Text>().text = num.ToString();
        LoadMap();
    }
    // テキストの数字を下げる
    public void StageCount_Decrement()
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
        MapManager.GetComponent<MapEdit>().SaveMap(int.Parse(StageCountText.GetComponent<Text>().text));
    }
    public void LoadMap()
    {
        MapManager.GetComponent<MapEdit>().LoadMap(int.Parse(StageCountText.GetComponent<Text>().text));
    }



    // ステージのサイズを変更するメソッド
    public void StageSize_Increment()
    {
        int num = int.Parse(StageCountText.GetComponent<Text>().text);
        num += 2;
        StageCountText.GetComponent<Text>().text = num.ToString();
        MapManager.GetComponent<MapEdit>().StageScaleUp();
    }
    public void StageSize_Decrement()
    {
        int num = int.Parse(StageCountText.GetComponent<Text>().text);
        if (num > 2)
        {
            num -= 2;
            StageCountText.GetComponent<Text>().text = num.ToString();
            MapManager.GetComponent<MapEdit>().StageScaleDown();
        }
    }
}
