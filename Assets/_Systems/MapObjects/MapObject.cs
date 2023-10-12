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
        public static event Action<MapObject, bool> OnInjected;
        public static event Action<MapObject> OnUnjected;

        [JsonProperty]
        public List<MapObject> PlaceHolders { get; private set; } = new();

        public event Action<MapObject> OnMapObjectUnjected;

        [JsonIgnore]
        public Map map => slot.map;
        [JsonProperty]
        public Slot slot { get; private set; }

        [JsonProperty]
        public int Direction { get; protected set; }

        public readonly static Vector2[] DirectionToVector2 = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

        [JsonIgnore]
        protected Transform father { get; private set; }
        [JsonIgnore]
        public GameObject gameObject => father.gameObject;

        public static bool CanBeInjected(Slot slot, Type detectedType) => slot.mapObjects.Accessible(detectedType);

        public bool Inject(Slot slot, bool force = false, int direction = 0)
        {
            if ((!slot.mapObjects.Contains(this) && slot.mapObjects.Accessible(GetType())) || force /*强制为true:读档时强制注入*/)
            {
                this.Direction = direction;
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

                UniqueRenderInvoke = delegate
                {
                    Render(config.Prefab, config.Prefabs, slot.slotRender);
                };
                slot.slotRender.OnRender += UniqueRenderInvoke;
                slot.slotRender.OnSlotClicked += _ => OnClick();

                slot.map.OnCreated += _ => OnCreated();

                OnInjected?.Invoke(this, force);

                Awake();
                if (GameManager.current.map == null)
                {
                    //地图尚未完成加载，(说明)此Inject为系统读档操作
                    GameManager.OnMapLoaded += _ => OnEnableInside();
                }
                else
                {
                    //地图已完成加载，(说明)此Inject为玩家操作
                    OnCreated();
                    OnCreatedInside();
                    OnEnableInside();
                }

                slot.OnSlotUpdate?.Invoke(); /*仅供表现层*/
                slot.OnInjected?.Invoke(slot, this); /*仅供逻辑层*/
                slot.slotRender.Refresh();
                return true;
            }
            else
            {
                Debug.LogError("注入失败！");
                return false;
            }
        }

        void OnEnableInside()
        {
            OnEnable();
        }

        #region  集群

        [JsonIgnore]
        protected virtual bool Clustered { get; } = false;

        [JsonProperty]
        public Cluster cluster { get; private set; }
        private void OnCreatedInside()
        {
            FetchCluster();
        }

        /// <summary>
        /// 重新计算集群
        /// </summary>
        /// <param name="deprecated">是否舍弃旧集群</param>
        public void FetchCluster(bool deprecated = false)
        {
            if (Clustered)
            {
                if (cluster == null || deprecated)
                {
                    Cluster foundCluster = null;
                    Action<Cluster> ApplyTo = null;
                    HashSet<MapObject> visited = new();
                    DFS(this);
                    void DFS(MapObject node)
                    {
                        visited.Add(node);

                        ApplyTo += newCluster =>
                        {
                            if (newCluster != node.cluster)
                            {
                                node.cluster = newCluster;
                                newCluster.Push(node);
                            }
                        };

                        if (node.cluster != null && !deprecated)
                        {
                            foundCluster = node.cluster;
                        }

                        foreach (var dir in Slot.上右下左)
                        {
                            Slot nextSlot = map[node.slot.position + dir];
                            if (nextSlot != null)
                            {
                                MapObject nextNode = nextSlot.GetMapObject(GetType());
                                if (nextNode != null && !visited.Contains(nextNode))
                                {
                                    DFS(nextNode);
                                }
                            }
                        }
                    }
                    if (foundCluster == null || deprecated)
                    {
                        foundCluster = new Cluster(GetType());
                    }
                    ApplyTo?.Invoke(foundCluster);
                    foundCluster.Recalculate();
                }
            }
        }

        #endregion

        Action UniqueRenderInvoke;

        public void Unject(bool force = false)
        {
            if (this.slot != null && this.slot.mapObjects.Contains(this))
            {
                if (this is PlaceHolder placeHolder)
                {
                    Debug.Assert(placeHolder.mapObject != null);
                    placeHolder.mapObject.Unject(force);
                }
                else
                {
                    if (CanBeUnjected || force)
                    {
                        UnjectSingle();
                        foreach (PlaceHolder p in PlaceHolders)
                        {
                            p.UnjectSingle();
                        }
                    }
                }
            }
        }

        void UnjectSingle()
        {
            slot.mapObjects.Remove(this);
            slot.slotRender.OnRender -= UniqueRenderInvoke;
            slot.slotRender.OnSlotClicked -= _ => OnClick();
            slot.map.OnCreated -= _ => OnCreated();

            UnityEngine.Object.Destroy(father.gameObject);
            slot.OnSlotUpdate?.Invoke();

            OnDisable();
            OnUnjected?.Invoke(this);
            OnMapObjectUnjected?.Invoke(this);
            this.cluster = null;
            this.slot = null;
        }

        public virtual void OnClick()
        {
        }
        protected virtual void Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            if (prefab != null)
            {
                father.DestroyAllChildren();
                GameObject obj = MonoBehaviour.Instantiate(prefab, father);
                obj.transform.DOScale(Vector3.zero, Settings.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);

                for (int i = 0; i < (Direction + 2) % 4; i++)
                {
                    obj.transform.rotation *= Quaternion.Euler(0, 90, 0);
                }

                //这里为了尽可能不用对原项目进行修改，所以进行了容错处理：
                // if (!obj.GetComponentInChildren<NavMeshModifier>() && !obj.GetComponentInChildren<NavMeshModifierVolume>())
                // {
                //     var navMeshModifier = obj.AddComponent<NavMeshModifier>(); navMeshModifier.overrideArea = true;
                //     navMeshModifier.area = NavMesh.GetAreaFromName("Not Walkable");
                // }

                //GameManager.current?.RefreshNavMesh();
            }
        }
        protected abstract void OnEnable();
        private void Awake()
        {
            
        }
        public abstract bool CanBeUnjected { get; }
        protected abstract void OnDisable();
        protected abstract void OnCreated();

        public bool IsOrPlaceHolder<T>()
        {
            return this is T || (this is PlaceHolder p && p.mapObject is T);
        }
    }

}


