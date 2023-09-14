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
        SlotRender.OnAnySlotEnter -= RefreshInBuildMode;
        SlotRender.OnAnySlotClicked -= RefreshInBuildMode;
        SetColor(defaultColor);
    }

    private void OnBuildModeEnter()
    {
        SlotRender.OnAnySlotEnter += RefreshInBuildMode;
        SlotRender.OnAnySlotClicked += RefreshInBuildMode;
    }


    public void RefreshInBuildMode(SlotRender render)
    {

        if (BuildingWindow.TryGetSelectedTypeConfig(out Type selectedType, out MapObjectDatabase.Config config))
        {
            SlotRender.ResetFloat();

            bool canBuild = true;
            Vector3 center = Vector3.zero;
            BuildingWindow.Foreach(render.slot.position, config.Size, (x, y) =>
                {
                    render.slot.map[x, y].slotRender.Float();
                    center += render.slot.map[x, y].worldPosition;

                    //判断是否可以建造，改变颜色
                    if (!Slot.MapObject.CanBeInjected(render.slot.map[x, y], selectedType))
                    {
                        canBuild = false;
                    }
                });
            center /= config.Size.x * config.Size.y;
            MainIndicator.transform.DOScale(new Vector3(config.Size.x * 0.1f, 1, config.Size.y * 0.1f), 0.1f).SetEase(Ease.OutQuad);
            MainIndicator.transform.DOMove(center + PlaneIndicator.position, 0.1f).SetEase(Ease.OutQuad);


            //根据次数旋转
            MainIndicator.transform.rotation = Quaternion.identity;
            for (int i = 0; i < (BuildingWindow.selectedDirection + 2) % 4; i++)
            {
                MainIndicator.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }

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
        if (!BuildMode.hasEntered)
        {
            MainIndicator.transform.DOMove(slotRender.transform.position + PlaneIndicator.position, 0.1f).SetEase(Ease.OutQuad);
            MainIndicator.transform.DOScale(new Vector3(1 * 0.1f, 1, 1 * 0.1f), 0.1f).SetEase(Ease.OutQuad);
        }
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
