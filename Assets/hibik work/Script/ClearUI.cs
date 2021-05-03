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

    AudioSource audioSource;
    [SerializeField] private AudioSource resultSeSource;

    [SerializeField] private float sceneChangeTime; // シーン遷移までの時間
    private int sceneChangeCnt = 0;                 // シーン遷移のカウンタ

    private bool clearFlg = false;
    private bool firstClearFlg = true;
    private bool nextFlg = false;
    private bool stageSelectFlg = false;

    [SerializeField] private float OperationTime;
    private int OperationCnt = 0;

    private enum STATUS
    {
        STAGE_SELECT,
        NEXT_STAGE,
        DECISION,
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
        if (clearFlg)
        {
            if (firstClearFlg)
            {
                resultSeSource.Play();
                firstClearFlg = false;
            }
            if (OperationCnt < OperationTime) OperationCnt++;
            else if (status != STATUS.DECISION) Operation();

            if (stageSelectFlg)
            {
                // 一定時間経過すると遷移する
                if (sceneChangeCnt > sceneChangeTime)
                {
                    SceneManager.LoadScene("SelectScene");
                }
                sceneChangeCnt++;
            }

            if (nextFlg)
            {
                // 一定時間経過すると遷移する
                if (sceneChangeCnt > sceneChangeTime)
                {
                    StageManager.stageNum++;
                    SceneManager.LoadScene("Stage1Scene");
                }
                sceneChangeCnt++;
            }
        }
    }

    public void ClearFlgOn()
    {
        if (!clearFlg)
        {
            clearFlg = true;
            StageSelectManager.score[StageManager.stageNum].isCopper = true;
        }
    }
    public bool GetCLearFlg()
    {
        return clearFlg;
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
                stageSelectFlg = true;
                status = STATUS.DECISION;
                this.GetComponent<PostEffectController>().SetFireFlg(false);
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
                nextFlg = true;
                status = STATUS.DECISION;
                this.GetComponent<PostEffectController>().SetFireFlg(false);
            }
        }
    }
}