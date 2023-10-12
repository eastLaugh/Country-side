using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using static Slot;

public class Cluster
{
    public event Action<MapObject> OnReach;

    [JsonProperty]
    readonly Type MapObjectType;

    public Cluster(Type mapObjectType)
    {
        MapObjectType = mapObjectType;
    }

    [JsonConstructor]
    public Cluster()
    {
    }

    [JsonProperty]
    public HashSet<MapObject> mapObjects { get; private set; } = new();

    public void Push(Slot.MapObject targetMapObject)
    {
        if (!MapObjectType.IsAssignableFrom(targetMapObject.GetType()))
        {
            throw new Exception("类型不匹配");
        }
        else
        {
            mapObjects.Add(targetMapObject);
            targetMapObject.OnMapObjectUnjected += OnMapObjectUnjected;
        }
    }

    [System.Runtime.Serialization.OnDeserialized]
    void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
    {
        //反序列化完成回调
        GameManager.OnMapLoaded += OnMapLoaded;

    }

    private void OnMapLoaded(Map map)
    {
        Recalculate();
        GameManager.OnMapLoaded -= OnMapLoaded;
    }

    public void Recalculate()
    {
        foreach (var mapObject in mapObjects)
        {
            mapObject.OnMapObjectUnjected += OnMapObjectUnjected;
            foreach (var dir in Slot.上右下左)
            {
                var target = mapObject.slot.map[mapObject.slot.position + dir];
                if (target != null)
                {
                    if (target.GetMapObject(MapObjectType) == null)
                    {
                        //此为集群边缘外一层
                        target.OnInjected += OnInjected;
                        foreach (var existedMapObject in target.mapObjects)
                        {
                            OnInjected(target, existedMapObject);
                        }
                    }
                }
            }
        }
    }

    [JsonProperty]
    public HashSet<MapObject> ReachableMapObjects = new();
    private void OnInjected(Slot slot, MapObject mapObject)
    {
        //TODO:这里有问题，如果是集群边缘外一层的话
        if (slot.GetMapObject(MapObjectType) != null)
        {
            //如果是集群内部的话
            slot.OnInjected -= OnInjected;
        }
        else if (slot.map[slot.position + Slot.上右下左[mapObject.Direction]].GetMapObject(MapObjectType) != null)
        {
            if (!ReachableMapObjects.Contains(mapObject))
            {
                ReachableMapObjects.Add(mapObject);
                OnReach?.Invoke(mapObject);
            }
        }
    }

    private void OnMapObjectUnjected(MapObject mapObject)
    {
        mapObject.OnMapObjectUnjected -= OnMapObjectUnjected;
        if (mapObjects.Contains(mapObject))
        {
            mapObjects.Remove(mapObject);
            foreach (var dir in Slot.上右下左)
            {
                var target = mapObject.slot.map[mapObject.slot.position + dir];
                if (target != null)
                {
                    var clusterObject = target.GetMapObject(MapObjectType);
                    if (clusterObject != null)
                    {
                        clusterObject.FetchCluster(true);
                    }
                }
            }
        }
    }
}
