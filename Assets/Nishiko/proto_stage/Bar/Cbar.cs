using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cbar : MonoBehaviour
{
    //ìñÇΩÇ¡ÇƒÇ¢ÇÈÇ©ÅH
    public bool stay { get; private set; }
     
    // Start is called before the first frame update
    void Start()
    {
 
    }

    
    void Update()
    {
        GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        if (stay == false)
        {
            GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    //private void OnTriggerEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Player")
    //    {
    //        stay = true;
    //        Debug.Log("aaaaa");
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.transform.tag == "Player")
    //    {
    //        stay = false;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            stay = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            stay = false;
        }
    }
}
