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
        protected Map map => slot.map;
        public static readonly Type[] BuiltMapObject = { typeof(House), typeof(Tree) };
        public static readonly Type[] AllTypes = { typeof(House), typeof(Road), typeof(Tree) };//WORKFLOW : 枚举所有类型

        [JsonProperty]
        public Slot slot { get; private set; } = null;



        protected Transform father { get; private set; }

        public static bool CanBeInjected(Slot slot, Type detectedType) => slot.mapObjects.Accessible(detectedType);

        public bool Inject(Slot slot, bool force = false)
        {
            if ((!slot.mapObjects.Contains(this) && slot.mapObjects.Accessible(GetType())) || force /*强制为true:仅在序列化等硬操作时起用*/)
            {
                this.slot = slot;
                slot.mapObjects.Add(this);

                var config = GameManager.current.MapObjectDatabase[GetType()];

                father = new GameObject(GetType().Name).transform;
                father.SetParent(slot.slotRender.transform);
                father.localPosition = Vector3.zero;

                slot.slotRender.RegisterRender(() => Render(config.Prefab, config.Prefabs, slot.slotRender));
                slot.slotRender.OnSlotClicked += _ => OnClick();

                slot.map.OnLoad += _ => Start();

                slot.OnSlotUpdate?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Unject()
        {
            //TODO
        }

        protected virtual void OnClick()
        {

        }

        protected virtual GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            GameObject obj = MonoBehaviour.Instantiate(prefab, father);
            obj.transform.DOScale(Vector3.zero, Settings.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);
            return new[] { obj };
        }

        protected abstract void Start();

        public void Update()
        {
            slot.OnSlotUpdate?.Invoke();
        }
    }
}


