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

    DefTower myTower;
    GameObject towerPrefab;

    public DefTower btnTower
    {
        get => myTower;
        set => myTower = value;
    }

    private void Awake()
    {
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
        BuildManager.Instance.MakeTowerPrefab(towerPrefab);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        chooseBtnOutline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chooseBtnOutline.SetActive(false);
    }
}
