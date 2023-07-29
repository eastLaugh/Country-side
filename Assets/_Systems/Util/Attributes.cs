using System;
using System.Reflection;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
}

public class TypeToString : PropertyAttribute
{
    public Type[] types;
    public TypeToString(params Type[] types)
    {
        this.types = types;
    }


    public TypeToString(Type mainClass)
    {
        this.types = mainClass.GetNestedTypes();
    }


}


