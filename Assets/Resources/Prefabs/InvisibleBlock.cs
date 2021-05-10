using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : MonoBehaviour
{
    private List<Renderer> MeshList = new List<Renderer>();
    private Color alpha = new Color(0.0f, 0.0f, 0.0f, 0.001f);

    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Renderer>() != null)
            {
                MeshList.Add(child.GetComponent<Renderer>());
            }
        }
    }

    void Update()
    {
        foreach (Renderer mesh in MeshList)
        {
            Color col = mesh.material.GetColor("_BaseColor");

            if (col.a > 0.0f)
            {
                Debug.Log(mesh.material.GetColor("_BaseColor").a);
                col -= alpha;
                if (col.a < 0.0f) col.a = 0.0f;
                mesh.material.SetColor("_BaseColor", col);
            }
        }
    }
}
