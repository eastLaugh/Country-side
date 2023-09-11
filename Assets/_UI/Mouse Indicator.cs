using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class MouseIndicator : MonoBehaviour
{
    public Renderer MainIndicator;
    public Transform PlaneIndicator;
    public Material DefaultMaterial;

    Color defaultColor;
    private void Awake()
    {
        defaultColor = MainIndicator.material.GetColor("_Color");
    }
    private void OnEnable()
    {
        SlotRender.OnAnySlotEnter += OnAnySlotEnter;
        SlotRender.OnAnySlotExit -= OnAnySlotExit;
        SlotRender.OnDragSlot += OnDragSlot;
        BuildMode.OnBuildModeEnter += OnBuildModeEnter;
        BuildMode.OnBuildModeExit += OnBuildModeExit;
    }

    private void OnDragSlot(SlotRender render, PointerEventData data)
    {
        MainIndicator.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SlotRender.OnAnySlotEnter -= OnAnySlotEnter;
        SlotRender.OnAnySlotExit -= OnAnySlotExit;
        BuildMode.OnBuildModeEnter -= OnBuildModeEnter;
        BuildMode.OnBuildModeExit -= OnBuildModeExit;

    }

    private void OnBuildModeExit()
    {
        SlotRender.OnAnySlotEnter -= DetectAccessibleForMapObject;
        SlotRender.OnAnySlotEnter -= AutoChangeSize;
        SlotRender.OnAnySlotClicked -= DetectAccessibleForMapObject;
        SetColor(defaultColor);
    }

    private void OnBuildModeEnter()
    {
        SlotRender.OnAnySlotEnter += DetectAccessibleForMapObject;
        SlotRender.OnAnySlotEnter += AutoChangeSize;
        SlotRender.OnAnySlotClicked += DetectAccessibleForMapObject;
    }

    private void AutoChangeSize(SlotRender render)
    {
        if (BuildingWindow.TryGetSelectedTypeConfig(out MapObjectDatabase.Config config))
        {
            MainIndicator.transform.localScale = new Vector3(config.Size.x * 0.1f, 1, config.Size.y * 0.1f);
        }
    }

    private void OnAnySlotEnter(SlotRender slotRender)
    {
        MainIndicator.gameObject.SetActive(true);
        MainIndicator.transform.DOMove(slotRender.transform.position + PlaneIndicator.position, 0.1f).SetEase(Ease.OutQuad);
        MainIndicator.material.SetFloat("_Thickness", 0.2f);
        DOTween.To(() => MainIndicator.material.GetFloat("_Thickness"), t => MainIndicator.material.SetFloat("_Thickness", t), 0.2f, 0.15f).From(DefaultMaterial.GetFloat("_Thickness")).SetEase(Ease.InOutQuad);
    }
    private void OnAnySlotExit(SlotRender slotRender)
    {
    }

    void DetectAccessibleForMapObject(SlotRender slotRender)
    {
        if (slotRender.slot.mapObjects.Accessible(BuildingWindow.SelectedType))
        {
            SetColor(Color.green);
        }
        else
        {
            SetColor(Color.red);
        }
    }

    void SetColor(Color color)
    {
        MainIndicator.material.DOColor(color, "_Color", 0.1f);
    }
}
