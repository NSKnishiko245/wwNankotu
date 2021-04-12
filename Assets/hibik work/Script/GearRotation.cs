using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotation : MonoBehaviour
{
    [SerializeField, Range(-5, 5)]
    private float rotSpeed = -0.2f;  // ‰ñ“]‘¬“x

    private bool rotFlg = true; // true:‰ñ“]‚·‚é

    private void Update()
    {
        if (rotFlg)
        {
            this.transform.Rotate(new Vector3(0.0f, 0.0f, rotSpeed));
        }
    }

    // ‰ñ“]ƒtƒ‰ƒO‚ðƒZƒbƒg
    public void SetRotFlg(bool sts)
    {
        rotFlg = sts;
    }
}
