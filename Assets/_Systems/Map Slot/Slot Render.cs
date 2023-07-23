using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SlotRender : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler
{
    public Slot slot;

    public static event Action<SlotRender> OnAnySlotClickedInBuildMode;
    public static event Action<SlotRender> OnAnySlotClicked;
    public static event Action<SlotRender> OnAnySlotEnter;
    public static event Action<SlotRender> OnAnySlotExit;

    public static event Action<SlotRender, PointerEventData> OnDragSlot;

    event Action OnRender;

    public void RegisterRender(Action onRender)
    {
        OnRender += onRender;
    }

    public void Refresh()
    {
        OnRender?.Invoke();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (BuildMode.hasEntered)
        {
            OnAnySlotClickedInBuildMode?.Invoke(this); //仅建造模式
        }
        else
        {
            OnAnySlotClicked?.Invoke(this); //全局
            slot.Click(); //处理MapObject点击事件
        }

#if UNITY_EDITOR
        Selection.SetActiveObjectWithContext(gameObject, null);
#endif
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnAnySlotEnter?.Invoke(this);

        //transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnAnySlotExit?.Invoke(this);

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
        Refresh();

        var totalNum = slot.map.size.x * slot.map.size.y;
        var pos = slot.position;
        var index = (int)(pos.x * slot.map.size.y + pos.y);
        var proportion = 2 * Mathf.Min(index, totalNum - index) / (float)totalNum;
        transform.DOLocalMoveY(Settings.相机初始高度, (-proportion * proportion + 1f) * 2f).From().SetEase(Ease.OutBack).SetDelay(LongestTime * (1f - proportion));
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragSlot?.Invoke(this, eventData);
    }
}