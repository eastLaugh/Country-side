using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Newtonsoft.Json.Serialization;
public class Map
{
    public event Action<Map> OnLoad;
    [JsonProperty]
    public EconomyWrapper economy { get; private set; }
    [JsonProperty]
    public readonly int MainRandomSeed;
    [JsonProperty]
    private Slot[] Slots;
    [JsonProperty]
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
    public Slot this[int x, int y] => Slots[x * size.y + y];

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

        //遍历格子
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var newSlot = new Plain(map, new Vector2(i, j), new());

                //按概率生成🌳
                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    new Tree().Inject(newSlot);
                }

                slots[i * size.y + j] = newSlot;
            }
        }

        map.OnLoad?.Invoke(map);
        return map;
    }


    [System.Runtime.Serialization.OnDeserialized]
    void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
    {
        Debug.Log("反序列化完成");
        OnLoad?.Invoke(this);
    }
}