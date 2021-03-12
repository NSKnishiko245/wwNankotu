using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameObject GoalText;

    // Start is called before the first frame update
    void Start()
    {
        GoalText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnCollisionEnter(Collision collision)
    {
        GoalText.SetActive(true);
        Debug.Log("Goal");
    }
}
