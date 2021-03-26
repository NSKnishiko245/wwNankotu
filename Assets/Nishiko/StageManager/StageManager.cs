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
    bool firstFlg = false;
        
    void StartEx()
    {
        // マップエディットクラスからマップ構築メソッドをコールし、そのマップデータを取得
        MapManager.GetComponent<MapEdit>().CreateStage_Game(stageNum);
        Block_Map = MapManager.GetComponent<MapEdit>().BlockList;
        BlockNum_Map = MapManager.GetComponent<MapEdit>().BlockMap;
        Tile_List = MapManager.GetComponent<MapEdit>().TileList;
        Bar_List = MapManager.GetComponent<MapEdit>().BarList;

        // ステージ内オブジェクトを１つのオブジェクトの子供にする
        //for (int i = 0; i < Tile_List.Count; i++)
        //{
        //    Tile_List[i].transform.parent = this.transform;
        //}
        //for (int i = 0; i < Bar_List.Count; i++)
        //{
        //    Bar_List[i].transform.parent = this.transform;
        //}
    }

    void Update()
    {
        if (!firstFlg)
        {
            StartEx();
            firstFlg = true;
        }

        // トリガーからの入力情報を取得
        float triggerValue = Input.GetAxis("L_R_Trigger");

        int rotBarNum = 0;

        // 回転可能かどうかをチェック
        for (int i = 0; i < Bar_List.Count; i++)
        {
            // 回転中や回転後のバーがあった場合、入力はさせない
            if (Bar_List[i].GetComponent<Cbar>().RotateState != Cbar.ROTATESTATE.NEUTRAL)
            {
                triggerValue = 0.0f;
                break;
            }
            // プレイヤーと衝突しているバーの番号を取得
            if (Bar_List[i].GetComponent<Cbar>().IsHit)
            {
                rotBarNum = i;
            }
        }

        // トリガーの入力値を見てどちらに回転するか判断
        if (triggerValue >= 1.0f)
        {
            ParentReset();
            SetParentToLeft();
            Bar_List[rotBarNum].GetComponent<Cbar>().Rotation_Left();
        }
        else if (triggerValue <= -1.0f)
        {
            ParentReset();
            SetParentToRight();
            Bar_List[rotBarNum].GetComponent<Cbar>().Rotation_Right();
        }

        
        // 回転済みのステージを戻す処理
        if (Input.GetKeyDown("joystick button 0"))
        {

        }
       

        // ワープ処理
        //Vector2Int hitBlockNum = new Vector2Int(-1, -1);
        //for (int y = 0; y < Block_Map.GetLength(0); y++)
        //{
        //    for (int x = 0; x < Block_Map.GetLength(1); x++)
        //    {
        //        if (BlockNum_Map[y, x] != 0)
        //        {
        //            if (Block_Map[y, x].tag == "BorderLine")
        //            {
        //                if (Block_Map[y, x].GetComponent<HitAction>().isHit)
        //                {
        //                    Debug.Log("find");
        //                    hitBlockNum = new Vector2Int(x, y);
        //                    // ２重のループを抜ける為の処理
        //                    x = Block_Map.GetLength(1);
        //                    y = Block_Map.GetLength(0);
        //                }
        //            }
        //        }
        //    }
        //}

        //if (hitBlockNum.x != -1)
        //{
        //    if (hitBlockNum.x < Block_Map.GetLength(1) / 2)
        //    {
        //        for (int y = 0; y < Block_Map.GetLength(0); y++)
        //        {
        //            for (int x = Block_Map.GetLength(1) - 1; x >= 0; x--)
        //            {
        //                if (BlockNum_Map[y, x] != 0)
        //                {
        //                    if (Block_Map[y, x].tag == "BorderLine")
        //                    {
        //                        if (y == Block_Map.GetLength(0) - hitBlockNum.y)
        //                        {
        //                            Player.transform.position = new Vector3(
        //                                Block_Map[y, x].transform.position.x - Block_Map[y, x].transform.lossyScale.x,
        //                                Block_Map[y, x].transform.position.y,
        //                                Block_Map[y, x].transform.position.z
        //                            );
        //                            // ２重のループを抜ける為の処理
        //                            x = -1;
        //                            y = Block_Map.GetLength(0);

        //                            if (y < Block_Map.GetLength(0) / 2)
        //                            {
        //                                Physics.gravity = new Vector3(0, -9.8f, 0);
        //                            }
        //                            else
        //                            {
        //                                Physics.gravity = new Vector3(0, 9.8f, 0);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int y = 0; y < Block_Map.GetLength(0); y++)
        //        {
        //            for (int x = 0; x < Block_Map.GetLength(1); x++)
        //            {
        //                if (BlockNum_Map[y, x] != 0)
        //                {
        //                    if (Block_Map[y, x].tag == "BorderLine")
        //                    {
        //                        if (y == Block_Map.GetLength(0) - hitBlockNum.y)
        //                        {
        //                            Player.transform.position = new Vector3(
        //                                Block_Map[y, x].transform.position.x + Block_Map[y, x].transform.lossyScale.x,
        //                                Block_Map[y, x].transform.position.y,
        //                                Block_Map[y, x].transform.position.z
        //                            );
        //                            // ２重のループを抜ける為の処理
        //                            x = Block_Map.GetLength(1);
        //                            y = Block_Map.GetLength(0);

        //                            if (y < Block_Map.GetLength(0) / 2)
        //                            {
        //                                Physics.gravity = new Vector3(0, -9.8f, 0);
        //                            }
        //                            else
        //                            {
        //                                Physics.gravity = new Vector3(0, 9.8f, 0);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }


    // 左から右へ順に親にしていく（２つのバーを比較した時、要素番号の大きい方が親）
    private void SetParentToRight()
    {
        for (int cnt = Tile_List.Count - 1; cnt > 0; cnt--)
        {
            Tile_List[cnt].transform.parent = Bar_List[cnt - 1].transform;
            Bar_List[cnt - 1].transform.parent = Tile_List[cnt - 1].transform;
        }
    }
    // 右から左へ順に親にしていく（２つのバーを比較した時、要素番号の小さい方が親）
    private void SetParentToLeft()
    {
        for (int cnt = 0; cnt < Tile_List.Count - 1; cnt++)
        {
            Tile_List[cnt].transform.parent = Bar_List[cnt].transform;
            Bar_List[cnt].transform.parent = Tile_List[cnt + 1].transform;
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
