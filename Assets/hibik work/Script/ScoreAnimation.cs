using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAnimation : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject stageUIManager;

    [SerializeField] private GameObject stageClear;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private GameObject nextStage;

    private Animator stageClearAnim;
    private Animator stageSelectAnim;
    private Animator nextStageAnim;

    [SerializeField] private GameObject[] medal = new GameObject[3];
    private Animator[] medalAnim = new Animator[3];

    // クリア後のカメラの位置調整用(ゴールからの差をセットする)
    [SerializeField] private Vector3 cameraAdjustmentPos;
    private Vector3 cameraNowPos;       // カメラの現在座標
    private Vector3 goalPos;            // ゴールの座標
    private Vector3 cameraTargetPos;    // カメラの移動先の座標
    private Vector3 cameraMovePos;      // カメラが１フレームに移動する座標
    [SerializeField] private int cameraMoveSpeed; // カメラが移動する速度
    private int cameraMoveCnt;          // カメラが移動する回数

    // ステージクリアしてからステージクリアのテキストが出るまでの時間
    [SerializeField] private int stageClearTextCnt;

    // ステージクリアしてからNextStageとStageSelectが出るまでの時間
    [SerializeField] private int clearUiCnt;
    // 〃が回転するまでの時間
    [SerializeField] private int clearUiRotCnt;

    // ステージクリアしてから最初のメダルが出るまでの時間
    [SerializeField] private int medalAnimStartCnt;

    // 次の歯車が出るまでの時間
    [SerializeField] private int medalIntervalCnt;
    private int medalInterval = 0;
    private int medalNum = 0;

    // ステージクリアしてからメダルが回転するまでの時間
    [SerializeField] private int medalRotationStartCnt;

    private bool firstClearFlg = true; // クリア後に1度だけ行う処理



    //==============================================================
    // 初期処理
    //==============================================================
    private void Start()
    {
        stageClearAnim = stageClear.GetComponent<Animator>();
        stageSelectAnim = stageSelect.GetComponent<Animator>();
        nextStageAnim = nextStage.GetComponent<Animator>();

        for (int i = 0; i < 3; i++) medalAnim[i] = medal[i].GetComponent<Animator>();

        cameraNowPos = mainCamera.transform.position;
        cameraMoveCnt = cameraMoveSpeed;

        // メダルの動作確認用
        //StageSelectManager.score[StageManager.stageNum].isCopper = true;
        //StageSelectManager.score[StageManager.stageNum].isSilver = true;
        //StageSelectManager.score[StageManager.stageNum].isGold = true;
    }

    //==============================================================
    // 更新処理
    //==============================================================
    private void Update()
    {
        // クリア後
        if (stageUIManager.GetComponent<ClearUI>().GetCLearFlg())
        {
            // カメラの位置を徐々に変更
            if (cameraMoveCnt != 0)
            {
                cameraNowPos -= cameraMovePos;
                mainCamera.transform.position = cameraNowPos;
                cameraMoveCnt--;
            }

            // クリア後に1度だけ行う処理
            FirstClearFunc();

            // クリア後のUIのアニメーション
            ClearUiAnimation();

            // メダルが出てくるアニメーション
            MedalAnimation();

            // ステージ内の全てのメダルを取得するとメダルが回転する
            MedalRotation();
        }
    }

    //==============================================================
    // クリア後に1度だけ行う処理
    //==============================================================
    private void FirstClearFunc()
    {
        if (firstClearFlg)
        {
            // ゴールの位置を取得
            goalPos = GameObject.Find("2(Clone)").transform.position;

            // カメラの移動先の座標をセット
            cameraTargetPos = goalPos + cameraAdjustmentPos;

            // カメラが１フレームに移動する座標をセット
            cameraMovePos = (cameraNowPos - cameraTargetPos) / cameraMoveSpeed;

            // ゴールの位置をメダルにセット
            for (int i = 0; i < 3; i++) medal[i].transform.position = goalPos;

            firstClearFlg = false;
        }
    }

    //==============================================================
    // クリア後のUIのアニメーション
    //==============================================================
    private void ClearUiAnimation()
    {
        // ステージクリアしてからステージクリアのテキストが出るまでの時間が０
        if (stageClearTextCnt == 0)
        {
            stageClearAnim.SetBool("isMove", true);
        }
        else stageClearTextCnt--;

        // ステージクリアしてからNextStageとStageSelectが出るまでの時間が０
        if (clearUiCnt == 0)
        {
            nextStageAnim.SetBool("isMove", true);
            stageSelectAnim.SetBool("isMove", true);
        }
        else clearUiCnt--;

        // 〃が回転するまでの時間が０
        if (clearUiRotCnt == 0)
        {
            nextStageAnim.SetBool("isRot", true);
            stageSelectAnim.SetBool("isRot", true);
        }
        else clearUiRotCnt--;
    }

    //==============================================================
    // メダルが出てくるアニメーション
    //==============================================================
    private void MedalAnimation()
    {
        // ステージクリアしてから最初のメダルが出るまでの時間が０
        if (medalAnimStartCnt == 0)
        {
            // 次の歯車が出るまでの時間が０
            if (medalInterval == 0)
            {
                if (medalNum < 3)
                {
                    // アニメーション開始
                    if (StageSelectManager.score[StageManager.stageNum].isCopper && medalNum == 0)
                    {
                        medalAnim[medalNum].SetBool("isMove", true);
                    }
                    if (StageSelectManager.score[StageManager.stageNum].isSilver && medalNum == 1)
                    {
                        medalAnim[medalNum].SetBool("isMove", true);
                    }
                    if (StageSelectManager.score[StageManager.stageNum].isGold && medalNum == 2)
                    {
                        medalAnim[medalNum].SetBool("isMove", true);
                    }
                    medalInterval = medalIntervalCnt;
                    medalNum++;
                }
            }
            else medalInterval--;
        }
        else medalAnimStartCnt--;
    }

    //==============================================================
    // ステージ内の全てのメダルを取得するとメダルが回転する
    //==============================================================
    private void MedalRotation()
    {
        // ステージクリアしてからメダルが回転するまでの時間が０
        if (medalRotationStartCnt == 0)
        {
            // 全てのメダルを取得していると回転する
            if (StageSelectManager.score[StageManager.stageNum].isCopper &&
                StageSelectManager.score[StageManager.stageNum].isSilver &&
                StageSelectManager.score[StageManager.stageNum].isGold)
            {
                for (int i = 0; i < 3; i++) medal[i].GetComponent<GearRotation>().SetRotFlg(true);
            }
        }
        else medalRotationStartCnt--;
    }
}