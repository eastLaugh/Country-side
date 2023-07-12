using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

//[JsonConverter(typeof(SlotConvertor))]
public abstract partial class Slot
{
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


    public Slot(Map map, Vector2 position)
    {
        this.map = map;
        this.position = position;

        gameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>(GetType().Name), /*new Vector3(position.x, 0, position.y)*/GameManager.one.grid.CellToWorld(new Vector3Int(((int)position.x), 0, ((int)position.y))), Quaternion.identity, GameManager.one.grid.transform); //暂时用Resources.Load，后续可以试试用Addressable替代
        slotRender = gameObject.GetComponent<SlotRender>();
        slotRender.slot = this;
    }


}