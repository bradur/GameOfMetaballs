using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaballRenderer : MonoBehaviour
{
    public static MetaballRenderer main;

    private void Awake()
    {
        main = this;
    }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private List<GOLCell> cells;

    private Material metaballMaterial;

    private int maxMetaBalls = 1000;


    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        metaballMaterial = meshRenderer.sharedMaterial;
        metaballMaterial.SetVectorArray("_MetaballData", new Vector4[maxMetaBalls]);
        metaballMaterial.SetInt("_NumberOfMetaBalls", 0);
        CreateQuadToCameraSize();
    }

    public void SetCells(List<GOLCell> newCells)
    {
        cells = newCells;
    }

    public void RenderMetaballs()
    {
        if (cells == null) {
            return;
        }
        Calculate();
        if (metaballMaterial.color != GOLCellGrid.main.BallColor) {
            metaballMaterial.color = GOLCellGrid.main.BallColor;
        }
    }

    private Vector4[] GetMetaBallData()
    {
        Vector4[] metaBallData = new Vector4[maxMetaBalls];

        int index = 0;
        foreach (GOLCell cell in cells)
        {
            if (!cell.IsAlive)
            {
                continue;
            }
            Vector4 invPosition = transform.InverseTransformPoint(cell.SpriteRenderer.transform.position);
            Vector4 pos = new Vector4(invPosition.x, invPosition.y, GOLCellGrid.main.MetaballRadius, 0);
            metaBallData[index] = pos;
            index += 1;
        }

        return metaBallData;
    }

    private void Calculate()
    {
        Vector4[] metaBallData = GetMetaBallData();

        metaballMaterial.SetInt("_NumberOfMetaBalls", metaBallData.Length);
        metaballMaterial.SetVectorArray("_MetaballData", metaBallData);
    }

    private void CreateQuadToCameraSize()
    {
        float orthographicSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;

        float height = orthographicSize * 2f;
        float width = height * aspectRatio;
        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[]{
                new Vector3(0f, 0f),
                new Vector3(0f, height),
                new Vector3(height, height),
                new Vector3(height, 0f)
            };

        int[] tris = new int[]{
            0, 1, 2,
            0, 2, 3
        };

        Vector2[] uvs = new Vector2[]{
            new Vector2(0f, 0f),
            new Vector2(0f, height),
            new Vector2(height, height),
            new Vector2(height, 0f)
        };

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
        transform.position = new Vector3(transform.position.x, -orthographicSize, transform.position.z);
    }
}
