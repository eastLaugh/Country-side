using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayDate;
    private float timeBuffer;
    private float timeScaleCache;
    private void OnEnable()
    {
        GameManager.OnMapLoaded+=OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }

    private void OnMapUnloaded()
    {
        enabled = false;
    }
    void Start()
    {
        enabled = false;
    }
    Map map;
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        map.dateTime = Convert.ToDateTime("2015/01/01");
        Play();
        enabled = true;
    }

    private void OnDisable()
    {

    }
    

    void Update()
    {
        timeBuffer = timeBuffer + Time.deltaTime;
        if (timeBuffer > 2f)
        {
            map.dateTime = map.dateTime.AddDays(1);
            EventHandler.CallDayPass();
            timeBuffer = 0;
        }
        displayDate.text = GameManager.current.map.dateTime.Date.ToString("yyyy/MM/dd");
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
