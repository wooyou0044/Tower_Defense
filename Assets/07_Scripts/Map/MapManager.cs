using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public struct MinimapSpawnStruct
{
    public Vector3 spawnPos;
    public Direction connectDireciton;
    public CreateMiniMap buttonRef;
}

public class MapManager : MonoBehaviour
{
    static MapManager _instance;

    [SerializeField] GameObject centralMiniMap;
    [SerializeField] GameObject[] miniMaps;
    [SerializeField] Transform miniMapsParent;
    [SerializeField] Material miniMapMaterial;

    Dictionary<Vector2, MinimapNode> dicMiniMaps;
    MiniMapManager[] miniMapMgrs;
    MiniMapManager minimapMgr;
    //List<Material> canBuildMats;

    Material originMat;

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
    }

    void Start()
    {
        // ���ζ� ���� �����Ҷ� ������ ���� �̴ϸ� Dictionary�� �ֱ�
        dicMiniMaps = new Dictionary<Vector2, MinimapNode>();

        // ���� �̴ϸ� ���� �ؿ� �ϳ� ��ġ
        MakeMiniMapBelowRandomMap(centralMiniMap);
    }

    void MakeMiniMapBelowRandomMap(GameObject miniMap)
    {
        // ���� �޾ƿ���
        Vector2 minimapPos = new Vector2(miniMap.transform.position.x, miniMap.transform.position.z);
        Direction minimapRoadDir = miniMap.GetComponent<MiniMapManager>().miniMapInfo.RoadEdges.First();
        Direction connectRoadDir = OppositeDirection(minimapRoadDir);

        // ���� �̴ϸ� ����
        int random = Random.Range(0, miniMaps.Length);
        newMinimap = Instantiate(miniMaps[random], miniMapsParent);
        //GameObject newMiniMap = Instantiate(miniMaps[0]);
        minimapMgr = newMinimap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = minimapMgr.miniMapInfo;

        // 90���� ȸ���ϸ鼭 �´�� ���� ���ö����� �ݺ�
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
        Vector3 offset = GetOffsetDirection(minimapRoadDir, newMiniNode.GetSize());
        newMinimap.transform.position = miniMap.transform.position + offset;

        // Dictionary�� ���
        dicMiniMaps.Add(minimapPos, miniMap.GetComponent<MiniMapManager>().miniMapInfo);

        Vector2 newMiniMapPos = new Vector2(newMinimap.transform.position.x, newMinimap.transform.position.z);
        // ������ ������
        dicMiniMaps.Add(newMiniMapPos, newMiniNode);

        // �̴ϸ� ��ư ����
        MakeCreateMinimapButton(newMinimap, connectRoadDir);

        newMinimap = null;
    }

    public void ExamineRotate(MiniMapManager mapMgr)
    {
        Debug.Log("Rotate �Լ� ȣ���");
        mapMgr.RotateRoadEdgesClockWise();
        mapMgr.gameObject.transform.Rotate(Vector3.up, 90);
    }

    public void MakeOrMoveRandomMinimap(MinimapSpawnStruct spawnStruct)
    {
        if(isAlreadyMinimapMade == false)
        {
            CreateRandomMinimap();
            isAlreadyMinimapMade = true;
        }
        newMinimap.transform.position = spawnStruct.spawnPos;

        Direction buttonDir = spawnStruct.connectDireciton;
        UpdateMaterialConnection(buttonDir);
    }

    public void CreateRandomMinimap()
    {
        int random = Random.Range(0, miniMaps.Length);
        newMinimap = Instantiate(miniMaps[random], miniMapsParent);

        minimapMgr = newMinimap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = minimapMgr.miniMapInfo;

        minimapMgr.ChangeMaterial(miniMapMaterial);
    }

    void UpdateMaterialConnection(Direction dir)
    {
        bool isConnected = IsConnectedRoad(dir);

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

    void MakeCreateMinimapButton(GameObject miniMap, Direction alreadyConnectDir)
    {
        MinimapNode newMiniNode = miniMap.GetComponent<MiniMapManager>().miniMapInfo;

        foreach (var dir in newMiniNode.RoadEdges)
        {
            if (dir == alreadyConnectDir)
            {
                continue;
            }
            miniMap.GetComponent<MiniMapManager>().CreateMinimapButton(GetOffsetDirection(dir, newMiniNode.GetSize()), dir);
        }
    }

    public void RotateNewMinimap(Direction dir)
    {
        ExamineRotate(minimapMgr);
        UpdateMaterialConnection(dir);
    }

    public bool IsConnectedRoad(Direction dir)
    {
        Direction oppositeButtonDir = OppositeDirection(dir);
        bool isConnected = minimapMgr.miniMapInfo.RoadEdges.Contains(oppositeButtonDir);

        return isConnected;
    }

    public void AddDictionaryNewMinimap()
    {
        Vector2 newMiniMapPos = new Vector2(newMinimap.transform.position.x, newMinimap.transform.position.z);
        MinimapNode node = newMinimap.GetComponent<MiniMapManager>().miniMapInfo;

        // ������ ������
        dicMiniMaps.Add(newMiniMapPos, node);
        Debug.Log("Dictionary ���� : " + dicMiniMaps.Count);

        minimapMgr.ResetOriginMaterial();
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
