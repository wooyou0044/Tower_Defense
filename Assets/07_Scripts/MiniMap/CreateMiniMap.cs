using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMiniMap : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PressCreateMinimap()
    {
        MapManager.Instance.MakeMiniMapBelowRandomMap(transform.parent.gameObject);
    }
}
