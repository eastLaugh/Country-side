using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Newtonsoft.Json;
using UnityEngine;
public interface IDataVector<T> where T : IDataVector<T>
{
    T Add(T other);
}


public class GameDataWrapper<T> : IMiddleware<T> where T : IDataVector<T>
{
    [JsonProperty]
    T current;
    [JsonProperty]
    public List<IMiddleware<T>> Middlewares {get;protected set;}= new();

    [JsonConstructor]
    public GameDataWrapper(List<IMiddleware<T>> middlewares)
    {

        foreach (var middleware in middlewares)
        {
            AddMiddleware(middleware);
        }

    }

    public void AddMiddleware(IMiddleware<T> middleware)
    {
        Middlewares.Add(middleware);

        T origin = middleware.GetValue();
        T final = middleware.Process(origin);
        current = current.Add(final);

        OnDataUpdated?.Invoke(current);
    }

    public T Process(T data)
    {
        return data;   //不做任何处理
    }

    public T GetValue()
    {
        return current;
    }

    public object GetHost()
    {
        return null;
    }

    public event Action<T> OnDataUpdated;
}

public abstract class Middleware<T> : IMiddleware<T>
{
    public abstract T GetValue();
    public abstract T Process(T data);

    public object GetHost()
    {
        return Host;
    }

    public readonly object Host;

    protected Middleware(object host)
    {
        this.Host = host;
    }
}


//该中间件表示一个定值
public class SolidMiddleware<T> : Middleware<T>
{
    public readonly T solidValue;
    public SolidMiddleware(T solidValue,object host):base(host)
    {
        this.solidValue = solidValue;
    }
    public override T GetValue()
    {
        return solidValue;  //返回定值
    }

    public override T Process(T data)
    {
        return data;   //不做任何处理
    }
}

public interface IMiddleware<T>
{
    T Process(T data);
    T GetValue();
    object GetHost();
}