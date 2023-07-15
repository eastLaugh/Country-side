using System;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
}

public class MapObjectAttribute : PropertyAttribute
{

    public Type[] types;
    public MapObjectAttribute(params Type[] types)
    {
        this.types = types;
    }

    public MapObjectAttribute()
    {
        this.types = Slot.MapObject.AllMapObject;
    }


}


