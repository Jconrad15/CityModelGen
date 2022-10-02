using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Color color;
    public float height;
    public Vector3[] lowerVertices;
    public Vector3[] upperVertices;

    public Cell(
        Color color, float height,
        Vector3[] lowerVertices)
    {
        this.color = color;
        this.height = height;
        this.lowerVertices = lowerVertices;
        
        upperVertices = new Vector3[lowerVertices.Length];
        for (int i = 0; i < upperVertices.Length; i++)
        {
            upperVertices[i] = lowerVertices[i];
            upperVertices[i].y = height;
        }
    }
}
