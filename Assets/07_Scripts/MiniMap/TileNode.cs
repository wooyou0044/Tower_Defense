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

    public float height { get; set; }

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
    }

    public TileNode(TileType tileType)
    {
        type = tileType;
        // 높이는 나중에
    }

    public int GetTileType()
    {
        return (int)type;
    }

    // 길이 연결되었는지 아니면 끊겨있는지로 적들이 생성되는 여부를 결정해야 함
}
