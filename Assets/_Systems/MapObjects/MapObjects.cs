using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    #region 能源&工业设施
    public class PVPS : MapObject, IConstruction, MustNotExist<IConstruction>, IPowerSupply
    {
        public float Cost => 200;

        public string Name => "光伏电站";

        public ConstructType constructType => ConstructType.EAI;

        public int phase => 2;

        public int energyConsumption => 0;

        public override bool CanBeUnjected => true;

        public float Power => 10;


        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
            map.PowerSupplies.Add(this);
        }

        protected override void OnDisable()
        {
            map.PowerSupplies.Remove(this);
        }

        protected override void OnEnable()
        {
            
        }
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

        public abstract int phase { get; }

        public abstract int energyConsumption { get; }

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
    public class RiceFarm : Farm, MustNotExist<IConstruction>
    {
        public override float Cost => 6f;

        public override string Name => "水稻田";

        public override int phase => 1;

        public override int energyConsumption => 0;

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
    public class VegeFarm : Farm, MustNotExist<IConstruction>
    {
        public override float Cost => 10f;

        public override string Name => "菜田";
        public override int phase => 1;
        public override int energyConsumption => 0;
        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(0.01f));
            map.Farms.Add(this);
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }

    }
    public class GreenHouse : Farm, MustExist<VegeFarm>
    {
        public override float Cost => 18f;
        public override int energyConsumption => 1;
        public override string Name => "温室大棚";
        public override int phase => 2;

        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(0.012f));
            map.Farms.Add(this);

            foreach(var holder in PlaceHolders)
            {
                holder.slot.GetMapObject<VegeFarm>().Unject();
            }
            slot.GetMapObject<VegeFarm>().Unject();
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }

    }
    public class intelGreenHouse : Farm, MustExist<GreenHouse>
    {
        public override float Cost => 20f;
        public override int energyConsumption => 2;
        public override string Name => "智慧大棚";
        public override int phase => 3;

        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(0.016f));
            map.Farms.Add(this);
            slot.GetMapObject<GreenHouse>().Unject();
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }

    }
    public class FarmDrone : Farm, MustExist<RiceFarm>
    {
        public override float Cost => 10f;
        public override int energyConsumption => 2;
        public override string Name => "农业无人机";
        public override int phase => 3;
        protected override void OnCreated()
        {
            base.OnCreated();
            m_profit = new SolidMiddleware<Float>(new Float(0.016f));
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
    public abstract class House : MapObject, IConstruction
    {
        //人口容量
        [JsonProperty] protected SolidMiddleware<Int> m_capacity;

        public int Capacity => m_capacity.currentValue.m_value;
        public override bool CanBeUnjected => true;
        public abstract float Cost { get; }
        public abstract string Name { get; }
        public string Warning;
        public ConstructType constructType => ConstructType.House;

        public abstract int phase { get; }
        IconPattern iconPattern;
        GameObject WarningIcon;
        public abstract int energyConsumption { get; }

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
            if (!Connected)
            {
                if(ConnectCPU.name == null)
                {
                    m_capacity.AddCPU(new SolidMiddleware<Int>.CPU
                    { name = "未连通", Addition = new Int(0), Multipliable = true, Multiplication = new Int(0) });
                    Warning = "未连通";
                }             
                if(WarningIcon!=null)
                {
                    WarningIcon.SetActive(true);
                    //Debug.Log("IconSetActive");
                }
                
            }
            else if (Connected)
            {
                if(ConnectCPU.name != null)
                {
                    m_capacity.RemoveCPU(ConnectCPU);
                    Warning = null;
                }               
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(false);
                }
                
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
        protected override void OnDisable()
        {
            EventHandler.DayPass -= CheckConnection;
        }
        private void OnMapUnloaded()
        {
            GameManager.OnMapUnloaded -= OnMapUnloaded;
            EventHandler.DayPass -= CheckConnection;
        }

        public override void OnClick()
        {
        }
        protected override void Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            base.Render(prefab, prefabs, slotRender);
            iconPattern = IconPattern.Create(father, Vector3.up);
            WarningIcon = iconPattern.New("!");
            WarningIcon.SetActive(false);
        }


    }



    /// <summary>
    /// 水泥房
    /// </summary>
    public class CementHouse : House, MustNotExist<IConstruction>
    {
        public override float Cost => 80;

        public override string Name => "水泥房";
        public override int phase => 1;

        public override int energyConsumption => 1;

        protected override void OnCreated()
        {
            base.OnCreated();
            m_capacity = new SolidMiddleware<Int>(new Int(60));
            map.Houses.Add(this);
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            map.Houses.Remove(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
    /// <summary>
    /// 光伏（住宅）
    /// </summary>
    public class PV : House, MustExist<CementHouse>, IPowerSupply
    { 
        public override float Cost => 15;

        public override string Name => "光伏(住宅)";
        public override int phase => 1;
        public override int energyConsumption => 1;

        public float Power => 1;

        protected override void OnCreated()
        {
            base.OnCreated();
            m_capacity = new SolidMiddleware<Int>(new Int(60));
            map.Houses.Add(this);
            map.PowerSupplies.Add(this);
            slot.GetMapObject<CementHouse>().Unject();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            map.Houses.Remove(this);
            map.PowerSupplies.Remove(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
    /// <summary>
    /// 民宿
    /// </summary>
    public class Homestay : House, MustNotExist<IConstruction>, IOtherProfit
    {
        public override float Cost => 65;

        public override string Name => "民宿";
        public override int phase => 4;
        public override int energyConsumption => 1;

        public float Profit => 0.05f;

        protected override void OnCreated()
        {
            base.OnCreated();
            m_capacity = new SolidMiddleware<Int>(new Int(25));
            map.Houses.Add(this);
            map.OtherProfits.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
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

        public ConstructType constructType => ConstructType.Sevice;

        public int phase => 1;

        public int energyConsumption => 1;

        public override void OnClick()
        {
            EventHandler.CallToAdiminstration();
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

    #region 中心
    public abstract class Center : MapObject,IConstruction, MustNotExist<IConstruction>
    {
        public abstract float Cost { get; }

        public abstract string Name { get; }
        public override bool CanBeUnjected => true;
        public ConstructType constructType => ConstructType.Sevice;

        public abstract int phase { get; }

        public abstract int energyConsumption { get; }


        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
            //Debug.Log(map.MainData.Money);
        }
        protected override void OnEnable()
        {
        }
        protected override void OnDisable()
        {
        }
        private void OnMapUnloaded()
        {
        }
    }
    public class DigitalCenter : Center
    {
        public override float Cost => 300f;
        public override int phase => 3;
        public override int energyConsumption => 5;
        public override string Name => "乡村数字中心";
    }
    public class ECommerceServiceCenter : Center
    {
        public override float Cost => 200f;
        public override int phase => 3;
        public override int energyConsumption => 3;
        public override string Name => "电商服务中心";
    }
    public class LogisticsCenter : Center
    {
        public override float Cost => 200f;
        public override int energyConsumption => 5;
        public override string Name => "物流中心";
        public override int phase => 3;
    }
    #endregion

    public class ChargingStation : MapObject,IConstruction
    {
        public float Cost => 90;

        public string Name => "新能源充电桩";
        public int energyConsumption => 5;
        public ConstructType constructType => ConstructType.Sevice;

        public int phase => 1;

        public override bool CanBeUnjected => true;

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
        DoCircle(RippleRadius, slot.position);
        foreach (MapObject placeHolder in PlaceHolders)
        {
            DoCircle(RippleRadius, placeHolder.slot.position);
        }
        void DoCircle(int RippleRadius, Vector2 center)
        {

            for (int i = -RippleRadius; i <= RippleRadius; i++)
            {
                for (int j = -RippleRadius; j <= RippleRadius; j++)
                {
                    if (i * i + j * j <= RippleRadius * RippleRadius)
                    {
                        Slot s = map[center + new Vector2(i, j)];
                        if (s != null)
                        {
                            IEnumerable<Eff> existed = s.GetMapObjects<Eff>();
                            if (existed == null || !existed.Any(eff => Effects.Contains(eff)))
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
public interface IConstruction: MustNotExist<MapObjects.Tree>, MustNotExist<MapObjects.Lake>
{
    public float Cost { get; }
    public string Name { get; }
    public ConstructType constructType { get; }
    public int phase { get; }
    public int energyConsumption { get; }
}
public interface IOtherProfit
{
    public float Profit { get; }
}
public interface IPowerSupply
{
    public float Power { get; }
}
public enum ConstructType
{
    House, Farm, EAI, Sevice, Road, Govern
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