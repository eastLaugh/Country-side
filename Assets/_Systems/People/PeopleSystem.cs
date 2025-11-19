using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSystem : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
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
        EventHandler.DayPass += MorePeople;
    }
    private void OnMapUnloaded()
    {
        EventHandler.DayPass -= MorePeople;
        isMaploaded = false;
    }
    private void OnDisable()
    {
        EventHandler.DayPass -= MorePeople;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        //Debug.Log("Ondisable");
    }
    private void MorePeople()
    {
        int rand = Random.Range(0, 10);
        if (rand == 1)
            map.MainData.People++;
    }
}
