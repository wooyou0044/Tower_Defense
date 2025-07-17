using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathInfo
{
    public Vector3 spawnPos { get; set; }
    public Vector3 goalPos { get; set; }
    public List<Vector3> movePath { get; set; }

    public EnemyPathInfo(Vector3 pos, Vector3 goal, List<Vector3> path)
    {
        spawnPos = pos;
        goalPos = goal;
        movePath = path;
    }
}
