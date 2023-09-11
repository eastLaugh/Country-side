using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Newtonsoft.Json;
using UnityEngine;
public interface IDataVector<T> where T : IDataVector<T>
{
    T Add(T other);
    T Minus(T other);
}


public class GameDataWrapper<T> where T : IDataVector<T>
{
    [JsonProperty]
    T current;
    [JsonProperty]
    public HashSet<SolidMiddleware<T>> Middlewares { get; protected set; } = new();

    [JsonConstructor]
    public GameDataWrapper()
    {

    }

    public void AddMiddleware(SolidMiddleware<T> middleware)
    {
        Middlewares.Add(middleware);

        current = current.Add(middleware.SolidValue);

        OnDataUpdated?.Invoke(current);
    }

    public void RemoveMiddleware(SolidMiddleware<T> middleware)
    {
        Middlewares.Remove(middleware);

        current = current.Minus(middleware.SolidValue);

        OnDataUpdated?.Invoke(current);
    }

    public T GetValue()
    {
        return current;
    }


    public event Action<T> OnDataUpdated;
}


//该中间件表示一个定值
public class SolidMiddleware<T> where T : IDataVector<T>
{
    public readonly GameDataWrapper<T> Wrapper;

    public readonly T SolidValue;
    public readonly object Host;
    public SolidMiddleware(T solidValue, object host, GameDataWrapper<T> wrapper)
    {
        this.SolidValue = solidValue;
        this.Host = host;
        this.Wrapper = wrapper;

        wrapper.AddMiddleware(this);
    }

    SolidMiddleware<T> UpdateValue(T solidValue)
    {
        Wrapper.RemoveMiddleware(this);
        var newMiddleware = new SolidMiddleware<T>(solidValue, Host, Wrapper);
        Wrapper.AddMiddleware(newMiddleware);
        return newMiddleware;
    }

    void Discard()
    {
        Wrapper.RemoveMiddleware(this);
    }

}
