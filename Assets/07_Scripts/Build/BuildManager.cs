using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    [SerializeField] DefTower[] towerSO;
    [SerializeField] UIManager uiMgr;
    [SerializeField] int initRndTowerNum;
    [SerializeField] Transform buildTowerTrans;
    [SerializeField] Material previewTowerMat;

    static BuildManager _instance;

    GameObject prevTower;
    int prevTowerID;

    Dictionary<int, GameObject> dicTowerObject;
    Camera cam;
    TowerController towerCtrl;
    Vector2Int towerSize;
    List<DefTower> initRndTowerList;
    TileNodeController hitTileNodeCtrl;

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
        cam = Camera.main;
    }

    void Start()
    {
        MakeInitRndTowerPack();
        InitiateDefTower();
        //prevTower = null;
    }

    void Update()
    {
        if(prevTower == null)
        {
            return;
        }
        UpdateTowerPlace(prevTower);

        if(Input.GetMouseButtonDown(1))
        {
            // 들고 있는 타워 회전
            RotatePrevTower();
        }
    }

    void InitiateDefTower()
    {
        dicTowerObject = new Dictionary<int, GameObject>();
        //for(int i=0; i<towerSO.Length; i++)
        //{
        //    GameObject prevTower = MakePrevTower(towerSO[i].defTowerObject);
        //    dicTowerObject.Add(towerSO[i].towerID, prevTower);
        //    towerCtrl = prevTower.GetComponent<TowerController>();
        //    towerCtrl.ChangeTowerMaterial(previewTowerMat);
        //    prevTower.SetActive(false);
        //}

        for (int i = 0; i < initRndTowerList.Count; i++)
        {
            GameObject prevTower = MakePrevTower(initRndTowerList[i].defTowerObject);
            dicTowerObject.Add(initRndTowerList[i].towerID, prevTower);
            towerCtrl = prevTower.GetComponent<TowerController>();
            towerCtrl.ChangeTowerMaterial(previewTowerMat);
            prevTower.SetActive(false);
        }

        uiMgr.MakeInitTowerBtn(initRndTowerList);
        uiMgr.SetBuildMapActive(false);
    }

    GameObject MakePrevTower(GameObject towerPrefab)
    {
        GameObject prevTowerObj = Instantiate(towerPrefab, buildTowerTrans);
        prevTowerObj.transform.position = Input.mousePosition;
        return prevTowerObj;
    }
    
    List<DefTower> MakeInitRndTowerPack()
    {
        initRndTowerList = new List<DefTower>();
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
        initRndTowerList = dupCheckDefTower.Values.ToList();
        return initRndTowerList;
    }

    //public void MakeTowerID(DefTower tower)
    //{
    //    tower.TowerID = ((int)tower.defType + 1) * 1000;
    //    tower.TowerID++;
    //}

    public void SetActiveTowerPrefab(DefTower tower)
    {
        if(tower == prevTower)
        {
            Debug.Log("prevTower 없음");
            return;
        }
        if(prevTower != null)
        {
            prevTower.SetActive(false);
        }

        if (tower.towerID == prevTowerID)
        {
            prevTower = null;
            prevTowerID = 0;
            towerCtrl = null;
            towerSize = Vector2Int.zero;
            return;
        }

        int towerID = tower.towerID;
        // 기존에 안 만들어졌으면 Instantiate하고 만들어졌으면 SetActive(true)
        if(dicTowerObject.ContainsKey(towerID) == true)
        {
            dicTowerObject[towerID].SetActive(true);
        }
        //dicTowerObject[towerID].SetActive(true);
        prevTower = dicTowerObject[towerID];
        prevTowerID = towerID;
        towerCtrl = prevTower.GetComponent<TowerController>();
        towerSize = towerCtrl.GetSize;
    }

    public void MakeNewTower(DefTower tower)
    {
        GameObject makeTower = null;
        for (int i = 0; i < towerSO.Length; i++)
        {
            if (towerSO[i].towerID == tower.towerID)
            {
                makeTower = towerSO[i].defTowerObject;
                break;
            }
        }

        GameObject towerObj = MakePrevTower(makeTower);
        dicTowerObject.Add(tower.towerID, towerObj);
        towerCtrl = towerObj.GetComponent<TowerController>();
        towerCtrl.ChangeTowerMaterial(previewTowerMat);
    }

    void UpdateTowerPlace(GameObject towerObj)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        prevTower.transform.position = worldPos;
        if(towerCtrl == null)
        {
            towerCtrl = towerObj.GetComponent<TowerController>();
        }
        GetTowerSizeTileType(towerCtrl.GetSize, mousePos);
    }

    void GetTowerSizeTileType(Vector2Int size, Vector3 mousePosition)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int buildLayer = LayerMask.GetMask("Buildable");
        if(Physics.Raycast(ray, out hit, 300f, buildLayer))
        {
            // 타워의 크기만큼 계산했을 때 건설할 수 있는 곳인지 확인하고 가능하면 타워 -> 초록색, 아니면 타워 -> 빨간색으로 표시, x표시 나오기
            hitTileNodeCtrl = hit.transform.GetComponent<TileNodeController>();
        }
        else
        {
            // 건설할 수 없는 곳이기 때문에 타워도 빨간색으로 하고 x표시 ui?나와야 함
            towerCtrl.ChangeMatCanBuild(false);
            uiMgr.SetCanNotBuildIconPos(mousePosition);
        }
    }

    void RotatePrevTower()
    {
        if(prevTower == null)
        {
            return;
        }
        towerCtrl.GetIsRotate = !towerCtrl.GetIsRotate;
        Debug.Log("towerSize : " + towerCtrl.GetSize);
        // 가지고 있는 타워 회전
        prevTower.transform.Rotate(Vector3.up, 90);
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
