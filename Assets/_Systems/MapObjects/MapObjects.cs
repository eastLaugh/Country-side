using System;
using Newtonsoft.Json;
using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class House : MapObject, MustNotExist<House>
    {

        protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            IconPattern iconPattern = IconPattern.Create(father, Vector3.zero);
            iconPattern.New("Building Icon");

            return base.Render(prefab, prefabs, slotRender);
        }

        [JsonProperty]
        Middleware<EconomyVector> consumption;
        protected override void OnEnable()
        {
            if (consumption == null)
            {
                consumption = new SolidMiddleware<EconomyVector>(new EconomyVector(0, -200f, 0));
                map.economyWrapper.AddMiddleware(consumption);
            }
        }

        protected override void OnDisable()
        {
        }

        protected override void OnCreated()
        {
        }

        public override bool CanBeUnjected => true;
    }


    //桑叶
    public class Mulberry : Resource<Mulberry>
    {

    }

    //纺织厂
    public class TextileMill : ResourceBuilding<Mulberry>
    {
        protected override void OnEnable()
        {
            
        }

        protected override void OnCreated()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDisable()
        {
            throw new System.NotImplementedException();
        }
    }


    public class 市中心 : MapObject , MustNotExist<Tree> ,IInfoProvider
    {
        public override bool CanBeUnjected => true;

        public void ProvideInfo(Action<string> provide)
        {
            provide("市中心");
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
        throw new System.NotImplementedException();
    }
}


public abstract class ResourceBuilding<R> : MapObject , MustExist<R> where R : Resource<R>
{
    public override bool CanBeUnjected => true;

}