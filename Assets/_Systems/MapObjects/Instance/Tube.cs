using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class Tube : RippleEffectBuilding<Tube.TubeArea>, IConstruction
    {
        public const int TubeRippleRadius = 5;

        public override bool CanBeUnjected => throw new System.NotImplementedException();

        public float Cost => 0.05f;

        public string Name => "水管";

        public ConstructType constructType => ConstructType.Supply;

        protected override int RippleRadius => TubeRippleRadius;

        protected override void OnCreated()
        {
            base.OnCreated();
        }

        protected override void OnDisable()
        {
            
        }

        protected override void OnEnable()
        {
        }


        public class TubeArea : MapObject.Virtual, IInfoProvider
        {
            public void ProvideInfo(Action<string> provide)
            {
                provide("已供水");
            }

            public static Action<bool> SetTubeAreaHighlight { get; private set; } = null;
            void SetHighlight(bool highlight)
            {
                slot.slotRender.SetLayer(LayerMask.NameToLayer(highlight ? "Highlight" : "Default"));
            }



            protected override void OnEnable()
            {
                SetTubeAreaHighlight += SetHighlight;
            }
            protected override void OnDisable()
            {
                SetTubeAreaHighlight -= SetHighlight;
            }

            protected override void OnCreated()
            {
                base.OnCreated();
                
            }
        }
    }


}
