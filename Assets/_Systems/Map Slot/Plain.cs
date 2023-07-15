using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plain : Slot
{
    public Plain(Map map, Vector2 position ,HashSet<MapObject> mapObjects) : base(map, position,mapObjects)
    {

    }


    //protected override GameObject GetPrefab => GameManager.SOLE.PlainPrefab;
}
