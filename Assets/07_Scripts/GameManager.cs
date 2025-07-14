using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int enemyPoolSize;
    [SerializeField] Transform enemyInstParent;

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
        //적들이 움직이는 중이면 Space도 막고 버튼도 집어넣어야 함
        MapManager.Instance.FindEnemyPath();
    }

    public void ReturnEnemy(EnemyController enemyObjCtrl)
    {
        enemyPool.ReturnObject(enemyObjCtrl);
    }
}
