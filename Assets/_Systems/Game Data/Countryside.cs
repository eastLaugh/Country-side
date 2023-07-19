using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Economy
{

    public float 人口;
    public float 总收入;
    public float 人均可支配收入 => 总收入 / 人口;

    public float 集约程度;
    public float 扶贫资金 => 总收入 * 集约程度;

    public int 城镇等级 => 人口 switch
    {
        0 => 0,
        >= 0 and < 500 => 1,
        >= 500 and <= 1000 => 2,
        > 1000 and <= 1500 => 3,

        _ => throw new System.NotImplementedException()
    };


}
