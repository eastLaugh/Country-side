using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;

using static Slot;
using DG.Tweening;
using static MapObjects;

partial class Slot
{

    public abstract class MapObject
    {

        protected Map map => slot.map;
        [JsonProperty]
        public Slot slot { get; private set; } = null;



        protected Transform father { get; private set; }
        protected GameObject gameObject => father.gameObject;

        public static bool CanBeInjected(Slot slot, Type detectedType) => slot.mapObjects.Accessible(detectedType);

        public bool Inject(Slot slot, bool force = false)
        {
            if ((!slot.mapObjects.Contains(this) && slot.mapObjects.Accessible(GetType())) || force /*强制为true:仅在序列化等硬操作时起用*/)
            {
                this.slot = slot;
                slot.mapObjects.Add(this);

                MapObjectDatabase.Config config;
                try
                {
                    config = GameManager.current.MapObjectDatabase[GetType()];

                }
                catch (KeyNotFoundException)
                {
                    Debug.Log($"未找到{GetType().Name}的配置信息，已忽略渲染");
                    config = default;
                }

                father = new GameObject(GetType().Name).transform;
                father.SetParent(slot.slotRender.transform);
                father.localPosition = Vector3.zero;

                RenderInvoke = () => Render(config.Prefab, config.Prefabs, slot.slotRender);
                slot.slotRender.OnRender += RenderInvoke;
                slot.slotRender.OnSlotClicked += _ => OnClick();

                slot.map.OnCreated += _ => OnCreated();


                Awake();
                if (GameManager.current.map == null)
                {
                    GameManager.OnMapLoaded += _ => OnEnable();
                }
                else
                {
                    OnEnable();
                }

                slot.OnSlotUpdate?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        Action RenderInvoke;

        public void Unject()
        {
            if (this.slot != null && this.slot.mapObjects.Contains(this) && CanBeUnjected)
            {

                slot.mapObjects.Remove(this);
                slot.slotRender.OnRender -= RenderInvoke;
                slot.slotRender.OnSlotClicked -= _ => OnClick();
                slot.map.OnCreated -= _ => OnCreated();

                MonoBehaviour.Destroy(father.gameObject);
                slot.OnSlotUpdate?.Invoke();

                OnDisable();
                this.slot = null;
            }
        }

        protected virtual void OnClick()
        {

        }

        protected virtual GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            if (prefab != null)
            {
                GameObject obj = MonoBehaviour.Instantiate(prefab, father);
                obj.transform.DOScale(Vector3.zero, Settings.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);
                return new[] { obj };
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 无论是创建还是加载，均会执行此。执行时机：读档时，在地图格子全部创建完成之后；读档后，在玩家操作后立即执行
        /// </summary>
        protected abstract void OnEnable();


        protected virtual void Awake()
        {

        }

        public abstract bool CanBeUnjected { get; }
        //地图格子被撤销注入格子后
        protected abstract void OnDisable();

        /// <summary>
        /// 地图被创建时会执行此。【加载时不会执行！】。执行时机：地图格子均已创建完成后
        /// </summary>
        protected abstract void OnCreated();

    }
}


