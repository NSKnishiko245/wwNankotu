using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("stageManager").GetComponent<StageManager>().IsGameClear)
        {
            transform.Find("Effect_Mist").gameObject.SetActive(false);
        }
    }
}
