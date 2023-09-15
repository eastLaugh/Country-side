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
            if (mapObjects.Count == 0 )
            {

                try
                {
                    var cfg = SlotDatabase.main[render.slot.GetType()];
                    nameText.text = cfg.name;
                    content.text = "";
                    warining.text = "";
                }
                catch (KeyNotFoundException)
                {

                    throw;
                }
                finally
                {
                    
                }

                

            }
            else if (mapObjects.Count == 1)
            {
                var mapObject = mapObjects.ToArray()[0];
                Type Mtype = mapObject.GetType();
                if (mapObject is MapObjects.House house)
                {
                    nameText.text = house.Name;
                    content.text = "容载人口："+house.Capacity.ToString();
                    warining.text = "";
                }
                else if(mapObject is MapObjects.Farm farm) 
                { 
                    
                    nameText.text = farm.Name;
                    content.text = "产出："+farm.Profit.ToString()+"万";
                    warining.text = "";
                }
            }
            
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
