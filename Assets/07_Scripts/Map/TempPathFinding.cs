using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPathFinding : MonoBehaviour
{
    [SerializeField] GameObject centralMinimap;
    [SerializeField] MiniMapManager minimapMgr;
    [SerializeField] GameObject enemy;

    Dictionary<Vector2, MinimapNode> dicMiniMaps;
    PathFinding path;
    List<Vector3> pathList;

    GameObject enemyObj;
    Vector3 enemyCurrentPos;

    int indexSize;

    private void Start()
    {
        dicMiniMaps = new Dictionary<Vector2, MinimapNode>();

        Vector2 centralMinimapPos = new Vector2(centralMinimap.transform.position.x, centralMinimap.transform.position.z);
        dicMiniMaps.Add(centralMinimapPos, centralMinimap.GetComponent<MiniMapManager>().miniMapInfo);

        Vector2 newMinimapPos = new Vector2(minimapMgr.transform.position.x, minimapMgr.transform.position.z);
        dicMiniMaps.Add(newMinimapPos, minimapMgr.miniMapInfo);

        //for(int i=0; i<dicMiniMaps.Count; i++)
        //{
        //    pathList = new List<Vector3>();
        //    path = new PathFinding(dicMiniMaps[]);
        //    pathList = path.FindPath();
        //}
        pathList = new List<Vector3>();
        path = new PathFinding(minimapMgr.miniMapInfo.TileNodes);
        pathList = path.FindPath();

        //MoveEnemy();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            MoveEnemy();
        }
        if(Input.GetMouseButtonDown(1))
        {
            ExamineRotate(minimapMgr);
            path = new PathFinding(minimapMgr.miniMapInfo.TileNodes);
            pathList = path.FindPath();
        }
    }

    void MoveEnemy()
    {
        // 적 생성 -> 길이 뚫려있는 곳에서 나오게 설정해야 함
        enemyObj = Instantiate(enemy, pathList[0], Quaternion.identity);
        StartCoroutine(EnemyMoving());

        enemyCurrentPos = enemyObj.transform.position;

    }

    IEnumerator EnemyMoving()
    {
        int index = 1;
        while(index < pathList.Count)
        {
            yield return new WaitForSeconds(2f);
            enemyObj.transform.position = pathList[index];
            Debug.Log("움직여야 하는 위치 : " + pathList[index]);
            index++;
        }
    }

    void EnemyMoving(Vector3 targetPos)
    {

    }

    public void ExamineRotate(MiniMapManager mapMgr)
    {
        Debug.Log("Rotate 함수 호출됨");
        mapMgr.RotateRoadEdgesClockWise();
        mapMgr.gameObject.transform.Rotate(Vector3.up, 90);

        indexSize = mapMgr.miniMapInfo.TileNodes.GetLength(0);
        for (int i = 0; i < indexSize; i++)
        {
            for (int j = 0; j < indexSize; j++)
            {
                float posY = mapMgr.miniMapInfo.TileNodes[j, i].worldPosition.y;
                Vector3 worldPos = mapMgr.SetTileNodeWorldPosition(i, j, mapMgr.transform, posY);
                mapMgr.miniMapInfo.TileNodes[j, i].worldPosition = worldPos;
            }
        }

        Debug.Log("미니맵의 중간 지점 : " + mapMgr.miniMapInfo.GetWorldPositionCenter());
    }
}
