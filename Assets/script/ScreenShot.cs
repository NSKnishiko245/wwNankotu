using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


            // このスクリプトがアタッチされているオブジェクトのサイズでスクショを撮る
            RectTransform trans = gameObject.GetComponent<RectTransform>();
            Vector2 pos = new Vector2(trans.position.x, trans.position.y);
            Vector2 scale = new Vector2(trans.localScale.x, trans.localScale.y);

            Vector2 leftBottom = RectTransformUtility.WorldToScreenPoint(Camera.main, pos - scale / 2.0f);
            Vector2 rightTop = RectTransformUtility.WorldToScreenPoint(Camera.main, pos + scale / 2.0f);

            // スクショしてみた！
            StartCoroutine(SelectAreaScreenShot(new Vector2Int((int)leftBottom.x, (int)leftBottom.y), (int)(rightTop.x - leftBottom.x), (int)(rightTop.y - leftBottom.y)));
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