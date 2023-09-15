using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    public SerializedDictionary<string, Camera> Cameras;

    public UnityEngine.Rendering.Volume SaturationVolume;

    enum CameraState
    {
        Default, Tube
    }
    FSM<CameraState> fsm = new FSM<CameraState>();

    private void Awake()
    {
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

            MapObjects.Tube.TubeArea.SetTubeAreaHighlight?.Invoke(true);
        }).OnExit(() =>
        {
            Cameras["Tube Camera"].enabled = false;
            SaturationVolume.enabled = false;
            SlotRender.OnAnySlotEnter -= RefreshTubeState;
            SlotRender.OnAnySlotClickedInBuildMode -= RefreshTubeState;

            ResetLayer?.Invoke();
            MapObjects.Tube.TubeArea.SetTubeAreaHighlight?.Invoke(false);
        });

        fsm.ChangeState(CameraState.Default);
    }

    private void OnEnable()
    {
        BuildingWindow.OnUpdate += OnBuildingWindowUpdate;
        GameManager.OnMapLoaded += OnMapLoaded;
    }

    private void OnMapLoaded(Map map)
    {
        ResetLayer = null;
    }

    private void OnDisable()
    {
        BuildingWindow.OnUpdate -= OnBuildingWindowUpdate;
        GameManager.OnMapLoaded -= OnMapLoaded;
    }

    private void Start()
    {
        OnBuildingWindowUpdate(null);
    }

    private void OnBuildingWindowUpdate(Type type)
    {
        if (type == typeof(MapObjects.Tube))
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
        MapObjects.Tube.TubeArea.SetTubeAreaHighlight?.Invoke(true);

        ResetLayer?.Invoke();
        ResetLayer = null;
        for (int i = -MapObjects.Tube.TubeRippleRadius; i <= MapObjects.Tube.TubeRippleRadius; i++)
        {
            for (int j = -MapObjects.Tube.TubeRippleRadius; j <= MapObjects.Tube.TubeRippleRadius; j++)
            {
                if (i * i + j * j <= MapObjects.Tube.TubeRippleRadius * MapObjects.Tube.TubeRippleRadius)
                {
                    var slot = slotRender.slot.map[slotRender.slot.position + new Vector2(i, j)];
                    if (slot != null)
                    {
                        int originLayer = slot.slotRender.SetLayer(LayerMask.NameToLayer("Highlight"));
                        if (originLayer == LayerMask.NameToLayer("Highlight"))
                        {
                            //说明这个slot原本就是高亮的,不需要恢复
                        }
                        else
                        {
                            ResetLayer += () => slot.slotRender.SetLayer(originLayer);

                        }
                    }

                }
            }
        }
    }
}
