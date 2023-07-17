using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using Cinemachine;
using System;
using DG.Tweening;
public class InputForCamera : MonoBehaviour
{
    enum CameraState
    {
        Overlook, Focus
    };
    private FSM<CameraState> fsm = new FSM<CameraState>();
    public PlayerInput playerInput;
    public CinemachineVirtualCamera virtualCamera;
    public float DragRatio;

    //public Transform turningPoint;

    private void Start()
    {
        // fsm.State(CameraState.Overlook).OnEnter(() =>
        // {
        //     playerInput.ActivateInput();
        // }).OnExit(() =>
        // {
        //     turningPoint = Instantiate(transform);
        // });
        // fsm.State(CameraState.Focus).OnEnter(() =>
        // {
        //     playerInput.DeactivateInput();


        // }).OnExit(() =>
        // {
        //     transform.position = turningPoint.position;
        //     transform.rotation = turningPoint.rotation;
        //     virtualCamera.LookAt = null;
        //     virtualCamera.Follow = null;
        // });
        // fsm.ChangeState(CameraState.Overlook);

    }
    public void OnDrag()
    {

    }

    private void OnEnable()
    {
        SlotRender.OnDragSlot += OnDragSlot;
        SlotRender.OnSlotSelected += OnSlotSelected;
    }

    public void OnDisable()
    {
        SlotRender.OnDragSlot -= OnDragSlot;
        SlotRender.OnSlotSelected -= OnSlotSelected;
    }

    private void OnSlotSelected(SlotRender render)
    {
        // virtualCamera.LookAt = render.transform;
        // virtualCamera.Follow = render.transform;
        // fsm.ChangeState(CameraState.Focus);
    }

    private void OnDragSlot(SlotRender render, PointerEventData data)
    {
        virtualCamera.transform.position -= new Vector3(data.delta.x, 0, data.delta.y) * DragRatio;
    }
    Sequence seq;
    public void OnZoom(InputValue value)
    {
        // virtualCamera.m_Lens.FieldOfView -= value.Get<float>() * 0.01f;
        // virtualCamera.transform.position += virtualCamera.transform.forward * value.Get<float>() * 0.01f;
        // virtualCamera.transform.Translate(virtualCamera.transform.forward * value.Get<float>() * 0.01f, Space.World);
        if (value.Get<float>() != 0)
        {
            if (seq != null && seq.IsPlaying())
            {
                seq.Kill();
            }
            seq = DOTween.Sequence();
            seq.Append(virtualCamera.transform.DOMove(virtualCamera.transform.position + virtualCamera.transform.forward * value.Get<float>() * 0.01f, 0.1f));
            seq.Join(DOTween.To(() => virtualCamera.m_Lens.FieldOfView, t => virtualCamera.m_Lens.FieldOfView = t, virtualCamera.m_Lens.FieldOfView - value.Get<float>() * 0.1f, 0.1f));
            seq.Play();
        }
    }
}
