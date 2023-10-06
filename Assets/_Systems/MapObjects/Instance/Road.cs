using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public abstract class Road : MapObject, MustNotExist<IConstruction>, IInfoProvider, IConstruction
    {
        public override bool CanBeUnjected => true;

        protected override bool Clustered => true;

        public abstract float Cost { get; }
        public abstract string Name { get; }
        public ConstructType constructType => ConstructType.Road;

        public abstract int phase { get; }

        public int energyConsumption => 0;

        protected override void OnEnable()
        {
            //foreach (var dir in Slot.上右下左)
            //{
            //    var target = slot.map[slot.position + dir];
            //    if (target != null)
            //    {
            //        var road = target.GetMapObject<Road>();
            //        if (road != null)
            //        {
            //            RoadRenderer.Instance.Connect(this, road);
            //        }
            //    }
            //}
        }

        protected override void OnDisable()
        {

        }
        protected override void OnCreated()
        {
            map.MainData.Money -= Cost;
        }

        public void ProvideInfo(Action<string> provide)
        {
            provide("道路 ");
        }

        public override void OnClick()
        {
            if (cluster != null)
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
                foreach (MapObject reachable in cluster.ReachableMapObjects)
                {
                    MapObject obj;
                    if(reachable is PlaceHolder p){
                        obj = p.mapObject;
                    }else{
                        obj = reachable;
                    }
                    var outline = obj.gameObject.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = obj.gameObject.AddComponent<Outline>();
                        outline.OutlineColor = Color.red;
                    }
                    GameManager.current.StartCoroutine(Wait());

                    IEnumerator Wait()
                    {
                        yield return new WaitForSeconds(0.5f);
                        if (outline)
                        {
                            GameManager.Destroy(outline);
                        }
                    }
                }
            }
        }




    }
    public class CementRoad : Road
    {
        public override float Cost => 0.35f;

        public override string Name => "水泥路";

        public override int phase => 1;
    }
    public class PitchRoad : Road
    {
        public override float Cost => 0.9f;

        public override string Name => "沥青路";

        public override int phase => 2;
    }
}