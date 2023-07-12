using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class SlotRender : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public Slot slot;

    public static event Action<SlotRender> OnSlotSelected; 
    public void OnClick()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotSelected?.Invoke(this);
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
    }

    public void UnClick()
    {
    }

    private void Awake()
    {


    }
    private void Start()
    {

    }
}