using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastingButton : MonoBehaviour
{
    Camera mainCam;
    CreateMiniMap currentHover = null;

    void Start()
    {
        mainCam = GetComponent<Camera>();
    }

    void Update()
    {
        HoverCreateMinimapButton();
    }

    void HoverCreateMinimapButton()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int minimapLayerMask = LayerMask.GetMask("MinimapButton");
        Debug.DrawRay(ray.origin, ray.direction * 300f, Color.red);
        if (Physics.Raycast(ray, out hit, 300f, minimapLayerMask))
        {
            CreateMiniMap mapBtn = hit.transform.GetComponent<CreateMiniMap>();
            if (mapBtn != null)
            {
                //mapBtn.PressCreateMinimap();
                if(currentHover != mapBtn)
                {
                    if(currentHover != null)
                    {
                        currentHover.OnMouseHoverExit();
                        SetActiveCreateMinimapButton(true, currentHover.gameObject);
                    }
                    currentHover = mapBtn;
                    SetActiveCreateMinimapButton(false, currentHover.gameObject);
                    currentHover.OnMouseHoverEnter();
                }
            }
            //else if(currentHover != null)
            //{
            //    currentHover.OnMouseHoverExit();
            //    currentHover = null;
            //}    
        }
        else if (currentHover != null && currentHover.IsMadeMinimap == false)
        {
            currentHover.OnMouseHoverExit();
            SetActiveCreateMinimapButton(true, currentHover.gameObject);
            currentHover = null;
        }
    }

    void SetActiveCreateMinimapButton(bool isActive, GameObject button)
    {
        GameObject child = button.transform.GetChild(0).gameObject;
        child.SetActive(isActive);
    }
}
