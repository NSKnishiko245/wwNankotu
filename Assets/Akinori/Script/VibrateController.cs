using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 振動タイプ
/// </summary>
internal enum VibrateType
{
    VERTICAL,
    HORIZONTAL
}

/// <summary>
/// 対象オブジェクトの振動を管理するクラス
/// </summary>
public class VibrateController : MonoBehaviour
{

    [SerializeField] private VibrateType vibrateType;          //振動タイプ
    [Range(0, 100)] [SerializeField] private float vibrateRange; //振動幅
    [SerializeField] private float vibrateSpeed;               //振動速度

    private float initPosition;   //初期ポジション
    private float testPosition;   //テスト用
    private float newPosition;    //新規ポジション
    private float minPosition;    //ポジションの下限
    private float maxPosition;    //ポジションの上限
    private bool directionToggle; //振動方向の切り替え用トグル(オフ：値が小さくなる方向へ オン：値が大きくなる方向へ)

    public float vibrateTime { get; private set; }

    public static int startFlg = 0;        //Start()を一回のみ呼び出すため
                                           // Use this for initialization
                                           /*
                                            * ステージ反転時にゴールを生成し直すとStart()が呼ばれバグる
                                            */
    void Start()
    {


        ResetPosition();

        //初期ポジションの設定を振動タイプ毎に分ける
        switch (this.vibrateType)
        {
            case VibrateType.VERTICAL:
                this.initPosition = transform.localPosition.y;
                testPosition = transform.localPosition.y;
                break;
            case VibrateType.HORIZONTAL:
                this.initPosition = transform.localPosition.x;
                testPosition = transform.localPosition.x;
                break;
        }

        this.newPosition = this.initPosition;
        this.minPosition = this.initPosition - this.vibrateRange;
        this.maxPosition = this.initPosition + this.vibrateRange;
        this.directionToggle = false;
    }

    // Update is called once per frame
    void Update()
    {
        vibrateTime--;
        if (vibrateTime > 0)
        {
            //毎フレーム振動を行う
            Vibrate();
        }
        else
        {
            switch (this.vibrateType)
            {
                case VibrateType.VERTICAL:
                    this.transform.localPosition = new Vector3(0, testPosition, 0);
                    break;
                case VibrateType.HORIZONTAL:
                    this.transform.localPosition = new Vector3(testPosition, 0, 0);
                    break;
            }
        }
        // Debug.Log("vibrate");
    }

    private void Vibrate()
    {

        //ポジションが振動幅の範囲を超えた場合、振動方向を切り替える
        if (this.newPosition <= this.minPosition ||
            this.maxPosition <= this.newPosition)
        {
            this.directionToggle = !this.directionToggle;
        }

        //新規ポジションを設定
        this.newPosition = this.directionToggle ?
            this.newPosition + (vibrateSpeed * Time.deltaTime) :
            this.newPosition - (vibrateSpeed * Time.deltaTime);
        this.newPosition = Mathf.Clamp(this.newPosition, this.minPosition, this.maxPosition);

        //新規ポジションを代入
        switch (this.vibrateType)
        {
            case VibrateType.VERTICAL:
                this.transform.localPosition = new Vector3(0, this.newPosition, 0);
                break;
            case VibrateType.HORIZONTAL:
                this.transform.localPosition = new Vector3(this.newPosition, 0, 0);
                break;
        }
    }

    public void SetVibrateTime(float _time)
    {
        vibrateTime = _time;
    }
    public void SetVibrateRange(float _range)
    {
        vibrateRange = _range;
    }
    public void SetSpeed(float _speed)
    {
        vibrateSpeed = _speed;
    }
    /*
     * 折るたびに初期位置をリセット 
     */
    public void ResetPosition()
    {
        if (startFlg < 2)
        {
            return;
        }
        Debug.Log("VibrateController::Start()");
        startFlg++;
        //初期ポジションの設定を振動タイプ毎に分ける
        switch (this.vibrateType)
        {
            case VibrateType.VERTICAL:
                this.initPosition = transform.localPosition.y;
                break;
            case VibrateType.HORIZONTAL:
                this.initPosition = transform.localPosition.x;
                break;
        }

        this.newPosition = this.initPosition;
        this.minPosition = this.initPosition - this.vibrateRange;
        this.maxPosition = this.initPosition + this.vibrateRange;
        this.directionToggle = false;
    }
}