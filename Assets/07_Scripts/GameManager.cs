using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public struct GameInfoStruct
{
    public int enemyCount;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int enemyPoolSize;
    [SerializeField] Transform enemyInstParent;
    [SerializeField] int enemySpawnCount;
    [SerializeField] int rerollMapCoin;
    [SerializeField] int waveCount;
    [Range(10f, 30f)]
    [SerializeField] float maxWaitingAppearTime;
    [SerializeField] GameObject uiCanvas;

    ObjectPool<EnemyController> enemyPool;

    static GameManager _instance;

    PlayerState getPlayerStat;
    UIManager uiMgr;

    int curEnemyCount = 0;
    int enemyPathCount;
    int fullBaseHP;
    int curWaveCount;

    // 이벤트 발생
    public event Action<int, int> OnChangeBaseHp;
    public event Action<int, int> OnChangeWaveCount;
    public event Action<int> OnChangeRerollCoin;

    public int GetPlayerBaseHP
    {
        get
        {
            return getPlayerStat._baseHP;
        }
    }

    public int GetPlayerMapRerollCoin
    {
        get
        {
            return getPlayerStat._mapRerollCoin;
        }
    }

    public static GameManager Instance
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

        uiMgr = uiCanvas.GetComponent<UIManager>();

        getPlayerStat = new PlayerState(1);
        fullBaseHP = getPlayerStat._baseHP;
    }

    void Start()
    {
        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        enemyPool = new ObjectPool<EnemyController>(enemyCtrl, enemyPoolSize, enemyInstParent);

        uiMgr.ChangeBaseHp(fullBaseHP, getPlayerStat._baseHP);
        curWaveCount = 1;
        uiMgr.ChangeWaveCount(waveCount, curWaveCount);
        uiMgr.ChangeRerollCoin(getPlayerStat._mapRerollCoin);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SendWave();
        }

        if(enemyPathCount > 0 && curEnemyCount>=(enemySpawnCount * enemyPathCount))
        {
            Debug.Log("여기서 완전히 한 웨이브가 끝남");
            uiMgr.SetActiveSendWave(true);
            uiMgr.SetBuildMapActive(false);
            // 원래 한 웨이브 끝나고 아이템 선택하면 waveCount++ 해야 하는데 임의로
            curWaveCount++;
            OnChangeWaveCount.Invoke(waveCount, curWaveCount);
            curEnemyCount = 0;

            // 한 웨이브가 끝나고 맵에 +버튼 생성
            MapManager.Instance.SetActiveAllCreateMinimapBtn(true);
        }
    }

    public void SendWave()
    {
        Debug.Log("스페이스 눌림");
        //적들이 움직이는 중이면 Space도 막고 버튼도 집어넣어야 함
        MapManager.Instance.SetActiveAllCreateMinimapBtn(false);
        uiMgr.SetActiveSendWave(false);
        uiMgr.SetBuildMapActive(false);
        //List<EnemyPathInfo> enemyPaths = MapManager.Instance.EnemyPathInfos;
        Dictionary<Vector3, EnemyPathInfo> dicEnemyPath = MapManager.Instance.EnemyPathInfos;
        List<EnemyPathInfo> enemyPaths = new List<EnemyPathInfo>(dicEnemyPath.Values);
        enemyPathCount = enemyPaths.Count;
        Debug.Log("enemyPathCount 개수 : " + enemyPathCount);
        foreach (var path in enemyPaths)
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

        Debug.Log("웨이브 끝남");
    }

    void InstantiateEnemy(EnemyPathInfo path, int spawnIndex)
    {
        EnemyController enemyCtrl = enemyPool.GetObject();
        enemyCtrl.transform.position = path.spawnPos;
        // 70%는 짧은 길로 30%는 다른 후보 길로 가기
        float ratio = (float) spawnIndex / enemySpawnCount;
        if(ratio > 0.2f && ratio < 0.8f)
        {
            Debug.Log("랜덤한 길로 배정했음!");
            if(path.candidatePaths.Count > 0)
            {
                int randIdx = UnityEngine.Random.Range(0, path.candidatePaths.Count);
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
        // 현재 웨이브에서 맵에 남아있는 적의 수를 세기 위해 풀에 돌아오는 적의 수 세놓기
        curEnemyCount++;
    }

    void WaitAndMakeEnemyMove(EnemyPathInfo pathInfo, int spawnIndex)
    {
        float appearTime = UnityEngine.Random.Range(10f, maxWaitingAppearTime);
        StartCoroutine(InstantiateAfterWaiting(pathInfo, appearTime, spawnIndex));
    }

    IEnumerator InstantiateAfterWaiting(EnemyPathInfo path, float time, int index)
    {
        yield return new WaitForSeconds(time);
        InstantiateEnemy(path, index);
    }

    public void EnemyAttackCentral(int enemyAtt)
    {
        // baseHP 깎기
        if(getPlayerStat._baseHP <= 0)
        {
            return;
        }
        getPlayerStat._baseHP -= enemyAtt;
        OnChangeBaseHp.Invoke(fullBaseHP, getPlayerStat._baseHP);
    }

    public int UseRerollMap()
    {
        // 25씩 사용
        getPlayerStat._mapRerollCoin -= rerollMapCoin;
        return getPlayerStat._mapRerollCoin;
    }
}
