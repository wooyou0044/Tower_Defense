using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildBtnController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject chooseBtnOutline;
    [SerializeField] TextMeshProUGUI buildCoinTMP;
    [SerializeField] TextMeshProUGUI buildCountTMP;
    [SerializeField] Image towerImg;
    [SerializeField] Image interactableImg;

    DefTower myTower;
    GameObject towerPrefab;

    bool isBtnActive = false;


    public bool IsBtnActive
    {
        get => isBtnActive;
        set => isBtnActive = value;
    }

    public DefTower btnTower
    {
        get => myTower;
        set => myTower = value;
    }

    private void Awake()
    {
        isBtnActive = false;
    }

    void Start()
    {
        chooseBtnOutline.SetActive(false);
        if(btnTower != null)
        {
            towerPrefab = btnTower.defTowerObject;
        }
    }

    void Update()
    {
        
    }

    public void ChangeBtnInfo(int buildCoin, int buildCount, Sprite towerIcon)
    {
        buildCoinTMP.text = buildCoin.ToString();
        buildCountTMP.text = buildCount.ToString();
        towerImg.sprite = towerIcon;
    }

    public void OnClickBuildButton()
    {
        Debug.Log("나의 Tower 정보 : " + myTower.towerName);
        BuildManager.Instance.SetActiveTowerPrefab(myTower);
    }

    public void SetInteractImgActive(bool isActive)
    {
        interactableImg.gameObject.SetActive(isActive);
    }

    public void SetOffChooseBtnOutline()
    {
        if(chooseBtnOutline.activeSelf == true)
        {
            chooseBtnOutline.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isBtnActive == false)
        {
            return;
        }
        chooseBtnOutline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isBtnActive == false)
        {
            return;
        }
        chooseBtnOutline.SetActive(false);
    }
}
