using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectAccumlator : MonoBehaviour
{
    private void OnEnable()
    {
        Slot.MapObject.OnInjected += OnInjected;
        Slot.MapObject.OnUnjected += OnUnjected;
    }

    private void OnInjected(Slot.MapObject mapObject, bool force)
    {
        if (!force) //由于读档时也会把每一个已存在的MapObject重新强制自注入，这份强制不是我们想要的，我们只需要统计玩家的操作，故舍去
        {

            string typename = mapObject.GetType().Name;
            Map map = mapObject.map; //获取地图
            if (map.BuildingsNum.ContainsKey(typename))
            {
                Debug.Log(typename + "+1");
                map.BuildingsNum[typename] += 1;
            }
            else
            {
                Debug.Log(typename + "+1");
                map.BuildingsNum.Add(typename, 1);
            }
        }

    }
    private void OnUnjected(Slot.MapObject mapObject)
    {
        Map map = mapObject.map;
        map.BuildingsNum[mapObject.GetType().Name] -= 1;
        Debug.Log(GetType().Name + "-1");
    }
    

    private void OnDisable()
    {
        Slot.MapObject.OnInjected -= OnInjected;
        Slot.MapObject.OnUnjected -= OnUnjected;
    }
}
