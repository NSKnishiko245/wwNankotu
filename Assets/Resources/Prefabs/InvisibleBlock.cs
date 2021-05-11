using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : MonoBehaviour
{
    public enum ALPHASTATE
    {
        NEUTRAL,
        INCREASE,
        DECREASE,
    }
    public ALPHASTATE AlphaState;

    private List<Renderer> MeshList = new List<Renderer>();
    private Color alpha = new Color(0.0f, 0.0f, 0.0f, 0.025f);

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
        if (AlphaState == ALPHASTATE.NEUTRAL) return;

        foreach (Renderer mesh in MeshList)
        {
            Color col = mesh.material.GetColor("_BaseColor");

            if (AlphaState == ALPHASTATE.DECREASE)
            {
                if (col.a > 0.0f) col -= alpha;
            }
            else if (AlphaState == ALPHASTATE.INCREASE)
            {
                if (col.a < 1.0f) col += alpha;
            }
            if (col.a < 0.0f) col.a = 0.0f;
            if (col.a > 1.0f) col.a = 1.0f;
            mesh.material.SetColor("_BaseColor", col);

            if (col.a == 0.0f)
            {
                Destroy(this.gameObject);
                return;
            }
        }
    }

    public void SetAlpha(float value)
    {
        MeshList.Clear();
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Renderer>() != null)
            {
                MeshList.Add(child.GetComponent<Renderer>());
            }
        }
        foreach (Renderer mesh in MeshList)
        {
            Color col = mesh.material.GetColor("_BaseColor");
            col.a = value;
            mesh.material.SetColor("_BaseColor", col);
        }
    }
}
