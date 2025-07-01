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
    // 전체 맵 상의 위치
    private Vector2Int gridPos;
    private TileNode[,] tileNodes;
    private HashSet<Direction> OpdenEdges;
}
