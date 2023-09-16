using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class Well : RippleEffectBuilding<WaterArea>, IConstruction
    {
        public const int WellRippleRadius = 2;

        public override bool CanBeUnjected => true;

        public float Cost => 0.05f;

        public string Name => "水井";

        public ConstructType constructType => ConstructType.Supply;

        protected override int RippleRadius => WellRippleRadius;

        protected override void OnCreated()
        {
            base.OnCreated();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
        }


        
    }
    public class WaterArea : MapObject, IInfoProvider
    {
        public void ProvideInfo(Action<string> provide)
        {
            provide("已供水");
        }

        public static Action<bool> SetTubeAreaHighlight { get; private set; } = null;

        public override bool CanBeUnjected => true;

        //void SetHighlight(bool highlight)
        //{
        //    slot.slotRender.SetLayer(LayerMask.NameToLayer(highlight ? "Highlight" : "Default"));
        //}



        protected override void OnEnable()
        {
            //SetTubeAreaHighlight += SetHighlight;
            GameManager.OnMapUnloaded += OnMapUnloaded;
        }

        private void OnMapUnloaded()
        {
            SetTubeAreaHighlight = null;
        }

        protected override void OnDisable()
        {
            //SetTubeAreaHighlight -= SetHighlight;
            GameManager.OnMapUnloaded -= OnMapUnloaded;
        }

        protected override void OnCreated()
        {


        }
    }

}
