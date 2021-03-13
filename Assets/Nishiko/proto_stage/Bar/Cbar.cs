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
            other.gameObject.GetComponent<Player>().SetHitNum(this.gameObject.GetComponent<OrimeObj>().GetObjNumber());
            stay = true;
            Debug.Log("aaaaa");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.gameObject.GetComponent<Player>().SetHitNum(99);

            stay = false;
            Debug.Log("aaaaa");
        }
    }
}
