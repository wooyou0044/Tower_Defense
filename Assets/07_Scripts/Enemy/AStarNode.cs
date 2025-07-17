using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public Vector3 worldPosition { get; set; }
    public AStarNode parent { get; set; }
    public float gCost = float.MaxValue;
    public float hCost { get; set; }

    public float fCost
    {
        get { return gCost + hCost; }
    }

    public AStarNode(Vector3 pos)
    {
        worldPosition = pos;
    }
}
