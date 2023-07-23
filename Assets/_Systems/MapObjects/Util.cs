using System.Collections;
using System.Linq;
using System;
//建筑物冲突
public interface IReject<T> where T : Slot.MapObject
{

}
//建筑物相容
public interface IAccept<T> where T : Slot.MapObject
{
    //还没实现
}
public static class IRejectUtil
{

    public static bool Accessible(this IEnumerable set, Type type)
    {
        foreach (var mapObject in set)
        {
            if (typeof(IReject<>).MakeGenericType(type).IsAssignableFrom(mapObject.GetType()))
                return false;
        }
        foreach (Type interf in type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAccept<>)))
        {
            foreach (var mapObject in set)
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