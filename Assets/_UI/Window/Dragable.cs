using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Dragable : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public RectTransform rectTransform;
    public void OnBeginDrag(PointerEventData eventData)
    {
        rectTransform.GetComponent<CanvasGroup>().alpha = 0.2f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.GetComponent<CanvasGroup>().alpha = 1f;
    }
}
