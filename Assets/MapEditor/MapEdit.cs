using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

// インスペクターからモード選択させる
public enum Mode
{
    Game = 0,
    Edit = 1
}

public class MapEdit : MonoBehaviour
{
    private List<GameObject> Block;
    private int BlockIndex = 1;

    public int[,] BlockMap { get; private set; }
    private List<int> BarMap;

    public GameObject[,] BlockList { get; private set; }
    public List<GameObject> BarList { get; private set; }
    public List<GameObject> TileList { get; private set; }


    private Vector2Int MapSize = new Vector2Int(16, 10);
    private int BarNum;

    private Vector3 BarScale;

    public Mode mode = Mode.Game;

    private GameObject MainCamera;
    private float NoInputBottom;

    private GameObject StageGrid = null;

    string PrefabPath;

    void Start()
    {
        PrefabPath = Application.dataPath + "/Resources/Prefabs/";

        MainCamera = GameObject.FindWithTag("MainCamera");
        NoInputBottom = 0.15f;

        BlockMap = new int[MapSize.y , MapSize.x];
        BlockList = new GameObject[MapSize.y, MapSize.x];

        BarNum = MapSize.x + 1;
        BarMap = new List<int>(new int[BarNum]);
        BarScale = new Vector3(0.2f, MapSize.y, 0.2f);

        TileList = new List<GameObject>();
        BarList = new List<GameObject>();
        if (mode == Mode.Edit)
        {
            for (int i = 0; i < BarNum; i++)
            {
                CreateBar(new Vector3(-(BarNum / 2) + (i * 1.0f), 0.0f, 0.0f), BarScale);
            }
        }

        // プレハブのロード
        Block = new List<GameObject>();
        for (int i = 1; true; i++)
        {
            if (!File.Exists(PrefabPath + i.ToString() + ".prefab")) return;
            Block.Add(Resources.Load("Prefabs/" + i.ToString()) as GameObject);
        }
    }

    void Update()
    {
        if (mode == Mode.Edit)
        {

            // エディター更新処理
            InputForEdit();

            // 初めて更新処理に入った時、ステージをロードしグリッドオブジェクトを取得
            if (StageGrid == null)
            {
                LoadMap(1);
                StageGrid = GameObject.Find("StageGrid");
            }
        }
    }

    // マウス位置がステージの何マス目かを返す
    private Vector2Int GetCellPosFromMousePos()
    {
        // マウスカーソルの位置をスクリーン座標からワールド座標へ
        Vector3 mousePoint_screen = Input.mousePosition;
        mousePoint_screen.z = 1.0f;
        Vector3 mousePoint_world = Camera.main.ScreenToWorldPoint(mousePoint_screen) * (-MainCamera.transform.position.z);
        mousePoint_world.x -= MainCamera.transform.position.x * (-MainCamera.transform.position.z - 1.0f);
        mousePoint_world.y -= MainCamera.transform.position.y * (-MainCamera.transform.position.z - 1.0f);
        // 補正
        if (mousePoint_world.x < 0) mousePoint_world.x--;
        if (mousePoint_world.y < 0) mousePoint_world.y--;
        // 小数点切り捨て
        return new Vector2Int((int)mousePoint_world.x, (int)mousePoint_world.y);
    }

    // 引数で渡されたマス目位置がしてされたマップサイズ内かどうか
    private bool IsOutArea(Vector2Int cellpos, Vector2Int mapSize)
    {
        if (cellpos.x >= mapSize.x / 2 || cellpos.x < -mapSize.x / 2) return false;
        if (cellpos.y >= mapSize.y / 2 || cellpos.y < -mapSize.y / 2) return false;

        // 範囲内
        return true;
    }

    // 指定されたマス目位置が配列の何番目の要素数になるかを返す
    private Vector2Int GetElementNumFormCellPos(Vector2Int cellpos, Vector2Int mapSize)
    {
        return new Vector2Int(cellpos.x + mapSize.x / 2, (mapSize.y / 2 - 1) - cellpos.y);
    }

    public void SaveMap(int stageNum)
    {
        gameObject.GetComponent<CsvWrite>().WriteMapFromCsv(BlockMap, "Stage" + stageNum.ToString());

        UpdateBarMap();
        gameObject.GetComponent<CsvWrite>().WriteBarMapFromCsv(BarMap, "Bar" + stageNum.ToString());
    }
    public void LoadMap(int stageNum)
    {
        // ロード失敗時は Map を 0 埋め
        // ステージマップのロード
        if (!gameObject.GetComponent<CsvWrite>().ReadMapFromCsv(BlockMap, "Stage" + stageNum.ToString()))
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                for (int x = 0; x < MapSize.x; x++)
                {
                    BlockMap[y, x] = 0;
                }
            }
        }
        // バーマップのロード
        if (!gameObject.GetComponent<CsvWrite>().ReadBarMapFromCsv(BarMap, "Bar" + stageNum.ToString()))
        {
            for (int i = 0; i < BarNum; i++)
            {
                BarMap[i] = 0;
            }
        }

        // 読み込んだマップを見てオブジェクトを再ロード
        if (mode == Mode.Edit)
        {
            ReloadMap_Bar();
        }
        ReloadMap_Block();
    }

    private void CreateBlock(Vector2Int cellpos, int blockIdx)
    {
        // 指定したブロックのプレハブが存在しない場合は中断
        if (blockIdx > Block.Count) return;

        // マウスの位置から、その位置に対応したマップの要素数に変換
        Vector2Int elementNum = GetElementNumFormCellPos(cellpos, MapSize);


        // プレイヤーブロックはeditモードの時のみ生成
        // 一度指定された場所のオブジェクトを削除、のち生成
        GameObject obj = Instantiate(Block[blockIdx - 1], new Vector3(cellpos.x + 0.5f, cellpos.y + 0.5f, 0.0f), new Quaternion(0, 0, 0, 1));
        if (Block[blockIdx - 1].transform.tag == "Player" && mode == Mode.Game)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
            obj.GetComponent<BoxCollider>().enabled = false;
        }
        Destroy(BlockList[elementNum.y, elementNum.x]);
        BlockMap[elementNum.y, elementNum.x] = blockIdx;
        BlockList[elementNum.y, elementNum.x] = obj;
    }
    private void DeleteBlock(Vector2Int cellpos)
    {
        Vector2Int elementNum = GetElementNumFormCellPos(cellpos, MapSize);

        // マップからブロックを削除
        BlockMap[elementNum.y, elementNum.x] = 0;
        Destroy(BlockList[elementNum.y, elementNum.x]);
    }
    private void DeletePlayerBlock()
    {
        for (int y = 0; y < BlockList.GetLength(0); y++)
        {
            for (int x = 0; x < BlockList.GetLength(1); x++)
            {
                if (BlockMap[y, x] != 0)
                {
                    if (BlockList[y, x].transform.tag == "Player")
                    {
                        Destroy(BlockList[y, x]);
                        BlockMap[y, x] = 0;
                    }
                }
            }
        }
    }
    private void CreateBar(Vector3 pos, Vector3 scale)
    {
        string barPath;
        if (mode == Mode.Edit)
        {
            barPath = "Bar_Editor";
        }
        else
        {
            barPath = "Bar_Game";
        }
        if (File.Exists(PrefabPath + barPath + ".prefab"))
        {
            GameObject bar = Instantiate(Resources.Load("Prefabs/" + barPath) as GameObject, pos, new Quaternion(0, 0, 0, 1));
            bar.transform.localScale = scale;
            BarList.Add(bar);
        }
        else
        {
            Debug.Log("Not found" + barPath + ".prefab");
        }
    }
    private GameObject CreateTile(Vector3 pos, Vector3 scale)
    {
        if (File.Exists(PrefabPath + "Tile.prefab"))
        {
            GameObject tile = Instantiate(Resources.Load("Prefabs/Tile") as GameObject, pos, new Quaternion(0, 0, 0, 1));
            tile.transform.localScale = scale;
            TileList.Add(tile);
            return tile;
        }
        else
        {
            Debug.Log("Not found Tile.prefab");
            return null;
        }
    }

    private void ReloadMap_Block()
    {
        // 初期化時は左上のセル位置をセット
        Vector2Int cellPos_leftTop = new Vector2Int(-MapSize.x / 2, (MapSize.y / 2) - 1);
        Vector2Int cellPos = cellPos_leftTop;
        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                if (BlockMap[y, x] != 0)
                {
                    CreateBlock(cellPos, BlockMap[y, x]);
                }
                else
                {
                    DeleteBlock(cellPos);
                }

                cellPos.x++;
            }
            cellPos.x = cellPos_leftTop.x;
            cellPos.y--;
        }
    }
    private void ReloadMap_Bar()
    {
        for (int i = 0; i < BarNum; i++)
        {
            if (BarMap[i] != 0)
            {
                BarList[i].GetComponent<BarHitAction>().BarState = BarHitAction.BARSTATE.DECIDED;
            }
            else
            {
                BarList[i].GetComponent<BarHitAction>().BarState = BarHitAction.BARSTATE.NEUTORAL;
            }
        }
    }

    private void InputForEdit()
    {
        if (Input.mousePosition.y > Screen.height * NoInputBottom)
        {
            // ブロックの設置
            if (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.Space)))
            {
                Vector2Int cellpos = GetCellPosFromMousePos();
                if (IsOutArea(cellpos, MapSize))
                {
                    // プレイヤーブロックの配置時、既に存在する場合は古い方を削除
                    if (BlockIndex <= Block.Count)
                    {
                        if (Block[BlockIndex - 1].transform.tag == "Player")
                        {
                            DeletePlayerBlock();
                        }
                    }
                    // 指定のブロックを配置
                    CreateBlock(cellpos, BlockIndex);
                }
            }
            // ブロックの削除
            if (Input.GetMouseButtonDown(1) || (Input.GetMouseButton(1) && Input.GetKey(KeyCode.Space)))
            {
                Vector2Int cellpos = GetCellPosFromMousePos();
                if (IsOutArea(cellpos, MapSize))
                {
                    Vector2Int elementNum = GetElementNumFormCellPos(cellpos, MapSize);

                    // マップからブロックを削除
                    BlockMap[elementNum.y, elementNum.x] = 0;
                    Destroy(BlockList[elementNum.y, elementNum.x]);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) BlockIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) BlockIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) BlockIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) BlockIndex = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) BlockIndex = 5;
    }
    private void UpdateBarMap()
    {
        for (int i = 0; i < BarNum; i++)
        {
            if (BarList[i].GetComponent<BarHitAction>().BarState == BarHitAction.BARSTATE.DECIDED)
            {
                BarMap[i] = 1;
            }
            else
            {
                BarMap[i] = 0;
            }
        }
    }


    // ステージサイズ変更時にボタンオブジェクトから呼び出されるメソッド
    public void StageScaleUp()
    {
        StageGrid.GetComponent<CreateGrid>().size += 2;
        MapSize.x += 2;
        MapSize.y += 2;
    }
    public void StageScaleDown()
    {
        StageGrid.GetComponent<CreateGrid>().size -= 2;
        MapSize.x -= 2;
        MapSize.y -= 2;
    }

    // ゲームシーン用のステージ生成
    public void CreateStage_Game(int stageNum)
    {
        for (int i = 0; i < TileList.Count; i++)
        {
            Destroy(TileList[i]);
        }
        TileList.Clear();
        for (int i = 0; i < BarList.Count; i++)
        {
            Destroy(BarList[i]);
        }
        BarList.Clear();


        LoadMap(stageNum);
        for (int i = 0; i < BarNum; i++)
        {
            if (BarMap[i] != 0)
            {
                // バーの生成
                CreateBar(new Vector3(-(BarNum / 2) + (i * 1.0f), 0.0f, 0.0f), BarScale);
                // タイルの生成
                if (BarList.Count > 1)
                {
                    Vector3 scale = new Vector3(
                        BarList[BarList.Count - 1].transform.position.x - BarList[BarList.Count - 2].transform.position.x, 
                        MapSize.y, 
                        1.0f
                    );
                    Vector3 pos = new Vector3(
                        BarList[BarList.Count - 2].transform.position.x + scale.x / 2.0f, 
                        BarList[BarList.Count - 2].transform.position.y, 
                        0.0f
                    );
                    GameObject tile = CreateTile(pos, scale);

                    // 作成したタイルにブロックとの親子関係を持たせる
                    for (int y = 0; y < BlockList.GetLength(0); y++)
                    {
                        for (int x = 0; x < BlockList.GetLength(1); x++)
                        {
                            if (BlockMap[y, x] != 0)
                            {
                                if ((pos.x - scale.x / 2.0f) < BlockList[y, x].transform.position.x &&
                                    (pos.x + scale.x / 2.0f) > BlockList[y, x].transform.position.x)
                                {
                                    BlockList[y, x].transform.parent = tile.transform;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapEdit))]
public class MapEditEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapEdit myTarget = (MapEdit)target;
        myTarget.mode = (Mode)EditorGUILayout.EnumPopup(myTarget.mode);
    }
}
#endif