using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None = -1,
    Road,
    Buildable
}

public class TileNode : MonoBehaviour
{
    public TileType type;

    public float height
    {
        get
        {
            float baseHeight = 2f;
            if(type != TileType.Buildable)
            {
                return 0;
            }
            return Mathf.RoundToInt(transform.localScale.y / baseHeight);
        }
    }

    public float gridSize
    {
        get
        {
            if(type == TileType.Buildable)
            {
                return 4;
            }
            return 2;
        }
    }

    public TileNode()
    {
        type = TileType.None;
        // 높이는 나중에
    }

    public int GetTileType()
    {
        return (int)type;
    }
}
