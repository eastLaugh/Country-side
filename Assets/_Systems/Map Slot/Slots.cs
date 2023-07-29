using System.Collections.Generic;
using UnityEngine;

public static class Slots
{
    public class Water : Slot
    {
        public Water(Map map, Vector2 position, HashSet<MapObject> mapObjects) : base(map, position, mapObjects)
        {
            Debug.Log("Water有参构造函数");
            new MapObjects.Lake().Inject(this); //不用担心读档时，该指令会被执行两次。因为Lake MustNotExist<Lake> 会阻止这种情况的发生。
        }
    }
    public class Plain : Slot
    {
        public Plain(Map map, Vector2 position, HashSet<MapObject> mapObjects) : base(map, position, mapObjects)
        {
        }

    }

}