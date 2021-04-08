using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAction : MonoBehaviour
{
    public bool isHit { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        isHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        isHit = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isHit = false;
    }
}
