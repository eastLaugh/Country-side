using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappinessSystem : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
    [SerializeField]
    HappinessEffectUI happinessEffectUI;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;

        // Debug.Log("OnEable");
    }

    private void CalcDailyHappiness()
    {
        if (!isMaploaded) return;
        CheckHomeless();
        CheckGreenPower();
        map.HappinessTotal.UpdateValue(new Int(map.MainData.Happiness));
        happinessEffectUI.Refresh(map.HappinessTotal);
        if (map.HappinessTotal.currentValue.m_value <= 0)
        {
            EventHandler.CallGameOver();
        }

    }
    private void OnDisable()
    {
        EventHandler.DayPass -= CalcDailyHappiness;
        EventHandler.DayPass -= CheckGameWin;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        EventHandler.DayPass -= CheckGameWin;
        EventHandler.DayPass -= CalcDailyHappiness;
        isMaploaded = false;
    }
    void Start()
    {

    }

    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded = true;
        EventHandler.DayPass += CalcDailyHappiness;
        EventHandler.DayPass += CheckGameWin;
    }
    private void CheckHomeless()
    {
        int temp = 0;
        for (int i = 0; i < map.Houses.Count; i++)
        {
            temp += map.Houses[i].Capacity;
        }
        map.MainData.totalCapacity = temp;
        var CPU = map.HappinessTotal.CPUs.Find((cpu) => { return cpu.name == "有家可归"; });

        if (map.MainData.totalCapacity >= map.MainData.People && CPU.name == null)
        {
            map.HappinessTotal.AddCPU(new SolidMiddleware<Int>.CPU { name = "有家可归", Addition = new Int(10) });
        }
        else if (map.MainData.totalCapacity < map.MainData.People && CPU.name != null)
        {
            map.HappinessTotal.RemoveCPU(CPU);
        }
    }
    private void CheckGreenPower()
    {
        var CPU = map.HappinessTotal.CPUs.Find((cpu) => { return cpu.name == "绿色能源"; });
        if (Mathf.Abs(PowerSystem.greenPowerRatio - 1f) < 0.01f && CPU.name == null)
            map.HappinessTotal.AddCPU(new SolidMiddleware<Int>.CPU { name = "绿色能源", Addition = new Int(15) });
        else if(Mathf.Abs(PowerSystem.greenPowerRatio - 1f) >= 0.01f && CPU.name != null)
            map.HappinessTotal.RemoveCPU(CPU);
    }
    private void CheckGameWin()
    {
        if (map.Phase == 4 && map.HappinessTotal.currentValue.m_value >= 100 && AssignmentSystem.displayList.Count == 0)
            EventHandler.CallGameWin();
    }
}
