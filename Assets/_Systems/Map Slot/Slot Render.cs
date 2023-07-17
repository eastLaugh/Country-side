using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class SlotRender : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler
{
    public Slot slot;

    public static event Action<SlotRender> OnSlotSelected;
    public static event Action<SlotRender> OnSlotEnter;
    public static event Action<SlotRender> OnSlotExit;

    public static event Action<SlotRender, PointerEventData> OnDragSlot;


    public void OnClick()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotSelected?.Invoke(this);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSlotEnter?.Invoke(this);

        //transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnSlotExit?.Invoke(this);

        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
    }

    public void UnClick()
    {
    }

    private void Awake()
    {


    }

    //加载动画
    const float LongestTime = 1f;
    private void Start()
    {
        var totalNum = slot.map.size.x * slot.map.size.y;
        var pos = slot.position;
        var index = (int)(pos.x * slot.map.size.y + pos.y);
        var proportion = 2 * Mathf.Min(index, totalNum - index) / (float)totalNum;
        transform.DOLocalMoveY(Enums.相机初始高度, (-proportion*proportion + 1f) * 2f).From().SetEase(Ease.OutBack).SetDelay(LongestTime * (1f - proportion));
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragSlot?.Invoke(this, eventData);
    }
}