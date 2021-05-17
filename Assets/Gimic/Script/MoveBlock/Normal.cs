using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レールブロックが通常ブロックと被った時にレールブロックをニョキニョキと出すのを止める。

public class Normal : MonoBehaviour
{
    public GameObject m_objMoveBlock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Block")
        {
            m_objMoveBlock.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
    }
}
