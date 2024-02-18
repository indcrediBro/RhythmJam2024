using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PathFinding2D))]
public class Grid2D : MonoBehaviour
{
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public Node2D[,] Grid;
    public Tilemap obstaclemap;
    public List<Node2D> path;
    public Vector3 worldBottomLeft;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        worldBottomLeft = Vector3.zero; // Set to (0,0) as specified
        CreateGrid();
    }




    void CreateGrid()
    {
        Grid = new Node2D[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                Grid[x, y] = new Node2D(false, worldPoint, x, y);

                if (obstaclemap.HasTile(obstaclemap.WorldToCell(Grid[x, y].worldPosition)))
                    Grid[x, y].SetObstacle(true);
                else
                    Grid[x, y].SetObstacle(false);
            }
        }
    }


    //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
    public List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        //checks and adds top neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY + 1]);

        //checks and adds bottom neighbor
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX, node.GridY - 1]);

        //checks and adds right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY]);

        //checks and adds left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY]);



        /* Uncomment this code to enable diagonal movement
        
        //checks and adds top right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1< gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY + 1]);

        //checks and adds bottom right neighbor
        if (node.GridX + 1>= 0 && node.GridX + 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY - 1]);

        //checks and adds top left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY + 1>= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY + 1]);

        //checks and adds bottom left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY  - 1>= 0 && node.GridY  - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY - 1]);
        */



        return neighbors;
    }
    public Node2D NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x) / nodeDiameter);
        int y = Mathf.RoundToInt((worldPosition.y) / nodeDiameter);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);

        return Grid[x, y];
    }

    //Draws visual representation of grid
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (Grid != null)
        {
            foreach (Node2D n in Grid)
            {
                if (n.obstacle)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.white;

                if (path != null && path.Contains(n))
                    Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));

            }

            if(path!= null)
            {
                foreach (Node2D n in path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                }
            }
        }
    }
}
