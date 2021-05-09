using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayCenter : MonoBehaviour
{
    public enum DIRECTION
    {
        LEFT,
        RIGHT,
    }
    public DIRECTION m_Direction { get; set; }

    private bool m_isPlayerHit = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            m_isPlayerHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            m_isPlayerHit = false;
        }
    }

    //プレイヤーと接触しているか？
    public bool GetIsPlayerHit()
    {
        return m_isPlayerHit;
    }
}
