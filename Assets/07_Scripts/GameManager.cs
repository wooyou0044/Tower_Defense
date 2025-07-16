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
        Debug.Log("스페이스 눌림");
        //적들이 움직이는 중이면 Space도 막고 버튼도 집어넣어야 함
        Debug.Log("미니맵 버튼 개수 : " + MapManager.Instance.minimapButtonList.Count);
        foreach(var button in MapManager.Instance.minimapButtonList)
        {
            Debug.Log("버튼");
            List<Vector3> path = MapManager.Instance.FindEnemyPath(button);
            for(int i=0; i< enemySpawnCount; i++)
            {
                EnemyController enemyCtrl = enemyPool.GetObject();
                enemyCtrl.SetPath(path);
            }
        }
    }

    public void ReturnEnemy(EnemyController enemyObjCtrl)
    {
        enemyPool.ReturnObject(enemyObjCtrl);
    }
}
