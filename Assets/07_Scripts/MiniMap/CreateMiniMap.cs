using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMiniMap : MonoBehaviour
{
    MiniMapManager parentMinimapMgr;
    Direction currentDirect;

    void Start()
    {
        parentMinimapMgr = transform.parent.GetComponent<MiniMapManager>();
    }

    void Update()
    {
        
    }

    public void InitializeDirection(Direction direct)
    {
        currentDirect = direct;
    }

    public void PressCreateMinimap()
    {
        Vector3 miniMapPos = transform.position + new Vector3(0, 1, 0);
        Debug.Log("Position : " + miniMapPos);
        //MapManager.Instance.MakeRandomMinimap(miniMapPos);

        parentMinimapMgr.RemoveCreateMapBtnList(this);
    }

    public void OnMouseHoverEnter()
    {
        MinimapSpawnContext ctx = new MinimapSpawnContext
        {
            spawnPos = transform.position + Vector3.up,
            connectDireciton = currentDirect,
            isAlreadyMade = MapManager.Instance.isAlreadyMinimapMade,
            buttonRef = this
        };
        //Vector3 miniMapPos = transform.position + new Vector3(0, 1, 0);
        //Debug.Log("Position : " + miniMapPos);
        //bool isAlreadyMade = MapManager.Instance.isAlreadyMinimapMade;
        MapManager.Instance.MakeOrMoveRandomMinimap(ctx);
        if (ctx.isAlreadyMade == true)
        {
            MapManager.Instance.SetActiveNewMinimap(true);
        }

        // 회전시키기
    }

    public void OnMouseHoverExit()
    {
        MapManager.Instance.SetActiveNewMinimap(false);
    }
}
