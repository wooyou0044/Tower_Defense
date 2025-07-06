using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static MapManager _instance;

    [SerializeField] GameObject centralMiniMap;
    [SerializeField] GameObject[] miniMaps;
    [SerializeField] Transform miniMapsParent;

    Dictionary<Vector2, MinimapNode> dicMiniMaps;
    MiniMapManager[] miniMapMgrs;

    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        // ���ζ� ���� �����Ҷ� ������ ���� �̴ϸ� Dictionary�� �ֱ�
        dicMiniMaps = new Dictionary<Vector2, MinimapNode>();

        // ���� �̴ϸ� ���� �ؿ� �ϳ� ��ġ
        MakeMiniMapBelowRandomMap(centralMiniMap);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {

        }
    }

    void MakeMiniMapBelowRandomMap(GameObject miniMap)
    {
        // ���� �޾ƿ���
        Vector2 minimapPos = new Vector2(miniMap.transform.position.x, miniMap.transform.position.z);
        Direction minimapRoadDir = miniMap.GetComponent<MiniMapManager>().miniMapInfo.RoadEdges.First();
        Direction connectRoadDir = OppositeDirection(minimapRoadDir);

        // ���� �̴ϸ� ����
        int random = Random.Range(0, miniMaps.Length);
        GameObject newMiniMap = Instantiate(miniMaps[random], miniMapsParent);
        //GameObject newMiniMap = Instantiate(miniMaps[0]);
        MiniMapManager miniMapMgr = newMiniMap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = miniMapMgr.miniMapInfo;

        // 90���� ȸ���ϸ鼭 �´�� ���� ���ö����� �ݺ�
        int rotateCount = 0;
        while(rotateCount < (int)Direction.None)
        {
            if(miniMapMgr.miniMapInfo.RoadEdges.Contains(connectRoadDir))
            {
                break;
            }
            miniMapMgr.RotateRoadEdgesClockWise();
            newMiniMap.transform.Rotate(Vector3.up, 90);
            rotateCount++;
        }

        if(rotateCount == 4)
        {
            Debug.Log("���� �� �ִ� �̴ϸ��� ����");
            //Destroy(newMiniMap);
            rotateCount = 0;
            return;
        }

        // �̴ϸ� ��ġ
        Vector3 offset = GetOffsetDirection(minimapRoadDir, newMiniNode.GetSize());
        newMiniMap.transform.position = miniMap.transform.position + offset;

        // Dictionary�� ���
        dicMiniMaps.Add(minimapPos, miniMap.GetComponent<MiniMapManager>().miniMapInfo);

        Vector2 newMiniMapPos = new Vector2(newMiniMap.transform.position.x, newMiniMap.transform.position.z);
        // ������ ������
        dicMiniMaps.Add(newMiniMapPos, newMiniNode);

        // �̴ϸ� ��ư ����
        MakeCreateMinimapButton(newMiniMap, connectRoadDir);
    }

    public void MakeRandomMinimap(Vector3 pos)
    {
        int random = Random.Range(0, miniMaps.Length);
        GameObject newMiniMap = Instantiate(miniMaps[random], miniMapsParent);

        MiniMapManager miniMapMgr = newMiniMap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = miniMapMgr.miniMapInfo;

        newMiniMap.transform.position = pos;

        dicMiniMaps.Add(pos, newMiniNode);
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
            miniMap.GetComponent<MiniMapManager>().CreateMinimapButton(GetOffsetDirection(dir, newMiniNode.GetSize()));
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
