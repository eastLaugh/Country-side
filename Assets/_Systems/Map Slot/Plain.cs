using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plain : Slot
{
    public Plain(Map map, Vector2 position, HashSet<MapObject> mapObjects) : base(map, position, mapObjects)
    {
        if (UnityEngine.Random.Range(0, 100) < 10){
            new Tree().Inject(this);
        }
    }


    //protected override GameObject GetPrefab => GameManager.SOLE.PlainPrefab;
}
