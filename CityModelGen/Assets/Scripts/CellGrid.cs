using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    public Cell[] Cells { get; private set; }

    private Vector3 purple = new Vector3(109, 19, 126);
    private Vector3 blue = new Vector3(100, 177, 183);
    private Color32 waterColor = new Color32(115, 115, 130, 255);

    private int xRange = 10;
    private int zRange = 10;

    private float maxHeight;
    private float waterHeight;

    private int xResolution;
    private int zResolution;

    private float scale;
    private float offsetX;
    private float offsetZ;
    private float heightRandomizationFactor;

    public CellGrid(
        int xResolution, int zResolution, int seed,
        float waterHeight = 0.2f, float maxHeight = 1f, float scale = 1f,
        float heightRandomizationFactor = 0f)
    {
        this.xResolution = xResolution;
        this.zResolution = zResolution;
        this.maxHeight = maxHeight;
        this.waterHeight = waterHeight;
        this.scale = scale;
        this.heightRandomizationFactor = heightRandomizationFactor;

        offsetX = (float)xRange / xResolution / 2f;
        offsetZ = (float)zRange / zResolution / 2f;

        Cells = new Cell[xResolution * zResolution];

        for (int x = 0, i = 0; x < xResolution; x++)
        {
            for (int z = 0; z < zResolution; z++, i++)
            {
                Cells[i] = GenerateCell(x, z, i, seed);
            }
        }
    }

    public Cell GenerateCell(int x, int z, int i, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed + (i * 100));

        float height = DetermineHeight(x, z, i, seed);

        Color color = HeightToColor(height, maxHeight);
        Vector3 center = new Vector3(
            (float)x / xResolution * xRange,
            0f,
            (float)z / zResolution * zRange);
        // Change edges to black
        if (x <= 0.01f ||
            z <= 0.01f ||
            x >= xResolution - 1 - 0.1f ||
            z >= zResolution - 1 - 0.1f)
        {
            color = Color.black;
        }


        Vector3[] lowerVertices = new Vector3[4];
        lowerVertices[0] = center + new Vector3(-offsetX, 0, -offsetZ);
        lowerVertices[1] = center + new Vector3(-offsetX, 0, offsetZ);
        lowerVertices[2] = center + new Vector3(offsetX, 0, offsetZ);
        lowerVertices[3] = center + new Vector3(offsetX, 0, -offsetZ);

        Random.state = oldState;

        return new Cell(color, height, lowerVertices);
    }

    private float DetermineHeight(int x, int z, int i, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed + (i * 100));

        float height = Mathf.PerlinNoise(
            ((float)x / xRange * scale) + seed,
            ((float)z / zRange * scale) + seed)
            * maxHeight;

        height += heightRandomizationFactor *
            Random.Range(-maxHeight, maxHeight);

        // Height to water floor
        if (height <= waterHeight)
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
