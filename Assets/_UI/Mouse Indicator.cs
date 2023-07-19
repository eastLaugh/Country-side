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
    private void OnEnable()
    {
        SlotRender.OnAnySlotEnter += OnSlotEnter;
        SlotRender.OnAnySlotExit -= OnSlotExit;
        SlotRender.OnDragSlot += OnDragSlot;
    }

    private void OnDragSlot(SlotRender render, PointerEventData data)
    {
        MainIndicator.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SlotRender.OnAnySlotEnter -= OnSlotEnter;
        SlotRender.OnAnySlotExit -= OnSlotExit;

    }
    private void OnSlotEnter(SlotRender slotRender)
    {
        MainIndicator.gameObject.SetActive(true);
        MainIndicator.transform.DOMove(slotRender.transform.position + PlaneIndicator.position, 0.1f);
        MainIndicator.material.SetFloat("_Thickness", 0.2f);
        DOTween.To(()=>MainIndicator.material.GetFloat("_Thickness"),t=>MainIndicator.material.SetFloat("_Thickness",t),0.2f,0.15f).From(DefaultMaterial.GetFloat("_Thickness")).SetEase(Ease.InOutQuad);
    }
    private void OnSlotExit(SlotRender slotRender){

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
