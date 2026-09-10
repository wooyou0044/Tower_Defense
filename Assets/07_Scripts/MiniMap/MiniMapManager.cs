using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    TileNode[,] tileNodes;

    int indexSize;
    GameObject makeMiniMapButton;

    List<Material> canBuildMats;
    Material originMat;
    float buildGridSize;

    public MinimapNode miniMapInfo { get; private set; }
    public int GetGridSize
    {
        get => indexSize;
    }

    public float GetBuildGridSize
    {
        get => buildGridSize;
    }

    private void Awake()
    {
        miniMapInfo = new MinimapNode();
        // 미니맵 만들수 있는 버튼 게임 오브젝트 저장
        makeMiniMapButton = Resources.Load<GameObject>("CanBuildMiniMap");

        // 각각의 노드가 무슨 타입인지 나누기
        DivideMinimapNode();

        //Debug.Log(miniMapInfo.GetWorldPositionCenter());
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    void DivideMinimapNode()
    {
        indexSize = 5;
        tileNodes = new TileNode[indexSize, indexSize];

        for (int i = 0; i < tileNodes.GetLength(0); i++)
        {
            for (int j = 0; j < tileNodes.GetLength(1); j++)
            {
                tileNodes[i, j] = new TileNode();
            }
        }

        foreach (Transform child in transform)
        {
            TileNodeController nodeCtrl = child.GetComponent<TileNodeController>();
            if (nodeCtrl == null)
            {
                Debug.Log(nodeCtrl.gameObject);
                continue;
            }
            Vector3 pos = child.localPosition;
            pos.x += 8f;
            pos.z -= 8f;
            Vector3 scale = child.localScale;

            int width = Mathf.FloorToInt(scale.x / nodeCtrl.gridSize);
            int height = Mathf.FloorToInt(scale.z / nodeCtrl.gridSize);

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
                        tileNodes[z, j].type = nodeCtrl.Type;
                        //tileNodes[z, j] = new TileNode(node.type);
                        //tileNodes[z, j].worldPosition = SetTileNodeWorldPosition(j, z, transform, pos.y);
                        if (nodeCtrl.Type == TileType.Road || tileNodes[z, j].height > ExamineHeight(pos.y, scale.y))
                        {
                            continue;
                        }
                        tileNodes[z, j].worldPosition = SetTileNodeWorldPosition(j, z, transform, pos.y);
                        tileNodes[z, j].height = ExamineHeight(pos.y, scale.y);
                        tileNodes[z, j].buildY = nodeCtrl.GetTopY + 1;
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
                        tileNodes[i, x].type = nodeCtrl.Type;
                        //tileNodes[i, x] = new TileNode(node.type);
                        //tileNodes[i, x].worldPosition = SetTileNodeWorldPosition(x, i, transform, pos.y);
                        if (nodeCtrl.Type == TileType.Road || tileNodes[i, x].height > ExamineHeight(pos.y, scale.y))
                        {
                            continue;
                        }
                        tileNodes[i, x].worldPosition = SetTileNodeWorldPosition(x, i, transform, pos.y);
                        tileNodes[i, x].height = ExamineHeight(pos.y, scale.y);
                        tileNodes[i, x].buildY = nodeCtrl.GetTopY + 1;
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
                if (width % 2 == 0)
                {
                    // 이건 있어야 할지 빼야 할지 이따가 결정
                    if (pos.x % 4 == 2)
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
                    if (pos.x % 4 >= 2)
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
                        tileNodes[z, x].type = nodeCtrl.Type;
                        //Vector3 centerOffset = new Vector3((x - indexSize / 2) * 4, pos.y, (indexSize / 2 - z) * 4);
                        //tileNodes[z, x].worldPosition = SetTileNodeWorldPosition(x, z, transform, pos.y);
                        if (nodeCtrl.Type == TileType.Road || tileNodes[z, x].height > ExamineHeight(pos.y, scale.y))
                        {
                            continue;
                        }
                        tileNodes[z, x].worldPosition = SetTileNodeWorldPosition(x, z, transform, pos.y);
                        tileNodes[z, x].height = ExamineHeight(pos.y, scale.y);
                        tileNodes[z, x].buildY = nodeCtrl.GetTopY + 1;
                    }
                }
            }
        }
        miniMapInfo.TileNodes = tileNodes;

        // 길이 있는 면 확인
        miniMapInfo.RoadEdges = ExamineRoadSide(tileNodes);
        //for (int i = 0; i < indexSize; i++)
        //{
        //    for (int j = 0; j < indexSize; j++)
        //    {
        //        Debug.Log($"tileNodes[{i},{j}].worldPosition = {tileNodes[i, j].worldPosition}");
        //    }
        //}
    }

    int ExamineHeight(float posY, float scaleY)
    {
        float examineY = posY + scaleY / 2;

        if(examineY <= 3)
        {
            return  1;
        }
        else if(examineY <= 5)
        {
            return 2;
        }
        else if(examineY <= 10)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    HashSet<Direction> ExamineRoadSide(TileNode[,] nodes)
    {
        HashSet<Direction> edgeRoads = new HashSet<Direction>();

        int sizeX = nodes.GetLength(0);
        int sizeY = nodes.GetLength(1);

        bool HasRoad(Func<int, (int x, int y)> coordFunc)
        {
            for(int i=0; i<Mathf.Max(sizeX, sizeY); i++)
            {
                var (x, y) = coordFunc(i);

                if(x >= 0 && x < sizeX && y >= 0 && y < sizeY)
                {
                    if (nodes[x, y].type == TileType.Road)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // 왼쪽, 오른쪽, 위, 아래 기준을 Scene창에 맞춰 수정
        if(HasRoad(i => (i, 0)))
        {
            edgeRoads.Add(Direction.Left);
        }
        if(HasRoad(i => (i, sizeY - 1)))
        {
            edgeRoads.Add(Direction.Right);
        }
        if(HasRoad(i => (0, i)))
        {
            edgeRoads.Add(Direction.Top);
        }
        if(HasRoad(i => (sizeX - 1, i)))
        {
            edgeRoads.Add(Direction.Bottom);
        }

        // 검사용
        //foreach(var roadType in edgeRoads)
        //{
        //    Debug.Log(roadType);
        //}

        return edgeRoads;
    }

    Direction RotateClockWiseDirection(Direction direct)
    {
        switch(direct)
        {
            case Direction.Top:
                return Direction.Right;
            case Direction.Right:
                return Direction.Bottom;
            case Direction.Bottom:
                return Direction.Left;
            case Direction.Left:
                return Direction.Top;
        }
        return Direction.None;
    }

    public void RotateRoadEdgesClockWise()
    {
        miniMapInfo.RoadEdges = miniMapInfo.RoadEdges.Select(dir => RotateClockWiseDirection(dir)).ToHashSet();
    }

    // 길이 연결되어 있지만 맵끼리 연결되지 않은 곳에 맵 생성 버튼 만들기
    public void CreateMinimapButton(Vector3 createPos, Direction createDirect)
    {
        GameObject button = Instantiate(makeMiniMapButton, transform);
        Vector3 rotatedOffset = transform.InverseTransformDirection(createPos);
        Vector3 buttonPos = new Vector3(rotatedOffset.x, rotatedOffset.y - 1, rotatedOffset.z);
        button.transform.localPosition = buttonPos;

        CreateMiniMap buttonMinimap = button.GetComponent<CreateMiniMap>();
        buttonMinimap.InitializeDirection(createDirect);

        MapManager.Instance.AddCreateMapBtnList(buttonMinimap);

        // 임시로 꺼둠
        //button.SetActive(false);
    }

    public void ChangeMaterial(Material newMaterial)
    {
        canBuildMats = new List<Material>();

        // 기존 Material외 맵 설치할때 생길 수 있는 Material 추가
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childNode = transform.GetChild(i).gameObject;
            TileType childNodeType = childNode.GetComponent<TileNodeController>().Type;
            if (childNodeType == TileType.Road)
            {
                continue;
            }
            MeshRenderer rend = childNode.GetComponent<MeshRenderer>();
            originMat = rend.material;

            Material newMat = new Material(newMaterial);
            // Texture 넣기
            newMat.SetTexture("_MainTexture", originMat.GetTexture("_BaseMap"));
            newMat.SetFloat("_isBuildMinimap", 1f);
            Material[] newMats = new Material[2] { originMat, newMat };
            rend.materials = newMats;

            canBuildMats.Add(newMats[1]);
        }
    }

    public void TurnIsRoadConnect(bool isConnected)
    {
        // 기존 Material외 맵 설치할때 생길 수 있는 Material 추가
        for (int i = 0; i < canBuildMats.Count; i++)
        {
            canBuildMats[i].SetFloat("_isRoadConnect", isConnected ? 1f : 0f);
        }
    }

    public void ResetOriginMaterial()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<TileNodeController>().Type == TileType.Road)
            {
                continue;
            }
            MeshRenderer rend = transform.GetChild(i).GetComponent<MeshRenderer>();
            rend.materials = new Material[] { originMat };
        }
    }

    public Vector3 SetTileNodeWorldPosition(int posX, int posZ, Transform minimapTrans, float roadPosY)
    {
        Vector3 localOffset = new Vector3((posX - indexSize / 2) * 4, roadPosY, (indexSize / 2 - posZ) * 4);
        Vector3 worldPos = minimapTrans.TransformPoint(localOffset);
        //worldPos.y = roadPosY;
        return worldPos;
    }

    /// <summary>
    /// 해당 미니맵의 중심 인덱스 저장
    /// </summary>
    /// <param name="direct">설치하려는 Minimap의 기존 미니맵의 방향</param>
    public void SetGlobalIndex(MinimapNode originMapNodeInfo, Direction direct)
    {
        Vector2Int originStartIdx = originMapNodeInfo.startGlobalIndex;
        int tileIndexSize = tileNodes.GetLength(0);
        switch (direct)
        {
            case Direction.Top:
                miniMapInfo.startGlobalIndex = new Vector2Int(originStartIdx.x, originStartIdx.y + tileIndexSize);
                break;
            case Direction.Bottom:
                miniMapInfo.startGlobalIndex = new Vector2Int(originStartIdx.x, originStartIdx.y - tileIndexSize);
                break;
            case Direction.Left:
                miniMapInfo.startGlobalIndex = new Vector2Int(originStartIdx.x - tileIndexSize, originStartIdx.y);
                break;
            case Direction.Right:
                miniMapInfo.startGlobalIndex = new Vector2Int(originStartIdx.x + tileIndexSize, originStartIdx.y);
                break;
        }
        SetTileNodeGlobalIndex(miniMapInfo.startGlobalIndex);
    }

    void SetTileNodeGlobalIndex(Vector2Int startIndex)
    {
        for(int i=0; i<tileNodes.GetLength(0); i++)
        {
            for(int j=0; j<tileNodes.GetLength(1); j++)
            {
                tileNodes[i, j].globalIndex = new Vector2Int(startIndex.x + j, startIndex.y - i);
                BuildManager.Instance.SetDicGlobalIndexTileNode(tileNodes[i, j].globalIndex, miniMapInfo.TileNodes[i, j]);
                if(buildGridSize <= 0 && tileNodes[i,j].type == TileType.Buildable)
                {
                    buildGridSize = tileNodes[i, j].gridSize;
                }
            }
        }
    }

    public void TempPrint()
    {
        //for(int i=0; i<miniMapInfo.TileNodes.GetLength(0); i++)
        //{
        //    for(int j=0; j<miniMapInfo.TileNodes.GetLength(1); j++)
        //    {
        //        Debug.Log($"tileNodes[{i},{j}].Type = {miniMapInfo.TileNodes[i, j].type}");
        //        Debug.Log($"tileNodes[{i},{j}].globalIndex = {miniMapInfo.TileNodes[i, j].globalIndex}");
        //        Debug.Log($"tileNodes[{i},{j}].height = {miniMapInfo.TileNodes[i, j].height}");
        //    }
        //}
        BuildManager.Instance.TempPrint();
    }

    void Rotate4<T>(ref T a, ref T b, ref T c, ref T d)
    {
        T temp = a;
        a = b;
        b = c;
        c = d;
        d = temp;
    }

    public void RotateTileData(int rotateCount)
    {
        Debug.Log("회전 횟수 : " + rotateCount);
        int size = miniMapInfo.TileNodes.GetLength(0);

        for(int rc = 0; rc<rotateCount; rc++)
        {
            for(int layer = 0; layer<size/2; layer++)
            {
                int first = layer;
                int last = size - 1 - layer;

                for(int i=first; i<last; i++)
                {
                    int offset = i - first;

                    Rotate4(ref miniMapInfo.TileNodes[first, i], ref miniMapInfo.TileNodes[last - offset, first], ref miniMapInfo.TileNodes[last, last - offset], ref miniMapInfo.TileNodes[i, last]);
                }
            }
        }
    }

    public Vector2Int WorldToGridIndex(Vector3 hitPos)
    {
        //Vector3 localPos = transform.InverseTransformPoint(hitPos);
        Vector3 center = transform.position;
        float mapHalfSize = (indexSize * buildGridSize) * 0.5f;

        int x = Mathf.FloorToInt((hitPos.x - (center.x - mapHalfSize)) / buildGridSize);
        int z = Mathf.FloorToInt(((center.z + mapHalfSize) - hitPos.z) / buildGridSize);

        x = Mathf.Clamp(x, 0, indexSize - 1);
        z = Mathf.Clamp(z, 0, indexSize - 1);

        return new Vector2Int(x, z);
    }
}
