using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public struct EconomyVector
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


}

public class EconomyWrapper : GameDataWrapper<EconomyVector>
{
    public EconomyWrapper( EconomyVector current, List<Middleware<EconomyVector>> middlewares) : base(current, middlewares)
    {
        Debug.Log("EconomyWrapper有参构造函数");
    }
}

public class GameDataWrapper<T>
{
    [JsonProperty]
    protected T current;
    [JsonProperty]
    protected readonly List<Middleware<T>> middlewares;

    public GameDataWrapper(T current, List<Middleware<T>> middlewares)
    {
        Debug.Log("GameDataWrapper有参构造函数");

        this.current = current;
        this.middlewares = middlewares;
    }
}

public abstract class Middleware<T>
{
    public abstract T Process(T standard);
}

public class UniversalMiddleware<T> : Middleware<T>
{
    public override T Process(T standard)
    {
        return default(T);
    }
}