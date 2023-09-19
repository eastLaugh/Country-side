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
    public event Action<SlotRender> OnSlotClicked;
    public static event Action<SlotRender> OnAnySlotClicked;

    public static event Action<SlotRender> OnAnySlotEnter;
    public static event Action<SlotRender> OnAnySlotExit;

    public static event Action<SlotRender, PointerEventData> OnDragSlot;


    public event Action OnRender;



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
            OnSlotClicked?.Invoke(this);
            slot.InvokeOnSlotUpdate();
        }

#if UNITY_EDITOR
        Selection.SetActiveObjectWithContext(gameObject, null);
#endif
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnAnySlotEnter?.Invoke(this);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnAnySlotExit?.Invoke(this);
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
        //transform.DOLocalMoveY(Settings.相机初始高度, (-proportion * proportion + 1f) * 2f).From().SetEase(Ease.OutBack).SetDelay(LongestTime * (1f - proportion));
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragSlot?.Invoke(this, eventData);
    }

    Tween lastTween;
    internal void Shake()
    {
        lastTween?.Complete();
        lastTween = transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, false);
    }


    Tween FloatTween;
    public void Float()
    {
        StopAllFloat += StopFloat;
        if (Settings.开启浮动效果)
            FloatTween = transform.DOLocalMoveY(transform.localPosition.y + 0.3f, 0.5f).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Yoyo);
    }
    public void StopFloat()
    {
        StopAllFloat -= StopFloat;
        //如何在这里让FloatTween销毁并回到初始状态
        FloatTween?.Restart();
        FloatTween?.Kill();
    }

    public static Action StopAllFloat { get; set; }=null;
    public static void ResetFloat()
    {
        StopAllFloat?.Invoke();
    }

    public int SetLayer(int v)
    {
        int origin = gameObject.layer;
        gameObject.layer = v;
        return origin;
    }

    private void OnEnable()
    {
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }

    private void OnDisable()
    {
        GameManager.OnMapUnloaded -= OnMapUnloaded;
    }
    private void OnMapUnloaded()
    {
        StopAllFloat = null;

    }
}