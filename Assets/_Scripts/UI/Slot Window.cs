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
    void Start()
    {
        Mouse.OnSelectionChange += OnSelectionChange;
    }

    void OnSelectionChange(IClickable obj)
    {
        if (obj is SlotRender slotRender)
        {
            JObject jObject = JObject.FromObject(slotRender.slot, JsonSerializer.CreateDefault(GameManager.SerializeSettings));
            jObject.Remove("map");
            content.SetText(jObject.ToString());
            //忽略map属性
        }
        else
        {
            content.SetText("未选中单元格");
        }


    }


}
