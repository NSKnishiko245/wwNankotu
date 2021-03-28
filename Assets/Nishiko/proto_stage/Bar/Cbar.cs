using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cbar : MonoBehaviour
{
    // 目標の角度
    private float Angle_Destination = 0.0f;
    // 角速度を目標角の何割にするか
    private float Angle_Speed = 1.0f;

    //当たっているか？
    public bool IsHit{ get; private set; }

    // ステージが反転したかどうか
    public bool ReverseRotateFlg { private get; set; }

    // 回転状態
    public enum ROTATESTATE
    {
        NEUTRAL,        // 通常状態
        ROTATED,        // 回転済み
        ROTATE_LEFT,    // 左回転を始める
        ROTATE_RIGHT,   // 右回転を始める
        REROTATE,       // 元に戻す回転中
    }
    public ROTATESTATE RotateState { get; private set; }

    // 回転命令（これを外部から呼ぶことで回転させる）
    public void Rotation(ROTATESTATE state)
    {
        RotateState = state;
        RotationInfo_Update();
    }

    void Start()
    {
        IsHit = false;
        ReverseRotateFlg = false;
        RotateState = ROTATESTATE.NEUTRAL;
        GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
    }

    void Update()
    {
        if (!(RotateState == ROTATESTATE.NEUTRAL || RotateState == ROTATESTATE.ROTATED))
        {
            // 回転処理
            Rotation_Update();
        }
    }

    // 回転状態の更新
    private void RotationInfo_Update()
    {
        // 回転前に目標の角度を決定
        switch (RotateState)
        {
            case ROTATESTATE.ROTATE_LEFT:
                Angle_Destination = -180.0f;
                if (ReverseRotateFlg) Angle_Destination *= -1.0f;
                break;
            case ROTATESTATE.ROTATE_RIGHT:
                Angle_Destination = 180.0f;
                if (ReverseRotateFlg) Angle_Destination *= -1.0f;
                break;
            case ROTATESTATE.REROTATE:
                Angle_Destination = 0.0f;
                break;
        }
        AngleSpeedJudge();
    }
    // 角速度の符号決定
    private void AngleSpeedJudge()
    {
        Angle_Speed = Mathf.Abs(Angle_Speed);
        if (transform.localRotation.ToEuler().y > Angle_Destination)
        {
            Angle_Speed *= -1.0f;
        }
    }

    // 回転処理
    private void Rotation_Update()
    {
        // 現在の角度が目標の角度に到達していなければ回転を続ける
        if (Mathf.Abs(Mathf.Abs(transform.localEulerAngles.y) - Mathf.Abs(Angle_Destination)) > 0.02f)
        {
            // 回転
            transform.Rotate(0.0f, Angle_Speed, 0.0f);

            // 目標の値を超えた場合は補正
            if (Mathf.Abs(Mathf.Abs(transform.localEulerAngles.y) - Mathf.Abs(Angle_Destination)) < 0.02f)
            {
                transform.localRotation = Quaternion.Euler(0.0f, -Angle_Destination, 0.0f);
                Debug.Log(transform.localRotation.y);
                // 回転終了時に回転状態を更新
                if (RotateState != ROTATESTATE.REROTATE)
                {
                    RotateState = ROTATESTATE.ROTATED;
                }
                else
                {
                    RotateState = ROTATESTATE.NEUTRAL;
                }
            }
        }
    }

    // プレイヤーに対する衝突検知
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") IsHit = true;
        GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") IsHit = false;
        GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
    }
}
