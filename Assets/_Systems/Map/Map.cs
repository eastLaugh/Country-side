using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using static MapObjects;
using System.Linq;

public class Map
{
    public event Action<Map> OnCreated;

    [JsonProperty]
    public List<Lake.LakeEcosystem> lakeEcosystems { get; protected set; } = new();
    [JsonProperty]
    public GameDataWrapper<EconomyVector> economyWrapper { get; private set; }
    [JsonProperty(Order = 9)]
    private Slot[] Slots;
    [JsonProperty(Order = 99)]
    public readonly int MainRandomSeed;
    [JsonProperty(Order = 999)]
    public Vector2Int size { get; private set; }
    [JsonProperty]
    public DateTime dateTime;

    #region 虚拟Slot VSlot 
    //虚拟Slot用以存放某些特别的MapObject或功能性MapObject
    [JsonProperty]
    public readonly VirtualSlot VSlot;

    public class VirtualSlot : Slot
    {
        public VirtualSlot(Map map, Vector2 position, HashSet<MapObject> mapObjects) : base(map, position, mapObjects)
        {
        }
    }

    #endregion
    public Map(Vector2Int size, Slot[] Slots, int RandomSeed, GameDataWrapper<EconomyVector> economyWrapper)
    {
        Debug.Log("Map公共有参构造函数");
        this.size = size;
        this.Slots = Slots;
        this.MainRandomSeed = RandomSeed;
        this.economyWrapper = economyWrapper;
        VSlot = new(this, new Vector2(-1, -1), new());
    }

    [JsonConstructor]
    public Map()
    {
        Debug.Log("Map公共无参构造函数");
    }
    public Slot this[Vector2 pos] => this[(int)pos.x, (int)pos.y];
    public Slot this[int x, int y]
    {
        get
        {
            if (0 <= x && x < size.x && 0 <= y && y < size.y)
                return Slots[x * size.y + y];
            else
                return null;
        }
    }


    public static Map Generate(Vector2Int size, int seed = -1)
    {
        var slots = new Slot[size.x * size.y];

        //设置随机数种子
        if (seed == -1)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        UnityEngine.Random.InitState(seed);


        //创建地图
        GameDataWrapper<EconomyVector> economyWrapper = new GameDataWrapper<EconomyVector>(new() { new SolidMiddleware<EconomyVector>(new EconomyVector(Random.Range(100f, 1000f), Random.Range(10000f, 1000000f), Random.Range(0f, 1f))) });

        var map = new Map(size, slots, seed, economyWrapper);

        //去中心化
        foreach (MapGenerator generator in InitAllGenerators())
        {
            generator.Generate(map, size, slots);
        }

        map.OnCreated?.Invoke(map);
        return map;
    }


    [System.Runtime.Serialization.OnDeserialized]
    void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
    {
        Debug.Log("反序列化完成");
        //OnLoad?.Invoke(this);
    }

    static IEnumerable<MapGenerator> InitAllGenerators()
    {
        IOrderedEnumerable<Type> types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(MapGenerator)) && type.IsDefined(typeof(RegisterAsMapLayer), false)).OrderBy(type => ((RegisterAsMapLayer)type.GetCustomAttributes(typeof(RegisterAsMapLayer), false)[0]).Order);
        foreach (Type type in types)
        {
            yield return (MapGenerator)Activator.CreateInstance(type);
        }
    }
}