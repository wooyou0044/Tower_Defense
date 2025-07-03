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
    private int mapSize;

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
}
