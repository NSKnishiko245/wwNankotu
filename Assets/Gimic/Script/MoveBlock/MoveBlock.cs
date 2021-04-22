using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    //’[‚Á‚±‚Ì“–‚½‚è”»’è
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "GimicMoveBar")
        {
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "GimicMoveBar")
        {
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

}
