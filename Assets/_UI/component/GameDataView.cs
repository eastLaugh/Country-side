using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class GameDataView : MonoBehaviour
{
    Map map;
    GameDataVector m_data;
    [SerializeField] TextMeshProUGUI Money;
    [SerializeField] TextMeshProUGUI People;
    [SerializeField] TextMeshProUGUI Happiness;
    float MoneyCache = 0f;
    int PeopleCache = 0;
    public bool isMapLoaded = false;
    private void OnEnable()
    {
        MoneyCache = 0f;
        PeopleCache = 0;
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
        
    }
    private void OnDisable()
    {
        GameManager.OnMapLoaded -= OnMapLoaded;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        
    }

    private void OnMapUnloaded()
    {
        isMapLoaded = false;
        EventHandler.MoneyUpdate -= OnMoneyChange;
        EventHandler.PeopleUpdate -= OnPeopleChange;
    }
    void Start()
    {
        
    }
    
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMapLoaded = true;
        EventHandler.MoneyUpdate += OnMoneyChange;
        EventHandler.PeopleUpdate += OnPeopleChange;
        OnMoneyChange(map.MainData.Money);
        OnPeopleChange(map.MainData.People);
    }
    private void OnMoneyChange(float newValue)
    {
        StartCoroutine(ReachValueM(MoneyCache, newValue));
        MoneyCache = newValue;
    }
    private void OnPeopleChange(int newValue)
    {
        StartCoroutine(ReachValueP(PeopleCache, newValue));
        PeopleCache = newValue;
    }
    IEnumerator ReachValueM(float startValue,float endValue)
    {
        float sign = Mathf.Sign(endValue -  startValue);
        //if(sign < 0) 
        //Money.color = Color.red;
        while(Mathf.Abs(endValue - startValue) > 0.01f)
        {
            startValue = Mathf.Lerp(startValue,endValue,0.3f);
            Money.text = startValue.ToString("F2") + "Íò";
            yield return new WaitForSeconds(0.07f);
        }
        //Money.color = Color.white;
        Money.text = m_data.Money.ToString("F2") + "Íò";
        yield break;
    }
    IEnumerator ReachValueP(int startValue, int endValue)
    {
        //float sign = Mathf.Sign(endValue - startValue);
        //if(sign < 0) 
        //Money.color = Color.red;
        while (Mathf.Abs(endValue - startValue) > 1f)
        {
            startValue = (int)(Mathf.Lerp(startValue, endValue, 0.3f));
            People.text = startValue.ToString() + "/" +map.MainData.totalCapacity;
            yield return new WaitForSeconds(0.07f);
        }
        //Money.color = Color.white;
        People.text = m_data.People.ToString() + "/" + map.MainData.totalCapacity;
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        if(!isMapLoaded) { return; }
        m_data = map.MainData;
        var totalHappiness = map.HappinessTotal.currentValue.m_value;
        
        if(totalHappiness < 20)
        {
            Happiness.color = Color.red;
        }
        else if (totalHappiness >= 20 && totalHappiness < 50)
        { 
            Happiness.color = Color.yellow;
        }
        else if (totalHappiness >= 50 && totalHappiness < 80)
        {
            Happiness.color = Color.blue;
        }
        else if (m_data.Happiness > 80)
        {
            Happiness.color = Color.green;
        }


        Happiness.text = totalHappiness.ToString();
    }
}
