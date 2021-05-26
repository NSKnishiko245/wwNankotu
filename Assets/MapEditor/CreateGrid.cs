using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CreateGrid : MonoBehaviour
{
    public bool AlphaIncrease { private get; set; }

    public enum Face
    {
        xy,
        zx,
        yz,
    };

    public float gridSize = 1f;
    public int size_x = 8;
    public int size_y = 4;
    public Color color = Color.white;
    public Face face = Face.xy;
    public bool back = true;

    public float alphaDownValue = 0.01f;

    //更新検出用
    float preGridSize = 0;
    int preSize = 0;
    Color preColor = Color.red;
    Face preFace = Face.zx;
    bool preBack = true;

    Mesh mesh;

    void Start()
    {
        color.a = 0.0f;
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh = ReGrid();
        AlphaIncrease = false;
    }

    public Mesh ReGrid(int x = 0, int y = 0)
    {
        if (back)
        {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
        else
        {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("GUI/Text Shader"));
        }

        mesh.Clear();

        int drawSize;
        float width;
        int resolution;
        float diff;
        Vector3[] vertices;
        Vector2[] uvs;
        int[] lines;
        Color[] colors;

        int size_w = size_x;
        int size_h = size_y;

        if (x != 0) size_w = x;
        if (y != 0) size_h = y;

        drawSize = size_w * 2;
        width = gridSize * drawSize / 4.0f;
        Vector2 startPosition = new Vector2(-width, -gridSize * size_h * 2 / 4.0f);
        Vector2 endPosition = new Vector2(width, gridSize * size_h * 2 / 4.0f);
        diff = width / drawSize;
        resolution = (drawSize + 2) * 2;
        //最期の２辺を追加している

        vertices = new Vector3[resolution];
        uvs = new Vector2[resolution];
        lines = new int[resolution];
        colors = new Color[resolution];

        for (int i = 0; i < vertices.Length; i += 4)
        {
            vertices[i] = new Vector3(startPosition.x + (diff * (float)i), startPosition.y, 0);
            vertices[i + 1] = new Vector3(startPosition.x + (diff * (float)i), endPosition.y, 0);
            vertices[i + 2] = new Vector3(startPosition.x, endPosition.y - (diff * (float)i), 0);
            vertices[i + 3] = new Vector3(endPosition.x, endPosition.y - (diff * (float)i), 0);
        }

        for (int i = 0; i < resolution; i++)
        {
            uvs[i] = Vector2.zero;
            lines[i] = i;
            colors[i] = new Color(color.r, color.g, color.b, color.a);
        }

        Vector3 rotDirection;
        switch (face)
        {
            case Face.xy:
                rotDirection = Vector3.forward;
                break;
            case Face.zx:
                rotDirection = Vector3.up;
                break;
            case Face.yz:
                rotDirection = Vector3.right;
                break;
            default:
                rotDirection = Vector3.forward;
                break;
        }

        mesh.vertices = RotationVertices(vertices, rotDirection);
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        preGridSize = gridSize;
        preSize = size_x;
        preColor = color;
        preFace = face;
        preBack = back;

        return mesh;
    }

    //頂点配列データーをすべて指定の方向へ回転移動させる
    Vector3[] RotationVertices(Vector3[] vertices, Vector3 rotDirection)
    {
        Vector3[] ret = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            ret[i] = Quaternion.LookRotation(rotDirection) * vertices[i];
        }
        return ret;
    }

    void Update()
    {
        //関係値の更新を検出したらメッシュも更新
        if (gridSize != preGridSize || size_x != preSize || preColor != color || preFace != face || preBack != back)
        {
            if (gridSize < 0) { gridSize = 0.000001f; }
            if (size_x < 0) { size_x = 1; }
            ReGrid();
        }
        Debug.Log(AlphaIncrease);
        if (AlphaIncrease)
        {
            color.a += alphaDownValue;
            if (color.a > 1) color.a = 1;
        }
        else
        {
            color.a -= alphaDownValue;
            if (color.a < 0) color.a = 0.0f;
        }
    }
    
    public void SetAlpha(float alphaValue)
    {
        color.a = alphaValue;
    }
}