using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshGrid
{
    public Mesh Mesh { get; private set; }
    private List<Vector3> vertices;
    private List<Color> colors;
    private List<int> triangles;

    public ProceduralMeshGrid(CellGrid grid)
    {
        Mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();

        Triangulate(grid);
    }

    private void Triangulate(CellGrid grid)
    {
        Mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        // Triangulate for each cell of the grid
        for (int i = 0; i < grid.Cells.Length; i++)
        {
            Triangulate(grid.Cells[i]);
        }

        // Set mesh data
        Mesh.vertices = vertices.ToArray();
        Mesh.colors = colors.ToArray();
        Mesh.triangles = triangles.ToArray();
        Mesh.RecalculateNormals();
    }

    private void Triangulate(Cell cell)
    {
        AddQuad(
            cell.lowerVertices[0], cell.lowerVertices[1],
            cell.upperVertices[1], cell.upperVertices[0]);
        AddQuadColor(cell.color);

        AddQuad(
            cell.lowerVertices[1], cell.lowerVertices[2],
            cell.upperVertices[2], cell.upperVertices[1]);
        AddQuadColor(cell.color);

        AddQuad(
            cell.lowerVertices[2], cell.lowerVertices[3],
            cell.upperVertices[3], cell.upperVertices[2]);
        AddQuadColor(cell.color);

        AddQuad(
            cell.lowerVertices[3], cell.lowerVertices[0],
            cell.upperVertices[0], cell.upperVertices[3]);
        AddQuadColor(cell.color);

        // Roof
        AddQuad(
            cell.upperVertices[0], cell.upperVertices[1],
            cell.upperVertices[2], cell.upperVertices[3]);
        AddQuadColor(cell.color);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color cellColor)
    {
        colors.Add(cellColor);
        colors.Add(cellColor);
        colors.Add(cellColor);
        colors.Add(cellColor);
    }

}