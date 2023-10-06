using Newtonsoft.Json;
using UnityEngine;
using static Slot;
using System.Linq;
using System;

partial class MapObjects
{

    //占位符MapObject，仅用来占位，例如为2x1的MapObject预留2个Slot
    public class PlaceHolder : MapObject, IInfoProvider
    {
        [JsonProperty]
        public readonly MapObject mapObject;


        public PlaceHolder(MapObject mapObject)
        {
            this.mapObject = mapObject;
            mapObject.PlaceHolders.Add(this);
            Direction = mapObject.Direction; //注意这个语句实际不奏效，因为PlaceHolder在被注入前，主MapObject尚未被注入，主MapObject不在地图上，尚未存在方向(0)
        }

        [JsonConstructor]
        public PlaceHolder()
        {

        }

        public override bool CanBeUnjected => mapObject.CanBeUnjected;

        public void ProvideInfo(Action<string> provide)
        {
            provide(mapObject.GetType() + " 占位符");
            provide("朝向: " + Direction);
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

        public override void OnClick()
        {
            mapObject.OnClick();
        }

    }

}