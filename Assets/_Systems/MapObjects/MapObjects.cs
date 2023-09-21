using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
    public abstract class Farm : MapObject, IConstruction, MustNotExist<IConstruction>
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
    //public class CuttonFarm : Farm
    //{
    //    public override float Cost => 10f;
    //
    //    public override string Name => "棉花田";
    //
    //    protected override void OnCreated()
    //    {
    //        base.OnCreated();
    //        m_profit = new SolidMiddleware<Float>(new Float(1.2f));
    //        map.Farms.Add(this);
    //    }
    //    protected override void OnDisable()
    //    {
    //        map.Farms.Remove(this);
    //    }
    //
    //}
    #endregion

    #region 住宅楼
    /// <summary>
    /// 住宅基类
    /// </summary>
    public abstract class House : MapObject, IConstruction, MustNotExist<IConstruction>, IInfoProvider
    {
        //人口容量
        [JsonProperty] protected SolidMiddleware<Int> m_capacity;

        public int Capacity => m_capacity.currentValue.m_value;
        public override bool CanBeUnjected => true;
        public abstract float Cost { get; }
        public abstract string Name { get; }
        public string Warning;
        public ConstructType constructType => ConstructType.House;

        public void CheckConnection()
        {
            Road r = map[slot.position + 上右下左[Direction]]?.GetMapObject<Road>();
            bool Connected = false;
            if (r != null)
            {
                foreach (MapObject reachable in r.cluster.GetReachableMapObject())
                {
                    if (reachable != this && reachable is Administration)
                    {
                        Connected = true;
                        break;
                    }
                }
            }

            var ConnectCPU = m_capacity.CPUs.Find((cpu) => { return cpu.name == "未连通"; });
            if (!Connected && ConnectCPU.name == null)
            {
                m_capacity.AddCPU(new SolidMiddleware<Int>.CPU
                { name = "未连通", Addition = new Int(0), Multipliable = true, Multiplication = new Int(0) });
                Warning = "未连通";

            }
            else if (Connected && ConnectCPU.name != null)
            {
                m_capacity.RemoveCPU(ConnectCPU);
                Warning = null;
            }
        }

        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
            //Debug.Log(map.MainData.Money);
        }
        protected override void OnEnable()
        {


            CheckConnection();
            EventHandler.DayPass += CheckConnection;
            GameManager.OnMapUnloaded += OnMapUnloaded;
        }
        private void OnMapUnloaded()
        {
            GameManager.OnMapUnloaded -= OnMapUnloaded;
            EventHandler.DayPass -= CheckConnection;
        }

        public override void OnClick()
        {
            if (MapObjectDatabase.main[GetType()].Size == Vector2Int.one)
            {
                //转变朝向
                Direction = (Direction + 1) % 4;
                slot.slotRender.Refresh();
            }
            else
            {
                //暂不支持多格建筑物转向
            }
        }

        public void ProvideInfo(Action<string> provide)
        {
            SlotRender.OnAnySlotExit += ResetArrow;

            void ResetArrow(SlotRender _)
            {
                for (int i = lastArrowRender.Count - 1; i >= 0; i--)
                {
                    MonoBehaviour.Destroy(lastArrowRender[i].gameObject);
                }
                lastArrowRender.Clear();

                SlotRender.OnAnySlotExit -= ResetArrow;
            }

            Road r = map[slot.position + 上右下左[Direction]]?.GetMapObject<Road>();

            if (r != null)
            {
                provide("道路");
                foreach (MapObject reachable in r.cluster.RechableMapObjects)
                {
                    if (reachable != this)
                    {
                        lastArrowRender.Add(ArrowRender.NewArrow(slot.worldPosition, reachable.slot.worldPosition));
                    }
                }
            }
        }
        [JsonIgnore]
        static List<ArrowRender> lastArrowRender = new();
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

            map.Houses.Add(this);
        }

        protected override void OnDisable()
        {
            map.Houses.Remove(this);
        }

        protected override void OnEnable()
        {
            m_capacity = new SolidMiddleware<Int>(new Int(100));
            base.OnEnable();
        }
    }
    /// <summary>
    /// 砖瓦房
    /// </summary>
    public class TileHouse : House
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
            base.OnEnable();
        }
    }
    /// <summary>
    /// 水泥房
    /// </summary>
    public class CementHouse : House
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
            base.OnEnable();
        }
    }
    #endregion

    #region 特殊建筑
    public class Administration : MapObject, MustNotExist<IConstruction>, IConstruction
    {
        public override bool CanBeUnjected => false;

        public float Cost => 0f;

        public string Name => "乡镇机关";

        public ConstructType constructType => ConstructType.Govern;


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
    public class Pollution : MapObject, IInfoProvider  //MapObject.Virtual是一个虚拟的MapObject，不会被渲染，且尽量简单
    {
        public override bool CanBeUnjected => throw new NotImplementedException();

        public void ProvideInfo(Action<string> provide)
        {
            provide("污染");
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

public abstract class RippleEffectBuilding<Eff> : MapObject where Eff : MapObject, new()
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
            eff.Unject(true);
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
    House, Farm, Factory, Supply, Road, Govern
}



#region 注释
/*
     public class House : MapObject, MustNotExist<House>
    {

        static House lastHouse;


        protected override void OnEnable()
        {
            // InfoWindow.Create(slot.worldPosition.ToString());
            if (lastHouse != null)
            {
                ArrowRender.NewArrow(lastHouse.slot.worldPosition, slot.worldPosition);
            }
            lastHouse = this;
        }


        [JsonProperty]
        SolidMiddleware<GameDataVector> ConsumptionAndSalary;
        protected override void OnCreated()
        {
            ConsumptionAndSalary = new SolidMiddleware<GameDataVector>(new GameDataVector(dailyIncome: 30, Money: -2000), this);
            map.economyWrapper.AddMiddleware(ConsumptionAndSalary);

        }


        protected override void OnDisable()
        {

        }

        public override bool CanBeUnjected => true;
    }
    public void ProvideInfo(Action<string> provide)
        {
            provide($"当前朝向{Direction}");

            SlotRender.OnAnySlotExit += ResetArrow;

            void ResetArrow(SlotRender _)
            {
                for (int i = lastArrowRender.Count - 1; i >= 0; i--)
                {
                    MonoBehaviour.Destroy(lastArrowRender[i].gameObject);
                }
                lastArrowRender.Clear();

                SlotRender.OnAnySlotExit -= ResetArrow;
            }

            Road r = map[slot.position + 上右下左[Direction]]?.GetMapObject<Road>();

            if (r != null)
            {
                foreach (MapObject reachable in r.cluster.GetReachableMapObject())
                {
                    if (reachable != this && reachable is House)
                    {
                        provide($"可到达 {reachable.slot.position}");
                        lastArrowRender.Add(ArrowRender.NewArrow(slot.worldPosition, reachable.slot.worldPosition));
                    }
                }
            }
        }
*/
#endregion