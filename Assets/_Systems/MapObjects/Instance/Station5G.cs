using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class Station5G : RippleEffectBuilding<FiveGArea>, IConstruction
    {

        public override bool CanBeUnjected => true;

        public float Cost => 0.05f;

        public string Name => "5G基站";

        public ConstructType constructType => ConstructType.Sevice;

        public int phase => 2;

        protected override int RippleRadius => 5;

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
    public class FiveGArea : MapObject, IInfoProvider
    {
        public void ProvideInfo(Action<string> provide)
        {
            provide("已供水");
        }

        public static Action<bool> Set5GAreaHighlight { get; private set; } = null;

        public override bool CanBeUnjected => false;

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
            Set5GAreaHighlight = null;
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
