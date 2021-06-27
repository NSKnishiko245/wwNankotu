using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotation : MonoBehaviour
{
    [SerializeField, Range(-5, 5)]
    private float rotSpeed = -0.2f;  // ‰ñ“]‘¬“x

   
    [SerializeField] private bool rotFlg = true; // true:‰ñ“]‚·‚é

    private int cnt = 0;
    [SerializeField] private int lifeframe = 60;

    public float target_Rotate;
    private void Update()
    {
       
        if (rotFlg)
        {
            this.transform.Rotate(new Vector3(0.0f, 0.0f, rotSpeed));

            if (cnt > lifeframe) rotSpeed = 0;
            cnt++;

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

    // ‰ñ“]ƒtƒ‰ƒO‚ðƒZƒbƒg
    public void SetRotFlg(bool sts)
    {
        rotFlg = sts;
    }

    //public void RotFlg(bool Flg)
    //{
    //    tutorialrotFlg = Flg;
    //}
}
