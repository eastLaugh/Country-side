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
        }

        [JsonConstructor]
        public PlaceHolder()
        {

        }

        public override bool CanBeUnjected => mapObject.CanBeUnjected;

        public void ProvideInfo(Action<string> provide)
        {
            provide(mapObject.GetType() + " 占位符");
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