using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PostEffectController : MonoBehaviour
{
    [SerializeField] private GameObject postProcess;    // ポストプロセス
    [SerializeField] private GameObject fire;   // ロウソクの火

    private Animator fireAnim;  // ロウソクの火のアニメーション
    private Volume volume;      // ブルーム
    private Vignette vignette;  // ビネット

    public static float intensity;  // ビネットの強度
    [SerializeField] private float intensityMax;    // ビネットの強度の最大値
    [SerializeField] private float intensityVal;    // ビネットの強度の増加量
    private bool vigFlg = false;    // true:ビネットON

    void Awake()
    {
        volume = postProcess.GetComponent<Volume>();
        volume.profile.TryGet<Vignette>(out vignette);
        fireAnim = fire.GetComponent<Animator>();

        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            intensity = vignette.intensity.value;
            vigFlg = true;
        }

        if (SceneManager.GetActiveScene().name == "Stage1Scene")
        {
            intensity = intensityMax;
            vigFlg = true;

            fireAnim.SetBool("isAnim", true);
        }
    }

    void Update()
    {
        // ビネットON
        if (vigFlg)
        {
            if (intensity < intensityMax) intensity += intensityVal;
        }
        // ビネットOFF
        else
        {
            if (intensity > 0.0f) intensity -= intensityVal;
        }

        vignette.intensity.value = intensity;
    }

    // ビネットをセット
    public void SetVigFlg(bool sts)
    {
        vigFlg = sts;
    }

    // ロウソクの火をセット
    public void SetFireFlg(bool sts)
    {
        fireAnim.SetBool("isAnim", sts);
    }
}
