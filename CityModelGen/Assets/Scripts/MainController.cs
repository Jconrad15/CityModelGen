using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    private GameObject createdGO;
    private MeshRenderer mr;
    private MeshFilter mf;

    [SerializeField]
    private Material material;

    private int seed = 0;
    private int xResolution = 100;
    private int zResolution = 100;
    private float waterHeight = 0.4f;
    private float maxHeight = 1.1f;
    private float noiseScale = 0.475f;
    private float heightRandomizationFactor = 0.1f;

    private void Start()
    {
        createdGO = new GameObject("grid");
        createdGO.transform.SetParent(transform);
        mr = createdGO.AddComponent<MeshRenderer>();
        mf = createdGO.AddComponent<MeshFilter>();

        CreateNewGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateNewGrid();
        }
    }

    private void CreateNewGrid()
    {
        CellGrid cellGrid = new CellGrid(
            xResolution, zResolution, seed,
            waterHeight, maxHeight, noiseScale,
            heightRandomizationFactor);

        ProceduralMeshGrid meshGrid =
            new ProceduralMeshGrid(cellGrid);

        mf.mesh = meshGrid.Mesh;
        mr.material = material;
    }

    public void SeedChanged(int seed)
    {
        this.seed = seed;
        CreateNewGrid();
    }

    public void XResolutionChanged(int xResolution)
    {
        this.xResolution = xResolution;
        CreateNewGrid();
    }

    public void ZResolutionChanged(int zResolution)
    {
        this.zResolution = zResolution;
        CreateNewGrid();
    }

    public void WaterHeightChanged(float waterHeight)
    {
        this.waterHeight = waterHeight;
        CreateNewGrid();
    }

    public void MaxHeightChanged(float maxHeight)
    {
        this.maxHeight = maxHeight;
        CreateNewGrid();
    }

    public void NoiseScaleChanged(float noiseScale)
    {
        this.noiseScale = noiseScale;
        CreateNewGrid();
    }

    public void HeightRandomizationFactorChanged(
        float heightRandomizationFactor)
    {
        this.heightRandomizationFactor = heightRandomizationFactor;
        CreateNewGrid();
    }
}
