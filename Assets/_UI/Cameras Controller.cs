using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class CamerasController : MonoBehaviour
{
    private static CamerasController m;
    public CinemachineVirtualCamera MacroVC;
    public SerializedDictionary<string, Camera> Cameras;

    public UnityEngine.Rendering.Volume SaturationVolume;

    enum CameraState
    {
        Default, Tube
    }
    FSM<CameraState> fsm = new FSM<CameraState>();

    private void Awake()
    {
        m = this;

        fsm.State(CameraState.Default).OnEnter(() =>
        {
            SlotRender.StopAllFloat?.Invoke();
        });
        fsm.State(CameraState.Tube).OnEnter(() =>
        {
            Cameras["Tube Camera"].enabled = true;
            SaturationVolume.enabled = true;

            SlotRender.OnAnySlotEnter += RefreshTubeState;
            SlotRender.OnAnySlotClickedInBuildMode += RefreshTubeState;
            // SlotRender.OnAnySlotClickedInBuildMode += _ => InfoWindow.Create("OnAnySlotClickedInBuildMode");

            MapObjects.FiveGArea.Set5GAreaHighlight?.Invoke(true);
        }).OnExit(() =>
        {
            Cameras["Tube Camera"].enabled = false;
            SaturationVolume.enabled = false;
            SlotRender.OnAnySlotEnter -= RefreshTubeState;
            SlotRender.OnAnySlotClickedInBuildMode -= RefreshTubeState;

            ResetLayer?.Invoke();
            MapObjects.FiveGArea.Set5GAreaHighlight?.Invoke(false);
        });

        fsm.ChangeState(CameraState.Default);
    }

    private void OnEnable()
    {
        BuildingWindow.OnUpdate += OnBuildingWindowUpdate;
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;

    }

    private void OnMapUnloaded()
    {
        Slot.MapObject.OnInjected -= OnInjected;

    }

    private void OnMapLoaded(Map map)
    {
        ResetLayer = null;
        Slot.MapObject.OnInjected += OnInjected;
    }

    private void OnInjected(Slot.MapObject @object, bool arg2)
    {
        Focus(@object.gameObject);
    }

    private void OnDisable()
    {
        BuildingWindow.OnUpdate -= OnBuildingWindowUpdate;
        GameManager.OnMapLoaded -= OnMapLoaded;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
    }

    private void Start()
    {
        OnBuildingWindowUpdate(null);
    }

    private void OnBuildingWindowUpdate(Type type)
    {
        if (type == typeof(MapObjects.Station5G))
        {
            fsm.ChangeState(CameraState.Tube);
        }
        else
        {
            fsm.ChangeState(CameraState.Default);
        }
    }

    Action ResetLayer = null;
    void RefreshTubeState(SlotRender slotRender)
    {
        MapObjects.FiveGArea.Set5GAreaHighlight?.Invoke(true);

        ResetLayer?.Invoke();
        ResetLayer = null;
        //for (int i = -MapObjects.Well.TubeRippleRadius; i <= MapObjects.Well.TubeRippleRadius; i++)
        //{
        //    for (int j = -MapObjects.Well.TubeRippleRadius; j <= MapObjects.Well.TubeRippleRadius; j++)
        //    {
        //        if (i * i + j * j <= MapObjects.Well.TubeRippleRadius * MapObjects.Well.TubeRippleRadius)
        //        {
        //            var slot = slotRender.slot.map[slotRender.slot.position + new Vector2(i, j)];
        //            if (slot != null)
        //            {
        //                int originLayer = slot.slotRender.SetLayer(LayerMask.NameToLayer("Highlight"));
        //                if (originLayer == LayerMask.NameToLayer("Highlight"))
        //                {
        //                    //说明这个slot原本就是高亮的,不需要恢复
        //                }
        //                else
        //                {
        //                    ResetLayer += () => slot.slotRender.SetLayer(originLayer);
        //
        //                }
        //            }
        //
        //        }
        //    }
        //}
    }

    private void Update()
    {
        Debug.Log(fsm.CurrentState);
    }

    public static void Focus(GameObject target)
    {
        m.MacroVC.enabled = true;
        m.MacroVC.Follow = target.transform;
        m.MacroVC.LookAt = target.transform;



    }

    Tween lastTween;
    public void OnMacroVCLive()
    {
        var transposer = m.MacroVC.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        // Tween tween = DOTween.To(() => transposer.m_XAxis.Value, f => transposer.m_XAxis.Value = f, 180f, 4f).From(0f);
        if (lastTween.IsActive())
        {
            lastTween.Kill();
        }

        lastTween = DOTween.To(() => transposer.m_Heading.m_Bias - 180f, f => transposer.m_Heading.m_Bias = f + 180f, 180f, 4f).From(-180f).SetEase(Ease.InOutCubic);

        lastTween.OnComplete(() =>
        {
            m.MacroVC.enabled = false;
        });
    }
}
