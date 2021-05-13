using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleMode : MonoBehaviour
{
    private float timeOut = 1;
    private float timeElapsed = 0;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbody‚ðŽæ“¾
        Vector3 force = new Vector3(Random.Range(-1, 1),    // XŽ²
                                    Random.Range(-1.0f, 0.0f), // YŽ²
                                    Random.Range(-1, -2));   // ZŽ²
        rb.AddForce(force, ForceMode.Impulse);          // —Í‚ð‰Á‚¦‚é
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += 1 * Time.deltaTime;

        transform.Rotate(new Vector3(5, 0, 5));

        if (timeElapsed >= timeOut)
        {
            Destroy(this.gameObject);
        }
    }
}
