using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathInfo
{
    public Vector3 spawnPos { get; private set; }
    public List<Vector3> movePath { get; private set; }

    public EnemyPathInfo(Vector3 pos, List<Vector3> path)
    {
        spawnPos = pos;
        movePath = path;
    }
}
