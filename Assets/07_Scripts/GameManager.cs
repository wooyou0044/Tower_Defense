using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int enemyPoolSize;
    [SerializeField] Transform enemyInstParent;
    [SerializeField] int enemySpawnCount;
    [Range(10f, 30f)]
    [SerializeField] float maxWaitingAppearTime;

    ObjectPool<EnemyController> enemyPool;

    static GameManager _instance;

    public static GameManager Instance
    {
        get 
        {
            return _instance; 
        }
    }

    void Start()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        enemyPool = new ObjectPool<EnemyController>(enemyCtrl, enemyPoolSize, enemyInstParent);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SendWave();
        }
    }

    public void SendWave()
    {
        Debug.Log("스페이스 눌림");
        //적들이 움직이는 중이면 Space도 막고 버튼도 집어넣어야 함
        //List<EnemyPathInfo> enemyPaths = MapManager.Instance.EnemyPathInfos;
        Dictionary<Vector3, EnemyPathInfo> dicEnemyPath = MapManager.Instance.EnemyPathInfos;
        List<EnemyPathInfo> enemyPaths = new List<EnemyPathInfo>();
        foreach(var enemyPath in dicEnemyPath.Values)
        {
            enemyPaths.Add(enemyPath);
        }
        foreach(var path in enemyPaths)
        {
            for(int i=0; i< enemySpawnCount; i++)
            {
                if(i == 0)
                {
                    InstantiateEnemy(path);
                }
                else
                {
                    WaitAndMakeEnemyMove(path);
                }
            }
        }
    }

    void InstantiateEnemy(EnemyPathInfo path)
    {
        EnemyController enemyCtrl = enemyPool.GetObject();
        enemyCtrl.transform.position = path.spawnPos;
        enemyCtrl.SetPath(path.movePath);
    }

    public void ReturnEnemy(EnemyController enemyObjCtrl)
    {
        enemyPool.ReturnObject(enemyObjCtrl);
    }

    void WaitAndMakeEnemyMove(EnemyPathInfo pathInfo)
    {
        float appearTime = Random.Range(10f, maxWaitingAppearTime);
        StartCoroutine(InstantiateAfterWaiting(pathInfo, appearTime));
    }

    IEnumerator InstantiateAfterWaiting(EnemyPathInfo path, float time)
    {
        yield return new WaitForSeconds(time);
        InstantiateEnemy(path);
    }
}
