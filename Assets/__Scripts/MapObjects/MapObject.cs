using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;

using static Slot;

partial class Slot
{
    public abstract class MapObject
    {
        [JsonProperty]
        public Slot slot { get; private set; } = null;

        public MapObject()
        {

        }

        public bool Inject(Slot slot)
        {
            if (slot.mapObjects.Accessible(GetType()))
            {
                this.slot = slot;
                slot.mapObjects.Add(this);
                slot.OnSlotUpdate?.Invoke();
                OnSlot();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract void OnSlot();
    }
}


public class House : MapObject /* , IReject<House>, IReject<Road> */
{
    protected override void OnSlot()
    {
    }
}

public class Road : MapObject
{
    protected override void OnSlot()
    {
    }
}
