using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using static Slot;
//建筑物冲突
public interface MustNotExist<T>
{

}
//建筑物相容
public interface MustExist<T>
{

}

public static class MapObjectUtil
{

    public static bool Accessible(this IEnumerable set, Type type)
    {
        foreach (var element in set)
        {
            if (typeof(MustNotExist<>).MakeGenericType(type).IsAssignableFrom(element.GetType()))
                return false;
        }
        foreach (Type interf in type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(MustExist<>)))
        {
            foreach (var element in set)
            {
                if (set.GetType() == interf)
                    goto go_on;
            }
            return false;
        go_on:;
        }
        return true;
    }
}

public interface IInfoProvider
{
    public void ProvideInfo(Action<string> provide);
}