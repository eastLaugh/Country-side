using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using Cinemachine;
using System;
using DG.Tweening;
using UnityEngine.UI;

// 有待重构
public class InputForCamera : MonoBehaviour
{
    [SerializeField] List<GraphicRaycaster> raycasterList;
    private EventSystem eventSystem;
    private PointerEventData eventData;
    public static event Action<CinemachineVirtualCamera> OnCameraInput;
    enum CameraState
    {
        Overlook, Focus
    };
    public PlayerInput playerInput;
    public CinemachineVirtualCamera virtualCamera;
    public float DragRatio = 0.01f;
    public float ZoomRatio = 0.01f;

    //[NaughtyAttributes.CurveRange(0f, 0f, 10f, 90f, NaughtyAttributes.EColor.Orange)]
    public AnimationCurve HeightToPicthAngleCurve;


    private void Start()
    {
        OnZoom(null);
        

    }
    public void OnDrag()
    {

    }

    private void OnEnable()
    {
        SlotRender.OnDragSlot += OnDragSlot;
        SlotRender.OnAnySlotClicked += OnSlotSelected;
    }

    public void OnDisable()
    {
        SlotRender.OnDragSlot -= OnDragSlot;
        SlotRender.OnAnySlotClicked -= OnSlotSelected;
    }

    private void OnSlotSelected(SlotRender render)
    {
    }

    private void OnDragSlot(SlotRender render, PointerEventData data)
    {
        virtualCamera.transform.position -= new Vector3(data.delta.x, 0, data.delta.y) * DragRatio;
        OnCameraInput?.Invoke(virtualCamera);
    }
    public bool IsOnUIElement()
    {
        eventSystem = EventSystem.current;
        if (eventData == null)
            eventData = new PointerEventData(eventSystem);
        List<RaycastResult> list = new List<RaycastResult>();
        foreach (GraphicRaycaster graphicRaycaster in raycasterList)
        {
            list.Clear();
            eventData.pressPosition = Input.mousePosition;
            eventData.position = Input.mousePosition;
            graphicRaycaster.Raycast(eventData, list);
            foreach (var temp in list)
            {
                if (temp.gameObject.layer.Equals(5)) return true;
            }
        }
        return false;
    }

    Sequence seq;
    public void OnZoom(InputValue value)
    {
        if(IsOnUIElement()) { return; }
        float time = 0.02f;

        float delta = (value?.Get<float>() ?? 0f) * ZoomRatio;

        seq?.Kill();
        seq = DOTween.Sequence();
        if (virtualCamera.transform.position.y < 10f)
        {
            seq.Append(virtualCamera.transform.DOMove(virtualCamera.transform.position + virtualCamera.transform.forward * delta, time));
        }
        else
        {
            seq.Append(virtualCamera.transform.DOMoveY(virtualCamera.transform.position.y - delta, time));
        }

        float PicthAngle = HeightToPicthAngleCurve.Evaluate(virtualCamera.transform.position.y);

        seq.Join(virtualCamera.transform.DORotate(new Vector3(PicthAngle, virtualCamera.transform.rotation.eulerAngles.y, virtualCamera.transform.rotation.eulerAngles.z), time));

        seq.Play();
        //virtualCamera.transform.rotation = Quaternion.Euler(PicthAngle, virtualCamera.transform.rotation.eulerAngles.y, virtualCamera.transform.rotation.eulerAngles.z);

        OnCameraInput?.Invoke(virtualCamera);
    }
}
