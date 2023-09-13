using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ink.Runtime;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
public interface IDataVector<T> where T : IDataVector<T>
{
    T Add(T other);

    T Minus(T other);

    T Multiply(T other);
}


public class GameDataWrapper<T> where T : IDataVector<T>
{
    [JsonProperty]
    public T current { get; private set; }
    [JsonProperty]
    public HashSet<SolidMiddleware<T>> Middlewares { get; protected set; } = new();

    [JsonConstructor]
    public GameDataWrapper()
    {
    }

    public GameDataWrapper(params SolidMiddleware<T>[] middlewares)
    {
        foreach (var middleware in middlewares)
        {
            AddMiddleware(middleware);
        }

    }

    [System.Runtime.Serialization.OnDeserialized]
    void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
    {
        foreach (var middleware in Middlewares)
        {
            AddMiddleware(middleware, true);
        }
    }

    public void AddMiddleware(SolidMiddleware<T> middleware, bool force = false)
    {
        if (Middlewares.Contains(middleware) && !force)
        {
            Debug.LogError("重复添加中间件");
            return;
        }
        else
        {
            Middlewares.Add(middleware);
            middleware.OnMiddlewareUpdated += OnMiddlewaresUpdated;
            middleware.Recaculate();
            //OnMiddlewareUpdated?.Invoke(current);
        }


    }

    private void OnMiddlewaresUpdated(T t)
    {
        current = current.Add(t);
        OnMiddlewareUpdated?.Invoke(t);
    }

    // public void DiscardMiddleware(IMiddleware<T> middleware)
    // {
    //     if (Middlewares.Contains(middleware))
    //     {
    //         T dieValue = middleware.GetValue();
    //         current = current.Minus(dieValue);
    //         Middlewares.Remove(middleware);
    //         middleware.OnMiddlewareUpdated -= OnMiddlewaresUpdated;

    //         OnMiddlewareUpdated?.Invoke(default(T).Minus(dieValue));
    //     }
    // }



    public object GetHost()
    {
        return null;
    }

    public event Action<T> OnMiddlewareUpdated;
}


//该中间件表示一个定值
public class SolidMiddleware<T> where T : IDataVector<T>
{
    public event Action<T> OnMiddlewareUpdated;

    [JsonProperty]
    public T solidValue { get; private set; }

    [JsonProperty]
    public T currentValue { get; private set; }

    [JsonProperty]
    public readonly object Host;

    [JsonProperty]
    public List<CPU> CPUs { get; private set; } = new();

//先加再乘处理单元
public struct CPU
    {
        public T Addition;
        public T Multiplication;

        public bool Multipliable;

    }
    public SolidMiddleware(T solidValue, object host)
    {
        this.solidValue = solidValue;
        this.Host = host;
    }

    public object GetHost()
    {
        return Host;
    }

    public void Recaculate()
    {
        T temp = solidValue;
        foreach (var cpu in CPUs)
        {
            temp = temp.Add(cpu.Addition);
            if (cpu.Multipliable)
            {
                temp = temp.Multiply(cpu.Multiplication);
            }
        }

        OnMiddlewareUpdated?.Invoke(temp.Minus(currentValue));
        currentValue = temp;
    }

    void UpdateValue(T solidValue)
    {
        this.solidValue = solidValue;
        Recaculate();
    }

    public T Value
    {
        set
        {
            UpdateValue(value);
        }
        get => currentValue;
    }

    public void AddCPU(CPU cpu)
    {
        CPUs.Add(cpu);
        Recaculate();
    }

    public void RemoveCPU(CPU cpu)
    {
        CPUs.Remove(cpu);
        Recaculate();
    }
}

// public interface IMiddleware<T>
// {
//     event Action<T> OnMiddlewareUpdated;
//     object GetHost();

// }