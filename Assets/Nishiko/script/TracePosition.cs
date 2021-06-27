using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracePosition : MonoBehaviour
{

    public GameObject m_obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = m_obj.transform.position;
    }
}
