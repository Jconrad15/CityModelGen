using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Color color;
    public Color groundColor;
    public Vector3[] lowerVertices;
    public Vector3[] upperVertices;
    public Vector3[] outerLowerVertices;
    public float height;
    public float groundHeight;
    public float waterHeight;
    public bool isWater;

    public Cell(
        Color color, Color groundColor, float height,
        Vector3[] lowerVertices, Vector3[] outerLowerVertices,
        float groundHeight, float waterHeight, bool isWater)
    {
        this.color = color;
        this.groundColor = groundColor;
        this.height = height;
        this.waterHeight = waterHeight;
        this.lowerVertices = lowerVertices;
        this.outerLowerVertices = outerLowerVertices;
        this.groundHeight = groundHeight;
        this.isWater = isWater;

        upperVertices = new Vector3[lowerVertices.Length];
        for (int i = 0; i < upperVertices.Length; i++)
        {
            upperVertices[i] = lowerVertices[i];
            upperVertices[i].y = height;
        }
    }
}
