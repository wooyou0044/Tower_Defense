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
        Debug.Log("�����̽� ����");
        //������ �����̴� ���̸� Space�� ���� ��ư�� ����־�� ��
        //List<EnemyPathInfo> enemyPaths = MapManager.Instance.EnemyPathInfos;
        Dictionary<Vector3, EnemyPathInfo> dicEnemyPath = MapManager.Instance.EnemyPathInfos;
        List<EnemyPathInfo> enemyPaths = new List<EnemyPathInfo>(dicEnemyPath.Values);
        foreach(var path in enemyPaths)
        {
            for(int i=0; i< enemySpawnCount; i++)
            {
                if(i == 0)
                {
                    InstantiateEnemy(path, i);
                }
                else
                {
                    WaitAndMakeEnemyMove(path, i);
                }
            }
        }
    }

    void InstantiateEnemy(EnemyPathInfo path, int spawnIndex)
    {
        EnemyController enemyCtrl = enemyPool.GetObject();
        enemyCtrl.transform.position = path.spawnPos;
        // 70%�� ª�� ��� 30%�� �ٸ� �ĺ� ��� ����
        float ratio = (float) spawnIndex / enemySpawnCount;
        if(ratio > 0.2f && ratio < 0.8f)
        {
            Debug.Log("������ ��� ��������!");
            if(path.candidatePaths.Count > 0)
            {
                int randIdx = Random.Range(0, path.candidatePaths.Count);
                enemyCtrl.SetPath(path.candidatePaths[randIdx]);
            }
            else
            {
                Debug.Log("path.candatePaths.Count == 0");
            }
        }
        else
        {
            enemyCtrl.SetPath(path.shortPaths);
        }
    }


    public void ReturnEnemy(EnemyController enemyObjCtrl)
    {
        enemyPool.ReturnObject(enemyObjCtrl);
    }

    void WaitAndMakeEnemyMove(EnemyPathInfo pathInfo, int spawnIndex)
    {
        float appearTime = Random.Range(10f, maxWaitingAppearTime);
        StartCoroutine(InstantiateAfterWaiting(pathInfo, appearTime, spawnIndex));
    }

    IEnumerator InstantiateAfterWaiting(EnemyPathInfo path, float time, int index)
    {
        yield return new WaitForSeconds(time);
        InstantiateEnemy(path, index);
    }
}
