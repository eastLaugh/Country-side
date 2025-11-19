using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using static Slot;

public class Cluster
{
    ~Cluster()
    {
        Debug.Log("Cluster被GC.");
    }
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

            if (!UnjectedEventRegistered.Contains(targetMapObject))
            {
                targetMapObject.OnMapObjectUnjected += OnMapObjectUnjected;
                UnjectedEventRegistered.Add(targetMapObject);
            }

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
    [JsonIgnore]
    HashSet<MapObject> UnjectedEventRegistered = new();
    public void Recalculate()
    {
        foreach (var mapObject in mapObjects)
        {
            if (!UnjectedEventRegistered.Contains(mapObject))
            {
                mapObject.OnMapObjectUnjected += OnMapObjectUnjected;
                UnjectedEventRegistered.Add(mapObject);
            }

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
        if (slot.GetMapObject(MapObjectType) != null)
        {
            //如果是集群内部的话
            slot.OnInjected -= OnInjected;
        }
        else if (slot.map[slot.position + Slot.上右下左[mapObject.Direction]].GetMapObject(MapObjectType) != null)
        {
            //集群边缘外一层的话
            if (!ReachableMapObjects.Contains(mapObject))
            {
                ReachableMapObjects.Add(mapObject);
                mapObject.OnMapObjectUnjected += OnReachableMapObjectUnjected;
                OnReach?.Invoke(mapObject);
            }
        }
    }

    private void OnReachableMapObjectUnjected(MapObject mapObject)
    {
        mapObject.OnMapObjectUnjected -= OnReachableMapObjectUnjected;
        if (ReachableMapObjects.Contains(mapObject))
        {
            ReachableMapObjects.Remove(mapObject);
        }
    }

    private void OnMapObjectUnjected(MapObject mapObject)
    {
        mapObject.OnMapObjectUnjected -= OnMapObjectUnjected;
        UnjectedEventRegistered.Remove(mapObject);

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

    internal void Unbind(MapObject mapobject, Cluster oldCluster)
    {
        mapobject.OnMapObjectUnjected -= oldCluster.OnMapObjectUnjected;

    }
}
