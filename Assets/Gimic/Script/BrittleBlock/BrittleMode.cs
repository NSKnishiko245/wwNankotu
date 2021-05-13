using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleMode : MonoBehaviour
{
    public GameObject m_objBrittleBlock;
    bool m_IsMove=false;//‰ó‚ê‚Ä‚¢‚éó‘Ô‚©H
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_objBrittleBlock.GetComponent<BrittleBlock>().GetIsBroke())
        {
            if (!m_IsMove)//“®‚¢‚Ä‚¢‚È‚¯‚ê‚Î
            {
                m_IsMove = true;
                Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbody‚ğæ“¾
                Vector3 force = new Vector3(Random.Range(-1, 1),    // X²
                                            Random.Range(1.0f, 2.0f), // Y²
                                            Random.Range(-1, -2));   // Z²
                rb.AddForce(force, ForceMode.Impulse);          // —Í‚ğ‰Á‚¦‚é
                this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }

    }
}
