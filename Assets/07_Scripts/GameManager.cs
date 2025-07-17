using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int enemyPoolSize;
    [SerializeField] Transform enemyInstParent;
    [SerializeField] int enemySpawnCount;

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
        List<EnemyPathInfo> enemyPaths = MapManager.Instance.FindEnemyPath();
        foreach(var path in enemyPaths)
        {
            for(int i=0; i< enemySpawnCount; i++)
            {
                EnemyController enemyCtrl = enemyPool.GetObject();
                enemyCtrl.transform.position = path.spawnPos;
                enemyCtrl.SetPath(path.movePath);
            }
        }
    }

    public void ReturnEnemy(EnemyController enemyObjCtrl)
    {
        enemyPool.ReturnObject(enemyObjCtrl);
    }
}
