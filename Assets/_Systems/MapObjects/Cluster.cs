using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Cluster
{

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
    public HashSet<Slot.MapObject> mapObjects {get;private set;}= new();

    public void Push(Slot.MapObject targetMapObject)
    {
        if (targetMapObject.GetType() != MapObjectType)
        {
            throw new Exception("类型不匹配");
        }
        mapObjects.Add(targetMapObject);
    }

}

public interface IInCluster<out M> where M : Slot.MapObject
{
    object GetCluster();

}