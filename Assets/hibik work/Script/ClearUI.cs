using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearUI : MonoBehaviour
{
    private GameObject stageUIManager;

    private GameObject stageClearImage;
    private GameObject stageSelectImage;
    private GameObject nextStageImage;

    private GameObject selectGear;
    private GameObject selectUnderGear;
    private GameObject nextGear;
    private GameObject nextUnderGear;

    [SerializeField] private AudioClip bgm;
    AudioSource audioSource;
    [SerializeField] private AudioSource resultSeSource;

    private bool clearFlg = false;
    private bool firstClearFlg = true;
    private bool firstAnimFlg = false;

    private enum STATUS
    {
        STAGE_SELECT,
        NEXT_STAGE,
    }
    STATUS status;

    void Awake()
    {
        stageUIManager = GameObject.Find("StageUIManager");

        stageClearImage = GameObject.Find("StageClear");
        stageSelectImage = GameObject.Find("C_StageSelect");
        nextStageImage = GameObject.Find("NextStage");

        selectGear = GameObject.Find("C_SelectGearImage");
        selectUnderGear = GameObject.Find("SelectUnderGearImage");
        nextGear = GameObject.Find("NextGearImage");
        nextUnderGear = GameObject.Find("NextUnderGearImage");

        selectGear.GetComponent<GearRotation>().SetRotFlg(false);
        selectUnderGear.GetComponent<GearRotation>().SetRotFlg(false);
        nextGear.GetComponent<GearRotation>().SetRotFlg(false);
        nextUnderGear.GetComponent<GearRotation>().SetRotFlg(false);

        audioSource = stageUIManager.GetComponent<AudioSource>();
        status = STATUS.STAGE_SELECT;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    ClearFlgOn();
        //}

        if (clearFlg)
        {
            if (firstClearFlg)
            {
                resultSeSource.Play();
                firstClearFlg = false;
            }

            if (!firstAnimFlg)
            {
                FirstAnimation();
            }
            else
            {
                Operation();
            }
        }
    }

    public void ClearFlgOn()
    {
        if (!clearFlg)
        {
            clearFlg = true;
            audioSource.PlayOneShot(bgm);
        }
    }
    public bool GetCLearFlg()
    {
        return clearFlg;
    }

    private void FirstAnimation()
    {
        Vector2 pos = stageClearImage.GetComponent<RectTransform>().anchoredPosition;
        

        if (pos.y > 400)
        {
            pos.y -= 10;
        }

        stageClearImage.GetComponent<RectTransform>().anchoredPosition = pos;

        pos = stageSelectImage.GetComponent<RectTransform>().anchoredPosition;

        if (pos.y < -280)
        {
            pos.y += 10;
        }
        stageSelectImage.GetComponent<RectTransform>().anchoredPosition = pos;

        pos = nextStageImage.GetComponent<RectTransform>().anchoredPosition;

        if (pos.y < -280)
        {
            pos.y += 10;
        }
        else
        {
            firstAnimFlg = true;
        }
        nextStageImage.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    private void Operation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("joystick button 5"))
        {
            status = STATUS.NEXT_STAGE;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("joystick button 4"))
        {
            status = STATUS.STAGE_SELECT;
        }

        if (status == STATUS.STAGE_SELECT)
        {
            selectGear.GetComponent<GearRotation>().SetRotFlg(true);
            selectUnderGear.GetComponent<GearRotation>().SetRotFlg(true);
            nextGear.GetComponent<GearRotation>().SetRotFlg(false);
            nextUnderGear.GetComponent<GearRotation>().SetRotFlg(false);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                SceneManager.LoadScene("SelectScene");
            }
        }
        if (status == STATUS.NEXT_STAGE)
        {
            selectGear.GetComponent<GearRotation>().SetRotFlg(false);
            selectUnderGear.GetComponent<GearRotation>().SetRotFlg(false);
            nextGear.GetComponent<GearRotation>().SetRotFlg(true);
            nextUnderGear.GetComponent<GearRotation>().SetRotFlg(true);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                int nextStageNum = stageUIManager.GetComponent<StageUIManager>().GetStageNum() + 1;
                SceneManager.LoadScene("Stage" + nextStageNum + "Scene");
            }
        }
    }
}
