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

    public eRangeState GetCurState
    {
        get => curState;
    }

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

    void ChangeAttRangeMat(bool isUpgrade, int upgradeLvl)
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
        // eRangeState에 따라 설정이 달라야 함
        switch(state)
        {
            case eRangeState.None:
                // Material이랑 숫자 변경
                ChangeAttRangeMat(false, 0);
                gameObject.SetActive(false);
                return;
            case eRangeState.BaseRange:
                ChangeAttRangeMat(false, 0);
                return;
            case eRangeState.Upgrade1:
                ChangeAttRangeMat(true, 1);
                return;
            case eRangeState.Upgrade2:
                ChangeAttRangeMat(true, 2);
                return;
        }
    }
}
