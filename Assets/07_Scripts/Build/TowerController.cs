using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] int tileSize = 3;

    Collider footPrintCol;
    Vector2Int towerSize;
    bool isPrevMatChanged;
    MeshRenderer towerRend;
    Material originMat;
    Material newTowerMat;
    Vector3 centerPos;
    DefTower myTower;

    public DefTower MyTower { get => myTower; }
    public int GetTileSize { get => tileSize; }

    public bool GetPrevMatChanged
    {
        get => isPrevMatChanged;
        set => isPrevMatChanged = value;
    }

    private void Awake()
    {
        footPrintCol = GetComponent<Collider>();
        towerRend = transform.GetChild(0).GetComponent<MeshRenderer>();
        originMat = towerRend.material;
        towerSize = GetTowerSize(footPrintCol);
        Debug.Log("towerSize : " + towerSize);
        centerPos = footPrintCol.bounds.center;
    }
    void Start()
    {
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
        newTowerMat.SetFloat("_isRoadConnect", 1f);
        towerRend.material = newTowerMat;
    }

    public void ChangeMatCanBuild(bool isCanBuild)
    {
        float matValue = newTowerMat.GetFloat("_isRoadConnect");
        float typeCast = isCanBuild ? 1f : 0f;
        if(matValue == typeCast)
        {
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

    public void SetCheckIdxOffset(Vector2Int offset, List<Vector2Int> offsetList)
    {
        offsetList.Clear();

        int minX = Mathf.Min(0, offset.x);
        int maxX = Mathf.Max(0, offset.x);

        int minY = Mathf.Min(0, offset.y);
        int maxY = Mathf.Max(0, offset.y);

        for(int y = minY; y<=maxY; y++)
        {
            for(int x= minX; x<=maxX; x++)
            {
                offsetList.Add(new Vector2Int(x, y));
            }
        }
    }

    public Vector2Int GetRotateTowerSize(int rotateNum)
    {
        if(rotateNum == 1 || rotateNum == 3)
        {
            return new Vector2Int(towerSize.y, towerSize.x);
        }
        return towerSize;
    }

    public Vector2Int RotateAnchorOffset(int rotateNum)
    {
        Vector2Int offset = myTower.anchorOffset;
        switch(rotateNum)
        {
            case 1:
                return new Vector2Int(offset.y, -offset.x);
            case 2:
                return new Vector2Int(-offset.x, -offset.y);
            case 3:
                return new Vector2Int(-offset.y, offset.x);
            default:
                return offset;
        }
    }

    public Vector2 RotateAnchorTileSizeOffset(int rotateNum)
    {
        Vector2Int rotateTowerSize = GetRotateTowerSize(rotateNum);
        Vector2 offset = new Vector2(myTower.anchorOffset.x + (rotateTowerSize.x - 1) * 0.5f, myTower.anchorOffset.y + (rotateTowerSize.y - 1)*0.5f);
        //Debug.Log($"RotateNum = {rotateNum}, Anchor = {myTower.anchorOffset}, Size = {rotateTowerSize}, offset = {offset}");
        switch (rotateNum)
        {
            case 1:
                return new Vector2(offset.y, -offset.x);
            case 2:
                return new Vector2(-offset.x, -offset.y);
            case 3:
                return new Vector2(-offset.y, offset.x);
            default:
                return offset;
        }
    }

    public Vector3 GetRotateCenterPos(int rotateNum)
    {
        switch(rotateNum)
        {
            case 1:
                return new Vector3(centerPos.z, 0, -centerPos.x);
            case 2:
                return new Vector3(-centerPos.x, 0, -centerPos.z);
            case 3:
                return new Vector3(-centerPos.z, 0, centerPos.x);
            default:
                return centerPos;
        }
    }

    public void SetTowerInfo(DefTower tower)
    {
        myTower = tower;
    }
}
