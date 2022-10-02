using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    private GameObject createdGO;

    [SerializeField]
    private Material material;

    [SerializeField]
    [Range(0, 10000)]
    private int seed = 0;

    [SerializeField]
    [Range(50, 200)]
    private int xResolution = 100;
    [SerializeField]
    [Range(50, 200)]
    private int zResolution = 100;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float waterHeight = 0.4f;
    [SerializeField]
    [Range(1f, 2f)]
    private float maxHeight = 1.1f;
    [SerializeField]
    [Range(0.1f, 2f)]
    private float noiseScale = 0.475f;
    [SerializeField]
    [Range(0f, 1f)]
    private float heightRandomizationFactor = 0.1f;

    private void Start()
    {
        CreateNewGrid(seed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateNewGrid(seed);
        }
    }

    private void CreateNewGrid(int seed)
    {
        if (createdGO != null)
        {
            Destroy(createdGO);
        }

        CellGrid cellGrid = new CellGrid(
            xResolution, zResolution, seed,
            waterHeight, maxHeight, noiseScale,
            heightRandomizationFactor);
        ProceduralMeshGrid meshGrid =
            new ProceduralMeshGrid(cellGrid);

        createdGO = new GameObject("grid");
        createdGO.transform.SetParent(transform);
        MeshRenderer mr = createdGO.AddComponent<MeshRenderer>();
        MeshFilter mf = createdGO.AddComponent<MeshFilter>();

        mf.mesh = meshGrid.Mesh;
        mr.material = material;
    }
}
