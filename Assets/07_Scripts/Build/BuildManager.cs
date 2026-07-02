using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    [SerializeField] DefTower[] towerSO;
    [SerializeField] int initRndTowerNum;
    [SerializeField] Transform buildTowerTrans;
    [SerializeField] Material previewTowerMat;

    static BuildManager _instance;

    GameObject prevTower;
    int prevTowerID;

    Dictionary<int, GameObject> dicTowerObject;
    Camera cam;
    TowerController towerCtrl;
    Vector2 towerSize;

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
        //InitiateDefTower();
        dicTowerObject = new Dictionary<int, GameObject>();
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

    public void SetActiveTowerPrefab(DefTower tower)
    {
        if(prevTower != null)
        {
            prevTower.SetActive(false);
        }

        if (tower.towerID == prevTowerID)
        {
            prevTower = null;
            prevTowerID = 0;
            return;
        }

        int towerID = tower.towerID;
        // 기존에 안 만들어졌으면 Instantiate하고 만들어졌으면 SetActive(true)
        if(dicTowerObject.ContainsKey(towerID) == true)
        {
            dicTowerObject[towerID].SetActive(true);
        }
        else
        {
            GameObject makeTower = null;
            for(int i=0; i<towerSO.Length; i++)
            {
                if(towerSO[i].towerID == tower.towerID)
                {
                    makeTower = towerSO[i].defTowerObject;
                    break;
                }
            }
            GameObject towerObj = MakePrevTower(makeTower);
            dicTowerObject.Add(tower.towerID, towerObj);
            towerCtrl = towerObj.GetComponent<TowerController>();
            ChangePrevTowerMaterial(towerCtrl.GetMainTower);
        }
        //dicTowerObject[towerID].SetActive(true);
        prevTower = dicTowerObject[towerID];
        prevTowerID = towerID;
        towerCtrl = prevTower.GetComponent<TowerController>();
        towerSize = towerCtrl.GetSize;
    }

    void ChangePrevTowerMaterial(GameObject mainTower)
    {
        MeshRenderer rend = mainTower.GetComponent<MeshRenderer>();
        Material originMat = rend.material;
        Material newTowerMat = new Material(previewTowerMat);
        newTowerMat.SetTexture("_MainTexture", originMat.GetTexture("_BaseMap"));
        newTowerMat.SetFloat("_isBuildMinimap", 1f);
        newTowerMat.SetFloat("_Transparency", 0.8f);
        rend.material = newTowerMat;
    }

    void UpdateTowerPlace(GameObject towerObj)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        prevTower.transform.position = worldPos;
    }

    void GetTowerSizeTileType(Vector2 size)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int buildLayer = LayerMask.GetMask("Buildable");
        if(Physics.Raycast(ray, out hit, 300f, buildLayer))
        {
            Debug.Log("건설할 수 있는 곳!!");
            // 타워의 크기만큼 계산했을 때 건설할 수 있는 곳인지 확인하고 가능하면 타워 -> 초록색, 아니면 타워 -> 빨간색으로 표시, x표시 나오기
        }
        else
        {
            // 건설할 수 없는 곳이기 때문에 타워도 빨간색으로 하고 x표시 ui?나와야 함
        }
    }

    void RotatePrevTower()
    {
        if(prevTower == null)
        {
            return;
        }
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
