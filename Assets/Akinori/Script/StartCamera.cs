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
    public bool isMoving { get; private set; } = true;

    public float startDelay;
    public float zoomWait;

    // Start is called before the first frame update
    void Start()
    {


        startPos = transform.position;

        isMoving = true;
        mode = E_CameraMode.ZOOM_IN;
        Debug.Log("StartCamera::Start()");

        Debug.Log(startPos);

        //transform.DetachChildren();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving || !GameObject.Find("StageUIManager").GetComponent<StageUIManager>().GetStageDisplayFlg())
        {
            GetComponent<MoveCamera>().enabled = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            isMoving = false;
            this.transform.position = startPos;
        }

        purposePos = GameObject.FindGameObjectWithTag("Player").gameObject.transform.position;
        purposePos.z = -3;

        purposePos.x *= 1;

        GetComponent<MoveCamera>().enabled = false;
        Debug.Log(mode);
        if (mode == E_CameraMode.ZOOM_IN)
        {
            Debug.Log(purposePos);
            this.transform.DOMove(new Vector3(purposePos.x, purposePos.y, purposePos.z), 0.8f).SetDelay(0.8f).OnComplete(() =>
             {

                 Debug.Log("OnComplete!/StartCamera.cs");
                 mode = E_CameraMode.ZOOM_OUT;
             });
        }
        else if (mode == E_CameraMode.ZOOM_OUT)
        {
            this.transform.DOMove(new Vector3(startPos.x, startPos.y, startPos.z), 1f).SetDelay(1.5f).OnComplete(() =>
            {
                Debug.Log("OnComplete!/StartCamera.cs");
                isMoving = false;
                mode = E_CameraMode.ZOOM_IN;
            });
        }
    }


}
