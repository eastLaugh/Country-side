using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Slot;

public static partial class MapObjects
{
    public class Road : MapObject, MustNotExist<Road>, IInfoProvider
    {


        public override bool CanBeUnjected => true;


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
            throw new System.NotImplementedException();
        }

        public void ProvideInfo(Action<string> provide)
        {
            provide("道路 ");
        }

        protected override void OnClick()
        {

        }

        protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            //覆盖默认的渲染方式
            return null;
        }
    }
}