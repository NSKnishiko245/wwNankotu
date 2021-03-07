using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    void Start()
    {
       // gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // マテリアルにテクスチャをセットするにはシェーダーを変更する必要がある
            string shader = "Legacy Shaders/Diffuse";
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find(shader);

            StartCoroutine(TurnOnScreenShotEx());
        }
    }

    // スクショし、その画像を画面に貼る
    private IEnumerator TurnOnScreenShotEx()
    {
        yield return new WaitForEndOfFrame();

        Vector2Int size = new Vector2Int(Screen.width, Screen.height);
        Texture2D tex = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);

        // 引数で指定した矩形内をスクショし、そのデータを tex に格納
        tex.ReadPixels(new Rect(0.0f, 0.0f, size.x, size.y), 0, 0);
        tex.Apply();

        // quadオブジェクトのマテリアルにスクショした画像をセット
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
    }
}