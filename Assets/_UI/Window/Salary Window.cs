using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SalaryWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool entered = false;
    GameDataWrapper<GameDataVector> economyWrapper;
    public void OnPointerEnter(PointerEventData eventData)
    {
        entered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        entered = false;
    }

    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }

    private void OnDisable() {
        GameManager.OnMapLoaded -= OnMapLoaded;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
    }

    private void OnMapUnloaded()
    {
        economyWrapper = null;
    }

    private void OnMapLoaded(Map map)
    {
        economyWrapper = map.economyWrapper;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public CinemachineVirtualCamera virtualCamera;

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        GUI.skin.label.normal.textColor = Color.yellow;
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        if (entered && economyWrapper != null)
        {
            foreach (var IMiddleware in economyWrapper.Middlewares)
            {
                if (IMiddleware.GetHost() is Slot.MapObject mapObject)
                {
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(mapObject.slot.gameObject.transform.position);
                    GUI.Label(new Rect(screenPoint.x, Screen.height - screenPoint.y, 100, 100), $"{middleware.SolidValue.dailyIncome}");
                }
            }
        }
    }
}
