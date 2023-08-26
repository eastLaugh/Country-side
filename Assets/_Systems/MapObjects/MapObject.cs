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
                    Debug.Log($"请遵从WorkFlow，在MapObject Database中为{GetType().Name}配置信息。由于未找到配置信息，已忽略Prefab");
                    config = default;
                }

                father = new GameObject(GetType().Name).transform;
                father.SetParent(slot.slotRender.transform);
                father.localPosition = Vector3.zero;

                OnlyRenderInvoke = () => Render(config.Prefab, config.Prefabs, slot.slotRender);
                slot.slotRender.OnRender += OnlyRenderInvoke;
                slot.slotRender.OnSlotClicked += _ => OnClick();

                slot.map.OnCreated += _ => OnCreated();


                Awake();
                if (GameManager.current.map == null)
                {
                    //地图尚未完成加载，(说明)此Inject为系统读档操作
                    GameManager.OnMapLoaded += _ => OnEnable();
                }
                else
                {
                    //地图已完成加载，(说明)此Inject为玩家操作
                    OnCreated();
                    OnEnable();
                }

                slot.OnSlotUpdate?.Invoke(); /*仅供表现层*/
                slot.slotRender.Refresh();
                return true;
            }
            else
            {
                return false;
            }
        }

        Action OnlyRenderInvoke;

        public void Unject()
        {
            if (this.slot != null && this.slot.mapObjects.Contains(this) && CanBeUnjected)
            {

                slot.mapObjects.Remove(this);
                slot.slotRender.OnRender -= OnlyRenderInvoke;
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
        /// 执行时机：在地图格子全部创建完成之后
        /// </summary>
        protected abstract void OnEnable();

        /// <summary>
        /// 执行时机：一旦被实例化，立即执行
        /// </summary>
        protected virtual void Awake()
        {

        }

        public abstract bool CanBeUnjected { get; }
        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnDisable();

        /// <summary>
        /// 执行时机：类似于OnEnable。不过，仅在首次被创建时才会执行。永远只执行一次，不会在读取存档时执行
        /// </summary>
        protected abstract void OnCreated();

        public class Virtual : MapObject
        {
            public override bool CanBeUnjected => true;

            protected override void OnCreated()
            {
            }

            protected override void OnDisable()
            {
            }

            protected override void OnEnable()
            {
            }
        }
    }

}


