using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct MinimapSpawnContext
{
    public Vector3 spawnPos;
    public Direction connectDireciton;
    public bool isAlreadyMade;
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

    int newMaterialIndex;

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
        // 본부랑 게임 시작할때 생성할 랜덤 미니맵 Dictionary에 넣기
        dicMiniMaps = new Dictionary<Vector2, MinimapNode>();

        // 랜덤 미니맵 본부 밑에 하나 설치
        MakeMiniMapBelowRandomMap(centralMiniMap);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {

        }
    }

    void MakeMiniMapBelowRandomMap(GameObject miniMap)
    {
        // 본부 받아오기
        Vector2 minimapPos = new Vector2(miniMap.transform.position.x, miniMap.transform.position.z);
        Direction minimapRoadDir = miniMap.GetComponent<MiniMapManager>().miniMapInfo.RoadEdges.First();
        Direction connectRoadDir = OppositeDirection(minimapRoadDir);

        // 랜덤 미니맵 생성
        int random = Random.Range(0, miniMaps.Length);
        newMinimap = Instantiate(miniMaps[random], miniMapsParent);
        //GameObject newMiniMap = Instantiate(miniMaps[0]);
        MiniMapManager miniMapMgr = newMinimap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = miniMapMgr.miniMapInfo;

        // 90도씩 회전하면서 맞닿는 방향 나올때까지 반복
        int rotateCount = 0;
        while (rotateCount < (int)Direction.None)
        {
            if (miniMapMgr.miniMapInfo.RoadEdges.Contains(connectRoadDir))
            {
                break;
            }
            ExamineRotate(miniMapMgr);
            rotateCount++;
        }

        if (rotateCount == 4)
        {
            Debug.Log("붙일 수 있는 미니맵이 없음");
            //Destroy(newMiniMap);
            rotateCount = 0;
            return;
        }

        // 미니맵 설치
        Vector3 offset = GetOffsetDirection(minimapRoadDir, newMiniNode.GetSize());
        newMinimap.transform.position = miniMap.transform.position + offset;

        // Dictionary에 등록
        dicMiniMaps.Add(minimapPos, miniMap.GetComponent<MiniMapManager>().miniMapInfo);

        Vector2 newMiniMapPos = new Vector2(newMinimap.transform.position.x, newMinimap.transform.position.z);
        // 생성한 랜덤맵
        dicMiniMaps.Add(newMiniMapPos, newMiniNode);

        // 미니맵 버튼 생성
        MakeCreateMinimapButton(newMinimap, connectRoadDir);

        newMinimap = null;
    }

    public void ExamineRotate(MiniMapManager minimapMgr)
    {
        minimapMgr.RotateRoadEdgesClockWise();
        newMinimap.transform.Rotate(Vector3.up, 90);
    }

    public void MakeOrMoveRandomMinimap(MinimapSpawnContext ctx)
    {
        if (ctx.isAlreadyMade == true)
        {
            newMinimap.transform.position = ctx.spawnPos;
            return;
        }

        int random = Random.Range(0, miniMaps.Length);
        newMinimap = Instantiate(miniMaps[random], miniMapsParent);

        isAlreadyMinimapMade = true;

        MiniMapManager miniMapMgr = newMinimap.GetComponent<MiniMapManager>();
        MinimapNode newMiniNode = miniMapMgr.miniMapInfo;

        newMinimap.transform.position = ctx.spawnPos;

        // 기존 Material외 맵 설치할때 생길 수 있는 Material 추가
        for(int i=0; i<newMinimap.transform.childCount; i++)
        {
            GameObject childNode = newMinimap.transform.GetChild(i).gameObject;
            TileType childNodeType = childNode.GetComponent<TileNode>().type;
            if(childNodeType == TileType.Road)
            {
                continue;
            }
            MeshRenderer rend = childNode.GetComponent<MeshRenderer>();
            Material originMat = rend.material;

            Material newMat = new Material(miniMapMaterial);
            // Texture 넣기
            newMat.SetTexture("_MainTexture", originMat.GetTexture("_BaseMap"));
            newMat.SetFloat("_isBuildMinimap", 1f);
            // 현재 만들어진 새로운 미니맵의 방향과 + 버튼의 반대방향이 같으면 초록색, 다르면 빨간색으로 해야 함
            
            Material[] newMats = new Material[2] { originMat, newMat };
            rend.materials = newMats;
            newMaterialIndex = 1;
        }
        //dicMiniMaps.Add(pos, newMiniNode);
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
