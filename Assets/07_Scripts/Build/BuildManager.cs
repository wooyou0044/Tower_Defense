using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    [SerializeField] DefTower[] towerSO;
    [SerializeField] int initRndTowerNum;
    [SerializeField] Transform buildTowerTrans;

    static BuildManager _instance;

    GameObject prevTower;

    Dictionary<int, GameObject> dicTowerObject;

    static public BuildManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        InitiateDefTower();
    }

    void Update()
    {
        
    }

    void InitiateDefTower()
    {
        dicTowerObject = new Dictionary<int, GameObject>();
        for(int i=0; i<towerSO.Length; i++)
        {
            GameObject prevTower = MakePrevTower(towerSO[i].defTowerObject);
            dicTowerObject.Add(towerSO[i].towerID, prevTower);
            prevTower.SetActive(false);
        }
    }

    public GameObject MakePrevTower(GameObject towerPrefab)
    {
        GameObject prevTowerObj = Instantiate(towerPrefab, buildTowerTrans);
        prevTowerObj.transform.position = Input.mousePosition;
        return prevTowerObj;
    }
    
    public List<DefTower> MakeInitRndTowerPack()
    {
        List<DefTower> defTowerList = new List<DefTower>();
        Dictionary<int, DefTower> dupCheckDefTower = new Dictionary<int, DefTower>();
        for(int i=0; i<initRndTowerNum; i++)
        {
            int rndNum = Random.Range(0, towerSO.Length);
            if(dupCheckDefTower.ContainsKey(rndNum))
            {
                dupCheckDefTower[rndNum].towerCount++;
            }
            else
            {
                dupCheckDefTower.Add(rndNum, towerSO[rndNum]);
                dupCheckDefTower[rndNum].towerCount++;
                //MakeTowerID(towerSO[rndNum]);
            }
        }
        //defTowerList = new List<DefTower>(dupCheckDefTower.Values);
        defTowerList = dupCheckDefTower.Values.ToList();
        return defTowerList;
    }

    //public void MakeTowerID(DefTower tower)
    //{
    //    tower.TowerID = ((int)tower.defType + 1) * 1000;
    //    tower.TowerID++;
    //}

    public void MakeTowerPrefab(GameObject tower)
    {
        if(prevTower != null && tower != prevTower)
        {

        }
    }

    // 임시로 꺼졌을 때 DefTower에 있는 Count 값이 초기화되게 만들기
    private void OnDisable()
    {
        for(int i=0; i<towerSO.Length; i++)
        {
            towerSO[i].towerCount = 0;
        }
    }
}
