using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static Slot;

public class Cluster
{

    [JsonProperty]
    readonly Type MapObjectType;
    /// <summary>
    /// 获取所有可达的地图对象
    /// </summary>
    /// <returns></returns>
    public HashSet<MapObject> GetReachableMapObject()
    {
        HashSet<MapObject> tmp = new();
        foreach (var mapObject in mapObjects)
        {
            foreach (var reachable in mapObject.slot.GetReachableMapObject())
            {
                tmp.Add(reachable);
            }
        }
        return tmp;
    }


    public Cluster(Type mapObjectType)
    {
        MapObjectType = mapObjectType;
    }

    [JsonConstructor]
    public Cluster()
    {
    }

    [JsonProperty]
    public HashSet<Slot.MapObject> mapObjects { get; private set; } = new();

    public void Push(Slot.MapObject targetMapObject)
    {
        if (targetMapObject.GetType() != MapObjectType)
        {
            throw new Exception("类型不匹配");
        }
        mapObjects.Add(targetMapObject);
    }

}
