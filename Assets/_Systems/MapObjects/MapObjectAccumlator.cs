using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectAccumlator : MonoBehaviour
{
    private void OnEnable()
    {
        Slot.MapObject.OnInjected += OnInjected;
    }

    private void OnInjected(Slot.MapObject mapObject, bool force)
    {
        if (!force) //由于读档时也会把每一个已存在的MapObject重新强制自注入，这份强制不是我们想要的，我们只需要统计玩家的操作，故舍去
        {

            string 类型名 = mapObject.GetType().Name;
            Map map = mapObject.map; //获取地图

            //操作map里的字段添加键值对，注意这里应该禁止修改map的字段，尽可访问字典

            //map.字典[类型名] ++

            //PS用Type作为字典的Key也可以
        }

    }
    

    private void OnDisable()
    {
        Slot.MapObject.OnInjected -= OnInjected;
    }
}
