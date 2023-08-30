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
}
