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
        // ���̴� ���߿�
    }

    public int GetTileType()
    {
        return (int)type;
    }

    // ���� ����Ǿ����� �ƴϸ� �����ִ����� ������ �����Ǵ� ���θ� �����ؾ� ��
}
