using System.Collections.Generic;
using UnityEngine;

public static class Slots
{
    public class Water : Slot
    {
        public Water(Map map, Vector2 position, HashSet<MapObject> mapObjects) : base(map, position, mapObjects)
        {
        }
    }
    public class Plain : Slot
    {
        public Plain(Map map, Vector2 position, HashSet<MapObject> mapObjects) : base(map, position, mapObjects)
        {
        }

    }

  

}

