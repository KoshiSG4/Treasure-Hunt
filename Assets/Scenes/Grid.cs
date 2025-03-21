
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public TerrainGenerator terrainGenerator; // Reference to your terrain generator script
    public Transform start;
    public Transform goal;

    public bool displayGridGizmos; 
    private Node[,] grid;
    private List<Node> openSet;
    private HashSet<Node> closedSet;
    public float nodeRadius;
    float nodeDiameter;
    private bool[,] wallGrid;

    float minHeight = 0;
    float maxHeight = 0;

    void Awake()
    {
        InitializeGrid();
        nodeDiameter = nodeRadius * 2;
    }

    public int MaxSize
    {
        get
        {
            return terrainGenerator.width * terrainGenerator.length;
        }
    }
    private void InitializeGrid()
    {
        int gridSizeX = terrainGenerator.width;
        int gridSizeZ = terrainGenerator.length;

        grid = new Node[gridSizeX, gridSizeZ];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                float terrainHeight = terrainGenerator.GetTerrainHeight(x, z); 
                if (terrainHeight > maxHeight)
                {
                    maxHeight = terrainHeight;
                }
                if (terrainHeight < minHeight)
                {
                    minHeight = terrainHeight;
                }

                Vector3 worldPosition = new Vector3(x, terrainHeight, z);
                bool walkable = IsPositionWalkable(terrainHeight,worldPosition) ;

                grid[x, z] = new Node(walkable, worldPosition, x, z);
            }
        }
    }
    public bool IsPositionWalkable(float terrainHeight, Vector3 position)
    {
        return terrainHeight >= minHeight && terrainHeight <= maxHeight-2;
    }
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < grid.GetLength(0) && checkZ >= 0 && checkZ < grid.GetLength(1))
                {
                    neighbors.Add(grid[checkX, checkZ]);
                }
            }
        }

        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01(worldPosition.x / terrainGenerator.width);
        float percentZ = Mathf.Clamp01(worldPosition.z / terrainGenerator.length);

        int x = Mathf.RoundToInt((grid.GetLength(0) - 1) * percentX);
        int z = Mathf.RoundToInt((grid.GetLength(1) - 1) * percentZ);

        return grid[x, z];
    }

    //public List<Node> path;
    /*void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(terrainGenerator.width, 1, terrainGenerator.length));

        if (grid != null)
        {

            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                *//*if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.cyan;
                    }
                }*//*
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }*/
}


