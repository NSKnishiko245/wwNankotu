using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // マテリアルにテクスチャをセットするにはシェーダーを変更する必要がある
            string shader = "Legacy Shaders/Diffuse";
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find(shader);

            // スクショしてみた！
            StartCoroutine(SelectAreaScreenShot(new Vector2Int(0, 0), 100, 100));
        }
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
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", tex2);
    }
}