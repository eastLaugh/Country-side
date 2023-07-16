using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class SlotRender : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Slot slot;

    public static event Action<SlotRender> OnSlotSelected;
    public static event Action<SlotRender> OnSlotEnter;
    public static event Action<SlotRender> OnSlotExit;
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
    const float Totaltime = 1.5f;
    private void Start()
    {
        
        var totalNum = slot.map.size.x * slot.map.size.y;
        var pos = slot.position;
        var index = (int)(pos.x * slot.map.size.y + pos.y);
        var delay = Mathf.Min(index, totalNum - index) / (float)totalNum * Totaltime;
        transform.DOLocalMoveY(10f,  0.4f).From().SetEase(Ease.InOutQuad).SetDelay(delay);
    }

}