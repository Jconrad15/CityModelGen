using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    private GameObject createdGO;

    [SerializeField]
    private Material material;

    [SerializeField]
    private int seed;

    [SerializeField]
    private int xResolution = 10;
    [SerializeField]
    private int zResolution = 10;

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

        CellGrid cellGrid =
            new CellGrid(xResolution, zResolution, seed);
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
