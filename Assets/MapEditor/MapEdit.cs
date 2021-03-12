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

    private int[,] Map;
    private GameObject[,] ObjMap;

    private Vector2Int MapSize = new Vector2Int(20, 20);

    public Mode mode = Mode.Game;

    private GameObject MainCamera;
    private float NoInputBottom;

    void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        NoInputBottom = 0.15f;

        Map = new int[MapSize.x , MapSize.y];
        ObjMap = new GameObject[MapSize.x, MapSize.y];

        Block = new List<GameObject>();
        for (int i = 1; true; i++)
        {
            if (!File.Exists(Application.dataPath + "/Resources/Prefabs/" + i.ToString() + ".prefab"))
            {
                return;
            }
            Block.Add(Resources.Load("Prefabs/" + i.ToString()) as GameObject);
        }
    }

    void Update()
    {
        if (mode == Mode.Edit)
        {
            InputForEdit();
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

    public void SaveMap(string fileName)
    {
        gameObject.GetComponent<CsvWrite>().WriteMapFromCsv(Map, fileName);
    }
    public void LoadMap(string fileName)
    {
        // ロード失敗時は Map を 0 埋め
        if (!gameObject.GetComponent<CsvWrite>().ReadMapFromCsv(Map, fileName))
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                for (int x = 0; x < MapSize.x; x++)
                {
                    Map[y, x] = 0;
                }
            }
        }
        ReloadMap();
    }

    private void CreateBlock(Vector2Int cellpos, int blockIdx)
    {
        // 指定したブロックのプレハブが存在しない場合は中断
        if (blockIdx > Block.Count) return;

        // マウスの位置から、その位置に対応したマップの要素数に変換
        Vector2Int elementNum = GetElementNumFormCellPos(cellpos, MapSize);

        // マップにブロックを登録
        Map[elementNum.y, elementNum.x] = blockIdx;

        // 一度指定された場所のオブジェクトを削除、のち生成
        Destroy(ObjMap[elementNum.y, elementNum.x]);
        GameObject obj = Instantiate(Block[blockIdx - 1], new Vector3(cellpos.x + 0.5f, cellpos.y + 0.5f, 0.0f), new Quaternion(0, 0, 0, 1));
        ObjMap[elementNum.y, elementNum.x] = obj;
    }
    private void DeleteBlock(Vector2Int cellpos)
    {
        Vector2Int elementNum = GetElementNumFormCellPos(cellpos, MapSize);

        // マップからブロックを削除
        Map[elementNum.y, elementNum.x] = 0;
        Destroy(ObjMap[elementNum.y, elementNum.x]);
    }

    private void ReloadMap()
    {
        // 初期化時は左上のセル位置をセット
        Vector2Int cellPos_leftTop = new Vector2Int(-MapSize.x / 2, (MapSize.y / 2) - 1);
        Vector2Int cellPos = cellPos_leftTop;
        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                if (Map[y, x] != 0)
                {
                    CreateBlock(cellPos, Map[y, x]);
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

    private void InputForEdit()
    {
        if (Input.mousePosition.y > Screen.height * NoInputBottom)
        {
            // ブロックの設置
            if (Input.GetMouseButtonDown(0))
            {
                Vector2Int cellpos = GetCellPosFromMousePos();
                if (IsOutArea(cellpos, MapSize))
                {
                    CreateBlock(cellpos, BlockIndex);
                }
            }
            // ブロックの削除
            if (Input.GetMouseButtonDown(1))
            {
                Vector2Int cellpos = GetCellPosFromMousePos();
                if (IsOutArea(cellpos, MapSize))
                {
                    Vector2Int elementNum = GetElementNumFormCellPos(cellpos, MapSize);

                    // マップからブロックを削除
                    Map[elementNum.y, elementNum.x] = 0;
                    Destroy(ObjMap[elementNum.y, elementNum.x]);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) BlockIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) BlockIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) BlockIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) BlockIndex = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) BlockIndex = 5;
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