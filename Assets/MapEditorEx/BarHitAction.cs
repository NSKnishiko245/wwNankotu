using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarHitAction : MonoBehaviour
{
    public enum BARSTATE
    {
        NEUTORAL,
        SELECT,
        DECIDED
    }
    private BARSTATE m_BarState = BARSTATE.NEUTORAL;
    public BARSTATE BarState
    {
        get { return m_BarState; }
        set { m_BarState = value; }
    }

    private bool m_IsPushButton = false;

    void Start()
    {

    }
    void Update()
    {
        if (m_IsPushButton && !Input.GetMouseButton(2))
        {
            switch (m_BarState)
            {
                case BARSTATE.SELECT:
                    m_BarState = BARSTATE.DECIDED;
                    break;
                case BARSTATE.DECIDED:
                    m_BarState = BARSTATE.NEUTORAL;
                    break;
            }
            m_IsPushButton = false;
        }

        Color myColor = Color.gray;
        switch (m_BarState)
        {
            case BARSTATE.NEUTORAL:
                myColor = Color.gray;
                break;
            case BARSTATE.SELECT:
                myColor = Color.yellow;
                break;
            case BARSTATE.DECIDED:
                myColor = Color.red;
                break;
        }
        this.GetComponent<Renderer>().material.color = myColor;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_BarState == BARSTATE.NEUTORAL)
        {
            m_BarState = BARSTATE.SELECT;
        }

        if (Input.GetMouseButton(2))
        {
            m_IsPushButton = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (m_BarState == BARSTATE.SELECT)
        {
            m_BarState = BARSTATE.NEUTORAL;
        }
    }
}