using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Voronoi
{
    private class VoronoiCell
    {
        public Vector2 position;
        public int index;
        public VoronoiCell seedCell;
        public bool IsSeedCell;
        public int currentCategory;
        public VoronoiCell(int x, int y, int index)
        {
            position = new Vector2(x, y);
            this.index = index;
            seedCell = null;
            IsSeedCell = false;
            currentCategory = -1;
        }
    }

    public static (int[] mapCategories, int[] seedIndicies) JumpFlood(
        int width, int height, int seed,
        int regionCount, int maxCategoryTypes)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        VoronoiCell[] cells = CreateStartingCells(width, height);
        int cellCount = width * height;

        int[] seedIndicies = DetermineSeedCells(
            seed, regionCount, cells, cellCount, maxCategoryTypes);

        float[] steps = new float[7]
        {
            cellCount,
            cellCount/2f,
            cellCount/4f,
            cellCount/8f,
            1,
            2,
            1
        };

        Vector2[] neighborRef = new Vector2[8]
        {
            new Vector2(-1, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),

            new Vector2(0, -1),
            //new Vector2(0, 0), itself
            new Vector2(0, 1),

            new Vector2(1, -1),
            new Vector2(1, 0),
            new Vector2(1, 1)
        };

        int defaultCategory = -1;

        // Iterate over each step size
        for (int k = 0; k < steps.Length; k++)
        {
            // For all x and z coordinates
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    // For neighboring cells based on step size
                    foreach (Vector2 offset in neighborRef)
                    {
                        // Neighbor positions
                        int neighborX = x + ((int)offset.x * k);
                        int neighborZ = z + ((int)offset.y * k);

                        // Only get in bounds neighbors
                        if (neighborX < 0 || neighborX > width - 1) { continue; }
                        if (neighborZ < 0 || neighborZ > height - 1) { continue; }

                        // Get cells
                        // Skip if this is a seed cell
                        VoronoiCell cell = GetCell(x, z, width, height, cells);
                        if (cell == null)
                        {
                            Debug.LogError("Found a null cell. Exiting.");
                            return (null, null);
                        }
                        
                        if (cell.IsSeedCell) { continue; }
                        VoronoiCell neighborCell = GetCell(neighborX, neighborZ, width, height, cells);

                        // Compare neighbor category to cell category
                        if (cell.currentCategory == defaultCategory)
                        {
                            if (neighborCell.currentCategory != defaultCategory)
                            {
                                // Change cell's category to neighbor's category
                                cell.currentCategory = neighborCell.currentCategory;
                                cell.seedCell = neighborCell.seedCell;
                            }
                            // Otherwise do nothing if both are default category
                        }
                        else
                        {
                            // The cell does not have a default category
                            if (neighborCell.currentCategory != defaultCategory)
                            {
                                if (cell.seedCell == null)
                                {
                                    Debug.LogError("SeedCell is null, what does this mean?");
                                    continue;
                                }
                                // Both cells are colored
                                float distCurrentSeed =
                                    Vector3.Distance(cell.position, cell.seedCell.position);
                                float distNeighborSeed =
                                    Vector3.Distance(cell.position, neighborCell.seedCell.position);

                                // If the neighbor's seed is closer than the current seed
                                if (distCurrentSeed > distNeighborSeed)
                                {
                                    // Change the seed cell and color
                                    cell.currentCategory = (neighborCell.currentCategory);
                                    cell.seedCell = neighborCell.seedCell;
                                }

                            }
                            // Do nothing if neighbor is default color
                        }
                    } // End neighbors
                } // End z
            } // End x
        } // End step iteration

        // Convert cell array to int array
        int[] mapCategories = new int[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            mapCategories[i] = cells[i].currentCategory;
        }

        Random.state = oldState;
        return (mapCategories, seedIndicies);
    }

    private static int[] DetermineSeedCells(
        int seed, int seedCount, VoronoiCell[] cells,
        int cellCount, int maxCategoryTypes)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        // Determine seed cells
        int[] seedIndices = SelectSeedCells(seed, seedCount, cellCount);
        foreach (int seedIndex in seedIndices)
        {
            cells[seedIndex].currentCategory =
                Random.Range(0, maxCategoryTypes);
            cells[seedIndex].IsSeedCell = true;
            cells[seedIndex].seedCell = cells[seedIndex];
        }

        Random.state = oldState;
        return seedIndices;
    }

    private static VoronoiCell[] CreateStartingCells(
        int width, int height)
    {
        VoronoiCell[] cells = new VoronoiCell[width * height];
        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[index] = new VoronoiCell(x, y, index);

                index += 1;
            }
        }

        return cells;
    }

    private static VoronoiCell GetCell(
        int x, int y, int width, int height, VoronoiCell[] cells)
    {
        if (y >= height || y < 0 ||
            x >= width || x < 0)
        {
            return null;
        }

        int i = (x * width) + y;

        if (i >= cells.Length || i < 0)
        {
            Debug.LogError("GetCell index out of bounds. i=" + i);
            return null;
        }
        return cells[i];
    }

    /// <summary>
    /// Returns an integer array for selected cell indicies.
    /// </summary>
    /// <returns></returns>
    private static int[] SelectSeedCells(
        int seed, int seedCount, int cellCount)
    {
        // Limit seeds by number of cells
        if (seedCount > cellCount)
        {
            Debug.LogError("seedCount is set to greater " +
                "than the cellCount");
            return null;
        }

        // Set state based on seed
        Random.State oldState = Random.state;
        Random.InitState(seed);

        int[] indices = new int[seedCount];
        for (int i = 0; i < seedCount; i++)
        {
            int randomValue = Random.Range(0, cellCount);
            while (indices.Contains(randomValue))
            {
                randomValue = Random.Range(0, cellCount);
            }

            indices[i] = randomValue;
        }

        // Reset state
        Random.state = oldState;
        return indices;
    }
}