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
    List<CreateMiniMap> createMapBtnlist;
    Direction alreadyConnectedDir;

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

        createMapBtnlist = new List<CreateMiniMap>();
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
        //newMinimap = Instantiate(miniMaps[9]);
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
        UpdateMaterialConnection();
    }

    public void CreateRandomMinimap()
    {
        int random = Random.Range(0, miniMaps.Length);
        newMinimap = Instantiate(miniMaps[random], miniMapsParent);

        minimapMgr = newMinimap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = minimapMgr.miniMapInfo;

        minimapMgr.ChangeMaterial(miniMapMaterial);
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
