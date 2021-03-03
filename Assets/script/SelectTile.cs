using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class SelectTile : MonoBehaviour
{
    public GameObject rect;

    public GameObject selectRect_up;
    public GameObject selectRect_down;

    public TileBase normalTile;
    public TileBase redTile;

    private Tilemap map;


    // Start is called before the first frame update
    void Start()
    {
        map = this.GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        // 矩形で選択されたエリアを２次元配列に格納
        List<List<Vector2Int>> selectBlockList = new List<List<Vector2Int>>();

        // 選択した範囲内のマスの位置情報を２次元配列に格納
        var bound = map.cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; y--)
        {
            // 行を生成
            List<Vector2Int> row = new List<Vector2Int>();
            for (int x = bound.min.x; x < bound.max.x; x++)
            {
                // そのマスが範囲内かどうか
                if (IsInRect(new Vector2(x + 0.5f, y + 0.5f)))
                {
                    row.Add(new Vector2Int(x, y));
                }
            }
            if (row.Count == 0) continue;
            selectBlockList.Add(row);
        }
        if (selectBlockList.Count == 0) return;


        // 矩形の内側に入っていた全てのマスを参照
        Vector2Int specialBlockPos = new Vector2Int(999, 999);
        bool isFindRedBlock = false;
        for (int y = 0; y < selectBlockList.Count; y++)
        {
            for (int x = 0; x < selectBlockList[y].Count; x++)
            {
                // 特定のブロックが見つかった場合、位置を取得
                if (redTile == map.GetTile<Tile>(new Vector3Int(selectBlockList[y][x].x, selectBlockList[y][x].y, 0)))
                {
                    specialBlockPos = new Vector2Int(selectBlockList[y][x].x, selectBlockList[y][x].y);
                    isFindRedBlock = true;
                }
            }
        }
        if (!isFindRedBlock) return;

        // 選択範囲を指定するときの始点と終点を取得
        Vector2 firstPos = rect.GetComponent<AreaSelectControl>().FirstPos;
        Vector2 endPos = rect.GetComponent<AreaSelectControl>().EndPos;


        List<List<Vector2Int>> firstList = new List<List<Vector2Int>>();    // 赤ブロックより上側の選択範囲リスト
        List<List<Vector2Int>> endList = new List<List<Vector2Int>>();      // 赤ブロックより下側の選択範囲リスト


        // 上側の選択範囲リスト作成
        for (int y = 0; y < selectBlockList.Count; y++)
        {
            if (firstPos.y > specialBlockPos.y)
            {
                if (selectBlockList[y][0].y <= specialBlockPos.y) break;
            }
            else if (firstPos.y < specialBlockPos.y)
            {
                if (selectBlockList[y][0].y >= specialBlockPos.y) break;
            }

            List<Vector2Int> row = new List<Vector2Int>();
            for (int x = 0; x < selectBlockList[y].Count; x++)
            {
                if (firstPos.x < specialBlockPos.x)
                {
                    if (selectBlockList[0][x].x < specialBlockPos.x)
                    {
                        row.Add(new Vector2Int(selectBlockList[y][x].x, selectBlockList[y][x].y));
                    }
                }
                else if (firstPos.x > specialBlockPos.x)
                {
                    if (selectBlockList[0][x].x > specialBlockPos.x)
                    {
                        row.Add(new Vector2Int(selectBlockList[y][x].x, selectBlockList[y][x].y));
                    }
                }
            }
            if (row.Count == 0) continue;
            firstList.Add(row);
        }

        // 下側の選択範囲リスト作成
        for (int y = 0; y < selectBlockList.Count; y++)
        {
            if (endPos.y < specialBlockPos.y)
            {
                if (selectBlockList[y][0].y >= specialBlockPos.y) continue;
            }

            List<Vector2Int> row = new List<Vector2Int>();
            for (int x = 0; x < selectBlockList[y].Count; x++)
            {
                if (endPos.x > specialBlockPos.x)
                {
                    if (selectBlockList[0][x].x > specialBlockPos.x)
                    {
                        row.Add(new Vector2Int(selectBlockList[y][x].x, selectBlockList[y][x].y));
                    }
                }
            }
            if (row.Count == 0) continue;
            endList.Add(row);
        }

        ResizeSelectRect(selectRect_up, firstList);
        ResizeSelectRect(selectRect_down, endList);

        // 範囲反転処理
        if (Input.GetMouseButtonUp(0))
        {
            ReverseBlock(firstList);
            ReverseBlock(endList);
        }
        if (Input.GetMouseButtonDown(0))
        {
            selectRect_up.GetComponent<RectTransform>().sizeDelta = selectRect_down.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }
    }

    // 引数で渡された位置情報が矩形の内側か外側かを判断
    private bool IsInRect(Vector2 pos)
    {
        Vector2 rectPos = rect.GetComponent<RectTransform>().position;
        Vector2 rectSize = rect.GetComponent<RectTransform>().sizeDelta;

        if ((rectPos.x - rectSize.x * 0.5f) > pos.x)
        {// 矩形の左辺より外側だった時
            return false;
        }
        if ((rectPos.x + rectSize.x * 0.5f) < pos.x)
        {// 矩形の右辺より外側だった時
            return false;
        }
        if ((rectPos.y + rectSize.y * 0.5f) < pos.y)
        {// 矩形の上辺より外側だった時
            return false;
        }
        if ((rectPos.y - rectSize.y * 0.5f) > pos.y)
        {// 矩形の下辺より外側だった時
            return false;
        }

        // 矩形より内側だった時
        return true;
    }

    private void ReverseBlock(List<List<Vector2Int>> selectMap)
    {
        if (selectMap.Count == 0) return;
        if (selectMap.Count != selectMap[0].Count) return;

        List<List<bool>> newMap = new List<List<bool>>();
        // 列を生成
        for (int y = 0; y < selectMap.Count; y++)
        {
            newMap.Add(new List<bool>());
            // 行を生成
            for (int x = 0; x < selectMap[y].Count; x++)
            {
                newMap[y].Add(false);
            }
        }


        // 反転
        for (int y = 0; y < selectMap.Count; y++)
        {
            for (int x = 0; x < selectMap[y].Count; x++)
            {
                // 回転の軸と被るマスはそのまま
                if (map.HasTile(new Vector3Int(selectMap[x][y].x, selectMap[x][y].y, 0)))
                {
                    newMap[y][x] = true;
                }
            }
        }

        for (int y = 0; y < selectMap.Count; y++)
        {
            for (int x = 0; x < selectMap[y].Count; x++)
            {
                if (newMap[y][x])
                {
                    map.SetTile(new Vector3Int(selectMap[y][x].x, selectMap[y][x].y, 0), normalTile);
                }
                else
                {
                    map.SetTile(new Vector3Int(selectMap[y][x].x, selectMap[y][x].y, 0), null);
                }
            }
        }
    }

    private void ResizeSelectRect(GameObject rect, List<List<Vector2Int>> mapList)
    {
        if (mapList.Count == 0) return;
        if (mapList.Count != mapList[0].Count) return;

        rect.GetComponent<RectTransform>().sizeDelta = new Vector2(mapList[0].Count, mapList.Count);

        Vector2 selectRectPos;
        if (mapList[0].Count % 2 == 0)
        {
            selectRectPos = mapList[mapList[0].Count / 2 - 1][mapList[0].Count / 2];
        }
        else
        {
            selectRectPos.x = mapList[mapList[0].Count / 2][mapList[0].Count / 2].x + 0.5f;
            selectRectPos.y = mapList[mapList[0].Count / 2][mapList[0].Count / 2].y + 0.5f;
        }
        rect.GetComponent<RectTransform>().position = new Vector3(selectRectPos.x, selectRectPos.y, 0.0f);
    }
}