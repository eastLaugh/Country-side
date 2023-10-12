using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cinemachine;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using static Slot;
using UnityEngine.UIElements;

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
        BuildMode.OnPlayerBuild -= OnPlayerBuild;
    }

    private void OnMapLoaded(Map map)
    {
        ResetLayer = null;
        BuildMode.OnPlayerBuild += OnPlayerBuild;
    }

    private void OnPlayerBuild(Slot.MapObject mapObject)
    {
        var item = GameManager.current.illuBookData.illuBookList.Find((item) => { return item.name == mapObject.GetType().Name; });
        if (item!=null)
        {
            if(!item.unclock)
            {
                EventHandler.CallilluBookUnlocked(mapObject.GetType().Name);
                Focus(mapObject);
            }            
        }
        
    }
    InfoWindow wnd;
    public void Focus(MapObject mapObject)
    {
        if(wnd != null)
        {
            wnd.Unexpand();
        }
        if(mapObject is IFocusable f)
        {
            wnd=InfoWindow.Create(f.Lore);
        }
        else
        {
            return;
        }
        
        if (mapObject.PlaceHolders.Count > 0)
        {
            Focus(mapObject.PlaceHolders[(mapObject.PlaceHolders.Count) / 2].gameObject);
        }
        else
        {
            Focus(mapObject.gameObject);
        }
        OnFocusMapObject?.Invoke(mapObject);
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
    }

    private void Update()
    {
    }

    public static Action<Slot.MapObject> OnFocusMapObject;
    public static Action UnFocus;

    [SerializeField] private UnityEvent OnFocusEnter;
    [SerializeField] private UnityEvent OnFocusExit;
    public static void Focus(GameObject target)
    {
        m.OnFocusEnter?.Invoke();
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
            
            UnFocus?.Invoke();
            OnFocusExit?.Invoke();

            if (wnd != null)
            {
                wnd.Unexpand();
                wnd = null;
            }
        });
    }
}
