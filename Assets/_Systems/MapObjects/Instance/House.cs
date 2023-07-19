using Newtonsoft.Json;
using UnityEngine;
using static Slot;
public class House : MapObject /* , IReject<House>, IReject<Road> */
{
    public House(int AppearanceSeed) : base(AppearanceSeed)
    {
    }

    protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender, Transform father)
    {
        IconPattern iconPattern = IconPattern.Create(father,Vector3.zero);
        iconPattern.New("Building Icon");

        return base.Render(prefab, prefabs, slotRender, father);
    }
}

public class Road : MapObject
{
    public Road(int AppearanceSeed) : base(AppearanceSeed)
    {
    }

}
