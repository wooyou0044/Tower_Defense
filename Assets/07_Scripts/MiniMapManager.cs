using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public TileNode[,] tileNodes;

    int indexSize;

    void Start()
    {
        indexSize = 5;
        tileNodes = new TileNode[indexSize, indexSize];

        for(int i=0; i<tileNodes.GetLength(0); i++)
        {
            for(int j=0; j<tileNodes.GetLength(1); j++)
            {
                tileNodes[i, j] = new TileNode();
            }
        }

        foreach(Transform child in transform)
        {
            TileNode node = child.GetComponent<TileNode>();
            if(node == null || node.type == TileType.Road)
            {
                Debug.Log(node.gameObject);
                continue;
            }
            Vector3 pos = child.localPosition;
            Vector3 scale = child.localScale;

            Debug.Log("node.gridSize : " + node.gridSize);
            int width = Mathf.FloorToInt(scale.x / node.gridSize);
            int height = Mathf.FloorToInt(scale.z / node.gridSize);

            int baseX = Mathf.RoundToInt(pos.x / 4);
            int baseZ = Mathf.RoundToInt(-pos.z / 4);

            for(int i=0; i<height; i++)
            {
                for(int j=0; j<width; j++)
                {
                    int x = baseZ + i;
                    int z = baseX + j;

                    if(x >= 0 && x <indexSize && z >= 0 && z <indexSize)
                    {
                        tileNodes[x, z].type = node.type;
                    }
                }
            }

            // 높이는 나중에
        }

        for(int i=0; i<indexSize; i++)
        {
            for(int j=0; j<indexSize; j++)
            {
                Debug.Log($"tileNodes[{i},{j}] = {tileNodes[i, j].type}");
            }
        }
    }

    void Update()
    {
        
    }
}
