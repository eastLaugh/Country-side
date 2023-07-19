using Newtonsoft.Json;
using UnityEngine;

public class Map
{

    [JsonProperty]
    public Economy eco { get; private set; }
    [JsonProperty]
    public readonly int MainRandomSeed;
    [JsonProperty]
    private Slot[] Slots;
    [JsonProperty]
    public Vector2Int size { get; private set; }


    public Map(Vector2Int size, Slot[] Slots, int RandomSeed, Economy eco)
    {
        this.size = size;
        this.Slots = Slots;
        this.MainRandomSeed = RandomSeed;
        this.eco = eco;
    }

    public Map()
    {
    }
    public Slot this[Vector2 pos] => this[(int)pos.x, (int)pos.y];
    public Slot this[int x, int y] => Slots[x * size.y + y];

    public static Map Generate(Vector2Int size, int seed = -1)
    {
        var slots = new Slot[size.x * size.y];
        if (seed == -1)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        UnityEngine.Random.InitState(seed);
        var map = new Map(size, slots, seed, new Economy()
        {
            人口 = 200,
            总收入 = 400000,
            集约程度 = 1f
        });
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var newSlot = new Plain(map, new Vector2(i, j), new());

                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    new Tree(-1).Inject(newSlot);
                }

                slots[i * size.y + j] = newSlot;
            }
        }
        return map;
    }
}
