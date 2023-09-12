using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
public interface IDataVector<T> where T : IDataVector<T>
{
    T Add(T other);

    T Minus(T other);
}


public class GameDataWrapper<T> : IMiddleware<T> where T : IDataVector<T>
{
    [JsonIgnore] //不要序列化是因为，我想要在每次反序列化后重新计算current
    T current;
    [JsonProperty]
    public HashSet<IMiddleware<T>> Middlewares { get; protected set; } = new();

    [JsonConstructor]
    public GameDataWrapper()
    {
    }

    public GameDataWrapper(params IMiddleware<T>[] middlewares)
    {
        foreach (var middleware in middlewares)
        {
            AddMiddleware(middleware);
        }

    }

    [System.Runtime.Serialization.OnDeserialized]
    void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
    {
        current = default;
        foreach (var middleware in Middlewares)
        {
            AddMiddleware(middleware, true);
        }
    }

    public void AddMiddleware(IMiddleware<T> middleware, bool force = false)
    {
        if (Middlewares.Contains(middleware) && !force)
        {
            Debug.LogError("重复添加中间件");
            return;
        }
        else
        {
            current = current.Add(middleware.GetValue());
            Middlewares.Add(middleware);
            middleware.OnMiddlewareUpdated += OnMiddlewaresUpdated;
            OnMiddlewareUpdated?.Invoke(current);
        }


    }

    private void OnMiddlewaresUpdated(T t)
    {
        current = current.Add(t);
        OnMiddlewareUpdated?.Invoke(t);
    }

    public void DiscardMiddleware(IMiddleware<T> middleware)
    {
        if (Middlewares.Contains(middleware))
        {
            T dieValue = middleware.GetValue();
            current = current.Minus(dieValue);
            Middlewares.Remove(middleware);
            middleware.OnMiddlewareUpdated -= OnMiddlewaresUpdated;

            OnMiddlewareUpdated?.Invoke(default(T).Minus(dieValue));
        }
    }

    public T GetValue()
    {
        return current;
    }

    public object GetHost()
    {
        return null;
    }

    public event Action<T> OnMiddlewareUpdated;
}


//该中间件表示一个定值
public class SolidMiddleware<T> : IMiddleware<T> where T : IDataVector<T>
{
    public event Action<T> OnMiddlewareUpdated;

    [JsonProperty]
    public T solidValue { get; private set; }

    [JsonProperty]
    public readonly object Host;

    public SolidMiddleware(T solidValue, object host)
    {
        this.solidValue = solidValue;
        this.Host = host;
    }

    public object GetHost()
    {
        return Host;
    }

    public T GetValue()
    {
        return solidValue;  //返回定值
    }

    void UpdateValue(T solidValue)
    {
        OnMiddlewareUpdated?.Invoke(solidValue.Minus(this.solidValue));
        this.solidValue = solidValue;
    }

    public T Value
    {
        set
        {
            UpdateValue(value);
        }
    }

}

public interface IMiddleware<T>
{
    event Action<T> OnMiddlewareUpdated;
    T GetValue();
    object GetHost();
}