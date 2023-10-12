using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    #region סլ¥
    /// <summary>
    /// סլ����
    /// </summary>
    public abstract class House : MapObject, IConstruction
    {
        //�˿�����
        [JsonProperty] protected SolidMiddleware<Int> m_capacity;

        public int Capacity => m_capacity.currentValue.m_value;
        public override bool CanBeUnjected => true;
        public abstract float Cost { get; }
        public abstract string Name { get; }
        public string Warning;
        public abstract string Requiments { get; }

        public abstract float HeightOffset { get; }
        public ConstructType constructType => ConstructType.House;

        public abstract int phase { get; }
        IconPattern iconPattern;
        GameObject WarningIcon;
        public abstract int energyConsumption { get; }
        public abstract void HouseParaInit();
        public void CheckConnection()
        {
            Road r = map[slot.position + ��������[Direction]]?.GetMapObject<Road>();
            bool Connected = false;
            if (r != null)
            {
                foreach (MapObject reachable in r.cluster.ReachableMapObjects)
                {
                    if (reachable != this && reachable.IsOrPlaceHolder<Administration>())
                    {
                        Connected = true;
                        break;
                    }
                }
            }

            var ConnectCPU = m_capacity.CPUs.Find((cpu) => { return cpu.name == "δ��ͨ"; });
            if (!Connected)
            {
                if (ConnectCPU.name == null)
                {
                    m_capacity.AddCPU(new SolidMiddleware<Int>.CPU
                    { name = "δ��ͨ", Addition = new Int(0), Multipliable = true, Multiplication = new Int(0) });
                    Warning = "δ��ͨ";
                }
                if (WarningIcon != null)
                {
                    WarningIcon.SetActive(true);
                    //Debug.Log("IconSetActive");
                }

            }
            else if (Connected)
            {
                if (ConnectCPU.name != null)
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
            iconPattern = IconPattern.Create(father, Vector3.up * HeightOffset);
            WarningIcon = iconPattern.New("!");
            WarningIcon.SetActive(false);
        }


    }



    /// <summary>
    /// ˮ�෿
    /// </summary>
    public class CementHouse : House, MustNotExist<IConstruction>
    {
        public override float Cost => 120;

        public override string Name => "ˮ�෿";
        public override int phase => 1;

        public override int energyConsumption => 1;
        public override string Requiments => "";

        public override float HeightOffset => 1f;
        public override void HouseParaInit()
        {
            m_capacity = new SolidMiddleware<Int>(new Int(35));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            HouseParaInit();
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
    /// �����סլ��
    /// </summary>
    public class PV : House, MustExist<CementHouse>, IPowerSupply, IFocusable
    {
        public override float Cost => 25;

        public override string Name => "���(סլ)";
        public override int phase => 1;
        public override int energyConsumption => 1;
        public override string Requiments => "�轨��ˮ�෿��";

        public override float HeightOffset => 1f;
        public float Power => 1;

        public string Lore => "ͼ���ѽ�����" + Name;

        public override void HouseParaInit()
        {
            m_capacity = new SolidMiddleware<Int>(new Int(35));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            HouseParaInit();
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
    /// ����
    /// </summary>
    public class Homestay : House, MustNotExist<IConstruction>, IOtherProfit, IFocusable
    {
        public override float Cost => 90;

        public override string Name => "����";
        public override int phase => 4;
        public override int energyConsumption => 1;
        public override string Requiments => "";

        public override float HeightOffset => 0.7f;
        public float Profit => 0.07f;

        public string Lore => "ͼ���ѽ�����" + Name;

        public override void HouseParaInit()
        {
            m_capacity = new SolidMiddleware<Int>(new Int(15));
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            HouseParaInit();
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
}
