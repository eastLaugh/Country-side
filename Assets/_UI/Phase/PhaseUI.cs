using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhaseUI : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
    [SerializeField] TextMeshProUGUI PhaseText;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
        // Debug.Log("OnEable");
    }
    private void OnDisable()
    {
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        EventHandler.PhaseUpdate -= PhaseUpdate;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        isMaploaded = false;
        EventHandler.PhaseUpdate -= PhaseUpdate;
    }
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded = true;
        EventHandler.PhaseUpdate += PhaseUpdate;
    }
    
    private void Update()
    {
        if (!isMaploaded) return;
        PhaseText.text = Extension.Phase(map.Phase);
    }
    public void PhaseUpdate(int Phase)
    {
        var Info = InfoWindow.Create("½øÈë" + Extension.Phase(Phase) + "½×¶Î£¡");
        StartCoroutine(Extension.Wait(3f, Info.Unexpand));
       
    }
}
