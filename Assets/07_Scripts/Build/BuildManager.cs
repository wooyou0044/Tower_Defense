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
    int rotateCount;

    Dictionary<int, GameObject> dicTowerObject;
    Dictionary<Vector2Int, TileNode> dicGlobalTileNode;
    Camera cam;
    TowerController towerCtrl;
    List<DefTower> initRndTowerList;
    MiniMapManager hitMinimapMgr;

    Vector2Int hitGridIndex;
    Vector2Int anchorStartOffset;
    Vector2 anchorTileSizeOffset;
    //TileNode prevHitTileNode;
    int tileSize;

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

        dicGlobalTileNode = new Dictionary<Vector2Int, TileNode>();
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

        if (Input.GetMouseButtonDown(1))
        {
            rotateCount++;
            if (rotateCount > 3)
            {
                rotateCount = 0;
            }
            // 들고 있는 타워 회전
            RotatePrevTower();
            anchorStartOffset = towerCtrl.RotateAnchorOffset(rotateCount);
            anchorTileSizeOffset = towerCtrl.RotateAnchorTileSizeOffset(rotateCount);
        }

        //UpdateTowerPlace(prevTower);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        GetTowerSizeTileType(prevTower, mousePos);
        if (Input.GetMouseButtonDown(0))
        {

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
            towerCtrl.SetTowerInfo(initRndTowerList[i]);
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
            // 회전되어 있으면 원래대로 돌려놓기
            prevTower.transform.rotation = Quaternion.identity;
            prevTower.SetActive(false);
            uiMgr.SetCanNotBuildIconActive(false);
        }

        rotateCount = 0;

        if (tower.towerID == prevTowerID)
        {
            prevTower = null;
            prevTowerID = 0;
            towerCtrl = null;
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
        anchorStartOffset = towerCtrl.RotateAnchorOffset(rotateCount);
        anchorTileSizeOffset = towerCtrl.RotateAnchorTileSizeOffset(rotateCount);
        if(tileSize == 0)
        {
            tileSize = towerCtrl.GetTileSize;
        }
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

    void UpdateTowerPlace(Vector3 mousePosition)
    {
        //타워 위치 조정
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 anchorOffset = new Vector3(anchorTileSizeOffset.x, 0, anchorTileSizeOffset.y) * tileSize;
        Debug.Log("anchorOffset : " + anchorOffset);
        prevTower.transform.position = worldPos + anchorOffset;
    }

    void GetTowerSizeTileType(GameObject towerObj, Vector3 mousePosition)
    {
        if (towerCtrl == null)
        {
            towerCtrl = towerObj.GetComponent<TowerController>();
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int buildLayer = LayerMask.GetMask("Buildable");
        if(Physics.Raycast(ray, out hit, 300f, buildLayer))
        {
            // 타워의 크기만큼 계산했을 때 건설할 수 있는 곳인지 확인하고 가능하면 타워 -> 초록색, 아니면 타워 -> 빨간색으로 표시, x표시 나오기
            //hitTileNodeCtrl = hit.transform.GetComponent<TileNodeController>();
            hitMinimapMgr = hit.transform.GetComponentInParent<MiniMapManager>();
            hitGridIndex = hitMinimapMgr.WorldToGridIndex(hit.point);
            TileNode hitTileNode = hitMinimapMgr.miniMapInfo.TileNodes[hitGridIndex.y, hitGridIndex.x];
            //anchorStartOffset = towerCtrl.RotateAnchorOffset(rotateCount);
            if (hitTileNode.height > 0 && hitTileNode.installedTower == null)
            {
                //Debug.Log("맞은 타일의 globalIndex : " + prevHitTileNode.globalIndex);
                if (CheckHeightByTowerSize(hitTileNode))
                {
                    //Debug.Log("건설 가능");
                    towerCtrl.ChangeMatCanBuild(true);
                    uiMgr.SetCanNotBuildIconActive(false);
                }
                else
                {
                    towerCtrl.ChangeMatCanBuild(false);
                    uiMgr.SetCanNotBuildIconPos(mousePosition);
                    Debug.Log("건설 불가능");
                }
            }
        }
        else
        {
            // 건설할 수 없는 곳이기 때문에 타워도 빨간색으로 하고 x표시 ui?나와야 함
            towerCtrl.ChangeMatCanBuild(false);
            uiMgr.SetCanNotBuildIconPos(mousePosition);
        }

        UpdateTowerPlace(mousePosition);
    }

    bool CheckHeightByTowerSize(TileNode hitTileNode)
    {
        //Debug.Log("타워 사이즈 : " + towerCtrl.GetSize);
        //타워 사이즈에 맞춰 1. 타워 타입이 Buildable인지 체크 2. installedTower가 있는지 확인 3. 높이가 같은지 체크
        Vector2Int towerSize = towerCtrl.GetRotateTowerSize(rotateCount);
        float hitTileHeight = hitTileNode.height;

        Vector2Int startGlobalIdx = hitTileNode.globalIndex;
        List<Vector2Int> footprintList = towerCtrl.SetCheckIdxOffset(anchorStartOffset);
        foreach(Vector2Int offset in footprintList)
        {
            Vector2Int checkIdx = startGlobalIdx + offset;
            if(!dicGlobalTileNode.TryGetValue(checkIdx, out TileNode checkTileNode))
            {
                return false;
            }
            else
            {
                if(checkTileNode.type != TileType.Buildable)
                {
                    Debug.Log($"checkIndex[{checkIdx.x},{checkIdx.y}].type = {checkTileNode.type}이라 오류");
                    return false;
                }
                if(checkTileNode.installedTower != null)
                {
                    Debug.Log($"checkIndex[{checkIdx.x},{checkIdx.y}].installedTower = {checkTileNode.installedTower}이라 오류");
                    return false;
                }
                if(checkTileNode.height != hitTileHeight)
                {
                    Debug.Log($"checkIndex[{checkIdx.x},{checkIdx.y}].heihgt = {checkTileNode.height}이라 오류");
                    return false;
                }
            }
        }
        return true;
    }

    void RotatePrevTower()
    {
        if(prevTower == null)
        {
            return;
        }
        //Debug.Log("towerSize : " + towerCtrl.GetSize);
        // 가지고 있는 타워 회전
        prevTower.transform.Rotate(Vector3.up, 90);
    }

    public void SetDicGlobalIndexTileNode(Vector2Int globalIndex, TileNode node)
    {
        if(dicGlobalTileNode.ContainsKey(globalIndex) == false)
        {
            dicGlobalTileNode.Add(globalIndex, node);
        }
        dicGlobalTileNode[globalIndex] = node;
    }

    public void TempPrint()
    {
        foreach(KeyValuePair<Vector2Int, TileNode> kvp in dicGlobalTileNode)
        {
            Debug.Log($"GlboalIndex({kvp.Key}).type = {kvp.Value.type}");
            Debug.Log($"GlboalIndex({kvp.Key}).glboalIndex = {kvp.Value.globalIndex}");
            Debug.Log($"GlboalIndex({kvp.Key}).height = {kvp.Value.height}");
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
