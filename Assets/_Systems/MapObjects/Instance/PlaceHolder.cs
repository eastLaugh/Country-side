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
        MapObject mapObject;

        public override bool CanBeUnjected => throw new System.NotImplementedException();

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

        protected override void OnClick()
        {
            
        }

    }

}