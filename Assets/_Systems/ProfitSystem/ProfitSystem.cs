using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfitSystem : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
        
       // Debug.Log("OnEable");
    }
   
    private void CalcDailyProfit()
    {
        if (!isMaploaded) return;
        //Debug.Log("Enter");
        float profit = 0;
        if (map != null) 
        {
            for (int i = 0;i<map.Farms.Count;i++)
            {
                profit += map.Farms[i].Profit;
            }
        }
        //Debug.Log(map.Farms.Count);
        map.FarmProfitTotal.UpdateValue(new Float(profit));
        float OtherProfit = 0;
        foreach(var ele in map.OtherProfits)
        {
            OtherProfit += ele.Profit;
        }
        map.MainData.Money += map.FarmProfitTotal.currentValue.m_value + OtherProfit;
        
    }
    private void OnDisable()
    {
        EventHandler.DayPass -= CalcDailyProfit;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        EventHandler.DayPass -= CalcDailyProfit;
        isMaploaded = false;
    }
    void Start()
    {
        
    }

    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded= true;
        EventHandler.DayPass += CalcDailyProfit;
    }
}
