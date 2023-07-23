using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class SlotDebugConvertor : JsonConverter<Slot>
{

    public override bool CanWrite => true;

    public override Slot ReadJson(JsonReader reader, Type objectType, Slot existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, Slot value, JsonSerializer serializer)
    {
        //不会写
    }
}

public class MapPropertyIgnore : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        if (property.PropertyType == typeof(Map))
        {
            property.ShouldSerialize = _ => false;
        }
        return property;
    }
}