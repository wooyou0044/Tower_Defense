using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] int tileSize = 3;

    Collider footPrintCol;
    Vector2Int towerSize;
    bool isPrevMatChanged;
    bool isRotate;
    MeshRenderer towerRend;
    Material originMat;
    Material newTowerMat;
    DefTower myTower;

    public DefTower MyTower { get => myTower; }

    public Vector2Int GetSize
    {
        get
        {
            if(isRotate == false)
            {
                return towerSize;
            }
            else
            {
                towerSize = new Vector2Int(towerSize.y, towerSize.x);
                return towerSize;
            }
        }
    }
    public bool GetPrevMatChanged
    {
        get => isPrevMatChanged;
        set => isPrevMatChanged = value;
    }
    public bool GetIsRotate
    {
        get => isRotate;
        set => isRotate = value;
    }

    private void Awake()
    {
        footPrintCol = GetComponent<Collider>();
        towerRend = transform.GetChild(0).GetComponent<MeshRenderer>();
        originMat = towerRend.material;
    }
    void Start()
    {
        towerSize = GetTowerSize(footPrintCol);
        isPrevMatChanged = false;
    }

    void Update()
    {
        
    }

    Vector2Int GetTowerSize(Collider col)
    {
        Vector2Int towerNodeSize;
        Vector3 size = col.bounds.size;
        int width = Mathf.RoundToInt(size.x / tileSize);
        int height = Mathf.RoundToInt(size.z / tileSize);
        towerNodeSize = new Vector2Int(width, height);
        Debug.Log("처음 만들어진 타워 사이즈 : " + towerNodeSize);
        return towerNodeSize;
    }

    public void ChangeTowerMaterial(Material newMat)
    {
        newTowerMat = new Material(newMat);
        newTowerMat.SetTexture("_MainTexture", originMat.GetTexture("_BaseMap"));
        newTowerMat.SetFloat("_isBuildMinimap", 1f);
        newTowerMat.SetFloat("_Transparency", 0.8f);
        towerRend.material = newTowerMat;
    }

    public void ChangeMatCanBuild(bool isCanBuild)
    {
        float matValue = newTowerMat.GetFloat("_isRoadConnect");
        float typeCast = isCanBuild ? 1f : 0f;
        if(matValue == typeCast)
        {
            Debug.Log("같으니까 돌아가!");
            return;
        }
        if (isCanBuild == true)
        {
            newTowerMat.SetFloat("_isRoadConnect", 1f);
        }
        else
        {
            newTowerMat.SetFloat("_isRoadConnect", 0f);
        }
    }

    public void SetTowerInfo(DefTower tower)
    {
        myTower = tower;
    }
}
