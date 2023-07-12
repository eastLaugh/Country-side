using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System;
using Newtonsoft.Json.Linq;
using DG.Tweening;

public class SlotWindow : MonoBehaviour
{
    public TextMeshProUGUI content;

    // Start is called before the first frame update
    private void Awake()
    {
    }

    private void OnEnable()
    {
        SlotRender.OnSlotSelected += OnSlotSelected;
    }

    private void OnDisable()
    {
        SlotRender.OnSlotSelected -= OnSlotSelected;
    }

    void OnSlotSelected(SlotRender slotRender)
    {
        if (selected != null)
        {
            selected.OnSlotUpdate -= OnSlotUpdate;
        }

        selected = slotRender.slot;
        selected.OnSlotUpdate += OnSlotUpdate;
        OnSlotUpdate();
    }

    private Slot selected;

    void OnSlotUpdate()
    {
        GetComponent<RectTransform>().DOShakeAnchorPos(0.2f, 10, 100, 90, false, true);
        
        JObject jObject = JObject.FromObject(selected, JsonSerializer.CreateDefault(GameManager.SerializeSettings));
        jObject.Remove("map");
        content.SetText(jObject.ToString());


    }


}
