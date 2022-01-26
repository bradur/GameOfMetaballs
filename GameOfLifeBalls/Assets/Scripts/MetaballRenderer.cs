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
    private List<GameOfLifeCell> cells;

    private Material metaballMaterial;

    private int maxMetaBalls = 1000;

    public float MetaballRadius { get; private set; }

    private bool initialized = false;

    [SerializeField]
    private bool useTexturePacking = false;
    [SerializeField]
    private Material materialWithTexturePacking;
    [SerializeField]
    private Material materialWithVectorArray;
    private TextureEncoder textureEncoder;

    [SerializeField]
    private bool debug = false;

    [SerializeField]
    private MeshRenderer debugX;
    [SerializeField]
    private MeshRenderer debugY;

    private float textureMultiplier = 100f;

    public void Initialize()
    {

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        if (useTexturePacking) {
            meshRenderer.material = materialWithTexturePacking;
        } else {
            meshRenderer.material = materialWithVectorArray;
        }
        metaballMaterial = meshRenderer.sharedMaterial;
        if (!useTexturePacking) {
            metaballMaterial.SetVectorArray("_MetaballData", new Vector4[maxMetaBalls]);
        } else {
            metaballMaterial.SetFloat("_MultiplyValue", textureMultiplier);
        }
        metaballMaterial.SetInt("_NumberOfMetaballs", 0);
        CreateQuadToCameraSize();
        initialized = true;
    }

    public void SetCells(List<GameOfLifeCell> newCells)
    {
        cells = newCells;
    }

    public void RenderMetaballs()
    {
        if (!initialized)
        {
            Initialize();
        }
        if (cells == null)
        {
            return;
        }
        Calculate();
        if (metaballMaterial.color != GOLCellGrid.main.BallColor)
        {
            metaballMaterial.color = GOLCellGrid.main.BallColor;
        }
    }

    public void SetMetaballRadius(float radius)
    {
        MetaballRadius = radius;
        RenderMetaballs();
    }

    private List<Vector2> GetMetaballVector2Data()
    {
        List<Vector2> metaBallData = new List<Vector2>();
        foreach (GameOfLifeCell cell in cells)
        {
            if (!cell.IsAlive)
            {
                continue;
            }
            Vector3 invPosition = transform.InverseTransformPoint(GOLCellGrid.main.GetCellPosition(cell) + ((Vector3.one) * GOLCellGrid.main.Scale / 2));
            Vector2 pos = new Vector2(invPosition.x / textureMultiplier, invPosition.y / textureMultiplier);
            metaBallData.Add(pos);
        }

        return metaBallData;
    }

    private Vector4[] GetMetaballVector4Data()
    {
        List<Vector4> metaBallData = new List<Vector4>();
        foreach (GameOfLifeCell cell in cells)
        {
            if (!cell.IsAlive)
            {
                continue;
            }
            Vector3 invPosition = transform.InverseTransformPoint(GOLCellGrid.main.GetCellPosition(cell));
            Vector4 pos = new Vector4(invPosition.x, invPosition.y, MetaballRadius, 0);
            metaBallData.Add(pos);
        }

        return metaBallData.ToArray();
    }


    private void Calculate()
    {
        if (useTexturePacking)
        {
            List<Vector2> metaballData = GetMetaballVector2Data();
            if (textureEncoder == null) {
                textureEncoder = new TextureEncoder();
            }
            TextureOutput output = textureEncoder.Encode(metaballData);
            metaballMaterial.SetInt("_NumberOfMetaballs", metaballData.Count);
            if (metaballData.Count > 0)
            {
                if (debug) {
                    debugX.material.mainTexture = output.TextureX;
                    debugY.material.mainTexture = output.TextureY;
                }
                metaballMaterial.SetInt("_MetaballTextureWidth", output.Size);
                metaballMaterial.SetTexture("_XPosTexture", output.TextureX);
                metaballMaterial.SetTexture("_YPosTexture", output.TextureY);
            }
        }
        else
        {
            Vector4[] metaBallData = GetMetaballVector4Data();
            metaballMaterial.SetInt("_NumberOfMetaballs", metaBallData.Length);
            if (metaBallData.Length > 0)
            {
                metaballMaterial.SetVectorArray("_MetaballData", metaBallData);
            }
        }

        metaballMaterial.SetFloat("_MetaballRadius", MetaballRadius * GOLCellGrid.main.Scale);
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
            new Vector3(0f, width),
            new Vector3(width, width),
            new Vector3(width, 0f)
        };

        int[] tris = new int[]{
            0, 1, 2,
            0, 2, 3
        };

        Vector2[] uvs = new Vector2[]{
            new Vector2(0f, 0f),
            new Vector2(0f, width),
            new Vector2(width, width),
            new Vector2(width, 0f)
        };

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
        transform.position = new Vector3(-width / 2, -width / 2, transform.position.z);
    }
}
