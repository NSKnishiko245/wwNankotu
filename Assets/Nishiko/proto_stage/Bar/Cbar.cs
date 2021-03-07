using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cbar : MonoBehaviour
{
    private bool stay;      //ìñÇΩÇ¡ÇƒÇ¢ÇÈÇ©ÅH
     
    // Start is called before the first frame update
    void Start()
    {
 
    }

    
    void Update()
    {
        GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        if (stay == false)
        {
            GetComponent<Renderer>().material.color = new Color(1, 1, 1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            stay = true;
            Debug.Log("aaaaa");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            stay = false;
        }
    }

}
