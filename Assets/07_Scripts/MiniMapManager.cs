using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    TileNode[,] tileNodes;
    bool[] isRoadSide;

    int indexSize;

    void Start()
    {
        indexSize = 5;
        tileNodes = new TileNode[indexSize, indexSize];
        isRoadSide = new bool[4];

        for (int i = 0; i < tileNodes.GetLength(0); i++)
        {
            for (int j = 0; j < tileNodes.GetLength(1); j++)
            {
                tileNodes[i, j] = new TileNode();
            }
        }

        foreach (Transform child in transform)
        {
            TileNode node = child.GetComponent<TileNode>();
            if (node == null)
            {
                Debug.Log(node.gameObject);
                continue;
            }
            Vector3 pos = child.localPosition;
            Vector3 scale = child.localScale;

            int width = Mathf.FloorToInt(scale.x / node.gridSize);
            int height = Mathf.FloorToInt(scale.z / node.gridSize);

            int gridPosX = 0;
            int gridPosZ = 0;
            int baseX;
            int baseZ;

            if (width == indexSize)
            {
                baseZ = Mathf.FloorToInt(-pos.z / 4);
                if (height >= 3)
                {
                    //baseZ -= 1;
                    baseZ -= Mathf.RoundToInt((float)height / 4);
                }
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int z = baseZ + i;
                        tileNodes[z, j].type = node.type;
                        //tileNodes[z, j] = new TileNode(node.type);
                    }
                }
                continue;
            }

            if (height == indexSize)
            {
                baseX = Mathf.FloorToInt(pos.x / 4);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int x = baseX + j;
                        tileNodes[i, x].type = node.type;
                        //tileNodes[i, x] = new TileNode(node.type);
                    }
                }
                continue;
            }

            if (pos.x % 4 == 0)
            {
                baseX = Mathf.FloorToInt(pos.x / 4);
            }

            else
            {
                // 4X4 => 0     1
                // 8X8 => 2     2
                // 12x12 => 4   3
                // 16X16 => 6   4
                // 20X20 => 8   5
                if(width % 2 == 0)
                {
                    // 이건 있어야 할지 빼야 할지 이따가 결정
                    if(pos.x % 4 == 2)
                    {
                        baseX = Mathf.FloorToInt(pos.x / 4);
                    }
                    else
                    {
                        gridPosX = Mathf.FloorToInt(pos.x + (2 - pos.x % 4));
                        baseX = gridPosX / 4;
                    }
                }
                else
                {
                    // 4로 나눈 나머지가 0으로 만들어야 함
                    if(pos.x % 4 >= 2)
                    {
                        gridPosX = Mathf.FloorToInt(pos.x - (pos.x % 4)) + 4;
                        baseX = gridPosX / 4;
                    }
                    else
                    {
                        gridPosX = Mathf.FloorToInt(pos.x - (pos.x % 4));
                        baseX = gridPosX / 4;
                    }
                }
            }

            if (-pos.z % 4 == 0)
            {
                baseZ = Mathf.FloorToInt(-pos.z / 4);
            }

            else
            {
                if (height % 2 == 0)
                {
                    // 4로 나눈 나머지가 2로 떨어지게 해야 함
                    if (-pos.z % 4 == 2)
                    {
                        baseZ = Mathf.FloorToInt(-pos.z / 4);
                    }
                    else
                    {
                        gridPosZ = Mathf.FloorToInt(-pos.z - (2 - (-pos.z % 4)));
                        baseZ = gridPosZ / 4;
                    }
                }
                else
                {
                    // 4로 나눈 나머지가 0을 떨어지게 해야 함
                    if (-pos.z % 4 >= 2)
                    {
                        gridPosZ = Mathf.FloorToInt(-pos.z - (-pos.z % 4)) + 4;
                        baseZ = gridPosZ / 4;
                    }
                    else
                    {
                        gridPosZ = Mathf.FloorToInt(-pos.z - (-pos.z % 4));
                        baseZ = gridPosZ / 4;
                    }
                }
            }

            if (width >= 3)
            {
                //baseX -= 1;
                baseX -= Mathf.RoundToInt((float)width / 4);
            }
            if (height >= 3)
            {
                //baseZ -= 1;
                baseZ -= Mathf.RoundToInt((float)height / 4);
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int z = baseZ + i;
                    int x = baseX + j;
                    if (x >= 0 && x < indexSize && z >= 0 && z < indexSize)
                    {
                        tileNodes[z, x].type = node.type;
                        //tileNodes[z, x] = new TileNode(node.type);
                    }
                }
            }

            //높이는 나중에

        }

        //for (int i = 0; i < indexSize; i++)
        //{
        //    for (int j = 0; j < indexSize; j++)
        //    {
        //        Debug.Log($"tileNodes[{i},{j}] = {tileNodes[i, j].type}");
        //    }
        //}

        ExamineRoadSide();
    }

    void Update()
    {
        
    }

    void ExamineRoadSide()
    {
        int sideIndex = 0;
        int indexX = 0;
        int indexY = 0;
        bool isRoadExist = false;
        int size = tileNodes.GetLength(0);
        // 위
        //for(int y = 0; y < size; y++)
        //{
        //    if (tileNodes[0, y].type == TileType.Road)
        //    {
        //        isRoadSide[sideIndex] = true;
        //        sideIndex++;
        //        break;
        //    }
        //    else
        //    {
        //        isRoadExist = false;
        //    }
        //}

        // tileNode의 xIndex가 0일 때 yIndex를 0부터 4까지 검사하고 Road면 isRoadSide는 True로 설정
        // tileNode의 xIndex가 4일 때 yIndex를 0부터 4까지 검사하고 Road면 isRoadSide는 True로 설정
        // tileNode의 yIndex가 0일 때 xIndex를 0부터 4까지 검사하고 Road면 isRoadSide는 True로 설정
        // tileNode의 yIndex가 4일 때 xIndex를 0부터 4까지 검사하고 Road면 isRoadSide는 True로 설정

        for(int dir = 0; dir<isRoadSide.Length; dir++)
        {

        }
    }


    //int DisConnectedRoadSideIndex(int arrSize)
    //{
    //    int examineIndex = 0;
    //    while (examineIndex < arrSize)
    //    {
    //        if (tileNodes[0, examineIndex].type == TileType.Road)
    //        {

    //        }
    //        examineIndex++;
    //    }
    //}
}
