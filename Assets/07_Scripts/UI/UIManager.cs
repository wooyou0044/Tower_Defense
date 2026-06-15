using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image playerInfoPanel;
    [SerializeField] Image rerollMapPanel;
    [SerializeField] Image sendWavePanel;
    [SerializeField] Slider baseHPSlider;
    [SerializeField] TextMeshProUGUI baseHPTxt;
    [SerializeField] TextMeshProUGUI curWaveCountTxt;
    [SerializeField] TextMeshProUGUI totWaveCountTxt;
    [Header("HP Slider")]
    [SerializeField] Image fillAreaImg;
    [SerializeField] Color[] hpColor;

    PlayerState getPlayerStat;
    Animator rerollMapAnimator;
    Animator sendWaveAnimator;

    Button rerollMapBtn;
    Button sendWaveBtn;

    private void Awake()
    {
        playerInfoPanel = playerInfoPanel.GetComponent<Image>();
        baseHPSlider = baseHPSlider.GetComponent<Slider>();
        rerollMapAnimator = rerollMapPanel.GetComponent<Animator>();
        sendWaveAnimator = sendWavePanel.GetComponent<Animator>();

        rerollMapBtn = rerollMapPanel.GetComponentInChildren<Button>();
        sendWaveBtn = sendWavePanel.GetComponentInChildren<Button>();
    }

    void Start()
    {
        Debug.Log(rerollMapPanel.gameObject);
        if(GameManager.Instance != null)
        {
            baseHPSlider.maxValue = GameManager.Instance.GetPlayerBaseHP;
            baseHPSlider.value = baseHPSlider.maxValue;
        }
        fillAreaImg.color = hpColor[0];

        // 이벤트 구독
        GameManager.Instance.OnChangeBaseHp += ChangeBaseHp;
        GameManager.Instance.OnChangeWaveCount += ChangeWaveCount;
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

    public void SetActiveSendWave(bool isActive)
    {
        rerollMapBtn.interactable = isActive;
        sendWaveBtn.interactable = isActive;
        if (isActive == false)
        {
            rerollMapAnimator.SetTrigger("Disappear");
            sendWaveAnimator.SetTrigger("Disappear");
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

    void OnDisable()
    {
        // 이벤트 구독 해제
        GameManager.Instance.OnChangeBaseHp -= ChangeBaseHp;
        GameManager.Instance.OnChangeWaveCount -= ChangeWaveCount;
    }
}
