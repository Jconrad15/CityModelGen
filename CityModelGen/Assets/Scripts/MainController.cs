using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    private Action saveStart;
    private Action saveEnd;

    private GameObject createdGO;
    private MeshRenderer mr;
    private MeshFilter mf;

    private UnityFBXExporter.RuntimeExporterMono exporter;

    [SerializeField]
    private Material material;

    private int seed = 0;
    private int xResolution = 50;
    private int zResolution = 50;
    private float waterHeight = 0.4f;
    private float maxHeight = 1.1f;
    private float noiseScale = 0.475f;
    private float heightRandomizationFactor = 0.1f;
    private float voronoiPerlinInfluence = 0.25f;
    private int voronoiRegionCount = 50;

    private void Start()
    {
        createdGO = new GameObject("grid");
        createdGO.transform.SetParent(transform);
        mr = createdGO.AddComponent<MeshRenderer>();
        mf = createdGO.AddComponent<MeshFilter>();

        // Initialize exporter
        exporter = createdGO
            .AddComponent<UnityFBXExporter.RuntimeExporterMono>();
        exporter.rootObjectToExport = createdGO;

        CreateNewGrid();
    }

/*    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateNewGrid();
        }
    }*/

    private void CreateNewGrid()
    {
        StopAllCoroutines();
        StartCoroutine(NewGridCoroutine());
    }

    private IEnumerator NewGridCoroutine()
    {
        CellGrid cellGrid = new CellGrid(
            xResolution, zResolution, seed,
            waterHeight, maxHeight, noiseScale,
            heightRandomizationFactor,
            voronoiPerlinInfluence, voronoiRegionCount);

        ProceduralMeshGrid meshGrid =
            new ProceduralMeshGrid(cellGrid);

        mf.mesh = meshGrid.Mesh;
        mr.material = material;
        yield return null;
    }

    public MeshFilter GetMeshFilter()
    {
        return mf;
    }

    public void SeedChanged(int seed)
    {
        this.seed = seed;
        CreateNewGrid();
    }

    public void ResolutionChanged(int resolution)
    {
        xResolution = resolution;
        zResolution = resolution;
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

    public void VoronoiPerlinInfluenceChanged(
        float voronoiPerlinInfluence)
    {
        this.voronoiPerlinInfluence = voronoiPerlinInfluence;
        CreateNewGrid();
    }

    public void VoronoiRegionCountChanged(
        int voronoiRegionCount)
    {
        this.voronoiRegionCount = voronoiRegionCount;
        CreateNewGrid();
    }

    public void SaveModel()
    {
        StartCoroutine(Save());
    }

    private IEnumerator Save()
    {
        saveStart?.Invoke();
        yield return new WaitForSeconds(0.1f);

        exporter.ExportGameObject();
        saveEnd?.Invoke();
    }

    public void RegisterSaveStart(Action callbackfunc)
    {
        saveStart += callbackfunc;
    }

    public void UnregisterSaveStart(Action callbackfunc)
    {
        saveStart -= callbackfunc;
    }

    public void RegisterSaveEnd(Action callbackfunc)
    {
        saveEnd += callbackfunc;
    }

    public void UnregisterSaveEnd(Action callbackfunc)
    {
        saveEnd -= callbackfunc;
    }
}
