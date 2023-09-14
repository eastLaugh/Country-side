using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler 
{
    public static event Action<string> illuBookUnlocked;
    public static void CallilluBookUnlocked(string name)
    {
        illuBookUnlocked?.Invoke(name);
    }
    public static event Action<BarUI> BarSelectedChanged;
    public static void CallBarSelectedChanged(BarUI bar)
    {
        BarSelectedChanged?.Invoke(bar);
    }
    public static event Action DayPass;
    public static void CallDayPass()
    {
        DayPass?.Invoke();
    }
    public static event Action SavefileDeleted;
    public static void CallSavefileDeleted()
    {
        SavefileDeleted?.Invoke();
    }
    public static event Action<float> MoneyUpdate;
    public static void CallMoneyUpdate(float newValue)
    {
        MoneyUpdate?.Invoke(newValue);
    }
    public static event Action<int> PeopleUpdate;
    public static void CallPeopleUpdate(int newValue)
    {
        PeopleUpdate?.Invoke(newValue);
    }
}
