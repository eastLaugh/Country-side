using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static MapObjects;
using static Slot;

public class BuildingBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Slot.MapObject BuildingData;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject detailPanel;
    private float TimeCache;
    private int currentPhase;
    private float currentMoney;
    private string warningText;
    public bool enter;
    public void Initialize(Slot.MapObject data,int currentPhase,float currentMoney,Action<Button> onClick)
    {
        BuildingData = data;
        var ConstructInfo = data as IConstruction;
        nameText.text = ConstructInfo.Name;
        priceText.text = ConstructInfo.Cost.ToString() + "万";
        button.onClick.AddListener(() => { onClick(button); });
        SetCurrentData(currentMoney, currentPhase);
        
    }
    public void SetCurrentData(float money = float.MaxValue,int phase = 0)
    {
       if(phase != 0)
            currentPhase = phase;
       if(money != float.MaxValue)
            currentMoney = money;
        var ConstructInfo = BuildingData as IConstruction;
       if (ConstructInfo.phase > currentPhase)
       {
            button.interactable = false;
            warningText = "到达" + Extension.Phase(ConstructInfo.phase) + "阶段解锁";
        }
       else if (ConstructInfo.Cost > money)
       {
            priceText.color = Color.red;            
            warningText = "没有足够的金钱";
            button.interactable = false;
       }
       else
       {
            priceText.color = Color.black;
            warningText = "";
            button.interactable = true;
       }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
       TimeCache = Time.time;
        enter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TimeCache = 0;
        var animator = detailPanel.GetComponent<Animator>();
        animator.SetBool("Exit", true);
        //Debug.Log("Exit");
        enter = false;
    }
    private void RefreshDetailPanel()
    {
        var texts = detailPanel.GetComponentsInChildren<TextMeshProUGUI>();
        IConstruction constructionInfo = BuildingData as IConstruction;
        texts[0].text = constructionInfo.Name;
        texts[1].text = "";
        if (constructionInfo.energyConsumption != 0)
            texts[1].text += "能源消耗：" + constructionInfo.energyConsumption.ToString() + "\n";
        if (BuildingData is House house)
        {
            house.HouseParaInit();
            texts[1].text += "初始容载人口：" + house.Capacity.ToString() + "\n";
        }
        if (BuildingData is Farm farm)
        {
            farm.FarmParaInit();
            texts[1].text += "初始产出：" + farm.Profit.ToString("F2") + "万" + "\n";
        }
        if (BuildingData is IPowerSupply power)
        {
            texts[1].text += "能源供给：" + power.Power.ToString() + "\n";
        }
        if (BuildingData is IOtherProfit profit)
        {
            texts[1].text += "附加产出：" + profit.Profit.ToString() + "万" + "\n";
        }
        if (constructionInfo.Requiments != "")
            texts[1].text += "建造条件：" + constructionInfo.Requiments + "\n";
        if(warningText != "")
            texts[1].text += "<color=red>"+warningText+"</color>";
        
    }
    private void Update()
    {
        if(detailPanel.activeInHierarchy) { return; }
        if(Time.time > TimeCache + Settings.BuildingBtnHoldTime && enter)
        {
            RefreshDetailPanel();
            detailPanel.transform.position = transform.position + new Vector3(0,50,0);
            detailPanel.SetActive(true);
        }
    }

}
