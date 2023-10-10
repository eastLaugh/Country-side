using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

//[JsonConverter(typeof(SlotConvertor))]
public abstract partial class Slot
{

    [JsonProperty]
    public Map map { get; private set; }
    [JsonProperty]
    public HashSet<MapObject> mapObjects { get; private set; } = new();
    [JsonProperty]
    public Vector2 position { get; private set; }

    public Vector3 worldPosition { get; private set; }

    [JsonIgnore]
    public SlotRender slotRender { get; private set; }
    [JsonIgnore]
    public GameObject gameObject { get; private set; }

    public event Action OnSlotUpdate; //当Slot更新时触发:被单击、注入建筑物时（频繁被调用，仅用于替代表现层的一些Update消息。相关逻辑实现不要使用这个）
    public event Action<Slot, MapObject> OnInjected;
    internal void InvokeOnSlotUpdate()
    {
        OnSlotUpdate?.Invoke();
    }

    internal string GetInfo(bool debugDetailed = false)
    {
        System.Text.StringBuilder builder = new();
        //从三个地方找信息，slot，slotRender，MapObjects, map
        IEnumerable potentialProviders = new object[] { map, this, slotRender }.Concat(mapObjects);
        foreach (var obj in potentialProviders)
        {
            if (debugDetailed)
            {
                builder.AppendLine("    " + obj.ToString() + " :");
            }

            if (obj is IInfoProvider provider)
            {
                bool appended = false;
                provider.ProvideInfo(str =>
                {
                    builder.Append(str);
                    appended = true;
                });
                if (!appended)
                {
                    builder.Append(obj.ToString() + " :未提供信息");
                }
                builder.AppendLine();
            }
        }
        return builder.ToString();
    }


    [JsonConstructor]
    public Slot(Map map, Vector2 position, HashSet<MapObject> mapObjects)
    {
        this.map = map;
        this.position = position;
        this.mapObjects = mapObjects ?? new();

        var offset = new Vector3(0.5f, 0, 0.5f);
        var prefab = GameManager.current.SlotDatabase[GetType()].Prefab;
        worldPosition = GameManager.current.grid.CellToWorld(new Vector3Int(((int)position.x), 0, ((int)position.y))) + offset;
        gameObject = MonoBehaviour.Instantiate(prefab, worldPosition, Quaternion.identity, GameManager.current.grid.transform);
        slotRender = gameObject.GetComponent<SlotRender>();
        slotRender.slot = this;

        foreach (var item in mapObjects)
        {
            item.Inject(this, true, item.Direction);//反序列化注入
        }
    }


    //上下左右
    public static readonly Vector2[] 上右下左 = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
    public static readonly Vector2[] AllDirections = { new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1) };

    public T GetMapObject<T>() /*where T : MapObject*/ where T : class
    {
        return mapObjects.FirstOrDefault(mapObject => mapObject is T) as T;
    }

    public IEnumerable<T> GetMapObjectsIfPlaceHolder<T>()
    {
        return mapObjects.SelectMany(m =>
        {
            if (m is T t)
                return new T[] { t };
            else if (m is MapObjects.PlaceHolder p && p.mapObject is T t2)
                return new T[] { t2 };
            else
                return new T[] { };
        });
    }

    public MapObject GetMapObject(Type type)
    {
        return mapObjects.FirstOrDefault(mapObject => mapObject.GetType() == type);
    }


    /// <summary>
    /// 获取所有指向该单元格的MapObject
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MapObject> GetReachableMapObject()
    {
        foreach (var dir in Slot.上右下左)
        {
            Slot neighborSlot = map[position + dir];

            if (neighborSlot != null)
            {
                foreach (MapObject neighbor in neighborSlot.mapObjects)
                {
                    if (上右下左[neighbor.Direction] == -dir)
                    {
                        yield return neighbor;
                    }
                }
            }
        }
    }

    internal IEnumerable<T> GetMapObjects<T>() where T : class
    {
        return mapObjects.Where(mapObject => mapObject is T) as IEnumerable<T>;
    }
}