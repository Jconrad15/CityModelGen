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
        // Building Sides
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

        // Roads on the side of the building
        if (cell.isWater == false)
        {
            // Four sides of roads around the building
            AddQuad(
                cell.lowerVertices[0],
                cell.outerLowerVertices[0],
                cell.outerLowerVertices[1],
                cell.lowerVertices[1]);
            AddQuadColor(cell.groundColor);

            AddQuad(
                cell.lowerVertices[1],
                cell.outerLowerVertices[1],
                cell.outerLowerVertices[2],
                cell.lowerVertices[2]);
            AddQuadColor(cell.groundColor);

            AddQuad(
                cell.lowerVertices[2],
                cell.outerLowerVertices[2],
                cell.outerLowerVertices[3],
                cell.lowerVertices[3]);
            AddQuadColor(cell.groundColor);

            AddQuad(
                cell.lowerVertices[3],
                cell.outerLowerVertices[3],
                cell.outerLowerVertices[0],
                cell.lowerVertices[0]);
            AddQuadColor(cell.groundColor);

            // Four sides reaching down roads
            AddQuad(
                cell.outerLowerVertices[1],
                cell.outerLowerVertices[0],
                new Vector3(cell.outerLowerVertices[0].x, cell.waterHeight, cell.outerLowerVertices[0].z),
                new Vector3(cell.outerLowerVertices[1].x, cell.waterHeight, cell.outerLowerVertices[1].z));
            AddQuadColor(cell.groundColor);

            AddQuad(
                cell.outerLowerVertices[2],
                cell.outerLowerVertices[1],
                new Vector3(cell.outerLowerVertices[1].x, cell.waterHeight, cell.outerLowerVertices[1].z),
                new Vector3(cell.outerLowerVertices[2].x, cell.waterHeight, cell.outerLowerVertices[2].z));
            AddQuadColor(cell.groundColor);

            AddQuad(
                cell.outerLowerVertices[3],
                cell.outerLowerVertices[2],
                new Vector3(cell.outerLowerVertices[2].x, cell.waterHeight, cell.outerLowerVertices[2].z),
                new Vector3(cell.outerLowerVertices[3].x, cell.waterHeight, cell.outerLowerVertices[3].z));
            AddQuadColor(cell.groundColor);

            AddQuad(
                cell.outerLowerVertices[0],
                cell.outerLowerVertices[3],
                new Vector3(cell.outerLowerVertices[3].x, cell.waterHeight, cell.outerLowerVertices[3].z),
                new Vector3(cell.outerLowerVertices[0].x, cell.waterHeight, cell.outerLowerVertices[0].z));
            AddQuadColor(cell.groundColor);
        }

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