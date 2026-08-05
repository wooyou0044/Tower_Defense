using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNodeController : MonoBehaviour
{
    [SerializeField] TileType type;

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
