using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

public class SlotWindow : MonoBehaviour
{
    public TextMeshProUGUI content;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI warining;

    // Start is called before the first frame update
    private void Awake()
    {
    }

    private void OnEnable()
    {
        SlotRender.OnAnySlotEnter += OnSlotSelected;
        SlotRender.OnAnySlotClicked += OnSlotClicked;
    }

    private void OnSlotClicked(SlotRender render)
    {
        EventHandler.CallInitSoundEffect(SoundName.SlotClick);
        if (!BuildMode.hasEntered)
        {
            var mapObjects = render.slot.mapObjects;
            foreach(var mapObject  in mapObjects)
            {
                if (mapObject is MapObjects.House house)
                {
                    nameText.text = house.Name;
                    content.text = "容载人口：" + house.Capacity.ToString();
                    warining.text = "";
                    return;
                }
                else if (mapObject is MapObjects.Farm farm)
                {

                    nameText.text = farm.Name;
                    content.text = "产出：" + farm.Profit.ToString() + "万";
                    warining.text = "";
                    return;
                }
            }
            var cfg = SlotDatabase.main[render.slot.GetType()];
            nameText.text = cfg.name;
            content.text = "";
            warining.text = "";
            return;
        }
            
    }

    private void OnDisable()
    {
        SlotRender.OnAnySlotExit -= OnSlotSelected;
        SlotRender.OnAnySlotClicked -= OnSlotClicked;
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
        //content.SetText(selectedSlot.GetInfo());

        
    }


}
