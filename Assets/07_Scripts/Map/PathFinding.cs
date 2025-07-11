using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    TileNode[,] allTiles;

    private void Start()
    {
        
    }

    public PathFinding(TileNode[,] tiles)
    {
        allTiles = tiles;
    }

    public List<Vector3> FindPath()
    {
        List<Vector3> path = new List<Vector3>();

        foreach(var tile in allTiles)
        {
            if(tile.type == TileType.Buildable)
            {
                continue;
            }
            path.Add(tile.worldPosition);
        }

        return path;
    }
}
