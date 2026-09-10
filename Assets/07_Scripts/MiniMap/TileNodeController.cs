using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNodeController : MonoBehaviour
{
    [SerializeField] TileType type;
    Collider col;
    float topY;

    public float GetTopY
    {
        get => topY;
    }

    private void Awake()
    {
        col = GetComponent<Collider>();
        topY = col.bounds.max.y;
    }

    public float gridSize
    {
        get
        {
            if (type == TileType.Buildable)
            {
                return 4;
            }
            return 2;
        }
    }


    public TileType Type
    {
        get => type;
    }
}
