using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class House : MapObject, MustNotExist<House>
    {
        protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            IconPattern iconPattern = IconPattern.Create(father, Vector3.zero);
            iconPattern.New("Building Icon");

            return base.Render(prefab, prefabs, slotRender);
        }

        [JsonProperty]
        SolidMiddleware<EconomyVector> ConsumptionAndSalary;


        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void OnCreated()
        {
            ConsumptionAndSalary = new SolidMiddleware<EconomyVector>(new EconomyVector(0, -200f, 0, 日均收入: 30f), this, map.economyWrapper);
        }

        public override bool CanBeUnjected => true;
    }
    /// <summary>
    /// 土坯房
    /// </summary>
    public class AdobeHouse : House
    {

    }
    /// <summary>
    /// 砖瓦房
    /// </summary>
    public class TileHouse : House, MustNotExist<TileHouse>
    {

    }
    /// <summary>
    /// 水泥房
    /// </summary>
    public class CementHouse : House
    {

    }
    //桑叶
    public class Mulberry : Resource<Mulberry>
    {

    }

    //纺织厂
    public class TextileMill : ResourceBuilding<Mulberry>
    {
        protected override void OnEnable()
        {

        }

        protected override void OnCreated()
        {
        }

        protected override void OnDisable()
        {
            throw new System.NotImplementedException();
        }
    }


    public class 市中心 : MapObject, MustNotExist<Tree>, IInfoProvider
    {
        public override bool CanBeUnjected => true;

        public void ProvideInfo(Action<string> provide)
        {
            provide("市中心");
        }

        protected override void OnCreated()
        {

        }

        protected override void OnDisable()
        {

        }

        protected override void OnEnable()
        {

        }
    }

    #region 污染源相关 测试Ripple系统
    public class Pollution : MapObject.Virtual, IInfoProvider  //MapObject.Virtual是一个虚拟的MapObject，不会被渲染，且尽量简单
    {
        public void ProvideInfo(Action<string> provide)
        {
            provide("污染");
        }
    }

    public class 污染源 : RippleEffectBuilding<Pollution>  //RippleEffectBuilding是一个泛型类，需要指定泛型参数，这个泛型参数代表Ripple的对象
    {
        public override bool CanBeUnjected => true;

        protected override int RippleRadius => 3;  //假设半径是3格

        protected override void OnEnable()
        {
            IconPattern iconPattern = IconPattern.Create(father, Vector3.zero);
            GameObject label = iconPattern.New("Label");
            label.GetComponent<TMPro.TextMeshProUGUI>().SetText("污染源");
        }
    }

    #endregion

    public class house1 : MapObject
    {
        public override bool CanBeUnjected => true;

        protected override void OnCreated()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void OnEnable()
        {
        }
    }
}

public class Resource<T> : MapObject, MustNotExist<T> where T : Resource<T>
{
    public override bool CanBeUnjected => true;

    protected override void OnEnable()
    {
    }

    protected override void OnDisable()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnCreated()
    {
    }
}


public abstract class ResourceBuilding<R> : MapObject, MustExist<R> where R : Resource<R>
{
    public override bool CanBeUnjected => true;

}

public abstract class RippleEffectBuilding<Eff> : MapObject where Eff : MapObject.Virtual, new()
{
    protected abstract int RippleRadius { get; }


    [JsonProperty]
    protected readonly List<Eff> Effects = new();
    protected override void OnCreated()
    {
        for (int i = -RippleRadius; i <= RippleRadius; i++)
        {
            for (int j = -RippleRadius; j <= RippleRadius; j++)
            {
                if (i * i + j * j <= RippleRadius * RippleRadius)
                {
                    Slot s = map[slot.position + new Vector2(i, j)];
                    if (s != null)
                    {
                        var eff = new Eff();
                        if (eff.Inject(s))
                        {
                            Effects.Add(eff);
                        }
                    }
                }
            }
        }
    }

    protected override void OnDisable()
    {
        foreach (var eff in Effects)
        {
            eff.Unject();
        }
    }

}