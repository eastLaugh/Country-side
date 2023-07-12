using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System;
using Newtonsoft.Json.Linq;

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
        JObject jObject = JObject.FromObject(slotRender.slot, JsonSerializer.CreateDefault(GameManager.SerializeSettings));
        jObject.Remove("map");
        content.SetText(jObject.ToString());
    }




}
