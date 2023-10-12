using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using static Slot;


public static partial class MapObjects
{
    #region 能源&工业设施
    public class BiomassPlant : MapObject, IConstruction, MustNotExist<IConstruction>, IPowerSupply, IFocusable
    {
        public float Cost => 250f;

        public string Name => "秸秆处理厂";

        public ConstructType constructType => ConstructType.EAI;

        public int phase => 2;

        public int energyConsumption => 0;

        public string Requiments => "";

        public float HeightOffset => 1f;

        public float Power => 5;

        public override bool CanBeUnjected => true;

        public string Lore => "图鉴已解锁：" + Name;

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
    public class PVPS : MapObject, IConstruction, MustNotExist<IConstruction>, IPowerSupply, IFocusable
    {
        public float Cost => 500f;

        public string Name => "光伏电站";

        public ConstructType constructType => ConstructType.EAI;

        public int phase => 2;

        public int energyConsumption => 0;

        public override bool CanBeUnjected => true;

        public float Power => 10;

        public string Requiments => "";

        public float HeightOffset => 1f;

        public string Lore => "图鉴已解锁：" + Name;

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

    #region 特殊建筑
    public class Administration : MapObject, MustNotExist<IConstruction>, IConstruction
    {
        public override bool CanBeUnjected => false;

        public float Cost => 0f;

        public string Name => "村民委员会";
        public string Requiments => "";

        public float HeightOffset => 1f;

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
    public abstract class Center : MapObject, IConstruction, MustNotExist<IConstruction>, IFocusable
    {
        public abstract float Cost { get; }

        public abstract string Name { get; }
        public override bool CanBeUnjected => true;
        public ConstructType constructType => ConstructType.Sevice;
        public abstract string Requiments { get; }
        public string Warning;
        public abstract float HeightOffset { get; }
        public abstract int phase { get; }
        protected GameObject WarningIcon;
        protected IconPattern iconPattern;
        public abstract int energyConsumption { get; }

        public string Lore => "图鉴已解锁：" + Name;

        public bool CheckConnection()
        {
            foreach (var holder in PlaceHolders)
            {
                Road r = map[holder.slot.position + 上右下左[Direction]]?.GetMapObject<Road>();
                if (r != null)
                {
                    foreach (MapObject reachable in r.cluster.ReachableMapObjects)
                    {
                        if (reachable != this && reachable.IsOrPlaceHolder<Administration>())
                        {
                            return true;
                        }
                    }
                }
            }
            Road r2 = map[slot.position + 上右下左[Direction]]?.GetMapObject<Road>();
            if (r2 != null)
            {
                foreach (MapObject reachable in r2.cluster.ReachableMapObjects)
                {
                    if (reachable != this && reachable.IsOrPlaceHolder<Administration>())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public abstract void Check();
        protected override void Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            base.Render(prefab, prefabs, slotRender);
            iconPattern = IconPattern.Create(father, Vector3.up * HeightOffset);
            WarningIcon = iconPattern.New("!");
            WarningIcon.SetActive(false);
        }


        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
            //Debug.Log(map.MainData.Money);
        }
        protected override void OnEnable()
        {
            Check();
            EventHandler.DayPass += Check;
            GameManager.OnMapUnloaded += OnMapUnloaded;
        }
        protected override void OnDisable()
        {
            EventHandler.DayPass -= Check;
        }
        private void OnMapUnloaded()
        {
            GameManager.OnMapUnloaded -= OnMapUnloaded;
            EventHandler.DayPass -= Check;
        }
    }
    public class DigitalCenter : Center
    {
        public override float Cost => 700f;
        public override int phase => 3;
        public override int energyConsumption => 5;
        public override string Name => "乡村数字中心";
        public override string Requiments => "";
        public override float HeightOffset => 1f;

        public override void Check()
        {
            if (!CheckConnection())
            {
                Warning = "未连通";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    //Debug.Log("IconSetActive");
                }
                if(map.Centers.Contains(this))
                {
                    map.Centers.Remove(this);
                }
            }
            else if (slot.GetMapObject<FiveGArea>() == null)
            {
                Warning = "需要5G网络";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    //Debug.Log("IconSetActive");
                }
                if (map.Centers.Contains(this))
                {
                    map.Centers.Remove(this);
                }
            }
            else
            {
                Warning = null;
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(false);
                }
                if (!map.Centers.Contains(this))
                {
                    map.Centers.Add(this);
                }
            }
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            map.Centers.Add(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            map.Centers.Remove(this);
        }
    }
    public class ECommerceServiceCenter : Center
    {
        public override float Cost => 400f;
        public override int phase => 3;
        public override int energyConsumption => 3;
        public override string Name => "电商服务中心";
        public override string Requiments => "";
        public override float HeightOffset => 0.75f;
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Check()
        {
            var CommerceCPU = map.FarmProfitTotal.CPUs.Find((cpu) => { return cpu.name == "电商服务"; });
            if (!CheckConnection())
            {
                if (CommerceCPU.name != null)
                {
                    map.FarmProfitTotal.RemoveCPU(CommerceCPU);
                }
                Warning = "未连通";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    //Debug.Log("IconSetActive");
                }

            }
            else if (slot.GetMapObject<FiveGArea>() == null)
            {
                if (CommerceCPU.name != null)
                {
                    map.FarmProfitTotal.RemoveCPU(CommerceCPU);
                }
                Warning = "需要5G网络";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    //Debug.Log("IconSetActive");
                }
            }
            else
            {
                if (CommerceCPU.name == null)
                {
                    map.FarmProfitTotal.AddCPU(
                        new SolidMiddleware<Float>.CPU { name = "电商服务", Multipliable = true, Multiplication = new Float(1.5f) });
                }
                Warning = null;
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(false);
                }
            }
        }
    }
    //public class LogisticsCenter : Center
    //{
    //    public override float Cost => 200f;
    //    public override int energyConsumption => 5;
    //    public override string Name => "物流中心";
    //    public override int phase => 3;
    //    public override string Requiments => "";
    //    public override float HeightOffset => 1f;
    //}
    #endregion

    public class ChargingStation : MapObject, IConstruction, IFocusable,MustNotExist<IConstruction>
    {
        public float Cost => 60;

        public string Name => "新能源充电桩";
        public string Lore => "图鉴已解锁："+Name;
        public int energyConsumption => 5;
        public ConstructType constructType => ConstructType.Sevice;
        public string Requiments => "";

        public float HeightOffset => 1f;
        public int phase => 4;

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

        public override void OnClick()
        {

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

}
public interface IFocusable
{
    public string Lore { get; }
}
public interface IConstruction : MustNotExist<MapObjects.Tree>, MustNotExist<MapObjects.Lake>
{
    public float Cost { get; }
    public string Name { get; }
    public ConstructType constructType { get; }
    public int phase { get; }
    public int energyConsumption { get; }
    public string Requiments { get; }
    public float HeightOffset { get; }
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