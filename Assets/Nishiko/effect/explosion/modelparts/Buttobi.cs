using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttobi : MonoBehaviour
{
    private float timeOut = 5;
    private float timeElapsed = 0;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbodyÇéÊìæ
        Vector3 force = new Vector3(Random.Range(-5, 5),    // Xé≤
                                    Random.Range(5.0f, 6.0f), // Yé≤
                                    Random.Range(-1,-5));   // Zé≤
        rb.AddForce(force, ForceMode.Impulse);          // óÕÇâ¡Ç¶ÇÈ
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += 1 * Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            //Instantiate(m_objMist, this.transform.position, Quaternion.identity);
            //âåê∂ê¨
            //timeElapsed = 0.0f;
            Destroy(this.gameObject);
        }
    }
}
