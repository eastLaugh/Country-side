using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class GameDataView : MonoBehaviour
{
    Map map;
    GameDataVector m_data;
    [SerializeField] TextMeshProUGUI Money;
    [SerializeField] TextMeshProUGUI People;
    [SerializeField] TextMeshProUGUI Happiness;
    [SerializeField] TextMeshProUGUI MoneyDelta;
    float MoneyCache = 0f;
    int PeopleCache = 0;
    public bool isMapLoaded = false;
    private void OnEnable()
    {
        MoneyCache = 0f;
        PeopleCache = 0;
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
        
    }
    private void OnDisable()
    {
        GameManager.OnMapLoaded -= OnMapLoaded;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        
    }

    private void OnMapUnloaded()
    {
        isMapLoaded = false;
        EventHandler.MoneyUpdate -= OnMoneyChange;
        EventHandler.PeopleUpdate -= OnPeopleChange;
    }
    void Start()
    {
        
    }
    
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMapLoaded = true;
        EventHandler.MoneyUpdate += OnMoneyChange;
        EventHandler.PeopleUpdate += OnPeopleChange;
        OnMoneyChange(map.MainData.Money);
        OnPeopleChange(map.MainData.People);
    }
    private void OnMoneyChange(float newValue)
    {
        StartCoroutine(ReachValueM(MoneyCache, newValue));
        MoneyCache = newValue;
    }
    private void OnPeopleChange(int newValue)
    {
        //StartCoroutine(ReachValueP(PeopleCache, newValue));
        //People.text = newValue + "/" + map.MainData.totalCapacity;   
        PeopleCache = newValue;
    }
    IEnumerator ReachValueM(float startValue,float endValue)
    {
        float sign = Mathf.Sign(endValue -  startValue);
        //if(sign < 0) 
        //Money.color = Color.red;
        while(Mathf.Abs(endValue - startValue) > 0.01f)
        {
            startValue = Mathf.Lerp(startValue,endValue,0.3f);          
            if(startValue > 1E4)
            {
                Money.text = (startValue / 1E4).ToString("F2") + "亿";
            }
            else
            {
                Money.text = startValue.ToString("F2") + "万";
            }
            yield return new WaitForSeconds(0.07f);
        }
        //Money.color = Color.white;
        if (m_data.Money > 1E4)
        {
            Money.text = (m_data.Money / 1E4).ToString("F2") + "亿";
        }
        else
        {
            Money.text = m_data.Money.ToString("F2") + "万";
        }
        yield break;
    }
    IEnumerator ReachValueP(int startValue, int endValue)
    {
        //float sign = Mathf.Sign(endValue - startValue);
        //if(sign < 0) 
        //Money.color = Color.red;
        while (Mathf.Abs(endValue - startValue) > 2f)
        {
            startValue = (int)(Mathf.Lerp(startValue, endValue, 0.3f));
            People.text = startValue.ToString() + "/" +map.MainData.totalCapacity;
            yield return new WaitForSeconds(0.07f);
        }
        //Money.color = Color.white;
        People.text = m_data.People.ToString() + "/" + map.MainData.totalCapacity;
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        if(!isMapLoaded) { return; }
        m_data = map.MainData;
        var totalHappiness = Mathf.Clamp(map.HappinessTotal.currentValue.m_value,0,100);
        if(totalHappiness < 30)
        {
            Happiness.color = Color.red;
        }
        else if (totalHappiness >= 30 && totalHappiness < 60)
        { 
            Happiness.color = Color.yellow;
        }
        else if (totalHappiness >= 50 && totalHappiness < 85)
        {
            Happiness.color = Color.blue;
        }
        else if (m_data.Happiness > 85)
        {
            Happiness.color = Color.green;
        }
        float OtherProfit = 0;
        foreach (var ele in map.OtherProfits)
        {
            OtherProfit += ele.Profit;
        }
        People.text = map.MainData.People + "/" + map.MainData.totalCapacity;
        MoneyDelta.text = "日产值："+(map.FarmProfitTotal.currentValue.m_value + OtherProfit).ToString("F2")+"万";
        Happiness.text = totalHappiness.ToString();

    }
}
