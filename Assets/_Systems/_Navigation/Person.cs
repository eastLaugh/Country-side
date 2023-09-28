using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Person
{
    public readonly string name;

    [JsonProperty]
    public Vector3 worldPosition { get; private set; }
    [JsonProperty]
    public Slot destination { get;  set; }
    public Person(string name)
    {
        this.name = name;
    }
}
