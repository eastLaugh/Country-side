using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using System.Data;

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
        SlotRender.OnAnySlotEnter -= Refresh;
        SlotRender.OnAnySlotClicked -= Refresh;
        SetColor(defaultColor);
    }

    private void OnBuildModeEnter()
    {
        SlotRender.OnAnySlotEnter += Refresh;
        SlotRender.OnAnySlotClicked += Refresh;
    }


    public void Refresh(SlotRender render)
    {
        if (BuildingWindow.TryGetSelectedTypeConfig(out Type selectedType, out MapObjectDatabase.Config config))
        {

            Vector2 delta = config.Size;

            bool canBuild = true;
            BuildingWindow.Foreach(render.slot.position, config.Size, (x, y) =>
            {
                if (!Slot.MapObject.CanBeInjected(render.slot.map[x, y], selectedType))
                {
                    canBuild = false;
                }
            });

            if (canBuild)
            {
                SetColor(Color.green);
            }
            else
            {
                SetColor(Color.red);
            }
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


    void SetColor(Color color)
    {
        MainIndicator.material.DOColor(color, "_Color", 0.1f);
    }
}
