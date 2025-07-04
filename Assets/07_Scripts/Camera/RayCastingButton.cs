using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastingButton : MonoBehaviour
{
    Camera mainCam;
    void Start()
    {
        mainCam = GetComponent<Camera>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                CreateMiniMap mapBtn = hit.transform.GetComponent<CreateMiniMap>();
                if(mapBtn != null)
                {
                    Debug.Log("´­¸²");
                    mapBtn.PressCreateMinimap();
                }
            }
        }
    }
}
