using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour
{
    private Texture DefaultTexture;
    private Vector2 DefaultScale;

    private bool IsFinishedScreenShot;
    public bool isFinishedScreenShot()
    {
        if (IsFinishedScreenShot)
        {
            IsFinishedScreenShot = false;
            return true;
        }
        return false;
    }

    void Start()
    {
        // 初期のTextureを保持しておく（画像リセット用）
        DefaultTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false);

        DefaultTexture = GetComponent<Renderer>().material.GetTexture("_BaseMap");

        // 親子関係が形成されるとスケールが変わるので、初めに取得しておく
        DefaultScale = new Vector2(transform.parent.localScale.x, transform.parent.localScale.y);

        IsFinishedScreenShot = false;
    }

    void Update()
    {
    }


    // 指定した範囲のスクショを取り、このスクリプトをアタッチしているオブジェクトに貼る
    // 引数でスクショしたい範囲の左下の点と範囲の幅を渡す（※範囲の指定はスクリーン座標で）
    private IEnumerator SelectAreaScreenShot(Vector2Int leftBottom, int width, int height)
    {
        // スクショは描画後にしか行えないので、描画後に実行
        yield return new WaitForEndOfFrame();

        // スクリーン全体をスクショし、そのデータを保持
        Vector2Int size = new Vector2Int(Screen.width, Screen.height);
        Texture2D screeTex = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
        screeTex.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        screeTex.Apply();

        

        // 上で撮ったスクショデータから指定した範囲のピクセルを取得
        Color[] colors = screeTex.GetPixels(leftBottom.x, leftBottom.y, width, height);

        // 取得したピクセルデータでテクスチャを作成
        Texture2D tex2 = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex2.SetPixels(colors);
        tex2.Apply();

        // オブジェクトのマテリアルにテクスチャをセット
        gameObject.GetComponent<Renderer>().material.SetTexture("_BaseMap", tex2);

        IsFinishedScreenShot = true;
    }

    // 初期テクスチャにリセット
    public void ResetTexture(bool isReverse = false)
    {
        string texname = "";
        if (isReverse)
        {
            if (StageManager.stageNum <= 6)
                texname = "Prefabs/Material/bG_L1ex";
            else if (StageManager.stageNum <= 12)
                texname = "Prefabs/Material/bG_L2ex";
            else if (StageManager.stageNum <= 18)
                texname = "Prefabs/Material/bG_L3ex";
            else if (StageManager.stageNum <= 24)
                texname = "Prefabs/Material/bG_L4ex";
            else if (StageManager.stageNum <= 30)
                texname = "Prefabs/Material/bG_L5ex";
            else if (StageManager.stageNum <= 36)
                texname = "Prefabs/Material/bG_L6ex";

            GetComponent<Renderer>().rendererPriority = 2;
            GetComponent<Renderer>().material.renderQueue = 2;
        }
        else
        {
            if (StageManager.stageNum <= 6)
                texname = "Prefabs/Material/bG_L1";
            else if (StageManager.stageNum <= 12)
                texname = "Prefabs/Material/bG_L2";
            else if (StageManager.stageNum <= 18)
                texname = "Prefabs/Material/bG_L3";
            else if (StageManager.stageNum <= 24)
                texname = "Prefabs/Material/bG_L4";
            else if (StageManager.stageNum <= 30)
                texname = "Prefabs/Material/bG_L5";
            else if (StageManager.stageNum <= 36)
                texname = "Prefabs/Material/bG_L6";

            GetComponent<Renderer>().rendererPriority = 1;
            GetComponent<Renderer>().material.renderQueue = 1;
        }

        DefaultTexture = Resources.Load(texname) as Texture2D;
        gameObject.GetComponent<Renderer>().material.SetTexture("_BaseMap", DefaultTexture);
    }

    public void TurnOnScreenShot()
    {
        transform.localEulerAngles = new Vector3(0, 0, 0);

        // このスクリプトがアタッチされているオブジェクトのサイズでスクショを撮る
        RectTransform trans = gameObject.GetComponent<RectTransform>();
        Vector2 pos = new Vector2(trans.parent.position.x, trans.parent.position.y);

        // quadの左下の点と右上の点を取得
        Vector2 leftBottom = pos - DefaultScale / 2.0f;
        Vector2 rightTop = pos + DefaultScale / 2.0f;
        // 裏を向いている時は補正
        //if (gameObject.transform.parent.localRotation.y != 0.0f)
        //{
        //    leftBottom.x -= DefaultScale.x;
        //    rightTop.x += DefaultScale.x;
        //}
        leftBottom = RectTransformUtility.WorldToScreenPoint(Camera.main, leftBottom);
        rightTop = RectTransformUtility.WorldToScreenPoint(Camera.main, rightTop);

        // スクショしてみた！
        StartCoroutine(SelectAreaScreenShot(new Vector2Int((int)leftBottom.x, (int)leftBottom.y), (int)(rightTop.x - leftBottom.x), (int)(rightTop.y - leftBottom.y)));

        IsFinishedScreenShot = false;
    }

    public void TurnTexture()
    {
        transform.Rotate(180, 0, 0);
    }
    public void ReverseTexture()
    {
        transform.Rotate(0, 180, 0);
    }
}