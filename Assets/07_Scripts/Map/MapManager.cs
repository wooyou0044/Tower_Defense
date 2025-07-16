using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct MinimapSpawnStruct
{
    public Vector3 miniMapspawnPos;
    public Direction connectDireciton;
    public CreateMiniMap buttonRef;
    public Vector2 spawnPos;
}

public class MapManager : MonoBehaviour
{
    static MapManager _instance;

    [SerializeField] GameObject centralMiniMap;
    [SerializeField] GameObject[] miniMaps;
    [SerializeField] Transform miniMapsParent;
    [SerializeField] Material miniMapMaterial;
    [SerializeField] Transform previewMinimapPos;

    Dictionary<Vector2, MinimapNode> dicMiniMaps;
    MiniMapManager minimapMgr;
    List<CreateMiniMap> createMapBtnlist;
    Queue<GameObject> qPreviewMinimap;
    Direction alreadyConnectedDir;
    PathFinding path;

    Material originMat;
    bool isMapChange = false;
    int minimapSize;

    public GameObject newMinimap { get; private set; }

    public bool isAlreadyMinimapMade { get; private set; }

    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public List<CreateMiniMap> minimapButtonList
    {
        get { return createMapBtnlist; }
    }    

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        isAlreadyMinimapMade = false;

        createMapBtnlist = new List<CreateMiniMap>();
    }

    void Start()
    {
        // ���ζ� ���� �����Ҷ� ������ ���� �̴ϸ� Dictionary�� �ֱ�
        dicMiniMaps = new Dictionary<Vector2, MinimapNode>();

        // ���� �̴ϸ� ���� �ؿ� �ϳ� ��ġ
        MakeMiniMapBelowRandomMap(centralMiniMap);

        // �ٲ� �̸����� �� �����
        MakePreviewRandomMap();
    }

    void MakeMiniMapBelowRandomMap(GameObject miniMap)
    {
        // ���� �޾ƿ���
        Vector2 minimapPos = new Vector2(miniMap.transform.position.x, miniMap.transform.position.z);
        //Direction minimapRoadDir = miniMap.GetComponent<MiniMapManager>().miniMapInfo.RoadEdges.First();
        //Direction connectRoadDir = OppositeDirection(minimapRoadDir);

        // Dictionary�� ���
        dicMiniMaps.Add(minimapPos, miniMap.GetComponent<MiniMapManager>().miniMapInfo);

        foreach (var direction in miniMap.GetComponent<MiniMapManager>().miniMapInfo.RoadEdges)
        {
            Direction connectRoadDir = OppositeDirection(direction);

            int random = Random.Range(0, miniMaps.Length);
            newMinimap = Instantiate(miniMaps[random], miniMapsParent);
            minimapMgr = newMinimap.GetComponent<MiniMapManager>();
            MinimapNode newMiniNode = minimapMgr.miniMapInfo;

            if(minimapSize == 0)
            {
                minimapSize = newMiniNode.GetSize();
            }

            int rotateCount = 0;
            while (rotateCount < (int)Direction.None)
            {
                if (minimapMgr.miniMapInfo.RoadEdges.Contains(connectRoadDir))
                {
                    break;
                }
                ExamineRotate(minimapMgr);
                rotateCount++;
            }

            if (rotateCount == 4)
            {
                Debug.Log("���� �� �ִ� �̴ϸ��� ����");
                //Destroy(newMiniMap);
                rotateCount = 0;
                return;
            }

            // �̴ϸ� ��ġ
            Vector3 offset = GetOffsetDirection(direction, newMiniNode.GetSize());
            newMinimap.transform.position = miniMap.transform.position + offset;

            Vector2 newMiniMapPos = new Vector2(newMinimap.transform.position.x, newMinimap.transform.position.z);
            // ������ ������
            dicMiniMaps.Add(newMiniMapPos, newMiniNode);

            // �̴ϸ� ��ư ����
            MakeCreateMinimapButton(newMinimap, connectRoadDir);

            newMinimap = null;
        }
    }

    public void ExamineRotate(MiniMapManager mapMgr)
    {
        Debug.Log("Rotate �Լ� ȣ���");
        mapMgr.RotateRoadEdgesClockWise();
        mapMgr.gameObject.transform.Rotate(Vector3.up, 90);

        int indexSize = mapMgr.miniMapInfo.TileNodes.GetLength(0);
        for (int i = 0; i < indexSize; i++)
        {
            for (int j = 0; j < indexSize; j++)
            {
                float posY = mapMgr.miniMapInfo.TileNodes[j, i].worldPosition.y;
                Vector3 worldPos = mapMgr.SetTileNodeWorldPosition(i, j, mapMgr.transform, posY);
                mapMgr.miniMapInfo.TileNodes[j, i].worldPosition = worldPos;
            }
        }
    }

    public void MakeOrMoveRandomMinimap(MinimapSpawnStruct spawnStruct)
    {
        if (isAlreadyMinimapMade == false)
        {
            newMinimap = CreateRandomMinimap(true);
            minimapMgr = newMinimap.GetComponent<MiniMapManager>();
            isAlreadyMinimapMade = true;
        }
        if(newMinimap.activeSelf == false)
        {
            newMinimap.SetActive(true);
        }
        newMinimap.transform.position = spawnStruct.miniMapspawnPos;

        Direction buttonDir = spawnStruct.connectDireciton;

        UpdateMaterialConnection();
    }

    public GameObject CreateRandomMinimap(bool isMatChange)
    {
        int random = Random.Range(0, miniMaps.Length);
        GameObject rdMinimap = Instantiate(miniMaps[random], miniMapsParent);

        MiniMapManager rdMinimapMgr = rdMinimap.GetComponent<MiniMapManager>();

        if(isMatChange == true)
        {
            rdMinimapMgr.ChangeMaterial(miniMapMaterial);
        }

        return rdMinimap;
    }

    void UpdateMaterialConnection()
    {
        Vector3 newMinimapPos = newMinimap.transform.position;
        Vector2 newMinimapPosXZ = new Vector2(newMinimapPos.x, newMinimapPos.z);

        bool isConnected = IsConnectedRoad(newMinimapPosXZ);

        minimapMgr.TurnIsRoadConnect(isConnected);
    }

    public void SetActiveNewMinimap(bool isActive)
    {
        if (newMinimap == null)
        {
            return;
        }
        newMinimap.SetActive(isActive);
    }

    public void AddCreateMapBtnList(CreateMiniMap miniMapBtn)
    {
        createMapBtnlist.Add(miniMapBtn);
    }

    public void RemoveCreateMapBtnList(CreateMiniMap miniMapBtn)
    {
        Debug.Log("������ List ���� : " + createMapBtnlist.Count);
        foreach (var button in createMapBtnlist)
        {
            button.gameObject.SetActive(false);
        }
        createMapBtnlist.Remove(miniMapBtn);
        Debug.Log("������ ���� List ���� : " + createMapBtnlist.Count);
    }

    bool IsOverlapMakeButton(Vector3 pos)
    {
        foreach(var button in createMapBtnlist)
        {
            if(button.transform.position == pos)
            {
                return true;
            }
        }
        return false;
    }

    void MakeCreateMinimapButton(GameObject newMinimap, Direction alreadyConnectDir)
    {
        MinimapNode newMiniNode = newMinimap.GetComponent<MiniMapManager>().miniMapInfo;

        foreach (var dir in newMiniNode.RoadEdges)
        {
            if (dir == alreadyConnectDir)
            {
                continue;
            }
            Vector3 buttonPos = newMinimap.transform.position + GetOffsetDirection(dir, newMiniNode.GetSize());
            // ������ �ִ� ��ư�� ��ġ�� ��ư ���� �� �ϰ� �Ѿ
            if (IsOverlapMakeButton(buttonPos + Vector3.down))
            {
                continue;
            }
            // �ش� ��ġ�� �̴ϸ��� ������ ��ư ���� �� �ϰ� �Ѿ
            if(dicMiniMaps.ContainsKey(new Vector2(buttonPos.x, buttonPos.z)))
            {
                continue;
            }
            newMinimap.GetComponent<MiniMapManager>().CreateMinimapButton(GetOffsetDirection(dir, newMiniNode.GetSize()), dir);
        }
        Debug.Log("���ο� �̴ϸ� ��ġ ���� List ���� : " + createMapBtnlist.Count);
    }

    public void RotateNewMinimap(Direction dir)
    {
        ExamineRotate(minimapMgr);
        UpdateMaterialConnection();
    }

    public bool IsConnectedRoad(Vector2 pos)
    {
        MinimapNode newMapNode = minimapMgr.miniMapInfo;

        foreach(Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            Vector3 offset = GetOffsetDirection(dir, newMapNode.GetSize());
            Vector2 neighborPos = pos + new Vector2(offset.x, offset.z);
            if(dicMiniMaps.TryGetValue(neighborPos, out MinimapNode neighbor))
            {
                Direction opposite = OppositeDirection(dir);

                bool isNewHasRoad = newMapNode.RoadEdges.Contains(dir);
                bool isNeighborHasRoad = neighbor.RoadEdges.Contains(opposite);

                if(isNewHasRoad != isNeighborHasRoad)
                {
                    return false;
                }
            }
            alreadyConnectedDir = dir;
        }
        return true;
    }

    public void AddDictionaryNewMinimap()
    {
        Vector2 newMiniMapPos = new Vector2(newMinimap.transform.position.x, newMinimap.transform.position.z);
        MinimapNode node = newMinimap.GetComponent<MiniMapManager>().miniMapInfo;

        // ������ ������
        dicMiniMaps.Add(newMiniMapPos, node);
        Debug.Log("Dictionary ���� : " + dicMiniMaps.Count);

        minimapMgr.ResetOriginMaterial();

        // ���� �̾��� �� �ܿ� ��ư ����
        MakeCreateMinimapButton(newMinimap, alreadyConnectedDir);

        // ������ ���� �����
        ResetNewMinimap();

        // �ӽ÷� �׽�Ʈ
        SetActiveAllCreateMinimapBtn(true);
    }

    void ResetNewMinimap()
    {
        newMinimap = null;
        isAlreadyMinimapMade = false;
        isMapChange = false;
    }

    public void SetActiveAllCreateMinimapBtn(bool isActive)
    {
        foreach(var btn in createMapBtnlist)
        {
            if(btn.gameObject.activeSelf == isActive)
            {
                continue;
            }
            btn.gameObject.SetActive(isActive);
        }
    }

    void MakePreviewRandomMap()
    {
        qPreviewMinimap = new Queue<GameObject>();
        for(int i=0; i<3; i++)
        {
            int random = Random.Range(0, miniMaps.Length);
            GameObject previewMiniObj = Instantiate(miniMaps[random], miniMapsParent);
            previewMiniObj.transform.position = previewMinimapPos.position;
            qPreviewMinimap.Enqueue(previewMiniObj);
            if(i == 0)
            {
                continue;
            }
            previewMiniObj.SetActive(false);
        }
    }

    void ChangePreviewMinimap()
    {
        if (qPreviewMinimap.Count > 0)
        {
            GameObject newPreviewMap = qPreviewMinimap.Peek();
            newPreviewMap.SetActive(true);
        }
    }

    public void ChangeMinimap()
    {
        Destroy(newMinimap);
        newMinimap = qPreviewMinimap.Dequeue();
        newMinimap.SetActive(false);
        minimapMgr = newMinimap.GetComponent<MiniMapManager>();
        minimapMgr.ChangeMaterial(miniMapMaterial);
        GameObject newPreviewMap = CreateRandomMinimap(false);
        newPreviewMap.transform.position = previewMinimapPos.position;
        newPreviewMap.SetActive(false);
        qPreviewMinimap.Enqueue(newPreviewMap);
        ChangePreviewMinimap();
        isMapChange = true;
    }

    public List<Vector3> FindEnemyPath(CreateMiniMap spawnButton)
    {
        Direction oppBtnDir = OppositeDirection(spawnButton.ButtonSpawnStruct.connectDireciton);
        Vector3 oppBtnMinimapPos = spawnButton.transform.position + GetOffsetDirection(oppBtnDir, minimapSize);
        Vector2 minimapPos = new Vector2(oppBtnMinimapPos.x, oppBtnMinimapPos.z);

        if (dicMiniMaps.TryGetValue(minimapPos, out var startNode) == false)
        {
            Debug.Log("�̴ϸ� ����");
            return null;
        }

        //startNodesPos.Add(minimapPos);

        List<Vector3> enemySpawnPoints = new List<Vector3>();

        foreach (var dir in startNode.RoadEdges)
        {
            Vector3 offset = GetOffsetDirection(dir, startNode.GetSize());
            Vector2 neighborMinimapPos = minimapPos + new Vector2(offset.x, offset.z);

            if (dicMiniMaps.ContainsKey(neighborMinimapPos) == false)
            {
                Vector3 spawnPos = startNode.GetWorldPositionCenter() + GetOffsetDirection(dir, startNode.GetSize() / 2);
                enemySpawnPoints.Add(spawnPos);
            }
        }

        List<MinimapNode> connectedMinimaps = CollectConnctedMinimaps(minimapPos);
        MiniMapManager centralMapMgr = centralMiniMap.GetComponent<MiniMapManager>();

        if (connectedMinimaps.Contains(centralMapMgr.miniMapInfo) == false)
        {
            Debug.Log("����� �̴ϸʿ� ���ΰ� ����");
            return null;
        }

        List<Vector3> pathCandidates = new List<Vector3>();
        foreach (var minimap in connectedMinimaps)
        {
            pathCandidates.AddRange(minimap.GetRoadWorldPositions());
        }
        Debug.Log("pathCandidates ���� : " + pathCandidates.Count);

        Vector3 goal = centralMapMgr.miniMapInfo.GetWorldPositionCenter();
        if(enemySpawnPoints.Count == 0)
        {
            Debug.Log("�� ���� ��ġ�� ����");
            return null;
        }

        List<Vector3> path = AStarPathFinding(enemySpawnPoints[0], goal, pathCandidates);
        Debug.Log("MapManager path ���� : " + path.Count);

        return path;
    }

    List<MinimapNode> CollectConnctedMinimaps(Vector2 startMinimapPos)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();
        List<MinimapNode> connectedMinimaps = new List<MinimapNode>();

        var centralMinimapNode = dicMiniMaps.First().Value;

        queue.Enqueue(startMinimapPos);
        visited.Add(startMinimapPos);

        while(queue.Count > 0)
        {
            var currentPos = queue.Dequeue();

            if(dicMiniMaps.TryGetValue(currentPos, out var currentNode) == false)
            {
                continue;
            }
            connectedMinimaps.Add(currentNode);

            if(currentNode == centralMinimapNode)
            {
                break;
            }

            foreach(var dir in currentNode.RoadEdges)
            {
                Vector3 offset = GetOffsetDirection(dir, currentNode.GetSize());
                Vector2 neighborPos = currentPos + new Vector2(offset.x, offset.z);

                if(visited.Contains(neighborPos))
                {
                    continue;
                }

                if(dicMiniMaps.TryGetValue(neighborPos, out var neighbor) && neighbor.RoadEdges.Contains(OppositeDirection(dir)))
                {
                    queue.Enqueue(neighborPos);
                    visited.Add(neighborPos);
                }
            }
        }
        return connectedMinimaps;
    }

    List<Vector3> AStarPathFinding(Vector3 enemySpawnPos, Vector3 goalPos, List<Vector3> pathCandidatesPos)
    {
        Dictionary<Vector3, AStarNode> dicNodeMap = new Dictionary<Vector3, AStarNode>();
        foreach(var pos in pathCandidatesPos)
        {
            dicNodeMap[pos] = new AStarNode(pos);
        }
        AStarNode startNode = GetCloseOpenNode(enemySpawnPos, dicNodeMap);
        AStarNode endNode = GetCloseOpenNode(goalPos, dicNodeMap);

        startNode.gCost = 0;
        startNode.hCost = Heurisitic(startNode.worldPosition, endNode.worldPosition);

        List<AStarNode> openList = new List<AStarNode>();
        HashSet<AStarNode> closedList = new HashSet<AStarNode>();

        openList.Add(startNode);
        
        while(openList.Count > 0)
        {
            openList.Sort((a, b) => a.fCost.CompareTo(b.fCost));

            AStarNode currentNode = openList[0];
            if(currentNode == endNode)
            {
                List<Vector3> path = new List<Vector3>();
                AStarNode current = endNode;
                while(current != null)
                {
                    path.Add(current.worldPosition);
                    current = current.parent;
                }
                path.Reverse();

                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<AStarNode> neighborNodeList = GetNeighbors(currentNode, dicNodeMap);

            foreach(var neighbor in neighborNodeList)
            {
                if(closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeG = currentNode.gCost + Vector3.Distance(currentNode.worldPosition, neighbor.worldPosition);

                if(tentativeG < neighbor.gCost)
                {
                    neighbor.parent = currentNode;
                    neighbor.gCost = tentativeG;
                    neighbor.gCost = Heurisitic(neighbor.worldPosition, endNode.worldPosition);

                    if(openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        return new List<Vector3>();
    }

    AStarNode GetCloseOpenNode(Vector3 target, Dictionary<Vector3, AStarNode> dicMap)
    {
        return dicMap.Values.OrderBy(n => Vector3.Distance(n.worldPosition, target)).First();
    }

    float Heurisitic(Vector3 startPos, Vector3 endPos)
    {
        return Mathf.Abs(startPos.x - endPos.x) - Mathf.Abs(startPos.z - endPos.z);
    }

    List<AStarNode> GetNeighbors(AStarNode node, Dictionary<Vector3, AStarNode> dicMap)
    {
        float gridSize = 4f;
        List<AStarNode> neighbors = new List<AStarNode>();
        Vector3[] directions =
        {
            new Vector3(gridSize, 0, 0),
            new Vector3(-gridSize, 0, 0),
            new Vector3(0, 0, gridSize),
            new Vector3(0, 0, -gridSize)
        };

        foreach(var dir in directions)
        {
            Vector3 neighboarPos = node.worldPosition + dir;
            if(dicMap.TryGetValue(neighboarPos, out var neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    Direction OppositeDirection(Direction direct)
    {
        switch (direct)
        {
            case Direction.Top:
                return Direction.Bottom;
            case Direction.Bottom:
                return Direction.Top;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
        }
        return Direction.None;
    }

    Vector3 GetOffsetDirection(Direction direct, int mapSize)
    {
        switch(direct)
        {
            case Direction.Top:
                return new Vector3(0, 0, mapSize);
            case Direction.Bottom:
                return new Vector3(0, 0, -mapSize);
            case Direction.Left:
                return new Vector3(-mapSize, 0, 0);
            case Direction.Right:
                return new Vector3(mapSize, 0, 0);
        }
        return Vector3.zero;
    }
}
