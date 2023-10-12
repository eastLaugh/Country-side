using Newtonsoft.Json;
using UnityEngine;
using static Slot;
using System.Linq;

partial class MapObjects
{

    //虚拟MapObject，用于表示地块被删除
    public class DeletingFlag : MapObject,MustNotExist<Tree>,MustNotExist<Lake>,MustNotExist<Administration>
    {
        public override bool CanBeUnjected => true;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            //在使用foreach循环遍历HashSet时，如果在循环中尝试删除HashSet的元素，可能会导致InvalidOperationException异常。
            //为了防止这种情况，我们需要在循环之前先复制HashSet的内容到一个数组中，然后再遍历这个数组
            foreach (var mapObject in slot.mapObjects.ToArray())
            {
                if (mapObject.CanBeUnjected)
                {
                    mapObject.Unject();
                }
            }
        }

        protected override void OnCreated()
        {
            Unject();
        }
    }

}