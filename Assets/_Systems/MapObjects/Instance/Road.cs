using System;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class Road : MapObject, MustNotExist<Road>, IInfoProvider
    {


        public override bool CanBeUnjected => true;

        protected override bool Clustered => true;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            foreach (var dir in Slot.上下左右)
            {
                var target = slot.map[slot.position + dir];
                if (target != null)
                {
                    var road = target.GetMapObject<Road>();
                    if (road != null)
                    {
                        RoadRenderer.Instance.Connect(this, road);
                    }
                }
            }
        }

        protected override void OnDisable()
        {

        }
        protected override void OnCreated()
        {
        }

        public void ProvideInfo(Action<string> provide)
        {
            provide("道路 ");
        }

        public override void OnClick()
        {
            foreach (var mapObject in cluster.mapObjects)
            {
                if (mapObject is Road road)
                {
                    road.slot.slotRender.Shake();
                    
                }
                else
                {
                    InfoWindow.Create("Cluster模块 出错了");
                }
            }
        }

        protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            //覆盖默认的渲染方式
            return null;
        }


    }
}