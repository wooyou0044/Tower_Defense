using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None = -1,
    Road,
    Buildable
}

public class TileInfo : MonoBehaviour
{
    public TileType type;

    public int GetTileType()
    {
        return (int)type;
    }
}
