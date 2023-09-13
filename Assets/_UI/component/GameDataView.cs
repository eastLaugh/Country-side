using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDataView : MonoBehaviour
{
    Map map;
    GameDataVector m_data;
    [SerializeField] TextMeshProUGUI Money;
    [SerializeField] TextMeshProUGUI People;
    [SerializeField] TextMeshProUGUI Happiness;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
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
    
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        m_data = map.MainData;
        Money.text = m_data.Money.ToString("F2")+"Íò";
        People.text = m_data.People.ToString();
        Happiness.text = m_data.Happiness.ToString();
    }
}
