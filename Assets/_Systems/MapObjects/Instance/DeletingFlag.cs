using Newtonsoft.Json;
using UnityEngine;
using static Slot;
using System.Linq;

//虚拟MapObject，用于表示地块被删除
public class DeletingFlag : MapObject
{
    public override bool CanBeUnjected { get; protected set; } = true;

    protected override void Awake()
    {
        this.Unject();
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
    }
}
