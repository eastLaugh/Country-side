using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

//[JsonConverter(typeof(SlotConvertor))]
public abstract partial class Slot
{
    public static Type[] AllTypes = {typeof(Plain)};//WORKFLOW : 枚举所有类型

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

    public event Action OnSlotUpdate;

    public Slot(Map map, Vector2 position,HashSet<MapObject> mapObjects)
    {
        this.map = map;
        this.position = position;
        this.mapObjects = mapObjects ?? new();

        var offset = new Vector3(0.5f, 0, 0.5f);
        var prefab= GameManager.current.SlotDatabase[GetType()].Prefab;
        gameObject = MonoBehaviour.Instantiate(prefab, GameManager.current.grid.CellToWorld(new Vector3Int(((int)position.x), 0, ((int)position.y))) + offset, Quaternion.identity, GameManager.current.grid.transform); 
        slotRender = gameObject.GetComponent<SlotRender>();
        slotRender.slot = this;

        foreach (var item in mapObjects)
        {
            item.Inject(this,true);//反序列化注入
        }
    }


}