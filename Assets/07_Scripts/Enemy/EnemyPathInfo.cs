using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathInfo
{
    public Vector3 spawnPos { get; set; }
    public Vector3 goalPos { get; set; }
    public List<Vector3> shortPaths { get; set; }
    public List<List<Vector3>> candidatePaths { get; set; }

    public EnemyPathInfo(Vector3 pos, Vector3 goal, List<Vector3> shortPath, List<List<Vector3>> candidate)
    {
        spawnPos = pos;
        goalPos = goal;
        shortPaths = shortPath;
        candidatePaths = candidate;
    }
}
