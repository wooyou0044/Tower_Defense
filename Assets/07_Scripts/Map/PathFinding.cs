using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    TileNode[,] allTiles;

    List<Vector3> path;

    Queue<Vector2> qMinimap;
    HashSet<Vector2> visited;
    HashSet<MinimapNode> connected;

    private void Start()
    {
        path = new List<Vector3>();
    }

    public PathFinding(TileNode[,] tiles)
    {
        allTiles = tiles;
    }

    public List<Vector3> FindPath()
    {
        path = new List<Vector3>();

        foreach (var tile in allTiles)
        {
            if (tile.type == TileType.Buildable)
            {
                continue;
            }
            path.Add(tile.worldPosition);
        }

        return path;
    }


    public void FindShortPath()
    {

    }
}
