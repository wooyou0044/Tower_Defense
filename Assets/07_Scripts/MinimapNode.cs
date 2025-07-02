using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right
}

public class MinimapNode
{
    private TileNode[,] tileNodes;
    private HashSet<Direction> roadEdges;

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
}
