using System.Collections;
using System.Linq;
using System;
//建筑物冲突
public interface MustNotExist<in T>
{

}
//建筑物相容
public interface MustExist<in T>
{

}

public static class TypeUtil
{

    public static bool Accessible(this IEnumerable set, Type type)
    {
        //foreach (var element in set)
        //{
        //    if (typeof(MustNotExist<>).MakeGenericType(type).IsAssignableFrom(FindTypeIfPlaceHolder(element)))
        //        return false;
        //}

        foreach (Type interf in type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(MustNotExist<>)))
        {
            foreach (var element in set)
            {
                if (interf.GetGenericArguments().First().IsAssignableFrom(FindTypeIfPlaceHolder(element)))
                    return false;
            }
        }

        foreach (Type interf in type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(MustExist<>)))
        {
            foreach (var element in set)
            {
                if (interf.GetGenericArguments().First().IsAssignableFrom(FindTypeIfPlaceHolder(element)))
                    goto go_on;
            }
            return false;
        go_on:;
        }
        return true;
    }

    public static Type FindTypeIfPlaceHolder(object unknownType)
    {
        if (unknownType is MapObjects.PlaceHolder placeholder)
        {
            return placeholder.mapObject.GetType();
        }
        else
        {
            return unknownType.GetType();
        }
    }
}

public interface IInfoProvider
{
    public void ProvideInfo(Action<string> provide);
}