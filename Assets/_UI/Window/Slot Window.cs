using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using static MapObjects;

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
            if(mapObjects.Count > 0 )
            {
                foreach (var mapObject in mapObjects)
                {
                    Slot.MapObject RMapObject;
                    if(mapObject is PlaceHolder)
                    {
                        RMapObject = (mapObject as PlaceHolder).mapObject;
                    }
                    else
                        RMapObject = mapObject;
                    if (typeof(IConstruction).IsAssignableFrom(RMapObject.GetType()))
                    {
                        Debug.Log("construction");
                        IConstruction construction = (IConstruction)RMapObject;
                        nameText.text = construction.Name;
                        if (construction.energyConsumption != 0)
                            content.text = "能源消耗：" + construction.energyConsumption.ToString() + "\n";
                        else
                            content.text = "";
                        warining.text = "";
                    }
                    else
                    {
                        var cfg = SlotDatabase.main[render.slot.GetType()];
                        nameText.text = cfg.name;
                        content.text = "";
                        warining.text = "";
                    }
                    if (RMapObject is House house)
                    {
                        content.text += "容载人口：" + house.Capacity.ToString() + "\n";
                        warining.text = house.Warning;
                        
                    }
                    if (RMapObject is Farm farm)
                    {
                        var profitTotal = GameManager.current.map.FarmProfitTotal;
                        profitTotal.UpdateValue(new Float(farm.Profit));
                        var profit = profitTotal.currentValue.m_value;
                        content.text += "产出：" + profit.ToString("F2") + "万" + "\n";
                        warining.text = "";                     
                    }
                    if(RMapObject is IPowerSupply power)
                    {
                        content.text += "能源供给：" + power.Power.ToString() + "\n";
                    }

                }
            }
            else
            {
                var cfg = SlotDatabase.main[render.slot.GetType()];
                nameText.text = cfg.name;
                content.text = "";
                warining.text = "";
            }
            
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
