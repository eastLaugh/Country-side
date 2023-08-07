using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using Cinemachine;
using System;
using DG.Tweening;

// 有待重构
public class InputForCamera : MonoBehaviour
{

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

    Sequence seq;
    public void OnZoom(InputValue value)
    {
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
