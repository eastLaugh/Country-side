using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfitSystem : MonoBehaviour
{
    Map map;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
        EventHandler.DayPass += CalcDailyProfit;
       // Debug.Log("OnEable");
    }
    private void CalcDailyProfit()
    {
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
        map.MainData.Money += map.FarmProfitTotal.currentValue.m_value;
    }
    private void OnDisable()
    {
        EventHandler.DayPass -= CalcDailyProfit;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        enabled = false;
    }
    void Start()
    {
        enabled = false;
    }

    private void OnMapLoaded(Map map)
    {
        this.map = map;
        enabled = true;
    }
}
