using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

//[JsonConverter(typeof(SlotConvertor))]
public abstract partial class Slot
{
    public static Type[] AllTypes = { typeof(Slots.Plain), typeof(Slots.Water) };//WORKFLOW : 枚举所有类型

    [JsonProperty]
    public Map map { get; private set; }
    [JsonProperty]
    public HashSet<MapObject> mapObjects { get; private set; } = new();
    [JsonProperty]
    public Vector2 position { get; private set; }



    [JsonIgnore]
    public SlotRender slotRender { get; private set; }
    [JsonIgnore]
    public GameObject gameObject { get; private set; }

    public event Action OnSlotUpdate; //当Slot更新时触发:被单击、注入建筑物时（频繁被调用，仅用于替代表现层的一些Update消息。相关逻辑实现不要使用这个）
    internal void InvokeOnSlotUpdate()
    {
        OnSlotUpdate?.Invoke();
    }

    internal string GetInfo()
    {
        System.Text.StringBuilder builder = new();
        //从三个地方找信息，slot，slotRender，MapObjects, map
        IEnumerable potentialProviders = new object[] { map, this, slotRender }.Concat(mapObjects);
        foreach (var obj in potentialProviders)
        {
            if (obj is IInfoProvider provider)
            {
                provider.ProvideInfo(str => builder.Append(str));
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
        gameObject = MonoBehaviour.Instantiate(prefab, GameManager.current.grid.CellToWorld(new Vector3Int(((int)position.x), 0, ((int)position.y))) + offset, Quaternion.identity, GameManager.current.grid.transform);
        slotRender = gameObject.GetComponent<SlotRender>();
        slotRender.slot = this;

        foreach (var item in mapObjects)
        {
            item.Inject(this, true);//反序列化注入
        }
    }


    //上下左右
    public static readonly Vector2[] 上下左右 = new Vector2[] { new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 0) };
    public static readonly Vector2[] AllDirections = { new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1) };


    public T GetMapObject<T>() where T : MapObject
    {
        return mapObjects.SingleOrDefault(mapObject => mapObject is T) as T;
    }

}