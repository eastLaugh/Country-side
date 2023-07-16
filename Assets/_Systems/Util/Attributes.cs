using System;
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
        this.types = mainClass.GetField("AllTypes", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null) as Type[];

        //如果在这里报错，检查一下数据库的字典类型是否拥有AllTypes
    }


}


