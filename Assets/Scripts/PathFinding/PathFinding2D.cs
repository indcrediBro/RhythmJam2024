using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid2D))]
public class PathFinding2D : MonoBehaviour
{
    //public Transform seeker, target;
    private Grid2D grid;
    Node2D seekerNode, targetNode;

    void Start()
    {
        grid = GetComponent<Grid2D>();
    }

    public List<Node2D> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        seekerNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node2D> path = new List<Node2D>();
        List<Node2D> openSet = new List<Node2D>();
        HashSet<Node2D> closedSet = new HashSet<Node2D>();
        openSet.Add(seekerNode);

        while (openSet.Count > 0)
        {
            Node2D node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                path = RetracePath(seekerNode, targetNode);
                return path;
            }

            foreach (Node2D neighbour in grid.GetNeighbors(node))
            {
                if (neighbour.obstacle || closedSet.Contains(neighbour))
                    continue;

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return path;
    }

    List<Node2D> RetracePath(Node2D startNode, Node2D endNode)
    {
        List<Node2D> path = new List<Node2D>();
        Node2D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node2D nodeA, Node2D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public Vector3 GetNextValidTile(Vector3 currentPos, Vector3 targetPos)
    {
        List<Node2D> path = FindPath(currentPos, targetPos);

        if (path.Count > 1)
        {
            // Get the second node in the path, which is the next valid tile
            return path[1].worldPosition;
        }

        // If no valid path or already at the target, return current node
        return currentPos;
    }

    public bool IsPathStraight(Vector3 startPos, Vector3 targetPos)
    {
        List<Node2D> path = FindPath(startPos, targetPos);

        if (path.Count > 1)
        {
            Vector3 direction = path[1].worldPosition - grid.NodeFromWorldPoint(startPos).worldPosition;
            return direction.x == 0 || direction.y == 0;
        }

        // If no valid path or already at the target, consider it straight
        return true;
    }
}
