using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    [SerializeField]
    Button BtnHouse;
    [SerializeField]
    Button BtnFarm;
    [SerializeField]
    Button BtnSevice;
    [SerializeField]
    Button BtnEAI;
    [SerializeField]
    Button BtnRoad;
    public Button currentButton;
    private void Start()
    {
        BtnHouse.onClick.AddListener(() =>
        {
            OnBtnClick(BtnHouse);
            EventHandler.CallBuildingWindowUpdate(ConstructType.House);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick);
        });
        BtnFarm.onClick.AddListener(() =>
        {
            OnBtnClick(BtnFarm);
            EventHandler.CallBuildingWindowUpdate(ConstructType.Farm);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick);
        });
        BtnEAI.onClick.AddListener(() =>
        {
            OnBtnClick(BtnEAI);
            EventHandler.CallBuildingWindowUpdate(ConstructType.EAI);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick);
        });
        BtnSevice.onClick.AddListener(() =>
        {
            OnBtnClick(BtnSevice);
            EventHandler.CallBuildingWindowUpdate(ConstructType.Sevice);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick);
        });
        BtnRoad.onClick.AddListener(() =>
        {
            OnBtnClick(BtnRoad);
            EventHandler.CallBuildingWindowUpdate(ConstructType.Road);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick);
        });
        //BtnHouse.onClick?.Invoke();
    }
    
    private void OnBtnClick(Button button)
    {
        if (currentButton == button) { return; }
        currentButton = button;
        button.interactable = false;
        var btns = GetComponentsInChildren<Button>();
        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i] != currentButton) 
            {
                btns[i].interactable = true;
            }
        }
    }
}
