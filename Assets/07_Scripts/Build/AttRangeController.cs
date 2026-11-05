using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttRangeController : MonoBehaviour
{
    public enum eRangeState
    {
        None,
        BaseRange,
        Upgrade1,
        Upgrade2
    }
    eRangeState curState;
    Renderer rend;
    Material mat;

    private void Awake()
    {
        mat = new Material(Resources.Load<Material>("AttRangeMat"));
        rend = transform.GetChild(0).GetComponent<Renderer>();
    }

    void Start()
    {
        mat.SetFloat("_IsUpgrade", 0f);
        mat.SetFloat("_UpgradeLevel", 0f);
        rend.material = mat;
        curState = eRangeState.None;
    }

    void Update()
    {
        
    }

    public void ChangeAttRangeMat(bool isUpgrade, int upgradeLvl)
    {
        float boolUp = (isUpgrade == true) ? 1f : 0f;
        mat.SetFloat("_IsUpgrade", boolUp);
        mat.SetFloat("_UpgradeLevel", upgradeLvl);
    }

    public void ChangeRangeState(eRangeState state)
    {
        if(curState == state)
        {
            return;
        }
        curState = state;
    }
}
