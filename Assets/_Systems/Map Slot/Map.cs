using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using static MapObjects;

public class Map
{
    public event Action<Map> OnCreated;

    [JsonProperty]
    public List<Lake.LakeEcosystem> lakeEcosystems { get; protected set; } = new();
    [JsonProperty]
    public EconomyWrapper economy { get; private set; }
    [JsonProperty(Order = 9)]
    private Slot[] Slots;
    [JsonProperty(Order = 99)]
    public readonly int MainRandomSeed;
    [JsonProperty(Order = 999)]
    public Vector2Int size { get; private set; }

    public Map(Vector2Int size, Slot[] Slots, int RandomSeed, EconomyWrapper economyWrapper)
    {
        Debug.Log("Map公共有参构造函数");
        this.size = size;
        this.Slots = Slots;
        this.MainRandomSeed = RandomSeed;
        this.economy = economyWrapper;
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
        var map = new Map(size, slots, seed,
            new EconomyWrapper(new EconomyVector(Random.Range(100f, 1000f), Random.Range(10000f, 1000000f), Random.Range(0f, 1f)), new() { new UniversalMiddleware<EconomyVector>() }));

        float PerlinOffsetX = Random.Range(0f, 1f);
        float PerlinOffsetY = Random.Range(0f, 1f);

        //遍历格子
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                float noise = Mathf.PerlinNoise(PerlinOffsetX + i * 0.1f, PerlinOffsetY + j * 0.1f);
                Slot newSlot;

                if (noise > 0.2f)
                {
                    newSlot = new Slots.Plain(map, new Vector2(i, j), new());
                    if (noise > 0.7f)
                    {
                        new MapObjects.Tree().Inject(newSlot);
                    }
                }
                else
                {
                    newSlot = new Slots.Water(map, new Vector2(i, j), new());
                }
                slots[i * size.y + j] = newSlot;
            }
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
}