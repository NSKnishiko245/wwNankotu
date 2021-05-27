using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotation : MonoBehaviour
{
    [SerializeField, Range(-5, 5)]
    private float rotSpeed = -0.2f;  // 回転速度

   
    [SerializeField] private bool rotFlg = true; // true:回転する

   

    public float target_Rotate;
    private void Update()
    {
       
        if (rotFlg)
        {
            this.transform.Rotate(new Vector3(0.0f, 0.0f, rotSpeed));
        }

        //if(tutorialrotFlg)
        //{
        //    if (Quaternion.Angle(nowrot, target) <= 1)
        //    {
        //        transform.rotation = target;
        //    }
        //    else
        //    {
        //        transform.Rotate(new Vector3(0, 0, -1f));
        //    }
        //    //this.transform.Rotate(new Vector3(0.0f, 0.0f, tutorialrotSpeed));
        //}

    }

    // 回転フラグをセット
    public void SetRotFlg(bool sts)
    {
        rotFlg = sts;
    }

    //public void RotFlg(bool Flg)
    //{
    //    tutorialrotFlg = Flg;
    //}
}
