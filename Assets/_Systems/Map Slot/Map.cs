using Newtonsoft.Json;
using UnityEngine;

public class Map
{
    [JsonProperty]
    public readonly int MainRandomSeed;
    [JsonProperty]
    private Slot[] Slots;
    [JsonProperty]
    public Vector2Int size { get; private set; }

    [JsonProperty]
    public Country country { get; private set; }


    public Map(Vector2Int size, Slot[] Slots, int RandomSeed)
    {
        this.size = size;
        this.Slots = Slots;
        this.MainRandomSeed = RandomSeed;
    }

    public Map()
    {

    }
    public Slot this[Vector2 pos] => this[(int)pos.x, (int)pos.y];
    public Slot this[int x, int y] => Slots[x * size.y + y];
}
