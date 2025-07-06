using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMiniMap : MonoBehaviour
{
    MiniMapManager parentMinimapMgr;

    void Start()
    {
        parentMinimapMgr = transform.parent.GetComponent<MiniMapManager>();
    }

    void Update()
    {
        
    }

    public void PressCreateMinimap()
    {
        Vector3 miniMapPos = transform.position + new Vector3(0, 1, 0);
        Debug.Log("Position : " + miniMapPos);
        MapManager.Instance.MakeRandomMinimap(miniMapPos);

        parentMinimapMgr.RemoveCreateMapBtnList(this);
    }
}
