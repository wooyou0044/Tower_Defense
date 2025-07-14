using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMiniMap : MonoBehaviour
{
    MiniMapManager parentMinimapMgr;
    Direction currentDirect;
    MinimapSpawnStruct miniMapStruct;

    Vector2 currentPos;
    bool isMouseOn;
    public bool IsMadeMinimap { get; private set; }
    public MinimapSpawnStruct ButtonSpawnStruct
    {
        get
        {
            return miniMapStruct;
        }
    }

    void Start()
    {
        parentMinimapMgr = transform.parent.GetComponent<MiniMapManager>();

        currentPos = new Vector2(transform.position.x, transform.position.z);

        miniMapStruct = new MinimapSpawnStruct
        {
            miniMapspawnPos = transform.position + Vector3.up,
            connectDireciton = currentDirect,
            buttonRef = this,
            spawnPos = currentPos
        };
    }

    private void Update()
    {
        if (isMouseOn == true)
        {
            if(Input.GetMouseButtonDown(1))
            {
                MapManager.Instance.RotateNewMinimap(currentDirect);
            }
            if(Input.GetMouseButtonDown(0))
            {
                if(MapManager.Instance.IsConnectedRoad(currentPos))
                {
                    PressCreateMinimap();
                    IsMadeMinimap = true;
                }
            }
        }
    }

    public void InitializeDirection(Direction direct)
    {
        currentDirect = direct;
    }

    public void PressCreateMinimap()
    {
        MapManager.Instance.RemoveCreateMapBtnList(this);
        MapManager.Instance.AddDictionaryNewMinimap();
    }

    public void OnMouseHoverEnter()
    {
        MapManager.Instance.MakeOrMoveRandomMinimap(miniMapStruct);
        if (MapManager.Instance.isAlreadyMinimapMade == true)
        {
            MapManager.Instance.SetActiveNewMinimap(true);
        }
        isMouseOn = true;
    }

    public void OnMouseHoverExit()
    {
        MapManager.Instance.SetActiveNewMinimap(false);
        isMouseOn = false;
    }
}
