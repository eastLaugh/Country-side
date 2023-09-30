using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public partial class Person
{
    public readonly string name;

    [JsonProperty]
    public Vector3 worldPosition { get; private set; }
    [JsonProperty]
    public Slot destination { get; set; }

    [JsonProperty]
    public List<Slot> PathPoints { get; set; } = new();
    public event Action OnDataUpdate;
    public Person(string name, Vector3 worldPosition)
    {
        this.name = name;
        this.worldPosition = worldPosition;
    }

    public void AddPathPoint(Slot slot)
    {
        PathPoints.Add(slot);
        OnDataUpdate?.Invoke();
    }


}
