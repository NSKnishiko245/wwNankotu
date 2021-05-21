using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreAnimation : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private GameObject stageClear;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private GameObject nextStage;

    private Animator stageClearAnim;
    private Animator stageSelectAnim;
    private Animator nextStageAnim;

    [SerializeField] private GameObject[] medal = new GameObject[3];
    private Animator[] medalAnim = new Animator[3];

    // クリア後のカメラの位置調整用(ゴールからの差をセットする)
    private Vector3 cameraStartPos;    // カメラの初期位置
    [SerializeField] private Vector3 cameraAdjustmentPos;
    private Vector3 cameraNowPos;       // カメラの現在座標
    private Vector3 goalPos;            // ゴールの座標
    private Vector3 cameraTargetPos;    // カメラの移動先の座標
    private Vector3 cameraMovePos;      // カメラが１フレームに移動する座標
    [SerializeField] private int cameraMoveSpeed; // カメラが移動する速度
    private int cameraStartMoveCnt;          // カメラが移動する回数
    private int stageDeleteCnt = 120;

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
    private bool startFlg = false;      // クリアアニメーション開始
    private bool endFlg = false;      // クリアアニメーション終了
    private bool silverFlg = false;
    private bool goldFlg = false;
    private bool operationFlg = false;

    //==============================================================
    // 初期処理
    //==============================================================
    private void Start()
    {
        stageClearAnim = stageClear.GetComponent<Animator>();
        stageSelectAnim = stageSelect.GetComponent<Animator>();
        nextStageAnim = nextStage.GetComponent<Animator>();

        for (int i = 0; i < 3; i++) medalAnim[i] = medal[i].GetComponent<Animator>();

        cameraStartPos = mainCamera.transform.position;
        cameraNowPos = mainCamera.transform.position;
        cameraStartMoveCnt = cameraMoveSpeed;

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
        if (startFlg)
        {
            // クリア後に1度だけ行う処理
            FirstClearFunc();

            // クリア後のUIのアニメーション
            ClearUiAnimation();

            // メダルが出てくるアニメーション
            MedalAnimation();

            // ステージ内の全てのメダルを取得するとメダルが回転する
            MedalRotation();
        }

        if (endFlg)
        {
            stageClearAnim.SetBool("isMove", false);
            stageSelectAnim.SetBool("isRot", false);
            stageSelectAnim.SetBool("isMove", false);
            nextStageAnim.SetBool("isRot", false);
            nextStageAnim.SetBool("isMove", false);

            if (stageDeleteCnt == 0)
            {
                for (int i = 0; i < 3; i++) medal[i].SetActive(false);

                this.GetComponent<StageUIManager>().StageDisplay(false);
                this.GetComponent<StageUIManager>().StageImageDisplay(true);
            }
            else stageDeleteCnt--;
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
            goalPos = GameObject.Find("Player").transform.position;
            goalPos.z += 0.4f;

            // カメラの移動先の座標をセット
            cameraTargetPos = goalPos + cameraAdjustmentPos;

            mainCamera.transform.DOLocalMove(cameraTargetPos, 1.0f);

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
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0")))
            {
                nextStageAnim.SetBool("isRot", true);
                stageSelectAnim.SetBool("isRot", true);
                operationFlg = true;
            }
        }
        else clearUiRotCnt--;

        if (operationFlg)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 pos = new Vector3(0.0f, -0.1f, 0.0f);
                medal[i].transform.position += pos;
            }
        }
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
                    if (silverFlg && medalNum == 1)
                    {
                        medalAnim[medalNum].SetBool("isMove", true);
                    }
                    if (goldFlg && medalNum == 2)
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

    public bool GetOperationFlg()
    {
        return operationFlg;
    }

    public void StartFlgOn()
    {
        startFlg = true;
    }

    public void EndFlgOn()
    {
        endFlg = true;
    }

    public void SilverFlgOn()
    {
        silverFlg = true;
    }

    public void GoldFlgOn()
    {
        goldFlg = true;
    }
}