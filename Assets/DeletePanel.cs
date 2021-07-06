using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeletePanel : MonoBehaviour
{
    public GameObject panel;
    public GameObject yes;
    public GameObject no;

    public GameObject galiver;
    public static bool Flg = false;
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
        if (Input.GetKeyDown(KeyCode.K) && !FinshManager.escFlg)
        {
            if (!Flg)
            {
                panel.SetActive(true);
                Flg = true;
            }
            else
            {
                panel.SetActive(false);
                Flg = false;
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

        if (Flg && Input.GetKeyUp(KeyCode.Z))
        {
            if (num == 1)
            {
                StageSelectManager.DeleteSaveDate();
                panel.SetActive(false);
                Flg = false;
                Destroy(galiver.gameObject);
                SceneManager.LoadScene("Op");
            }
            else
            {
                panel.SetActive(false);
                Flg = false;
            }
        }
    }
}
