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
        SlotRender.OnAnySlotEnter += OnSlotSelected;
    }

    private void OnDisable()
    {
        SlotRender.OnAnySlotExit -= OnSlotSelected;
    }

    void OnSlotSelected(SlotRender slotRender)
    {
        if (selectedSlot != null)
        {
            selectedSlot.OnSlotUpdate -= OnSlotUpdate;
        }

        selectedSlot = slotRender.slot;
        selectedSlot.OnSlotUpdate += OnSlotUpdate;
        OnSlotUpdate();
    }

    private Slot selectedSlot;

    void OnSlotUpdate()
    {
        //GetComponent<RectTransform>().DOShakeAnchorPos(0.2f, 10, 100, 90, false, true);
        
        // var text = JsonConvert.SerializeObject(selected, GameManager.SerializeSettings);
        // JObject jObject = JObject.FromObject(selected, JsonSerializer.CreateDefault(GameManager.SerializeSettings));
        // jObject.Remove("map");
        content.SetText(selectedSlot.GetInfo());

        GetComponent<Window>().SetTitle(selectedSlot.GetType().Name);


    }


}
