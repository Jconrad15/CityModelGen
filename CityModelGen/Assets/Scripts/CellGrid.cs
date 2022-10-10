using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    public Cell[] Cells { get; private set; }

    private Vector3 purple = new Vector3(109, 19, 126);
    private Vector3 blue = new Vector3(100, 177, 183);
    private Color32 waterColor = new Color32(115, 115, 130, 255);

    private readonly int xRange = 10;
    private readonly int zRange = 10;

    private float maxHeight;
    private float waterHeight;

    private int xResolution;
    private int zResolution;

    private float scale;
    private float buildingRadius;
    private float waterRadius;
    private float heightRandomizationFactor;

    private int voronoiRegionCount;
    private int maxCategoryTypes = 5;

    private int[] voronoiCategories;
    private int[] voronoiSeedIndices;
    private float voronoiPerlinInfluence; // 0 is all Perlin

    private float buildingWidth;

    public CellGrid(
        int xResolution, int zResolution, int seed,
        float waterHeight = 0.2f, float maxHeight = 1f, float scale = 1f,
        float heightRandomizationFactor = 0f,
        float voronoiPerlinInfluence = 0f, int voronoiRegionCount = 50,
        float buildingWidth = 1)
    {
        this.xResolution = xResolution;
        this.zResolution = zResolution;
        this.maxHeight = maxHeight;
        this.waterHeight = waterHeight;
        this.scale = scale;
        this.heightRandomizationFactor = heightRandomizationFactor;
        this.voronoiPerlinInfluence = voronoiPerlinInfluence;
        this.voronoiRegionCount = voronoiRegionCount;
        this.buildingWidth = buildingWidth;

        buildingRadius = ((float)xRange / xResolution / 2f) * buildingWidth;
        waterRadius = (float)xRange / xResolution / 2f;

        Cells = new Cell[xResolution * zResolution];
        CreateVoronoi(xResolution, zResolution, seed, voronoiRegionCount);

        // Create cells
        for (int x = 0, i = 0; x < xResolution; x++)
        {
            for (int z = 0; z < zResolution; z++, i++)
            {
                Cells[i] = GenerateCell(x, z, i, seed);
            }
        }
    }

    private void CreateVoronoi(int xResolution, int zResolution, int seed, int voronoiRegionCount)
    {
        // Perform Voronoi process to determine height regions
        (voronoiCategories, voronoiSeedIndices) = Voronoi.JumpFlood(
            xResolution, zResolution, seed, voronoiRegionCount, maxCategoryTypes);
        // Check if Voronoi is null
        if (voronoiCategories == null)
        {
            voronoiCategories = new int[xResolution * zResolution];
            for (int i = 0; i < voronoiCategories.Length; i++)
            {
                voronoiCategories[i] = 0;
            }
            voronoiSeedIndices = new int[1] { 0 };
        }
    }

    private Cell GenerateCell(int x, int z, int i, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed + (i * 100));

        // Determine location 
        Vector3 center = new Vector3(
            (float)x / xResolution * xRange,
            0f,
            (float)z / zResolution * zRange);

        // Determine height
        float height = DetermineHeight(x, z, i, seed);

        // Determine Color
        Color color = HeightToColor(height, maxHeight);
        // Change edges to black
        if (x <= 0.01f ||
            z <= 0.01f ||
            x >= xResolution - 1 - 0.1f ||
            z >= zResolution - 1 - 0.1f)
        {
            color = Color.black;
        }

        // Determine which radius to use
        float radius;
        if (height <= waterHeight)
        {
            radius = waterRadius;
        }
        else
        {
            radius = buildingRadius;
        }

        Vector3[] lowerVertices = new Vector3[4];
        lowerVertices[0] = center + new Vector3(-radius, 0, -radius);
        lowerVertices[1] = center + new Vector3(-radius, 0, radius);
        lowerVertices[2] = center + new Vector3(radius, 0, radius);
        lowerVertices[3] = center + new Vector3(radius, 0, -radius);

        Random.state = oldState;
        return new Cell(color, height, lowerVertices);
    }

    private float DetermineHeight(int x, int z, int i, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed + (i * 100));

        float perlinHeight = Mathf.PerlinNoise(
            ((float)x / xRange * scale) + seed,
            ((float)z / zRange * scale) + seed)
            * maxHeight;

        float voronoiHeight =
            voronoiCategories[i] / (float)maxCategoryTypes * maxHeight;

        float height =
            (Mathf.Abs(voronoiPerlinInfluence - 1) * perlinHeight) +
            (voronoiPerlinInfluence * voronoiHeight);

        height += heightRandomizationFactor *
            Random.Range(-maxHeight, maxHeight);

        // Height to water floor
        if (height <= waterHeight)
        {
            height = waterHeight;
        }

        // Lower short buildings to water
        if (height <= waterHeight + (waterHeight * 0.2f))
        {
            height = waterHeight;
        }

        Random.state = oldState;
        return height;
    }

    private Color HeightToColor(float height, float maxHeight)
    {
        Vector3 lerped = Vector3.Lerp(
            purple, blue, height / maxHeight);

        Color32 newColor;
        if (height <= waterHeight)
        {
            newColor = waterColor;
        }
        else
        {
            newColor = new Color32(
                (byte)lerped.x, (byte)lerped.y,
                (byte)lerped.z, 255);
        }

        return newColor;
    }
}
