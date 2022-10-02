using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    public Cell[] Cells { get; private set; }

    private Vector3 purple = new Vector3(109, 19, 126);
    private Vector3 blue = new Vector3(100, 177, 183);

    private int xRange = 10;
    private int zRange = 10;

    private int xResolution;
    private int zResolution;

    private float scale = 1f;
    private float offsetX;
    private float offsetZ;

    public CellGrid(int xResolution, int zResolution, int seed)
    {
        this.xResolution = xResolution;
        this.zResolution = zResolution;

        offsetX = (float)xRange / xResolution / 2f;
        offsetZ = (float)zRange / zResolution / 2f;
        Debug.Log("offsetX = " + offsetX);
        Debug.Log("offsetZ = " + offsetZ);

        Cells = new Cell[xResolution * zResolution];

        for (int x = 0, i = 0; x < xResolution; x++)
        {
            for (int z = 0; z < zResolution; z++, i++)
            {
                Cells[i] = GenerateCell(x, z, seed);
            }
        }
    }

    public Cell GenerateCell(int x, int z, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed + (x * z) + x + z);

        float maxHeight = 4f;

        float height = Mathf.PerlinNoise(
            ((float)x / xRange * scale) + seed,
            ((float)z / zRange * scale) + seed)
            * maxHeight;

        Color color = HeightToColor(height, maxHeight);
        Vector3 center = new Vector3(
            (float)x / xResolution * xRange,
            0f,
            (float)z / zResolution * zRange);
        
        Vector3[] lowerVertices = new Vector3[4];
        lowerVertices[0] = center + new Vector3(-offsetX, 0, -offsetZ);
        lowerVertices[1] = center + new Vector3(-offsetX, 0, offsetZ);
        lowerVertices[2] = center + new Vector3(offsetX, 0, offsetZ);
        lowerVertices[3] = center + new Vector3(offsetX, 0, -offsetZ);

        Random.state = oldState;

        return new Cell(color, height, lowerVertices);
    }

    private Color HeightToColor(float height, float maxHeight)
    {
        Vector3 lerped = Vector3.Lerp(
            purple, blue, height / maxHeight);

        Color32 newColor = new Color32(
            (byte)lerped.x, (byte)lerped.y,
            (byte)lerped.z, 255);

        return newColor;
    }
}
