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
        Rigidbody rb = this.GetComponent<Rigidbody>();  // rigidbodyを取得
        Vector3 force = new Vector3(Random.Range(-5, 5),    // X軸
                                    Random.Range(5.0f, 6.0f), // Y軸
                                    Random.Range(-1,-5));   // Z軸
        rb.AddForce(force, ForceMode.Impulse);          // 力を加える
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += 1 * Time.deltaTime;

        transform.Rotate(new Vector3(5, 0, 5));

        if (timeElapsed >= timeOut)
        {
           //オブジェクト破棄
            Destroy(this.gameObject);
        }
    }
}
