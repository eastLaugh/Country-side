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
        priceText.text = ConstructInfo.Cost.ToString() + "��";
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
            warningText = "����" + Extension.Phase(ConstructInfo.phase) + "�׶ν���";
        }
       else if (ConstructInfo.Cost > money)
       {
            priceText.color = Color.red;            
            warningText = "û���㹻�Ľ�Ǯ";
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
            texts[1].text += "��Դ���ģ�" + constructionInfo.energyConsumption.ToString() + "\n";
        if (BuildingData is House house)
        {
            house.HouseParaInit();
            texts[1].text += "��ʼ�����˿ڣ�" + house.Capacity.ToString() + "\n";
        }
        if (BuildingData is Farm farm)
        {
            farm.FarmParaInit();
            texts[1].text += "��ʼ������" + farm.Profit.ToString("F2") + "��" + "\n";
        }
        if (BuildingData is IPowerSupply power)
        {
            texts[1].text += "��Դ������" + power.Power.ToString() + "\n";
        }
        if (BuildingData is IOtherProfit profit)
        {
            texts[1].text += "���Ӳ�����" + profit.Profit.ToString() + "��" + "\n";
        }
        if (constructionInfo.Requiments != "")
            texts[1].text += "����������" + constructionInfo.Requiments + "\n";
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
