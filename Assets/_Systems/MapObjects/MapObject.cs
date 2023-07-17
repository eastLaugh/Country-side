using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;

using static Slot;
using DG.Tweening;

partial class Slot
{
    public abstract class MapObject
    {

        public static readonly Type[] BuiltMapObject = { typeof(House), typeof(Tree) };
        public static readonly Type[] AllTypes = { typeof(House), typeof(Road), typeof(Tree) };//WORKFLOW : 枚举所有类型

        [JsonProperty]
        public Slot slot { get; private set; } = null;

        [JsonProperty]
        int AppearanceSeed;

        protected System.Random random { get; private set; }


        public MapObject(int AppearanceSeed)
        {
            if (AppearanceSeed == -1)
            {
                this.AppearanceSeed = UnityEngine.Random.Range(0, int.MaxValue);
            }
            else
            {
                this.AppearanceSeed = AppearanceSeed;
            }

            random = new System.Random(this.AppearanceSeed);
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

                    var config = GameManager.current.MapObjectDatabase[GetType()];
                    Render(config.Prefab, config.Prefabs, slot.slotRender);
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

        protected virtual GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            GameObject obj = MonoBehaviour.Instantiate(prefab, slotRender.transform) ;
            obj.transform.DOScale(Vector3.zero, Enums.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);
            return new[] {obj};
        }
    }
}


