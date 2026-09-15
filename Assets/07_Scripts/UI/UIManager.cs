using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image rerollMapPanel;
    [SerializeField] Image sendWavePanel;
    [SerializeField] Image rewardPanel;
    [SerializeField] Image canNotBuildIcon;
    [SerializeField] Slider baseHPSlider;
    [SerializeField] TextMeshProUGUI baseHPTxt;
    [SerializeField] TextMeshProUGUI curWaveCountTxt;
    [SerializeField] TextMeshProUGUI totWaveCountTxt;
    [SerializeField] TextMeshProUGUI towerBuildCointTxt;
    [SerializeField] TextMeshProUGUI rerollMapCoinTxt;
    [Header("Build Button Area")]
    [SerializeField] Transform buildBtnGridArea;
    [SerializeField] GameObject buildBtnPrefab;
    [Header("HP Slider")]
    [SerializeField] Image fillAreaImg;
    [SerializeField] Color[] hpColor;

    Animator rerollMapAnimator;
    Animator sendWaveAnimator;

    Button rerollMapBtn;
    Button sendWaveBtn;

    int prevBuildCoin;

    bool isRerollBtnActive;

    List<BuildBtnController> towerBtnCtrlList;

    RectTransform iconRectTrans;

    private void Awake()
    {
        baseHPSlider = baseHPSlider.GetComponent<Slider>();
        rerollMapAnimator = rerollMapPanel.GetComponent<Animator>();
        sendWaveAnimator = sendWavePanel.GetComponent<Animator>();

        rerollMapBtn = rerollMapPanel.GetComponentInChildren<Button>();
        sendWaveBtn = sendWavePanel.GetComponentInChildren<Button>();

        iconRectTrans = canNotBuildIcon.GetComponent<RectTransform>();
    }

    void Start()
    {
        if(GameManager.Instance != null)
        {
            baseHPSlider.maxValue = GameManager.Instance.GetPlayerBaseHP;
            baseHPSlider.value = baseHPSlider.maxValue;
        }
        fillAreaImg.color = hpColor[0];

        isRerollBtnActive = true;

        // 임시로 시작할 때 빌드 버튼 안 생기게 만들기
        //SetBuildMapActive(false);
        canNotBuildIcon.gameObject.SetActive(false);

        // 이벤트 구독
        GameManager.Instance.OnChangeBaseHp += ChangeBaseHp;
        //GameManager.Instance.OnChangeWaveCount += ChangeWaveCount;
        //GameManager.Instance.OnChangeRerollCoin += ChangeRerollCoin;
    }

    void Update()
    {

    }

    IEnumerator WaitPanelAnimatorEnd(bool isActive)
    {
        yield return new WaitForSeconds(1f);
        rerollMapPanel.gameObject.SetActive(isActive);
        sendWavePanel.gameObject.SetActive(isActive);
    }

    IEnumerator WaitRerollBtnAnimatorOffEnd()
    {
        yield return new WaitForSeconds(1f);
        rerollMapPanel.gameObject.SetActive(false);
    }

    public void SetDeactiveRerollPanel()
    {
        rerollMapBtn.interactable = false;
        rerollMapAnimator.SetTrigger("Disappear");
        StartCoroutine(WaitRerollBtnAnimatorOffEnd());
        isRerollBtnActive = false;
    }

    public void SetActiveSendWave(bool isActive)
    {
        rerollMapBtn.interactable = isActive;
        sendWaveBtn.interactable = isActive;
        if (isActive == false)
        {
            rerollMapAnimator.SetTrigger("Disappear");
            if(isRerollBtnActive == true)
            {
                sendWaveAnimator.SetTrigger("Disappear");
                isRerollBtnActive = false;
            }
            StartCoroutine(WaitPanelAnimatorEnd(isActive));
        }
        else
        {
            rerollMapPanel.gameObject.SetActive(isActive);
            sendWavePanel.gameObject.SetActive(isActive);

            rerollMapAnimator.Play("Idle", -1, 0f);
            sendWaveAnimator.Play("Idle", -1, 0f);
        }
    }

    public void ChangeBaseHp(int baseHp, int currBaseHp)
    {
        baseHPTxt.text = currBaseHp + "/" + baseHp;
        baseHPSlider.value = currBaseHp;
        if(currBaseHp >= baseHp * 0.7f)
        {
            fillAreaImg.color = hpColor[0];
        }
        else if(currBaseHp >= baseHp * 0.3f)
        {
            fillAreaImg.color = hpColor[1];
        }
        else
        {
            fillAreaImg.color = hpColor[2];
        }
    }

    public void SetRerollMapBtn(bool isActive)
    {
        rerollMapBtn.interactable = isActive;
    }

    public void ChangeWaveCount(int waveCount, int curWaveCount)
    {
        curWaveCountTxt.text = curWaveCount.ToString();
        totWaveCountTxt.text = "/" + waveCount;
    }

    public void ChangeRerollCoin(int rerollCoin)
    {
        rerollMapCoinTxt.text = rerollCoin.ToString();
    }

    public void ChangeBuildTowerCoin(int buildCoin)
    {
        if(buildCoin <= 0)
        {
            buildCoin = 0;
        }
        if(prevBuildCoin != buildCoin)
        {
            prevBuildCoin = buildCoin;
        }
        towerBuildCointTxt.text = buildCoin.ToString();
    }

    public void MakeInitTowerBtn(List<DefTower> towerList)
    {
        towerBtnCtrlList = new List<BuildBtnController>();

        for (int i=0; i<towerList.Count; i++)
        {
            MakeTowerBtnPrefab(towerList[i]);
        }
    }

    public void MakeTowerBtnPrefab(DefTower tower)
    {
        GameObject towerBtn = Instantiate(buildBtnPrefab, buildBtnGridArea);
        BuildBtnController btnCtrl = towerBtn.GetComponent<BuildBtnController>();
        btnCtrl.btnTower = tower;
        btnCtrl.ChangeBtnInfo(tower.buildCoin, tower.towerCount, tower.defTowerImage);
        btnCtrl.SetInteractImgActive(false);
        towerBtnCtrlList.Add(btnCtrl);
    }

    public void SetBuildMapActive(bool isActive)
    {
        if(sendWavePanel.gameObject.activeSelf != isActive)
        {
            sendWavePanel.gameObject.SetActive(isActive);
        }
        InteractTowerButton(isActive);
    }

    public void InteractTowerButton(bool isActive)
    {
        for(int i=0; i<towerBtnCtrlList.Count; i++)
        {
            if(towerBtnCtrlList[i].btnTower.towerCount < 1)
            {
                continue;
            }
            if(towerBtnCtrlList[i].btnTower.buildCoin > prevBuildCoin)
            {
                ChangeTowerInfo(towerBtnCtrlList[i], false);
                continue;
            }
            ChangeTowerInfo(towerBtnCtrlList[i], isActive);
        }
    }

    public void SetRewardPanelActive(bool isActive)
    {
        rewardPanel.gameObject.SetActive(isActive);
    }

    public void SetCanNotBuildIconActive(bool isActive)
    {
        canNotBuildIcon.gameObject.SetActive(isActive);
    }

    public void SetCanNotBuildIconPos(Vector3 pos)
    {
        if(canNotBuildIcon.gameObject.activeSelf == false)
        {
            SetCanNotBuildIconActive(true);
        }
        canNotBuildIcon.transform.position = pos + new Vector3(70, 30, 0);
    }

    void ChangeTowerInfo(BuildBtnController buildBtnCtrl, bool isActive)
    {
        Button btn = buildBtnCtrl.GetComponent<Button>();
        if (isActive == false)
        {
            buildBtnCtrl.SetOffChooseBtnOutline();
        }
        buildBtnCtrl.IsBtnActive = isActive;
        buildBtnCtrl.SetInteractImgActive(!isActive);
        btn.interactable = isActive;
    }

    void ChangeRewardPanel()
    {

    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        GameManager.Instance.OnChangeBaseHp -= ChangeBaseHp;
        //GameManager.Instance.OnChangeWaveCount -= ChangeWaveCount;
        //GameManager.Instance.OnChangeRerollCoin -= ChangeRerollCoin;
    }
}
