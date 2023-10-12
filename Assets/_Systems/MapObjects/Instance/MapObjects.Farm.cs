using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapObjects;
using static Slot;


public static partial class MapObjects
{
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

        public abstract string Requiments { get; }
        public string Warning;
        public abstract float HeightOffset { get; }
        public abstract void Check();
        protected GameObject WarningIcon;
        protected IconPattern iconPattern;

        [JsonProperty]
        protected SolidMiddleware<Float> m_profit;
        public abstract void FarmParaInit();
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
        public bool CheckDigital()
        {
            foreach(var ele in slot.map.Centers)
            {
                if (ele.Name == "乡村数字中心" && ele.Warning != "")
                    return true;
                Debug.Log(ele.Name);
            }
            return false;
        }
        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
        }

        protected override void OnDisable()
        {
            EventHandler.DayPass -= Check;
            
        }

        protected override void OnEnable()
        {
            Check();
            EventHandler.DayPass += Check;
            GameManager.OnMapUnloaded += OnMapUnloaded;
        }
        private void OnMapUnloaded()
        {
            GameManager.OnMapUnloaded -= OnMapUnloaded;
            EventHandler.DayPass -= Check;
        }
        protected override void Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            base.Render(prefab, prefabs, slotRender);
            iconPattern = IconPattern.Create(father, Vector3.up * HeightOffset);
            WarningIcon = iconPattern.New("!");
            WarningIcon.SetActive(false);
        }
    }
    public class RiceFarm : Farm, MustNotExist<IConstruction>
    {
        public override float Cost => 6f;

        public override string Name => "水稻田";

        public override int phase => 1;

        public override int energyConsumption => 0;
        public override string Requiments => "";

        public override float HeightOffset => 1f;

        public override void Check()
        {
           
        }

        public override void FarmParaInit()
        {
            m_profit = new SolidMiddleware<Float>(new Float(0.06f));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            FarmParaInit();
            map.Farms.Add(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            map.Farms.Remove(this);
        }

    }
    public class VegeFarm : Farm, MustNotExist<IConstruction>
    {
        public override float Cost => 16f;

        public override string Name => "菜田";
        public override int phase => 1;
        public override int energyConsumption => 0;
        public override string Requiments => "";

        public override float HeightOffset => 1f;

        public override void Check()
        {
            
        }

        public override void FarmParaInit()
        {
            m_profit = new SolidMiddleware<Float>(new Float(0.09f));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            FarmParaInit();
            map.Farms.Add(this);
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
        }

    }
    public class GreenHouse : Farm, MustExist<VegeFarm>, IFocusable
    {
        public override float Cost => 30f;
        public override int energyConsumption => 1;
        public override string Name => "温室大棚";
        public override int phase => 2;
        public override string Requiments => "需建在菜田上";

        public override float HeightOffset => 0.7f;

        public string Lore => "图鉴已解锁：" + Name;

        public override void Check()
        {
            if (!CheckConnection())
            {
                Warning = "未连通";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    m_profit.UpdateValue(new Float(0));
                    //Debug.Log("IconSetActive");
                }

            }
            else
            {
                Warning = null;
                if (WarningIcon != null)
                {
                    m_profit.UpdateValue(new Float(0.85f));
                    WarningIcon.SetActive(false);
                }
            }
        }

        public override void FarmParaInit()
        {
            m_profit = new SolidMiddleware<Float>(new Float(0.85f));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            FarmParaInit();
            map.Farms.Add(this);

            foreach (var holder in PlaceHolders)
            {
                holder.slot.GetMapObject<VegeFarm>().Unject();
            }
            slot.GetMapObject<VegeFarm>().Unject();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            map.Farms.Remove(this);


        }

    }
    public class intelGreenHouse : Farm, MustExist<GreenHouse>, IFocusable
    {
        public override float Cost => 150f;
        public override int energyConsumption => 3;
        public override string Name => "智慧大棚";
        public override int phase => 3;
        public override string Requiments => "需建在温室大棚上";

        public override float HeightOffset => 0.7f;

        public string Lore => "图鉴已解锁：" + Name;

        public override void Check()
        {
            if (!CheckConnection())
            {
                Warning = "未连通";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    m_profit.UpdateValue(new Float(0));
                    //Debug.Log("IconSetActive");
                }

            }
            else if(!CheckDigital())
            {
                Warning = "需要农业数字中心";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    m_profit.UpdateValue(new Float(0));
                    //Debug.Log("IconSetActive");
                }
            }
            else if (slot.GetMapObject<FiveGArea>() == null)
            {
                Warning = "需要5G网络";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    m_profit.UpdateValue(new Float(0));
                    //Debug.Log("IconSetActive");
                }
            }
            else
            {
                Warning = null;
                if (WarningIcon != null)
                {
                    m_profit.UpdateValue(new Float(1.2f));
                    WarningIcon.SetActive(false);
                }
            }
        }

        public override void FarmParaInit()
        {
            m_profit = new SolidMiddleware<Float>(new Float(1.2f));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            FarmParaInit();
            map.Farms.Add(this);
            slot.GetMapObjectsIfPlaceHolder<GreenHouse>().Single().Unject();

        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
            base.OnDisable();
        }

    }
    public class FarmDrone : Farm, MustExist<RiceFarm>, IFocusable
    {
        public override float Cost => 35f;
        public override int energyConsumption => 1;
        public override string Name => "农业无人机";
        public override int phase => 3;
        public override string Requiments => "需建在水稻田上";

        public override float HeightOffset => 0.4f;

        public string Lore => "图鉴已解锁：" + Name;

        public override void Check()
        {
            if (!CheckDigital())
            {
                Warning = "需要农业数字中心";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    m_profit.UpdateValue(new Float(0));
                    //Debug.Log("IconSetActive");
                }
            }
            else if (slot.GetMapObject<FiveGArea>() == null)
            {
                Warning = "需要5G网络";
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    m_profit.UpdateValue(new Float(0));
                    //Debug.Log("IconSetActive");
                }
            }
            else
            {
                Warning = null;
                if (WarningIcon != null)
                {
                    m_profit.UpdateValue(new Float(0.1f));
                    WarningIcon.SetActive(false);
                }
            }
        }

        public override void FarmParaInit()
        {
            m_profit = new SolidMiddleware<Float>(new Float(0.1f));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            FarmParaInit();
            map.Farms.Add(this);
            slot.GetMapObject<RiceFarm>().Unject();
        }
        protected override void OnDisable()
        {
            map.Farms.Remove(this);
            base.OnDisable();
        }
    }
    #endregion
}
