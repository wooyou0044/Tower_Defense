using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject centralMiniMap;
    [SerializeField] GameObject[] miniMaps;

    Dictionary<Vector2, MinimapNode> dicMiniMaps;
    MiniMapManager[] miniMapMgrs;

    private void Awake()
    {
        
    }

    void Start()
    {
        // 랜덤 미니맵 본부 밑에 하나 설치
        MakeCentralBelowRandomMap();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {

        }
    }

    void MakeCentralBelowRandomMap()
    {
        // 본부 받아오기
        Vector2 centralPos = new Vector2(centralMiniMap.transform.position.x, centralMiniMap.transform.position.z);
        Direction centralRoadDir = centralMiniMap.GetComponent<MiniMapManager>().miniMapInfo.RoadEdges.First();
        Direction connectRoadDir = OppositeDirection(centralRoadDir);

        // 랜덤 미니맵 생성
        int random = Random.Range(0, miniMaps.Length);
        GameObject newMiniMap = Instantiate(miniMaps[random]);
        //GameObject newMiniMap = Instantiate(miniMaps[0]);
        MiniMapManager miniMapMgr = newMiniMap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = miniMapMgr.miniMapInfo;

        // 90도씩 회전하면서 맞닿는 방향 나올때까지 반복
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
            Debug.Log("붙일 수 있는 미니맵이 없음");
            //Destroy(newMiniMap);
            rotateCount = 0;
            return;
        }

        // 미니맵 설치
        Vector3 offset = GetOffsetDirection(centralRoadDir, newMiniNode.GetSize());
        newMiniMap.transform.position = centralMiniMap.transform.position + offset;

        // 본부랑 게임 시작할때 생성할 랜덤 미니맵 Dictionary에 넣기
        dicMiniMaps = new Dictionary<Vector2, MinimapNode>();
        // 본부
        dicMiniMaps.Add(centralPos, centralMiniMap.GetComponent<MiniMapManager>().miniMapInfo);

        Vector2 newMiniMapPos = new Vector2(newMiniMap.transform.position.x, newMiniMap.transform.position.z);
        // 생성한 랜덤맵
        dicMiniMaps.Add(newMiniMapPos, newMiniNode);
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
