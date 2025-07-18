using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right,
    None
}

public class MinimapNode
{
    private TileNode[,] tileNodes;
    private HashSet<Direction> roadEdges;
    private Vector3 currentPos;

    public TileNode[,] TileNodes
    {
        get { return tileNodes; }
        set { tileNodes = value; }
    }

    public HashSet<Direction> RoadEdges
    {
        get { return roadEdges; }
        set { roadEdges = value; }
    }

    public Vector3 CurrentPos
    {
        get { return currentPos; }
        set { currentPos = value; }
    }

    public int GetSize()
    {
        int size = 0;
        foreach(var node in tileNodes)
        {
            if (node.type == TileType.Buildable)
            {
                size = (int)node.gridSize;
            }
        }

        size *= tileNodes.GetLength(0);
        return size;
    }

    public List<Vector3> GetRoadWorldPositions()
    {
        List<Vector3> roadWorldPos = new List<Vector3>();
        for (int i = 0; i < tileNodes.GetLength(0); i++)
        {
            for (int j = 0; j < tileNodes.GetLength(1); j++)
            {
                if (tileNodes[i, j].type == TileType.Road)
                {
                    roadWorldPos.Add(tileNodes[i, j].worldPosition);
                }
            }
        }
        return roadWorldPos;
    }

    public Vector3 GetWorldPositionCenter()
    {
        int width = tileNodes.GetLength(0) - 1;
        int height = tileNodes.GetLength(1) - 1;
            
        TileNode centerTileNode = tileNodes[width / 2, height / 2];
        return centerTileNode.worldPosition;
    }
}
