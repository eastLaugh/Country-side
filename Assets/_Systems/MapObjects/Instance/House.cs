using Newtonsoft.Json;
using static Slot;
public class House : MapObject /* , IReject<House>, IReject<Road> */
{
    [JsonConstructor]
    public House(int AppearanceSeed) : base(AppearanceSeed)
    {
    }

    public House():base(-1)
    {
        
    }
    protected override void OnSlot()
    {

    }
}

public class Road : MapObject
{
    public Road(int AppearanceSeed) : base(AppearanceSeed)
    {
    }

    public Road():base(-1)
    {
        
    }
    protected override void OnSlot()
    {
    }
}
