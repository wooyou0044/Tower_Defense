using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPathFinding : MonoBehaviour
{
    [SerializeField] MiniMapManager minimapMgr;
    [SerializeField] GameObject enemy;

    PathFinding path;
    List<Vector3> pathList;

    GameObject enemyObj;

    private void Start()
    {
        pathList = new List<Vector3>();
        path = new PathFinding(minimapMgr.miniMapInfo.TileNodes);
        pathList = path.FindPath();

        MoveEnemy();
    }

    void MoveEnemy()
    {
        enemyObj = Instantiate(enemy);
        StartCoroutine(EnemyMoving());
    }

    IEnumerator EnemyMoving()
    {
        int index = 0;
        while(index < pathList.Count)
        {
            yield return new WaitForSeconds(2f);
            enemyObj.transform.position = pathList[index];
            Debug.Log("움직여야 하는 위치 : " + pathList[index]);
            index++;
        }
    }
}
