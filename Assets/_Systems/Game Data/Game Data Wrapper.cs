using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public interface IDataVector<T> where T : IDataVector<T>
{
    T Add(T other);
}
public struct EconomyVector : IDataVector<EconomyVector>
{

    public float 人口 { get; private set; }

    public EconomyVector(float 人口, float 总收入, float 集约程度)
    {
        Debug.Log("EconomyVector有参构造函数");
        this.人口 = 人口;
        this.总收入 = 总收入;
        this.集约程度 = 集约程度;
    }
    public float 总收入 { get; private set; }
    public float 人均可支配收入 => 总收入 / 人口;

    public float 集约程度 { get; private set; }

    public float 扶贫资金 => 总收入 * 集约程度;

    public int 城镇等级 => 人口 switch
    {
        0 => 0,
        >= 0 and < 500 => 1,
        >= 500 and <= 1000 => 2,
        > 1000 and <= 1500 => 3,

        _ => throw new System.NotImplementedException()
    };

    public static EconomyVector operator +(EconomyVector left, EconomyVector right)
    {
        return new EconomyVector(
            left.人口 + right.人口,
            left.总收入 + right.总收入,
            left.集约程度 + right.集约程度
        );
    }

    public static EconomyVector operator -(EconomyVector left, EconomyVector right)
    {
        return new EconomyVector(
            left.人口 - right.人口,
            left.总收入 - right.总收入,
            left.集约程度 - right.集约程度
        );
    }
    public EconomyVector Add(EconomyVector other)
    {
        return this + other;
    }
}


public class GameDataWrapper<T> : IMiddleware<T> where T : IDataVector<T>
{
    [JsonProperty]
    T current;
    [JsonProperty]
    protected readonly List<IMiddleware<T>> Middlewares = new();

    [JsonConstructor]
    public GameDataWrapper(List<IMiddleware<T>> middlewares)
    {
        Debug.Log("GameDataWrapper有参构造函数");

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

    public event Action<T> OnDataUpdated;
}

public abstract class Middleware<T> : IMiddleware<T>
{
    public abstract T GetValue();
    public abstract T Process(T data);
}


//该中间件表示一个定值
public class SolidMiddleware<T> : Middleware<T>
{
    public readonly T solidValue;
    public SolidMiddleware(T solidValue)
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
}