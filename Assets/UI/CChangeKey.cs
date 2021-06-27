using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChangeKey : MonoBehaviour
{
    public GameObject[] m_Keyobj;

    public GameObject[] m_Conobj;

    enum MODE
    {
        KEYBOARD,
        CONTROLLER
    }
    private MODE m_mode;
   
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int cnt=99;
        var controllerNames = Input.GetJoystickNames();

        for (int i = 0; i < controllerNames.Length; i++)
        {
            if (controllerNames[i] == "")
            {

            }
            else
            {
                cnt = i;
                break;
            }
        }

        if (cnt == 99)
        {
            for (int i = 0; i < m_Keyobj.Length; i++)
            {
                m_Keyobj[i].SetActive(true);
            }
            for (int i = 0; i < m_Conobj.Length; i++)
            {
                m_Conobj[i].SetActive(false);
            }
            return;
        }

        //コントローラーが繋がっているかチェック
        if (controllerNames[cnt] == "")
        {
            m_mode = MODE.KEYBOARD;
        }
        else
        {
            m_mode = MODE.CONTROLLER;
        }


        if (m_mode == MODE.KEYBOARD)
        {
            for (int i = 0; i < m_Keyobj.Length; i++)
            {
                m_Keyobj[i].SetActive(true);
            }
            for (int i = 0; i < m_Conobj.Length; i++)
            {
                m_Conobj[i].SetActive(false);
            }
        }
        else if (m_mode == MODE.CONTROLLER)
        {
            for (int i = 0; i < m_Keyobj.Length; i++)
            {
                m_Keyobj[i].SetActive(false);
            }
            for (int i = 0; i < m_Conobj.Length; i++)
            {
                m_Conobj[i].SetActive(true);
            }
        }
        else
        {
            Debug.Log("CChageKey script error");
        }

        //Debug.Log(Input.GetJoystickNames());
        //var joystickNames = Input.GetJoystickNames();

        //if (CacheJoystickNames.Length > joystickNames.Length)
        //{
        //    Debug.Log("切断" + CacheJoystickNames.Except(joystickNames).ToList()[0]);
        //    //for (int i = 0; i < m_Keyobj.Length; i++)
        //    //{
        //    //    m_Keyobj[i].SetActive(true);
        //    //}
        //    //for (int i = 0; i < m_Conobj.Length; i++)
        //    //{
        //    //    m_Conobj[i].SetActive(false);
        //    //}
        //}

        //if (CacheJoystickNames.Length < joystickNames.Length)
        //{
        //    Debug.Log("接続" + joystickNames.Except(CacheJoystickNames).ToList()[0]);
        //    //for (int i = 0; i < m_Keyobj.Length; i++)
        //    //{
        //    //    m_Keyobj[i].SetActive(false);
        //    //}
        //    //for (int i = 0; i < m_Conobj.Length; i++)
        //    //{
        //    //    m_Conobj[i].SetActive(true);
        //    //}
        //}

        //CacheJoystickNames = joystickNames;

        //Debug.Log(CacheJoystickNames[0]);
        //Debug.Log(CacheJoystickNames[1]);
        //Debug.Log(CacheJoystickNames[2]);
        //Debug.Log(CacheJoystickNames[3]);
        Debug.Log(cnt);
    }
}
