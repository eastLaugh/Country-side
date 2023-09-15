using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayDate;
    private float timeBuffer;
    private float timeScaleCache;
    private bool isMapLoaded;
    private void OnEnable()
    {
        GameManager.OnMapLoaded+=OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }

    private void OnMapUnloaded()
    {
        isMapLoaded = false;
    }
    void Start()
    {
    }
    Map map;
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        Play();
        isMapLoaded=true;
    }


    void Update()
    {
        if (!isMapLoaded) return;
        timeBuffer = timeBuffer + Time.deltaTime;
        if (timeBuffer > 2f)
        {
            map.dateTime = map.dateTime.AddDays(1);
            EventHandler.CallDayPass();
            //Debug.Log("DayPass");
            timeBuffer = 0;
        }
        displayDate.text = map.dateTime.Date.ToString("yyyy/MM/dd");
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
