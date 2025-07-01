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
    // ��ü �� ���� ��ġ
    private Vector2Int gridPos;
    private TileNode[,] tileNodes;
    private HashSet<Direction> OpdenEdges;
}
