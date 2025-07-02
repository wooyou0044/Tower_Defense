using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    Dictionary<Vector2Int, MinimapNode> dicMiniMaps;

    private void Awake()
    {
        
    }

    void Start()
    {
        dicMiniMaps = new Dictionary<Vector2Int, MinimapNode>();
    }

    void Update()
    {
        
    }

    void MakeCentralBelowRandomMap()
    {

    }
}
