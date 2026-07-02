using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] int tileSize = 3;

    Collider footPrintCol;
    Vector2 towerSize;
    bool isPrevMatChanged;

    public Vector2 GetSize
    {
        get => towerSize;
    }
    public bool GetPrevMatChanged
    {
        get => isPrevMatChanged;
        set => isPrevMatChanged = value;
    }

    public GameObject GetMainTower
    {
        get => transform.GetChild(0).gameObject;
    }

    private void Awake()
    {
        footPrintCol = GetComponent<Collider>();
    }
    void Start()
    {
        towerSize = GetTowerSize(footPrintCol);
        Debug.Log("타워 사이즈 : ( " + towerSize.x + " , " + towerSize.y + " )");
        isPrevMatChanged = false;
    }

    void Update()
    {
        
    }

    Vector2 GetTowerSize(Collider col)
    {
        Vector2 towerNodeSize;
        Vector3 size = col.bounds.size;
        int width = Mathf.RoundToInt(size.x / tileSize);
        int height = Mathf.RoundToInt(size.z / tileSize);
        towerNodeSize = new Vector2(width, height);
        return towerNodeSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        TileNode tileNode = other.GetComponent<TileNode>();
        // 임시로 TileType 알아보기 위해
        if(tileNode == null)
        {
            return;
        }
        else
        {
            Debug.Log("부딪힌 타일 노드 타입 : " + tileNode.type);
        }
    }
}
