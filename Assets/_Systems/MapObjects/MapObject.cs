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

        public static readonly Type[] BuiltMapObject =  { typeof(House),typeof(Tree) };
        public static readonly Type[] AllTypes = { typeof(House), typeof(Road),typeof(Tree) };//WORKFLOW : 枚举所有类型

        [JsonProperty]
        public Slot slot { get; private set; } = null;

        public MapObject()
        {

        }

        public bool Inject(Slot slot, bool force = false)
        {
            if (!slot.mapObjects.Contains(this) || force)
            {
                if (slot.mapObjects.Accessible(GetType()))
                {
                    this.slot = slot;
                    slot.mapObjects.Add(this);
                    slot.OnSlotUpdate?.Invoke();

                    Render(GameManager.current.MapObjectDatabase[GetType()].Prefab, slot.slotRender.transform,slot.map);
                    OnSlot();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        protected abstract void OnSlot();

        protected virtual void Render(GameObject prefab,Transform parent,Map map)
        {
            MonoBehaviour.Instantiate(prefab, parent);
        }
    }
}


