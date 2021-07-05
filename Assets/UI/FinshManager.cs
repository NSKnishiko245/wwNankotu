using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinshManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject yes;
    public GameObject no;

    public static bool escFlg = false;
    private int num = 0;

    // Start is called before the first frame update
    void Start()
    {
        //panel = GameObject.Find("Panel");
        //yes = GameObject.Find("yesImage");
        //no = GameObject.Find("noImage");
        panel.SetActive(false);
        no.transform.localScale = new Vector3(3, 3, 1);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!escFlg)
            {
                panel.SetActive(true);
                escFlg = true;
            }
            else
            {
                panel.SetActive(false);
                escFlg = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            num = 1;
            yes.transform.localScale = new Vector3(3, 3, 1);
            no.transform.localScale = new Vector3(2, 2, 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            num = 0;
            yes.transform.localScale = new Vector3(2, 2, 1);
            no.transform.localScale = new Vector3(3, 3, 1);
        }

        if (escFlg && Input.GetKeyDown(KeyCode.Z))
        {
            if (num == 1)
            {
                Application.Quit();
            }
            else
            {
                panel.SetActive(false);
                escFlg = false;
            }
        }
    }
}
