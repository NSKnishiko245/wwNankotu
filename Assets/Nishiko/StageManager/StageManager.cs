//============================
//name:StageManager
//概要:タイルを折り曲げる処理
//警告が多すぎる問題勃発
//============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Player Player;

    public GameObject MapManager;

    private int[,] BlockNum_Map;
    private GameObject[,] Block_Map;

    private List<GameObject> Tile_List;
    private List<GameObject> Bar_List;

    //パーティクル描画用
    public GameObject particleObject;

    // ステージ番号
    public int stageNum = 1;
    private bool initFlg = true;

    private bool turnFlg = false;

    // 一番初めの更新処理時に行う初期化処理
    void Init()
    {
        // マップエディットクラスからマップ構築メソッドをコールし、そのマップデータを取得
        MapManager.GetComponent<MapEdit>().CreateStage_Game(stageNum);
        Block_Map = MapManager.GetComponent<MapEdit>().BlockList;
        BlockNum_Map = MapManager.GetComponent<MapEdit>().BlockMap;
        Tile_List = MapManager.GetComponent<MapEdit>().TileList;
        Bar_List = MapManager.GetComponent<MapEdit>().BarList;

        // ステージ内オブジェクトを１つのオブジェクトの子供にする
        ParentReset();
    }

    void Update()
    {
        if (initFlg)
        {
            Init();
            initFlg = false;
        }



        if (CanYouMovePlayer())
        {
            Player.GetComponent<Player>().TurnOnRigidbody();
            Player.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else
        {
            Player.GetComponent<Player>().TurnOffRigidbody();
        }





        // トリガーからの入力情報を取得
        float triggerValue = Input.GetAxis("L_R_Trigger");

        // ステージの状況を見て回転できるかどうかを判断
        if (CanYouRotate())
        {
            // プレイヤーと衝突しているバーを取得
            int hitBarNum = GetHitBar();
            if (hitBarNum != -1)
            {
                // ステージを折る直前、バーにステージが反転しているかどうかを知らせる
                for (int i = 0; i < Bar_List.Count; i++)
                {
                    Bar_List[i].GetComponent<Cbar>().ReverseRotateFlg = GetComponent<StageRotate>().isReverse;
                }

                // 右トリガー押下時
                if (triggerValue >= 1.0f)
                {
                    ParentReset();
                    SetParentToLeft();
                    Bar_List[hitBarNum].GetComponent<Cbar>().Rotation(Cbar.ROTATESTATE.ROTATE_LEFT);
                    Player.transform.position = new Vector3(Bar_List[hitBarNum].transform.position.x + 1.0f, Player.transform.position.y, Player.transform.position.z);
                }
                // 左トリガー押下時
                else if (triggerValue <= -1.0f)
                {
                    ParentReset();
                    SetParentToRight();
                    Bar_List[hitBarNum].GetComponent<Cbar>().Rotation(Cbar.ROTATESTATE.ROTATE_RIGHT);
                    Player.transform.position = new Vector3(Bar_List[hitBarNum].transform.position.x - 1.0f, Player.transform.position.y, Player.transform.position.z);
                }
            }
        }
        
        // 回転済みのステージを戻す処理
        if (Input.GetKeyDown("joystick button 0"))
        {
            // 回転済みのバーを検出したら戻す処理を開始
            for (int i = 0; i < Bar_List.Count; i++)
            {
                if (Bar_List[i].GetComponent<Cbar>().RotateState == Cbar.ROTATESTATE.ROTATED)
                {
                    Bar_List[i].GetComponent<Cbar>().Rotation(Cbar.ROTATESTATE.REROTATE);
                }
            }
        }


        if (CanYouWarp())
        {
            // ステージの回転（仮）
            if (!GetComponent<StageRotate>().isRotNow)
            {
                Player.transform.parent = null;
                Player.TurnOnRigidbody();


                // プレイヤーのワープ
                int barNum = GetHitBar();
                if (barNum != -1)
                {
                    if (isLeftBar(barNum))
                    {
                        WarpPlayerToRight();
                    }
                    else if (isRightBar(barNum))
                    {
                        WarpPlayerToLeft();
                    }
                }
            }
        }
    }

    // 回転可能かどうかをチェック
    private bool CanYouRotate()
    {
        // 回転中や回転後のバーがあった場合、入力はさせない
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].GetComponent<Cbar>().RotateState != Cbar.ROTATESTATE.NEUTRAL)
            {
                return false;
            }
        }
        return true;
    }
    // プレイヤーと衝突しているバーを返す（戻り値が -1 以外なら衝突している）
    private int GetHitBar()
    {
        int rotBarNum;
        for (rotBarNum = Bar_List.Count - 1; rotBarNum >= 0; rotBarNum--)
        {
            // カウント変数がプレイヤーと衝突しているバーと同じになればループを抜ける
            if (Bar_List[rotBarNum].GetComponent<Cbar>().IsHit) break;
        }
        return rotBarNum;
    }

    // 右から左へ順に親にしていく（２つのバーを比較した時、要素番号の小さい方が親）
    private void SetParentToLeft()
    {
        for (int cnt = Bar_List.Count - 1; cnt > 0; cnt--)
        {
            Tile_List[cnt - 1].transform.parent = Bar_List[cnt].transform;
            Bar_List[cnt - 1].transform.parent = Tile_List[cnt - 1].transform;
        }
    }
    // 左から右へ順に親にしていく（２つのバーを比較した時、要素番号の大きい方が親）
    private void SetParentToRight()
    {
        for (int cnt = 0; cnt < Bar_List.Count - 1; cnt++)
        {
            Tile_List[cnt].transform.parent = Bar_List[cnt].transform;
            Bar_List[cnt + 1].transform.parent = Tile_List[cnt].transform;
        }
    }
    //親子関係をリセット
    private void ParentReset()
    {
        for (int i = 0; i < Tile_List.Count; i++)
        {
            Tile_List[i].transform.parent = this.gameObject.transform;
        }
        for (int i = 0; i < Bar_List.Count; i++)
        {
            Bar_List[i].transform.parent = this.gameObject.transform;
        }
    }
    

    // 引数で渡された要素番号のバーが全てのバーの中で一番左に位置しているかどうか
    private bool isLeftBar(int bar_idx)
    {
        float min_x = Bar_List[bar_idx].transform.position.x;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x < min_x)
            {
                if (min_x - Bar_List[i].transform.position.x > 0.02) return false;
            }
        }
        return true;
    }
    // 引数で渡された要素番号のバーが全てのバーの中で一番右に位置しているかどうか
    private bool isRightBar(int bar_idx)
    {
        float max_x = Bar_List[bar_idx].transform.position.x;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x > max_x)
            {
                if (Bar_List[i].transform.position.x - max_x > 0.02f) return false;
            }
        }
        return true;
    }

    // 一番左のバーの要素番号を取得
    private int GetMaxLeftBar()
    {
        int bar_idx = 0;
        float min_x = 999;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x < min_x)
            {
                min_x = Bar_List[i].transform.position.x;
                bar_idx = i;
            }
        }
        return bar_idx;
    }
    // 一番右のバーの要素番号を取得
    private int GetMaxRightBar()
    {
        int bar_idx = 0;
        float max_x = -999;
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (Bar_List[i].transform.position.x > max_x)
            {
                max_x = Bar_List[i].transform.position.x;
                bar_idx = i;
            }
        }
        return bar_idx;
    }

    private void WarpPlayerToLeft()
    {
        Vector3 leftBar_pos = new Vector3(
            Bar_List[GetMaxLeftBar()].transform.position.x,
            - Player.transform.position.y,
            Player.transform.position.z
        );
        Ray ray = new Ray(leftBar_pos, Vector3.right);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f))
        {
            if (hit.collider.CompareTag("Block"))
            {
                return;
            }
        }
        Player.transform.localPosition = new Vector3(leftBar_pos.x + Player.transform.localScale.x, leftBar_pos.y, leftBar_pos.z);
        turnFlg = true;
        GetComponent<StageRotate>().TurnOnRotate();
        Player.transform.parent = this.transform;
        Player.TurnOffRigidbody();
       
    }
    private void WarpPlayerToRight()
    {
        Vector3 rightBar_pos = new Vector3(
            Bar_List[GetMaxRightBar()].transform.position.x,
            -Player.transform.position.y,
            Player.transform.position.z
        );
        Ray ray = new Ray(rightBar_pos, Vector3.left);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f))
        {
            if (hit.collider.CompareTag("Block"))
            {
                return;
            }
        }
        Player.transform.localPosition = new Vector3(rightBar_pos.x - Player.transform.localScale.x, rightBar_pos.y, rightBar_pos.z);
        turnFlg = true;
        GetComponent<StageRotate>().TurnOnRotate();
        Player.transform.parent = this.transform;
        Player.TurnOffRigidbody();
        
    }

    private bool CanYouMovePlayer()
    {
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (!(Bar_List[i].GetComponent<Cbar>().RotateState == Cbar.ROTATESTATE.NEUTRAL ||
                Bar_List[i].GetComponent<Cbar>().RotateState == Cbar.ROTATESTATE.ROTATED) ||
                GetComponent<StageRotate>().isRotNow)
            {
                return false;
            }
        }
        return true;
    }

    // ワープ可能かどうかをチェック
    private bool CanYouWarp()
    {
        for (int i = 0; i < Bar_List.Count; i++)
        {
            if (!(Bar_List[i].GetComponent<Cbar>().RotateState == Cbar.ROTATESTATE.NEUTRAL ||
                Bar_List[i].GetComponent<Cbar>().RotateState == Cbar.ROTATESTATE.ROTATED))
            {
                return false;
            }
        }
        return true;
    }

    //タイルのコンポーネント
    private void TileReset()
    {
        //for (int i = 0; i < m_tileObj.Count; i++)
        //{
        //    m_tileObj[i].GetComponent<ScreenShot>().ResetTexture();
        //}
    }

    //タイルの子供を
    private void SetChildActive(GameObject obj,bool flg)
    {
        //obj.transform.Find("Cube").gameObject.SetActive(flg);
        //if (flg)
        //{
        //    Instantiate(particleObject, obj.transform.Find("Cube").transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成
        //}
    }
}
