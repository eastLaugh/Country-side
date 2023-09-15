using System;
using System.Collections.Generic;
using Ink.Runtime;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    #region 工业设施
    public class Factory
    {

    }
    #endregion
    #region 农业设施
    public abstract class Farm : MapObject, IConstruction
    {
        public override bool CanBeUnjected => true;
        public abstract float Cost { get; }
        public abstract string Name { get; }
        public float Profit => m_profit.currentValue.m_value;

        public ConstructType constructType => ConstructType.Farm;

        [JsonProperty]
        protected SolidMiddleware<Float> m_profit;
        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
        }

        protected override void OnDisable()
        {

        }

        protected override void OnEnable()
        {

        }
    }
    public class WheatFarm : Farm
    {
        public override float Cost => 5f;

        public override string Name => "小麦田";

        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(0.03f));
            map.Farms.Add(this);
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }
    }
    public class RiceFarm : Farm
    {
        public override float Cost => 6f;

        public override string Name => "水稻田";

        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(0.035f));
            map.Farms.Add(this);
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }

    }
    public class CuttonFarm : Farm
    {
        public override float Cost => 10f;

        public override string Name => "棉花田";

        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(1.2f));
            map.Farms.Add(this);
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }

    }
    #endregion

    #region 住宅楼
    /// <summary>
    /// 住宅基类
    /// </summary>
<<<<<<< Updated upstream
    public abstract class House : MapObject, MustNotExist<House>, IConstruction, IInfoProvider
=======
    public abstract class House : MapObject, MustNotExist<House>, IConstruction , MustNotExist<IConstruction>
>>>>>>> Stashed changes
    {
        //人口容量
        [JsonProperty] protected SolidMiddleware<Int> m_capacity;

        public int Capacity => m_capacity.currentValue.m_value;
        public override bool CanBeUnjected => true;
        public abstract float Cost { get; }
        public abstract string Name { get; }
        public ConstructType constructType => ConstructType.House;

        public void ProvideInfo(Action<string> provide)
        {
            provide($"当前朝向{Direction}");

            foreach (var dir in 上右下左)
            {
                Road r = map[slot.position + dir].GetMapObject<Road>();
                if (r != null)
                {
                    foreach (MapObject reachable in r.cluster.GetReachableMapObject())
                    {
                        if (reachable != this && reachable is House)
                        {
                            provide($"可到达 {reachable.slot.position}");
                        }
                    }
                }
            }
        }

        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
            //Debug.Log(map.MainData.Money);
        }
    }

    
    /// <summary>
    /// 土坯房
    /// </summary>
    public class AdobeHouse : House
    {
        public override float Cost => 8;

        public override string Name => "土坯房";

        protected override void OnCreated()
        {
            base.OnCreated();
            m_capacity = new SolidMiddleware<Int>(new Int(10));
            map.Houses.Add(this);
        }

        protected override void OnDisable()
        {
            map.Houses.Remove(this);
        }

        protected override void OnEnable()
        {

        }
    }
    /// <summary>
    /// 砖瓦房
    /// </summary>
    public class TileHouse : House, MustExist<AdobeHouse>
    {
        public override float Cost => 25;

        public override string Name => "砖瓦房";

        protected override void OnCreated()
        {
            base.OnCreated();
            m_capacity = new SolidMiddleware<Int>(new Int(30));
            map.Houses.Add(this);
        }

        protected override void OnDisable()
        {
            map.Houses.Remove(this);
        }

        protected override void OnEnable()
        {

        }
    }
    /// <summary>
    /// 水泥房
    /// </summary>
    public class CementHouse : House, MustExist<TileHouse>
    {
        public override float Cost => 80;

        public override string Name => "水泥房";

        protected override void OnCreated()
        {
            base.OnCreated();
            m_capacity = new SolidMiddleware<Int>(new Int(60));
            map.Houses.Add(this);

        }

        protected override void OnDisable()
        {
            map.Houses.Remove(this);
        }

        protected override void OnEnable()
        {

        }
    }
    #endregion

    #region 特殊建筑
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
    #endregion

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
public interface IConstruction
{
    public float Cost { get; }
    public string Name { get; }
    public ConstructType constructType { get; }
}

public enum ConstructType
{
    House, Farm, Factory, Supply, Road
}



#region 注释
/*
     public class House : MapObject, MustNotExist<House>
    {

        static House lastHouse;




        [JsonProperty]
        SolidMiddleware<GameDataVector> ConsumptionAndSalary;
        protected override void OnCreated()
        {
            ConsumptionAndSalary = new SolidMiddleware<GameDataVector>(new GameDataVector(dailyIncome: 30, Money: -2000), this);
            map.economyWrapper.AddMiddleware(ConsumptionAndSalary);

        }

        protected override void OnEnable()
        {
            // InfoWindow.Create(slot.worldPosition.ToString());
            if (lastHouse != null)
            {
                ArrowRender.NewArrow(lastHouse.slot.worldPosition, slot.worldPosition);
            }
            lastHouse = this;
        }

        protected override void OnDisable()
        {

        }

        public override bool CanBeUnjected => true;
    }*/
#endregion