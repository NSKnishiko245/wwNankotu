using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayBlock : MonoBehaviour
{
    [Header("Center")]
    public GameObject m_centerobj;

    [Header("è¡ÇµÇΩÇËÇ∑ÇÈObj")]
    public GameObject m_Collobj;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_centerobj.GetComponent<OneWayCenter>().GetIsPlayerHit())
        {
            m_Collobj.SetActive(false);
        }
        else
        {
            m_Collobj.SetActive(true);
        }
    }


}
