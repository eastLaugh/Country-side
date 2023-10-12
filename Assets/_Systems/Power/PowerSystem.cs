using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSystem : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
    [SerializeField]
    PowerUI powerUI;
    public static float greenPowerRatio;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;

        // Debug.Log("OnEable");
    }
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded = true;
        EventHandler.DayPass += CalcPowerSupply;
    }
    private void OnDisable()
    {
        EventHandler.DayPass -= CalcPowerSupply;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        EventHandler.DayPass -= CalcPowerSupply;
        isMaploaded = false;
    }
    private void CalcPowerSupply()
    {
        if (!isMaploaded) return;
        float consumePower = 0, generatePower = 0;
        
        foreach(var ele in map.BuildingsNum)
        {
            var ins = Activator.CreateInstance(ele.Key) as IConstruction;
            consumePower += ele.Value * ins.energyConsumption;
        }
        foreach(var ele in map.PowerSupplies)
        {
            generatePower += ele.Power;
        }
        
        if (consumePower - generatePower > 0 )
        {
            greenPowerRatio = generatePower / consumePower;
        }        
        else
            greenPowerRatio = 1;
        powerUI.Show(greenPowerRatio);
        //Debug.Log("consume" + consumePower.ToString() + "| Generate" + generatePower.ToString()); 
    }
    private void CalcBiomassPower()
    {

    }

    void Start()
    {
        
    }
}
