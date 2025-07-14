using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    List<MinimapNode> startExamineNodes;

    Material originMat;
    bool isMapChange = false;
    int minimapSize;

    Queue<Vector2> qExamineNode;
    HashSet<Vector2> visited;
    List<MinimapNode> connectedMinimaps;

    public GameObject newMinimap { get; private set; }

    public bool isAlreadyMinimapMade { get; private set; }

    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        isAlreadyMinimapMade = false;

        createMapBtnlist = new List<CreateMiniMap>();
        qExamineNode = new Queue<Vector2>();
        visited = new HashSet<Vector2>();
        connectedMinimaps = new List<MinimapNode>();
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

    void GetStartTileFromDirection()
    {
        startExamineNodes = new List<MinimapNode>();

        foreach(var button in createMapBtnlist)
        {
            Direction oppBtnDir = OppositeDirection(button.ButtonSpawnStruct.connectDireciton);
            Vector3 oppBtnMinimapPos = GetOffsetDirection(oppBtnDir, minimapSize);
            if (dicMiniMaps[new Vector2(oppBtnMinimapPos.x, oppBtnMinimapPos.z)] != null)
            {
                startExamineNodes.Add(dicMiniMaps[new Vector2(oppBtnMinimapPos.x, oppBtnMinimapPos.z)]);
            }
        }
        qExamineNode.Clear();
        visited.Clear();
        connectedMinimaps.Clear();
    }

    void CollectConnctedMinimaps(MinimapNode nextNode, Vector2 minimapPos)
    {
        qExamineNode.Enqueue(minimapPos);
        visited.Add(minimapPos);

        while(qExamineNode.Count > 0)
        {
            var current = qExamineNode.Dequeue();
            if (dicMiniMaps.TryGetValue(current, out var currentNode) == false)
            {
                continue;
            }
            connectedMinimaps.Add(currentNode);

            foreach(var dir in currentNode.RoadEdges)
            {
                Vector3 next = GetOffsetDirection(dir, minimapSize);

                if(visited.Contains(new Vector2(next.x, next.z)))
                {
                    continue;
                }
                if(dicMiniMaps.TryGetValue(next, out var neighbor) && neighbor.RoadEdges.Contains(OppositeDirection(dir)))
                {
                    qExamineNode.Enqueue(next);
                    visited.Add(next);
                }
            }
        }
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
        float tileSize = 4f;

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
