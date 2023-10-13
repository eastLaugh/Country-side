using Newtonsoft.Json;
using UnityEngine;
using static Slot;
using System.Linq;

partial class MapObjects
{

    //虚拟MapObject，用于表示地块被删除
    public class DeletingFlag : MapObject, MustNotExist<Tree>, MustNotExist<Lake>, MustNotExist<Administration>
    {
        public override bool CanBeUnjected => true;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {

        }

        protected override void OnCreated()
        {
            foreach (var mapObject in slot.mapObjects.ToArray())
            {
                if (mapObject.CanBeUnjected)
                {
                    mapObject.Unject();
                }
            }

            Unject();
        }
    }

}