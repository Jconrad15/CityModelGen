using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { N, E, S, W };

public class CellGrid
{
    public Cell[] Cells { get; private set; }

    private Vector3 purple = new Vector3(109, 19, 126);
    private Vector3 blue = new Vector3(100, 177, 183);
    private Color32 waterColor = new Color32(115, 115, 130, 255);

    public Color32 roadColor = new Color32(50, 50, 55, 255);
    public bool useRoadColor = false;

    private readonly int xRange = 10;
    private readonly int zRange = 10;

    private float maxHeight;
    private float waterHeight;

    private int xResolution;
    private int zResolution;

    private float scale;
    private float buildingRadius;
    private float fullRadius;
    private float heightRandomizationFactor;

    private int voronoiRegionCount;
    private int maxCategoryTypes = 5;

    private int[] voronoiCategories;
    private int[] voronoiSeedIndices;
    private float voronoiPerlinInfluence; // 0 is all Perlin

    private float buildingWidth;
    private float emptyLotPercent;
    private float loneIslandThreshold;

    public CellGrid(
        int xResolution, int zResolution, int seed,
        float waterHeight = 0.2f, float maxHeight = 1f, float scale = 1f,
        float heightRandomizationFactor = 0f,
        float voronoiPerlinInfluence = 0f, int voronoiRegionCount = 50,
        float buildingWidth = 1, float emptyLotPercent = 0,
        float loneIslandThreshold = 4, 
        bool useRoadColor = false)
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
        this.emptyLotPercent = emptyLotPercent;
        this.loneIslandThreshold = loneIslandThreshold;
        this.useRoadColor = useRoadColor;

        buildingRadius = ((float)xRange / xResolution / 2f) * buildingWidth;
        fullRadius = (float)xRange / xResolution / 2f;

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

        // Second pass on created cells
        ChangeWaterSurroundedCellsToWater();
        ChangeWaterSurroundedCellsToWater();

    }

    private void ChangeWaterSurroundedCellsToWater()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            // Skip cell is already water
            if (Cells[i].isWater) { continue; }

            // Get cell's neighbors
            int surroundingWaterCells =
                DetermineSurroundingWaterCells(Cells[i]);

            if (surroundingWaterCells > loneIslandThreshold)
            {
                Color currentColor;
                if (CheckIfEdge(i))
                {
                    currentColor = Color.black;
                }
                else
                {
                    currentColor = waterColor;
                }

                Cells[i].SetAsWater(currentColor, fullRadius);
            }
        }
    }

    private int DetermineSurroundingWaterCells(Cell cell)
    {
        Cell[] neighbors = GetNeighboringCells(cell);
        int waterNeighbors = 0;
        for (int j = 0; j < neighbors.Length; j++)
        {
            if (neighbors[j] != null)
            {
                if (neighbors[j].isWater)
                {
                    waterNeighbors++;
                }
            }
        }

        return waterNeighbors;
    }

    private Cell[] GetNeighboringCells(Cell c)
    {
        List<Cell> neighbors = new List<Cell>();
        foreach (Direction d in
            (Direction[])System.Enum.GetValues(typeof(Direction)))
        {
            neighbors.Add(GetNeighboringCell(d, c));
        }

        return neighbors.ToArray();
    }

    private Cell GetNeighboringCell(Direction d, Cell c)
    {
        int neighborIndex = c.index;

        if (d == Direction.N)
        {
            neighborIndex += xResolution;
        }
        else if (d == Direction.E)
        {
            neighborIndex += 1;
        }
        else if (d == Direction.S)
        {
            neighborIndex -= xResolution;
        }
        else // d == Direction.W
        {
            neighborIndex -= 1;
        }

        return GetCell(neighborIndex);
    }

    private Cell GetCell(int index)
    {
        if (index < 0 || index >= Cells.Length) { return null; }
        return Cells[index];
    }

    private void CreateVoronoi(
        int xResolution, int zResolution, int seed,
        int voronoiRegionCount)
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
        (float height, float groundHeight) =
            DetermineHeight(x, z, i, seed);

        // Determine Color
        Color color = HeightToColor(height, maxHeight);
        Color groundColor = HeightToColor(groundHeight, maxHeight);

        if (useRoadColor) { groundColor = roadColor; }

        // Change edges to black
        if (CheckIfEdge(x, z))
        {
            color = Color.black;
            groundColor = Color.black;
        }

        bool isWater;
        // Determine which radius to use
        float radius;
        if (height <= waterHeight)
        {
            radius = fullRadius;
            isWater = true;
        }
        else
        {
            radius = buildingRadius;
            isWater = false;
        }

        // Lower vertices
        Vector3[] lowerVertices = new Vector3[4];
        lowerVertices[0] = center + new Vector3(-radius, groundHeight, -radius);
        lowerVertices[1] = center + new Vector3(-radius, groundHeight, radius);
        lowerVertices[2] = center + new Vector3(radius, groundHeight, radius);
        lowerVertices[3] = center + new Vector3(radius, groundHeight, -radius);

        // Outer lower vertices
        Vector3[] outerLowerVertices = new Vector3[4];
        outerLowerVertices[0] = center + new Vector3(-fullRadius, groundHeight, -fullRadius);
        outerLowerVertices[1] = center + new Vector3(-fullRadius, groundHeight, fullRadius);
        outerLowerVertices[2] = center + new Vector3(fullRadius, groundHeight, fullRadius);
        outerLowerVertices[3] = center + new Vector3(fullRadius, groundHeight, -fullRadius);

        Random.state = oldState;
        return new Cell(
            color, groundColor, height,
            lowerVertices, outerLowerVertices,
            groundHeight, waterHeight, isWater, i, center);
    }

    private (float height, float groundHeight)
        DetermineHeight(int x, int z, int i, int seed)
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
            Random.Range(-maxHeight / 2f, maxHeight / 2f);

        float groundHeight = waterHeight + (height * 0.1f);

        // Change some cells to be empty lots
        if (Random.value < emptyLotPercent)
        {
            height = groundHeight;
        }

        // Height to water floor
        if (height <= waterHeight)
        {
            height = waterHeight;
            groundHeight = waterHeight;
        }

        // Lower short buildings to water
        if (height <= waterHeight + (waterHeight * 0.1f))
        {
            height = waterHeight;
            groundHeight = waterHeight;
        }

        Random.state = oldState;
        return (height, groundHeight);
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

    private bool CheckIfEdge(int x, int z)
    {
        return
            x <= 0.01f ||
            z <= 0.01f ||
            x >= xResolution - 1 - 0.1f ||
            z >= zResolution - 1 - 0.1f;
    }

    private bool CheckIfEdge(int index)
    {
        (int x, int z) = GetCoordFromIndex(index);
        return CheckIfEdge(x, z);
    }

    private (int, int) GetCoordFromIndex(int i)
    {
        int x = i % xResolution;
        int z = i / xResolution % zResolution;
        return (x, z);
    }
}
