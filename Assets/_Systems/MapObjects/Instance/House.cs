using Newtonsoft.Json;
using UnityEngine;
using static Slot;
public class House : MapObject, IReject<House>
{

    protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
    {
        IconPattern iconPattern = IconPattern.Create(father, Vector3.zero);
        iconPattern.New("Building Icon");

        return base.Render(prefab, prefabs, slotRender);
    }

    protected override void OnInjected()
    {
    }

    protected override void OnUnjected()
    {
        
    }

    public override bool CanBeUnjected { get; protected set; }=true;
}

public class Road : MapObject
{
    public override bool CanBeUnjected { get; protected set; }=true;

    protected override void OnInjected()
    {
    }

    protected override void OnUnjected()
    {
        throw new System.NotImplementedException();
    }
}
