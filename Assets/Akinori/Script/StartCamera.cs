using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;




public class StartCamera : MonoBehaviour
{

    public enum E_CameraMode
    {
        ZOOM_IN,
        ZOOM_OUT
    }

    public E_CameraMode mode = E_CameraMode.ZOOM_IN;
    private Vector3 purposePos;
    private Vector3 startPos;


    //    private GameObject player;
    public bool isMoving { get; private set; }

    public float startDelay;
    public float zoomWait;

    public bool notMove = false;

    // Start is called before the first frame update
    void Start()
    {


        startPos = transform.position;

        if (StageBgm.bgmFlg) isMoving = true;
        else isMoving = false;

        mode = E_CameraMode.ZOOM_IN;

        //if (GameObject.Find("StageUIManager").GetComponent<StageUIManager>().retryFlg)
        //{
        //    Debug.Log(GameObject.Find("StageUIManager").GetComponent<StageUIManager>().retryFlg);
        //    //return;
        //    isMoving = false;
        //}
        //transform.DetachChildren();
    }

    // Update is called once per frame
    void Update()
    {
        if (notMove)
        {
            return;
        }

        if (!isMoving || !GameObject.Find("StageUIManager").GetComponent<StageUIManager>().GetStageDisplayFlg())
        {
            GetComponent<MoveCamera>().enabled = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 7"))
        {
            isMoving = false;
            mode = E_CameraMode.ZOOM_IN;
            this.transform.DOKill();
            this.transform.position = startPos;
            return;
        }
        
        purposePos = GameObject.FindGameObjectWithTag("Player").gameObject.transform.position;
        purposePos.z = -3;

        purposePos.x *= 1;

        GetComponent<MoveCamera>().enabled = false;

        if (mode == E_CameraMode.ZOOM_IN)
        {
            this.transform.DOMove(new Vector3(purposePos.x, purposePos.y, purposePos.z), 0.8f).SetDelay(0.8f).OnComplete(() =>
             {
                 mode = E_CameraMode.ZOOM_OUT;
             });
        }
        else if (mode == E_CameraMode.ZOOM_OUT)
        {
            this.transform.DOMove(new Vector3(startPos.x, startPos.y, startPos.z), 1f).SetDelay(1.5f).OnComplete(() =>
            {
                isMoving = false;
                mode = E_CameraMode.ZOOM_IN;
                this.transform.DOKill();
            });
        }
    }


}
