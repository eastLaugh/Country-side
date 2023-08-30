using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem 
{
    Map map => GameManager.current.map;
    private float timeBuffer;
    private float timeScaleCache;
    public TimeSystem()
    {
        GameManager.OnGameUpdate += OnUpdate;
        GameManager.AfterMapLoaded += OnLoad;
    }
    void OnUpdate()
    {
        timeBuffer = timeBuffer + Time.deltaTime;
        if(timeBuffer > 2f)
        {
            map.dateTime = map.dateTime.AddDays(1);
            EventHandler.CallDayPass();
            timeBuffer = 0;
        }
    }
    void OnLoad(Map map)
    {
        map.dateTime = Convert.ToDateTime("2015/01/01");
        Play();
    }
    public void Play()
    {
        Time.timeScale = 1.0f;
    }
    public void Stop()
    {
        Time.timeScale = 0.0f;
    }
    public void FastForward()
    {
        Time.timeScale = 5.0f;
    }
    public void Pause()
    {
        timeScaleCache = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    public void Continue()
    {
        Time.timeScale = timeScaleCache;
    }
}
