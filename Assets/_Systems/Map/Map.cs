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
    [JsonProperty]
    private Slot[] Slots;
    [JsonProperty]
    public readonly int MainRandomSeed;
    [JsonProperty]
    public Vector2Int size { get; private set; }
    [JsonProperty]
    public DateTime dateTime;
    [JsonProperty]
    public List<string> FinishedAssignments = new List<string>();
    [JsonProperty]
    public List<string> UnlockedAssignments = new List<string>();
    //[JsonProperty]
    public Dictionary<string, int> BuildingsNum = new Dictionary<string, int>();


    public Map(Vector2Int size, Slot[] Slots, int RandomSeed, GameDataWrapper<EconomyVector> economyWrapper)
    {
        this.size = size;
        this.Slots = Slots;
        this.MainRandomSeed = RandomSeed;
        this.economyWrapper = economyWrapper;
    }

    [JsonConstructor]
    public Map()
    {
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
        Map map = null;
        GameDataWrapper<EconomyVector> economyWrapper = new();

        new SolidMiddleware<EconomyVector>(new EconomyVector(Random.Range(100f, 1000f), Random.Range(10000f, 1000000f), Random.Range(0f, 1f), 0f), map, economyWrapper);

        map = new Map(size, slots, seed, economyWrapper);

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
        //反序列化完成回调
    }

    static IEnumerable<MapGenerator> InitAllGenerators()
    {
        IOrderedEnumerable<Type> types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(MapGenerator)) && type.IsDefined(typeof(RegisterAsMapLayer), false)).OrderBy(type => ((RegisterAsMapLayer)type.GetCustomAttributes(typeof(RegisterAsMapLayer), false)[0]).Order);
        foreach (Type type in types)
        {
            yield return (MapGenerator)Activator.CreateInstance(type);
        }
    }


    public int GetBuildingNum(string name)
    {
        if (BuildingsNum.ContainsKey(name))
        {
            return BuildingsNum[name];
        }          
        else
        {
            BuildingsNum.Add(name, 0);  
            return 0;
        }
    }
}