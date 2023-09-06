using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public struct EconomyVector : IDataVector<EconomyVector>
{

    public EconomyVector(float 人口, float 总收入, float 集约程度, float 日均收入=0f)
    {
        Debug.Log("EconomyVector有参构造函数");
        this.人口 = 人口;
        this.总收入 = 总收入;
        this.集约程度 = 集约程度;
        this.日均收入 = 日均收入;
    }
    public float 人口 { get; private set; }
    public float 日均收入 {get; private set;}
    public float 总收入 { get; private set; }

    [JsonIgnore]
    public float 人均可支配收入 => 总收入 / 人口;

    public float 集约程度 { get; private set; }  //这个东西没用，只是放在这里充数的

    [Obsolete][JsonIgnore]
    public float 扶贫资金 => 总收入 * 1f;

    [JsonIgnore]
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
            left.集约程度 + right.集约程度,
            left.日均收入 + right.日均收入
        );
    }

    public static EconomyVector operator -(EconomyVector left, EconomyVector right)
    {
        return new EconomyVector(
            left.人口 - right.人口,
            left.总收入 - right.总收入,
            left.集约程度 - right.集约程度,
            left.日均收入 - right.日均收入
        );
    }
    public EconomyVector Add(EconomyVector other)
    {
        return this + other;
    }
}
