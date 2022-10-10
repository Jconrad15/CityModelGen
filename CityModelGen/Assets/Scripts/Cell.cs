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
    public int index;
    public Vector3 center;

    public Cell(
        Color color, Color groundColor, float height,
        Vector3[] lowerVertices, Vector3[] outerLowerVertices,
        float groundHeight, float waterHeight, bool isWater, int index, Vector3 center)
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

        this.index = index;
        this.center = center;
    }

    public void SetAsWater(Color color, float fullRadius)
    {
        this.color = color;
        isWater = true;
        height = waterHeight;
        groundHeight = waterHeight;

        // Lower vertices
        lowerVertices[0] = center + new Vector3(-fullRadius, waterHeight, -fullRadius);
        lowerVertices[1] = center + new Vector3(-fullRadius, waterHeight, fullRadius);
        lowerVertices[2] = center + new Vector3(fullRadius, waterHeight, fullRadius);
        lowerVertices[3] = center + new Vector3(fullRadius, waterHeight, -fullRadius);

        upperVertices = new Vector3[lowerVertices.Length];
        for (int i = 0; i < upperVertices.Length; i++)
        {
            upperVertices[i] = lowerVertices[i];
            upperVertices[i].y = waterHeight;
        }

        // Outer lower vertices
        outerLowerVertices[0] = center + new Vector3(-fullRadius, waterHeight, -fullRadius);
        outerLowerVertices[1] = center + new Vector3(-fullRadius, waterHeight, fullRadius);
        outerLowerVertices[2] = center + new Vector3(fullRadius, waterHeight, fullRadius);
        outerLowerVertices[3] = center + new Vector3(fullRadius, waterHeight, -fullRadius);

    }
}
