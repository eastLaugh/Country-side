using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
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

    [System.Runtime.Serialization.OnDeserialized]
    void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
    {
        //反序列化完成回调
        Recalculate();
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
                    }
                }
            }
        }
    }

    [JsonProperty]
    public HashSet<MapObject> ReachableMapObjects = new();
    private void OnInjected(Slot slot, MapObject @object)
    {
        //TODO:这里有问题，如果是集群边缘外一层的话
        if (slot.GetMapObject(MapObjectType) != null)
        {
            //如果是集群内部的话
            slot.OnInjected -= OnInjected;
        }
        else
        {

        }
    }

    private void OnMapObjectUnjected(MapObject mapObject)
    {
        Debug.Log("Cluster.OnMapObjectUnjected");
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
